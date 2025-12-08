using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Jeeroway.Api;

public class LiveFrameBroadcaster
{
    private readonly ConcurrentDictionary<Guid, HashSet<Channel<byte[]>>> _subscribersByRobo = new();
    private readonly object _sync = new();

    public Channel<byte[]> Subscribe(Guid roboId, int capacity = 10)
    {
        var ch = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(capacity)
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest
        });

        _subscribersByRobo.AddOrUpdate(
            roboId,
            _ => new HashSet<Channel<byte[]>> { ch },
            (_, existing) =>
            {
                lock (existing)
                {
                    existing.Add(ch);
                }
                return existing;
            });

        return ch;
    }

    public void Unsubscribe(Guid roboId, Channel<byte[]> channel)
    {
        if (_subscribersByRobo.TryGetValue(roboId, out var subscribers))
        {
            lock (subscribers)
            {
                subscribers.Remove(channel);
            }
        }
        channel.Writer.TryComplete();
    }

    public void Broadcast(Guid roboId, byte[] frame)
    {
        if (!_subscribersByRobo.TryGetValue(roboId, out var subscribers))
            return;

        Channel<byte[]>[] subs;
        lock (subscribers)
        {
            subs = subscribers.ToArray();
        }

        foreach (var ch in subs)
        {
            ch.Writer.TryWrite(frame);
        }
    }
}

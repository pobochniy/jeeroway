using System.Threading.Channels;

namespace Jeeroway.Api;

public class LiveFrameBroadcaster
{
    private readonly object _sync = new();
    private readonly HashSet<Channel<byte[]>> _subscribers = new();

    public Channel<byte[]> Subscribe(int capacity = 10)
    {
        var ch = Channel.CreateBounded<byte[]>(new BoundedChannelOptions(capacity)
        {
            SingleReader = false,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest
        });
        lock (_sync)
        {
            _subscribers.Add(ch);
        }
        return ch;
    }

    public void Unsubscribe(Channel<byte[]> channel)
    {
        lock (_sync)
        {
            _subscribers.Remove(channel);
        }
        channel.Writer.TryComplete();
    }

    public void Broadcast(byte[] frame)
    {
        Channel<byte[]>[] subs;
        lock (_sync)
        {
            subs = _subscribers.ToArray();
        }

        foreach (var ch in subs)
        {
            // Best-effort non-blocking delivery; drop if subscriber is slow
            ch.Writer.TryWrite(frame);
        }
    }
}

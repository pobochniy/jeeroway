using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Jeeroway.Api;

public class UdpVideoReceiverService : BackgroundService
{
    private readonly Channel<byte[]> _frameChannel;
    private readonly UdpClient _udpClient;
    private readonly RecordingSessionManager _recorder;
    private readonly LiveFrameBroadcaster _broadcaster;
    private readonly ILogger<UdpVideoReceiverService> _logger;

    private readonly ConcurrentDictionary<IPAddress, List<byte>> _buffersByIp = new();
    private readonly ConcurrentDictionary<IPAddress, Guid> _roboIdByIp = new();

    private static readonly byte[] JpegStart = { 0xFF, 0xD8 };
    private static readonly byte[] JpegEnd   = { 0xFF, 0xD9 };

    public UdpVideoReceiverService(
        Channel<byte[]> frameChannel, 
        RecordingSessionManager recorder, 
        LiveFrameBroadcaster broadcaster,
        ILogger<UdpVideoReceiverService> logger)
    {
        _frameChannel = frameChannel;
        _udpClient = new UdpClient(5000);
        _recorder = recorder;
        _broadcaster = broadcaster;
        _logger = logger;
    }

    public void RegisterRobot(IPAddress ipAddress, Guid roboId)
    {
        _roboIdByIp[ipAddress] = roboId;
        _logger.LogInformation("Registered robot {RoboId} from IP {IpAddress}", roboId, ipAddress);
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var result = await _udpClient.ReceiveAsync(ct);
            var data = result.Buffer;
            var senderIp = result.RemoteEndPoint.Address;

            // Получаем или создаем буфер для данного IP
            var buffer = _buffersByIp.GetOrAdd(senderIp, _ => new List<byte>());

            List<byte[]> frames = new();
            
            lock (buffer)
            {
                buffer.AddRange(data);

                // Ищем кадры
                while (true)
                {
                    int startIdx = IndexOf(buffer, JpegStart);
                    if (startIdx == -1) break;

                    int endIdx = IndexOf(buffer, JpegEnd, startIdx + 2);
                    if (endIdx == -1) break;

                    // Вырезаем кадр
                    int length = endIdx + 2 - startIdx;
                    var frame = buffer.Skip(startIdx).Take(length).ToArray();
                    frames.Add(frame);

                    // Удаляем из буфера использованные байты
                    buffer.RemoveRange(0, startIdx + length);
                }
            }

            // Обрабатываем кадры вне lock
            foreach (var frame in frames)
            {
                // Отправляем в канал
                await _frameChannel.Writer.WriteAsync(frame, ct);

                // Определяем roboId по IP адресу
                if (_roboIdByIp.TryGetValue(senderIp, out var roboId))
                {
                    // Сохраняем кадр
                    _recorder.SaveFrame(frame);

                    // Транслируем в live подписчиков конкретного робота
                    _broadcaster.Broadcast(roboId, frame);
                }
                else
                {
                    _logger.LogWarning("Received frame from unknown robot IP: {IpAddress}", senderIp);
                }
            }
        }
    }

    private static int IndexOf(List<byte> buffer, byte[] pattern, int startIndex = 0)
    {
        for (int i = startIndex; i <= buffer.Count - pattern.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (buffer[i + j] != pattern[j]) { match = false; break; }
            }
            if (match) return i;
        }
        return -1;
    }
}

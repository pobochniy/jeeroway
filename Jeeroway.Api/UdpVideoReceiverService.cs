using System.Net.Sockets;
using System.Threading.Channels;

namespace Jeeroway.Api;

public class UdpVideoReceiverService : BackgroundService
{
    private readonly Channel<byte[]> _frameChannel;
    private readonly UdpClient _udpClient;
    private readonly RecordingSessionManager _recorder;

    private List<byte> _buffer = new();

    private static readonly byte[] JpegStart = { 0xFF, 0xD8 };
    private static readonly byte[] JpegEnd   = { 0xFF, 0xD9 };

    public UdpVideoReceiverService(Channel<byte[]> frameChannel, RecordingSessionManager recorder)
    {
        _frameChannel = frameChannel;
        _udpClient = new UdpClient(5000);
        _recorder = recorder;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var result = await _udpClient.ReceiveAsync(ct);
            var data = result.Buffer;

            // Добавляем полученные данные в буфер
            _buffer.AddRange(data);

            // Ищем кадры
            while (true)
            {
                int startIdx = IndexOf(_buffer, JpegStart);
                if (startIdx == -1) break;

                int endIdx = IndexOf(_buffer, JpegEnd, startIdx + 2);
                if (endIdx == -1) break;

                // Вырезаем кадр
                int length = endIdx + 2 - startIdx;
                var frame = _buffer.Skip(startIdx).Take(length).ToArray();

                // Отправляем в канал
                await _frameChannel.Writer.WriteAsync(frame, ct);

                // Сохраняем кадр
                _recorder.SaveFrame(frame);

                // Удаляем из буфера использованные байты
                _buffer.RemoveRange(0, startIdx + length);
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

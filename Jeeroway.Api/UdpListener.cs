using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Jeeroway.Api;

public class UdpListener : BackgroundService
{
    public static string Data = "";

    // private readonly IPAddress _host = IPAddress.Parse("0.0.0.0");
    // private const int ListenPort = 5175;
    // private static readonly IPAddress HostServer = IPAddress.Parse("192.168.1.33");
    // private const int ClientPort = 5176;
    private const int ServerPort = 5175;
    // private readonly IPEndPoint _clientEndpoint = new (HostServer, ClientPort);
    // private readonly IPEndPoint _serverEndpoint = new (HostServer, ServerPort);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udpClient = new UdpClient(ServerPort);
        Console.WriteLine("UDP-сервер запущен...");

        while (!stoppingToken.IsCancellationRequested)
        {
            Console.WriteLine("Waiting for broadcast");
            var res = await udpClient.ReceiveAsync(default);
            if (res.Buffer.Length == 0)
            {
                await Task.Delay(500, stoppingToken);
                continue;
            }

            await File.WriteAllBytesAsync($"/Users/viktorpobochniy/webcam/{DateTime.UtcNow.Ticks}.jpg", res.Buffer,
                stoppingToken);
        }
    }
}
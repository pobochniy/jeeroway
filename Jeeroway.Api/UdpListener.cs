using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Jeeroway.Api;

public class UdpListener : BackgroundService
{
    public static string Data = "";
    // private readonly IPAddress _host = IPAddress.Parse("0.0.0.0");
    // private const int ListenPort = 5175;
    private static readonly IPAddress HostServer = IPAddress.Parse("192.168.0.10");
    private static readonly IPAddress HostClient = IPAddress.Parse("192.168.0.18");
    private const int ClientPort = 5176;
    private const int ServerPort = 5175;
    private readonly IPEndPoint _clientEndpoint = new (HostClient, ClientPort);
    private readonly IPEndPoint _serverEndpoint = new (HostServer, ServerPort);
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // var groupEndpoint = new IPEndPoint(IPAddress.Any, ListenPort);
        // var groupEndpoint = new IPEndPoint(IPAddress.Any, ListenPort);
        // var listener = new UdpClient(groupEndpoint);
        using var udpClient = new UdpClient(_serverEndpoint);
        udpClient.Connect(_clientEndpoint);
        // listener.ExclusiveAddressUse = false;
        // listener.Client.Bind(groupEndpoint);
        // await listener.Client.ConnectAsync(groupEndpoint, stoppingToken);
        // listener.Connect(_host, ListenPort);
        // var listener = new UdpClient(new IPEndPoint(_host, ListenPort));
        // var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
 
        // try
        // {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Waiting for broadcast");
                var bytes = await udpClient.ReceiveAsync(default);
                if (bytes.Buffer.Length == 0)
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }
                
                // Console.WriteLine($"Received broadcast from {groupEndpoint} :");
                var res = $" {Encoding.UTF8.GetString(bytes.Buffer)}";
                Data = res;
                Console.WriteLine(res);
 
            }
        // }
        // catch (OperationCanceledException)
        // {
        // }
        // finally
        // {
        //     listener.Close();
        //     listener.Dispose();
        // }
    }
}
using System.Net;
using System.Net.Sockets;
using System.Text;
 
Console.WriteLine("Hello, Robo!");
 
var hostServer = IPAddress.Parse("192.168.0.10");
var hostClient = IPAddress.Parse("192.168.0.18");
const int clientPort = 5176;
const int serverPort = 5175;
var clientEndpoint = new IPEndPoint(hostClient, clientPort);
var serverEndpoint = new IPEndPoint(hostServer, serverPort);
using var udpClient = new UdpClient(clientEndpoint);
udpClient.Connect(serverEndpoint);
while (true)
{
    var input = Console.ReadLine();
    if (!string.IsNullOrWhiteSpace(input))
    {
        var data = Encoding.UTF8.GetBytes(input);
        await udpClient.SendAsync(data, CancellationToken.None);
    }
}
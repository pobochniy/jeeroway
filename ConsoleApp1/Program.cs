using System.Net;
using System.Net.Sockets;
using System.Drawing;
using AForge.Video;
using AForge.Video.DirectShow;

class Program
{
    
    // IPAddress hostServer = ;
// var hostClient = IPAddress.Parse("192.168.0.18");
    const int clientPort = 5176;
    const int serverPort = 5175;
// var clientEndpoint = new IPEndPoint(hostClient, clientPort);
    static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("192.168.1.33"), serverPort);
    static readonly UdpClient UdpClient = new UdpClient();
        
    public static void Main(string[] args)
    {
        
        Console.WriteLine("Hello, Robo!");
        
        
        using var udpClient = new UdpClient();
        udpClient.Connect(serverEndpoint);

        FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
        videoSource.NewFrame += VideoSource_NewFrame;
        videoSource.Start();


        Console.ReadLine();
    }


    private static async void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
    {
        var bmp = new Bitmap(eventArgs.Frame, 800, 600);
        try
        {
            using var ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            var bytes = ms.ToArray();
            await UdpClient.SendAsync(bytes, bytes.Length, default);
        }
        catch (Exception ex)
        {
            // ignored
        }
    }
}
 
//
// while (true)
// {
//     var input = Console.ReadLine();
//     if (!string.IsNullOrWhiteSpace(input))
//     {
//         var data = Encoding.UTF8.GetBytes(input);
//         Console.WriteLine("try sending data");
//         await udpClient.SendAsync(data, CancellationToken.None);
//     }
// }

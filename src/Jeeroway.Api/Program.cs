using System.Threading.Channels;
using Jeeroway.Api;
using Jeeroway.Api.configs;
using Jeeroway.Api.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Настройка URL для прослушивания на всех интерфейсах
builder.WebHost.UseUrls("http://192.168.1.33:5000");

var frameChannel = Channel.CreateUnbounded<byte[]>();

builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddHostedService<UdpListener>();

// Регистрируем UdpVideoReceiverService как singleton и как HostedService
builder.Services.AddSingleton<UdpVideoReceiverService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<UdpVideoReceiverService>());

builder.Services.AddSingleton(frameChannel);
builder.Services.AddSingleton<RecordingSessionManager>();
builder.Services.AddSingleton<LiveFrameBroadcaster>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<RoboHub>("hub/robocontrol");
app.MapHub<ChatHub>("hub/chat");
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();

await app.RunAsync();
using System.Threading.Channels;
using Jeeroway.Api;
using Jeeroway.Api.configs;
using Jeeroway.Api.SignalR;

var builder = WebApplication.CreateBuilder(args);
var frameChannel = Channel.CreateUnbounded<byte[]>();

builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddHostedService<UdpListener>();
builder.Services.AddHostedService<UdpVideoReceiverService>();
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
app.MapHub<RoboHub>("/robocontrol");
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
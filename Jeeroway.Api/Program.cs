using System.Threading.Channels;
using Jeeroway.Api;

var builder = WebApplication.CreateBuilder(args);
var frameChannel = Channel.CreateUnbounded<byte[]>();

builder.Services.AddHostedService<UdpListener>();
builder.Services.AddHostedService<UdpVideoReceiverService>();
builder.Services.AddSingleton(frameChannel);
builder.Services.AddSingleton<RecordingSessionManager>();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
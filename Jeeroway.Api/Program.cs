using Jeeroway.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<UdpListener>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
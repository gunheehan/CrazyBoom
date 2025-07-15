using System.Net;
using System.Net.WebSockets;
using System.Text;
using CrazyBombChat.Connection;
using CrazyBombChat.Services;

var builder = WebApplication.CreateBuilder();
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5267);
});

var app = builder.Build();

var chatManager = new ChatManager();

app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
        var connection = new ClientConnection(socket, chatManager);
        await connection.ProcessAsync();
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

Console.WriteLine("🚀 Chat server running on ws://localhost:5267/ws");
app.Run();
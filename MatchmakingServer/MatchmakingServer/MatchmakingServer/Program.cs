using MatchmakingServer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RoomService>();
builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolver = MyJsonContext.Default;
    });

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(7800); // 모든 IP에서 7800 포트로 리스닝
});

var app = builder.Build();

app.MapControllers();

app.Run();
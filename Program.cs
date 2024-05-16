using ServerApp;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;


/*var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls( "http://localhost:6969" );
var app = builder.Build();
app.UseWebSockets();
app.Map( "/ws", async context => {
    if ( context.WebSockets.IsWebSocketRequest ) {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        while ( true ) {
            var message = "The current time is: " + DateTime.Now.ToString("HH:mm:ss");
            var bytes = Encoding.UTF8.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            if ( ws.State == WebSocketState.Open )
                await ws.SendAsync( arraySegment,
                                    WebSocketMessageType.Text,
                                    true,
                                    CancellationToken.None );
            else if ( ws.State == WebSocketState.Closed || ws.State == WebSocketState.Aborted ) {
                break;
            }
            Thread.Sleep( 1000 );

        }
    }
    else {
        context.Response.StatusCode = 400;
    }
} );
await app.RunAsync();*/

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<StateWebSocketManager>();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls( "http://localhost:6969" );

var app = builder.Build();

if ( app.Environment.IsDevelopment() ) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

var webSocketOptions = new WebSocketOptions {
    KeepAliveInterval = TimeSpan.FromMinutes(2) // Set the keep-alive interval to 2 minutes
};

app.UseWebSockets( webSocketOptions );
app.Map( "/ws", async ( context ) => {
    if ( context.WebSockets.IsWebSocketRequest ) {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var socketFinishedTcs = new TaskCompletionSource<object>();
        var clientId = Guid.NewGuid().ToString();

        var webSocketManager = context.RequestServices.GetRequiredService<StateWebSocketManager>();
        webSocketManager.AddSocket( clientId, webSocket, socketFinishedTcs );

        await socketFinishedTcs.Task;
    }
    else {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
} );

// Configure the HTTP request pipeline.
//app.UseHttpsRedirection();
//app.UseAuthorization();
app.MapControllers();

app.Run();

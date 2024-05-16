using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace ServerApp {

    public class StateWebSocketManager {
        private readonly ConcurrentDictionary<string, (WebSocket Socket, TaskCompletionSource<object> Tcs)> _sockets = new ConcurrentDictionary<string, (WebSocket, TaskCompletionSource<object>)>();

        public void AddSocket( string clientId, WebSocket socket, TaskCompletionSource<object> tcs ) {
            Console.WriteLine( "Adding socket with id:" + clientId );
            _sockets[clientId] = (socket, tcs);
        }

        public async Task SendMessageAsync( string clientId, string message ) {
            if ( _sockets.TryGetValue( clientId, out var socketInfo ) ) {
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);
                await socketInfo.Socket.SendAsync( segment, WebSocketMessageType.Text, true, CancellationToken.None );
            }
        }

        public async Task RemoveSocket( string clientId ) {
            if ( _sockets.TryRemove( clientId, out var socketInfo ) ) {
                await socketInfo.Socket.CloseAsync( WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None );
                socketInfo.Tcs.TrySetResult( null );
            }
        }
    };


    public static class StateLisenters {
        private static List<WebSocket> webSockets = new List<WebSocket>();
        public static async void AddWebSocketListeners( HttpContext context ) {
            if ( context.WebSockets.IsWebSocketRequest ) {
                Console.WriteLine( "WebSocket request received!" );
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                webSockets.Add( webSocket );
                //await HandleWebSocketConnection( webSocket );
            }
            else {
                Console.WriteLine( "Invalid WebSocket request!" );
                context.Response.StatusCode = 400;
            }
        }

        public static void SendToAllListeners() {
            foreach ( var webSocket in webSockets ) {
                if ( webSocket.State == WebSocketState.Open ) {
                    var responseMessage = "Hello from the server!";
                    var responseBytes = Encoding.UTF8.GetBytes( responseMessage );
                    webSocket.SendAsync( new ArraySegment<byte>( responseBytes, 0, responseBytes.Length ), WebSocketMessageType.Text, true, CancellationToken.None );
                }
            }
        }

        private static async Task HandleWebSocketConnection( WebSocket webSocket ) {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            while ( !result.CloseStatus.HasValue ) {
                var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine( $"Received: {receivedMessage}" );
                result = await webSocket.ReceiveAsync( new ArraySegment<byte>( buffer ), CancellationToken.None );
            }
            webSockets.Remove( webSocket );
            await webSocket.CloseAsync( result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None );
        }
    }
}

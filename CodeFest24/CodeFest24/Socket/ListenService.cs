using CodeFest24.Model;
using Quobject.SocketIoClientDotNet.Client;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace CodeFest24.Socket
{
    public class ListenService
    {
        private static RegisterPlayerDto _player = new();
        private static string server = string.Empty;
        private static Quobject.SocketIoClientDotNet.Client.Socket socketClient;
        private static void InitSocket(string url)
        {
            socketClient = IO.Socket(url);

        }

        private static void JoinGame()
        {
            // Emitting the 'join game' event
            socketClient.Emit("join game", _player);

            // Listening for the 'join game' event
            socketClient.On("join game", (data) =>
            {
                // Your callback logic here
                Console.WriteLine("Joined game with data: " + data);
            });
            //	Return a ticktack every time game updates an event		

            socketClient.On("ticktack player", (data) =>
            {

            });
            
            socketClient.Connect();
        }



    static void Reconnect()
    {
        Console.WriteLine("Attempting to reconnect...");
        socketClient.Connect();
    }

    public static async Task Main()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine("Server started at http://localhost:5000/");

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
                    WebSocket webSocket = wsContext.WebSocket;

                    await HandleWebSocketConnection(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }

        private static async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            byte[] buffer = new byte[1024];
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Received: " + message);

                string response = "Echo: " + message;
                byte[] responseBuffer = Encoding.UTF8.GetBytes(response);
                await webSocket.SendAsync(new ArraySegment<byte>(responseBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public static void SetPlayer(string gameId, string playerId)
        {
            _player = new()
            {
                PlayerId = playerId,
                GameId = gameId
            };
        }

        private static void JoinGame()
        {

        }
    }
}


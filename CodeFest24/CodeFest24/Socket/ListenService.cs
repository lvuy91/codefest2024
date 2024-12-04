using CodeFest24.Model;
using CodeFest24.Services;
using EngineIOSharp.Common.Enum;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using SocketIOSharp.Common;
using System.Text.Json;

namespace CodeFest24.SocketService
{
    public class ListenService
    {
        private static RegisterPlayerDto _player = new();
        private static string server = string.Empty;
        //private static Quobject.SocketIoClientDotNet.Client.Socket socket;

        private static SocketIOClient socket;// = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, "localhost", 80));
        private static Main service = new Main();

        private static void InitEventHandlers(SocketIOClient client)
        {
            client.On(SocketIOEvent.CONNECTION, () =>
            {
                Console.WriteLine("Connected!");
                var message = JsonSerializer.Serialize(_player);
                Console.WriteLine(message);
                client.Emit("join game", message);
            });

            client.On(SocketIOEvent.DISCONNECT, () =>
            {
                Console.WriteLine();
                Console.WriteLine("Disconnected!");
            });

            client.On("ticktack player", (Data) =>
            {
                Console.WriteLine("Echo : " + (Data[0].Type == JTokenType.Bytes ? BitConverter.ToString(Data[0].ToObject<byte[]>()) : Data[0]));
            });

            client.On("join game", (Data) =>
            {
                Console.WriteLine("Echo1 : " + Data[0]);
                Console.WriteLine("Echo2 : " + Data[1]);
            });

            //// Listening for the 'join game' event
            //client.On("join game", (Data) =>
            //{
            //    // Your callback logic here
            //    Console.WriteLine("Joined game with data: " + Data);
            //});
            ////	Return a ticktack every time game updates an event		

            //client.On("ticktack player", (Data) =>
            //{
            //    //service.ParseTicktack((TicktackResponse)data);
            //});
        }
        public static void InitSocket(string url = "localhost", ushort port = 80)
        {
            socket = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, url, port));
            InitEventHandlers(socket);

            //// Listening for the 'join game' event
            //socket.On("join game", (data) =>
            //{
            //    // Your callback logic here
            //    Console.WriteLine("Joined game with data: " + data);
            //});
            ////	Return a ticktack every time game updates an event		

            //socket.On("ticktack player", (data) =>
            //{
            //    //service.ParseTicktack((TicktackResponse)data);
            //});

            //// Emitting the 'join game' event

            //socket.On(Socket.EVENT_CONNECT, () =>
            //{
            //    Console.WriteLine("on connect");
            //    var message = JsonSerializer.Serialize(_player);
            //    Console.WriteLine(message);
            //    var em = socket.Emit("join game", message);

            //    em.On(Socket.EVENT_CONNECT_ERROR, (a) =>
            //    {
            //        Console.WriteLine("Error");
            //        Console.WriteLine(a);
            //    });

            //    em.On(Socket.EVENT_CONNECT, (a) =>
            //    {
            //        Console.WriteLine("Connect");
            //        Console.WriteLine(a);
            //    });
            //});
            socket.Connect();
        }

        private static void JoinGame()
        {

        }



        static void Reconnect()
        {
            Console.WriteLine("Attempting to reconnect...");
            socket.Connect();
        }

        public static void Main()
        {
            JoinGame();
        }

        public static void SetPlayer(string gameId, string playerId)
        {
            _player = new()
            {
                PlayerId = playerId,
                GameId = gameId
            };
        }

    }
}


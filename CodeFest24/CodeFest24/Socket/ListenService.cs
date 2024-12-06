using CodeFest24.Model;
using CodeFest24.Services;

namespace CodeFest24.SocketService
{
    public class ListenService
    {
        private static RegisterPlayerDto _player = new();
        private static UserDto _user = new();
        private static string server = string.Empty;
        //private static Quobject.SocketIoClientDotNet.Client.Socket socket;

        private static SocketIOClient.SocketIO socket;// = new SocketIOClient(new SocketIOClientOption(EngineIOScheme.http, "localhost", 80));
        private static Main service = new Main();

        private static void InitService(string playerId, string gameId, SocketIOClient.SocketIO socket)
        {
            service.SetPlayerId(playerId);
            service.SetSocket(socket);
        }
        private static async Task InitEventHandlers(SocketIOClient.SocketIO socket)
        {
            Console.WriteLine("Start");

            socket.On("join game", response =>
            {
                // You can print the returned data first to decide what to do next.
                // output: ["hi client"]
                Console.WriteLine(response);

                //   string text = response.GetValue<string>();


                // The socket.io server code looks like this:
                // socket.emit('hi', 'hi client');
            });

            socket.On("ticktack player", response =>
            {
                // You can print the returned data first to decide what to do next.
                // output: ["ok",{"id":1,"name":"tom"}]
                Console.WriteLine(response);
                //service.ParseTicktack((TicktackResponse)response.);

                // Get the first data in the response
                //string text = response.GetValue<string>();
                // Get the second data in the response
                var dto = response.GetValue<TicktackResponse>(0);
                service.ParseTicktack(dto);

                // The socket.io server code looks like this:
                // socket.emit('hi', 'ok', { id: 1, name: 'tom'});
            });

            socket.OnConnected += async (sender, e) =>
            {
                Console.WriteLine("On Connect");
                // Emit a string
                await socket.EmitAsync("join game", _player);
                await socket.EmitAsync("register character power", _user);
                Console.WriteLine("End emit");

                // Emit a string and an object
                //var dto = new TestDTO { Id = 123, Name = "bob" };
                //await socket.EmitAsync("register", "source", dto);
            };

            socket.OnError += async (sender, e) =>
            {
                Console.WriteLine("On Error");
                // Emit a string

                // Emit a string and an object
                //var dto = new TestDTO { Id = 123, Name = "bob" };
                //await socket.EmitAsync("register", "source", dto);
            };


            socket.OnDisconnected += async (sender, e) =>
            {
                Console.WriteLine("On Disconect");
                // Emit a string
            };
            await socket.ConnectAsync();
        }


        public static async Task InitSocket(string url = "localhost:81")
        {
            socket = new SocketIOClient.SocketIO(url);
            await InitEventHandlers(socket);
        }

        public static void SetPlayer(string gameId, string playerId, string type)
        {
            _player = new()
            {
                PlayerId = playerId,
                GameId = gameId
            };
            _user = new UserDto()
            {
                gameId = gameId,
                playerId = playerId,
                type = type
            };
        }


    }
}


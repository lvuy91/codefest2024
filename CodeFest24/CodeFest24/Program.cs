// See https://aka.ms/new-console-template for more information
using CodeFest24.Services;
using CodeFest24.SocketService;

Console.WriteLine("Hello, World!");
ListenService.SetPlayer("07c00a15-ac7b-424f-bc43-d9f457052b51", "player2-xxx");
Main.SetMapWidth(33); //35 for trial, 55 for gamw
ListenService.InitSocket();
ListenService.Main();

//var socket = IO.Socket("http://34.142.149.116:8824");

//socket.On("connect", () =>
//{
//    Console.WriteLine("Connected to server");

//    // Emit an event to the server
//    socket.Emit("clientEvent", "Hello from client!");
//});

//socket.On("disconnect", () =>
//{
//    Console.WriteLine("Disconnected from server");
//});

//socket.On("serverEvent", (data) =>
//{
//    Console.WriteLine("Received event from server: " + data);
//});
//socket.Connect();
var key = Console.ReadLine();


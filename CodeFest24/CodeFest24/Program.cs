using CodeFest24.Services;
using CodeFest24.SocketService;

Console.WriteLine("Hello, World!");
ListenService.SetPlayer("bb72c8a8-930d-4ec3-9bea-38fa4848ed0c", "player2-xxx", "2");
Main.SetMapWidth(35); //35 for trial, 55 for gamw
await ListenService.InitSocket("http://localhost:81");
Console.ReadLine();


// See https://aka.ms/new-console-template for more information
using CodeFest24.Services;
using CodeFest24.Socket;

Console.WriteLine("Hello, World!");
ListenService.SetPlayer("", "");
Main.SetMapWidth(55); //35 for trial, 55 for gamw
await ListenService.Main();

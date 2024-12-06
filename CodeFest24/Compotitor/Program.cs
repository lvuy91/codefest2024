using Compotitor.Services;
using System;
using System.Threading.Tasks;

namespace Compotitor
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            Console.WriteLine("Hello, World!");
            ListenService.SetPlayer("bb72c8a8-930d-4ec3-9bea-38fa4848ed0c", "player1-xxx", "1");
            CompotitorService.SetMapWidth(35); //35 for trial, 55 for gamw
            await ListenService.InitSocket("http://localhost:81");
            Console.ReadLine();
        }
    }
}

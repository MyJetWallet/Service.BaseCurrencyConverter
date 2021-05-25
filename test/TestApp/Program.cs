using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.BaseCurrencyConverter.Client;
using Service.BaseCurrencyConverter.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}

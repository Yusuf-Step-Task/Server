// Client
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
public class Program
{

    static void Main(string[] args)
    {
        var client = new TcpClient("192.168.56.1", 1234);
        var serverStream = client.GetStream();
        var br = new BinaryReader(serverStream);
        var bw = new BinaryWriter(serverStream);
        Console.ReadKey();
        while (true)
        {
            Console.Write("Enter Process name and program name : ");

            string input = Console.ReadLine() ?? "";
            string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            bw.Write(input);
            bw.Flush();

            Console.WriteLine($"process Name: {words[0]}");
            Console.WriteLine($"program Name: {words[1]}");
            Console.Write("Emeliyyatlar yerine yetirilsin ? \nyes | no Choise: ");
            string Choise = Console.ReadLine() ?? "yes";
            if (Choise == "yes")
            {
                Console.WriteLine(" ");
                break;
            }
            else if (Choise == "no")
            {
                continue;
            }
        }
        int count = br.ReadInt32();
        Console.WriteLine( $"{"ID",-10} {"Process Name",-30}" );
        for (int i = 0; i < count; i++)
        {
            int id = br.ReadInt32();
            string name = br.ReadString();
            Console.WriteLine($"{id,-10} {name,-30}");
        }

    }
}
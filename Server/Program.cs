using Shared;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
namespace Server;
class Program
{
    static void Main()
    {
        var clients = new HashSet<TcpClient>();
        var ip = IPAddress.Parse("192.168.56.1");
        var port = 1234;
        var tcpListener = new TcpListener(ip, port);
        tcpListener.Start(10);
        while (true)
        {
            Console.WriteLine($"{tcpListener.Server.LocalEndPoint} listening.....");

            var client = tcpListener.AcceptTcpClient();

            Console.WriteLine($"{client.Client.RemoteEndPoint} connected server....");
            clients.Add(client);

            Task.Run(() =>
            {
                var clientStream = client.GetStream();
                var br = new BinaryReader(clientStream);
                var readString = string.Empty;
                var userIpEndPoint = string.Empty;
                var index = 0;
                Process process = new Process();
                
                string input = br.ReadString();

                string[] words = input.Split(' ' , StringSplitOptions.RemoveEmptyEntries);
                string processName = words[0];
                string programName = words[1];
                
                //----------------------------------------
                while (true)
                {
                    Console.WriteLine($"{clientStream.Socket.RemoteEndPoint} - {readString}");
                    if (processName == Command.KILL)
                    {
                        Process[] processes = Process.GetProcessesByName(programName);
                        if (processes.Length == 0)
                        {
                            Console.WriteLine($"Error:{programName} Bele process yoxdur !!!");
                        }
                        foreach (Process p in processes)
                        {
                            try
                            {
                                p.Kill();
                                p.WaitForExit();
                                Console.WriteLine($"{p.ProcessName} killed succesfuly");
                                break;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                                continue;
                            }
                            finally
                            {
                                p.Dispose();
                            }
                        }
                    }
                    else if (processName == Command.RUN)
                    {
                        process.StartInfo.FileName = programName;
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                        Console.WriteLine($"Process started \nId: {process.Id}");
                    }
                    else if (processName == Command.PROCLIST)
                    {
                        Process[] processes = Process.GetProcesses();
                        using var bw = new BinaryWriter( clientStream , Encoding.UTF8 , leaveOpen: true );
                        bw.Write(processes.Length);
                        foreach (Process p in processes)
                        {
                            try
                            {
                                bw.Write(p.Id);
                                bw.Write(p.ProcessName);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error: {ex.Message}");
                            }
                            finally
                            {
                                p.Dispose();
                            }
                        }
                        bw.Flush();
                        Console.WriteLine("Process list sent to client");
                        break;
                    }
                }
            });
            Console.ReadKey();
            //---------------------------------------------
        }

    }
}
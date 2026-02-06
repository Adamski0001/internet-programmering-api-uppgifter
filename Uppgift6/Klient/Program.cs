using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private const int portNum = 5002;
    private const string hostName = "localhost";

    public static int Main(string[] args)
    {
        string host = args.Length >= 1 ? args[0] : hostName;
        int port = portNum;
        if (args.Length >= 2 && int.TryParse(args[1], out int parsedPort))
        {
            port = parsedPort;
        }

        Console.Write("Namn: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            name = "Client";
        }

        try
        {
            var client = new TcpClient(host, port);
            client.NoDelay = true;
            NetworkStream ns = client.GetStream();
            var reader = new StreamReader(ns, Encoding.UTF8);
            var writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };

            writer.WriteLine(name);

            var cts = new CancellationTokenSource();
            var readTask = Task.Run(() =>
            {
                try
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    cts.Cancel();
                }
            });

            Console.WriteLine("Skriv meddelanden. Skriv /quit för att avsluta.");

            while (!cts.IsCancellationRequested)
            {
                string? line = Console.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (line.Equals("/quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                writer.WriteLine(line);
            }

            try
            {
                client.Close();
            }
            catch
            {
            }

            readTask.Wait();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return 0;
    }
}

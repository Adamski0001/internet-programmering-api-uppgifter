using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    private const int portNum = 13;

    public static int Main(string[] args)
    {
        var listener = new TcpListener(IPAddress.Any, portNum);
        listener.Start();

        var clients = new List<TcpClient>();
        var clientsLock = new object();
        var cts = new CancellationTokenSource();

        Task.Run(() =>
        {
            while (!cts.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    lock (clientsLock)
                    {
                        clients.Add(client);
                    }

                    Console.WriteLine("Client connected.");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        });

        Console.WriteLine("Type a message and press Enter to broadcast. Type /quit to stop.");
        while (true)
        {
            string? message = Console.ReadLine();
            if (message == null)
            {
                continue;
            }

            if (message.Equals("/quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            byte[] payload = Encoding.UTF8.GetBytes(message + Environment.NewLine);
            List<TcpClient> toRemove = new List<TcpClient>();

            lock (clientsLock)
            {
                foreach (TcpClient client in clients)
                {
                    try
                    {
                        NetworkStream ns = client.GetStream();
                        ns.Write(payload, 0, payload.Length);
                    }
                    catch
                    {
                        toRemove.Add(client);
                    }
                }

                foreach (TcpClient client in toRemove)
                {
                    clients.Remove(client);
                    try
                    {
                        client.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }

        cts.Cancel();
        listener.Stop();
        return 0;
    }
}

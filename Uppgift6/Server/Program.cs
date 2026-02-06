using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private const int portNum = 5002;
    private static int nextId = 0;

    private sealed class ClientState
    {
        public ClientState(TcpClient client, StreamWriter writer, string name)
        {
            Client = client;
            Writer = writer;
            Name = name;
        }

        public TcpClient Client { get; }
        public StreamWriter Writer { get; }
        public string Name { get; }
    }

    public static int Main(string[] args)
    {
        var listener = new TcpListener(IPAddress.Any, portNum);
        listener.Start();

        var clients = new List<ClientState>();
        var clientsLock = new object();
        var broadcastLock = new object();

        Console.WriteLine($"Chat server kör på port {portNum}.");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            client.NoDelay = true;

            ThreadPool.QueueUserWorkItem(_ =>
                HandleClient(client, clients, clientsLock, broadcastLock));
        }
    }

    private static void HandleClient(
        TcpClient client,
        List<ClientState> clients,
        object clientsLock,
        object broadcastLock)
    {
        ClientState? state = null;

        try
        {
            NetworkStream ns = client.GetStream();
            var reader = new StreamReader(ns, Encoding.UTF8);
            var writer = new StreamWriter(ns, Encoding.UTF8) { AutoFlush = true };

            string? name = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                name = $"Client{Interlocked.Increment(ref nextId)}";
            }

            state = new ClientState(client, writer, name);

            lock (clientsLock)
            {
                clients.Add(state);
            }

            Broadcast($"{name} anslöt.", clients, clientsLock, broadcastLock);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                Broadcast($"{name}: {line}", clients, clientsLock, broadcastLock);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            if (state != null)
            {
                lock (clientsLock)
                {
                    clients.Remove(state);
                }

                Broadcast($"{state.Name} lämnade.", clients, clientsLock, broadcastLock);
            }

            try
            {
                client.Close();
            }
            catch
            {
            }
        }
    }

    private static void Broadcast(
        string message,
        List<ClientState> clients,
        object clientsLock,
        object broadcastLock)
    {
        List<ClientState> snapshot;
        lock (clientsLock)
        {
            snapshot = new List<ClientState>(clients);
        }

        List<ClientState> dead = new List<ClientState>();

        lock (broadcastLock)
        {
            foreach (ClientState c in snapshot)
            {
                try
                {
                    c.Writer.WriteLine(message);
                }
                catch
                {
                    dead.Add(c);
                }
            }
        }

        if (dead.Count > 0)
        {
            lock (clientsLock)
            {
                foreach (ClientState c in dead)
                {
                    clients.Remove(c);
                    try
                    {
                        c.Client.Close();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}

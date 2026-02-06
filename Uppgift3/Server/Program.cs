using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private const int portNum = 5001;

    public static int Main(string[] args)
    {
        bool done = false;

        string message;
        if (args.Length > 0)
        {
            message = string.Join(' ', args);
        }
        else
        {
            Console.Write("Skriv meddelande som ska skickas: ");
            message = Console.ReadLine() ?? "";
        }

        var listener = new TcpListener(IPAddress.Any, portNum);
        listener.Start();

        while (!done)
        {
            Console.Write("Waiting for connection...");
            TcpClient client = listener.AcceptTcpClient();

            Console.WriteLine("Connection accepted.");
            NetworkStream ns = client.GetStream();

            byte[] payload = Encoding.ASCII.GetBytes(message);

            try
            {
                ns.Write(payload, 0, payload.Length);
                ns.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        listener.Stop();
        return 0;
    }
}

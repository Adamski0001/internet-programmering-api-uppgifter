using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    private const int portNum = 13;

    public static int Main(string[] args)
    {
        bool done = false;

        var listener = new TcpListener(IPAddress.Any, portNum);
        listener.Start();

        while (!done)
        {
            Console.Write("Waiting for connection...");
            TcpClient client = listener.AcceptTcpClient();
            client.NoDelay = true;

            Console.WriteLine("Connection accepted.");
            NetworkStream ns = client.GetStream();

            try
            {
                for (int i = 0; i <= 9; i++)
                {
                    byte[] digit = Encoding.ASCII.GetBytes(i.ToString());
                    ns.Write(digit, 0, digit.Length);
                    ns.Flush();
                    Thread.Sleep(50);
                }
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

using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    private const int portNum = 5001;
    private const string hostName = "localhost";

    public static int Main(string[] args)
    {
        try
        {
            string host = args.Length >= 1 ? args[0] : hostName;
            int port = portNum;
            if (args.Length >= 2 && int.TryParse(args[1], out int parsedPort))
            {
                port = parsedPort;
            }

            var client = new TcpClient(host, port);
            NetworkStream ns = client.GetStream();

            byte[] bytes = new byte[1024];
            int bytesRead = ns.Read(bytes, 0, bytes.Length);

            Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytesRead));

            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return 0;
    }
}

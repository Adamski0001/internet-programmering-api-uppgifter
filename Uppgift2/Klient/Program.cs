using System;
using System.Net.Sockets;
using System.Text;

class Program
{
    private const int portNum = 13;
    private const string hostName = "localhost";

    public static int Main(string[] args)
    {
        try
        {
            var client = new TcpClient(hostName, portNum);
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

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

            for (int i = 0; i <= 9; i++)
            {
                int value = ns.ReadByte();
                if (value == -1)
                {
                    break;
                }

                Console.WriteLine(Encoding.ASCII.GetString(new byte[] { (byte)value }));
            }

            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return 0;
    }
}

using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class UDPListener
    {
        private const int listenPort = 11000;

        private static void StartListener()
        {
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for broadcast");
                    byte[] bytes = listener.Receive(ref groupEP); // Get data from clients
                    Utilities.ParseRequest(listener, groupEP, bytes); // Parse the request according to specific code
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                listener.Close();
            }
        }

        public static void Main()
        {
            StartListener();
        }
    }
}
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;


namespace Client_Students
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connection
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("xxx.xxx.x.xxx"), 11000); // endpoint where server is listening
            client.Connect(ep);

            // We fill this object according to user answers
            StudentQuery query = new StudentQuery();

            // First Question
            Console.WriteLine("Which faculty you studied ?\n");
            var response = SendRequest(client, ep, 1);
            Console.WriteLine(response);
            query.FirstDegreeFaculty = Convert.ToInt32(Console.ReadLine()); // get user choice

            // Second Question
            Console.WriteLine("Please enter your grade in the first degree:");
            query.Grade = Convert.ToInt32(Console.ReadLine());

            // Third Question
            Console.WriteLine("Which faculty do you wish to study second degree?\n");
            Console.WriteLine(response);
            query.SecondDegree = Convert.ToInt32(Console.ReadLine()); // get user choice

            // Serialize object and send to server
            var objectAsString = JsonConvert.SerializeObject(query);
            byte[] bytes = Encoding.ASCII.GetBytes(objectAsString);
            SendRequest(client, ep, 2, bytes);
        }


        public static string SendRequest(UdpClient client, IPEndPoint ep, byte code, byte[] data = null)
        {
            byte[] sendbuf = new byte[] { code };
            if (data != null)
            {
                sendbuf = sendbuf.Concat(data).ToArray();
            }
            client.Send(sendbuf); // send data
            var receivedData = client.Receive(ref ep); // receive data
            return Encoding.UTF8.GetString(receivedData); // display data
        }
    }
}

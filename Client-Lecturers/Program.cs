using System.Net;
using System.Net.Sockets;
using System.Text;



namespace Client_Lecturers
{
    class Program
    {
        static void Main(string[] args)
        {
            // Connection 
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("xxx.xxx.x.xxx"), 11000); // endpoint where server is listening
            client.Connect(ep);

            // Main Loop
            while (true)
            {
                // First Question
                Console.WriteLine("Please select faculty: \n");
                var response = SendRequest(client, ep, 1);
                Console.WriteLine(Encoding.UTF8.GetString(response));
                var facultyNumberSelected = Convert.ToInt32(Console.ReadLine()); // get user choice

                // Second Question
                Console.WriteLine("\nWhat do you want to know about the selected faculty?\n" +
                    "1. Get the number of students applied to study second degree in this faculty\n" +
                    "2. Get students average\n" +
                    "3. Get the top 2 students grades who applied to study in this faculty\n");
                int userChoice = Convert.ToInt32(Console.ReadLine()); // get user choice

                // Send request to server according to user choice
                if(userChoice == 1)
                {
                    response = SendRequest(client, ep, 3,new byte[] {Convert.ToByte(facultyNumberSelected) });
                    Console.WriteLine("\nAnswer: " + Convert.ToInt32(Encoding.UTF8.GetString(response)) + "\n\n");
                }
                else if(userChoice == 2)
                {
                    response = SendRequest(client, ep, 4, new byte[] { Convert.ToByte(facultyNumberSelected) });
                    if(Encoding.UTF8.GetString(response) == "none")
                    {
                        Console.WriteLine("\nAnswer: faculty empty right now.\n\n");
                    }
                    else
                    {
                        Console.WriteLine("\nAnswer: " + Convert.ToDouble(Encoding.UTF8.GetString(response)) + "\n\n");
                    }
                }
                else if(userChoice == 3)
                {
                    response = SendRequest(client, ep, 5, new byte[] { Convert.ToByte(facultyNumberSelected) });
                    Console.Write("\nAnswer: ");
                    int first = Convert.ToInt32(response[0]);


                    if (first == 255)
                    {
                        Console.Write("faculty empty right now.");
                    }
                    else
                    {
                        Console.Write(first);
                        int second = Convert.ToInt32(response[1]);
                        if (second != 255)
                        {
                            Console.Write(", " + second);
                        }
                    }
                    Console.WriteLine("\n");
                }
                else
                {
                    Console.WriteLine("No such choice");
                }

            }
        }

        public static byte[] SendRequest(UdpClient client, IPEndPoint ep, byte code, byte[] data = null)
        {
            byte[] sendbuf = new byte[] { code };
            if (data != null)
            {
                sendbuf = sendbuf.Concat(data).ToArray();
            }
            client.Send(sendbuf); // send data
            var receivedData = client.Receive(ref ep); // receive data
            //return Encoding.UTF8.GetString(receivedData); // display data
            return receivedData;
        }
    }

}

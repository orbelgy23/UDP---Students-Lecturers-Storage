using Client_Students;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace Server
{
    public static class Utilities
    {
        public static List<string> faculties = new List<string>() 
        { 
            "Electrical Engineering",
            "Mechanical Engineering",
            "Computer Science",
            "Mathematics"
        };

        const string pathToFile = "./Storage.txt"; // Path to txt file


        public static string GetFaculties()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < faculties.Count; i++)
            {
                sb.Append($"{i + 1}. {faculties[i]}\n");
            }
            return sb.ToString();
        }

        public static void ParseRequest(UdpClient listener, IPEndPoint ep, byte[] bytes)
        {
            if(bytes[0] == 1)
            {
                Console.WriteLine("got 1");
                listener.Send(Encoding.ASCII.GetBytes(GetFaculties()), ep); // reply back
                return;
            }
            if (bytes[0] == 2)
            {
                Console.WriteLine("got 2");

                // Remove header
                int len = bytes.Length - 1;
                byte[] dataWithoutHeader = new byte[len];
                Array.Copy(bytes, 1, dataWithoutHeader, 0, len);
                string objectAsString = System.Text.Encoding.UTF8.GetString(dataWithoutHeader);
                Console.WriteLine(objectAsString);

                // Add data to file
                AddToFile(objectAsString);

                listener.Send(new byte[] {0}, ep); // reply back
                return;
            }
            if (bytes[0] == 3)
            {
                Console.WriteLine("got 3");

                int counter = 0;
                using (StreamReader sr = File.OpenText(pathToFile))
                {
                    sr.ReadLine(); // First row is a title (skip)
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (Convert.ToInt32(s.Split(',')[2]) == Convert.ToInt32(bytes[1]))
                        {
                            counter++;
                        }
                    }
                }

                // Transform int to string then to bytes and send to client
                string toReturn = counter.ToString();
                listener.Send(Encoding.ASCII.GetBytes(toReturn), ep);
                return;
            }
            if (bytes[0] == 4)
            {
                Console.WriteLine("got 4");

                double counter = 0, sum = 0, avg = 0;
                using (StreamReader sr = File.OpenText(pathToFile))
                {
                    sr.ReadLine(); // First row is a title (skip)
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        var splitted = s.Split(',').Select(x => int.Parse(x)).ToList();
                        if (splitted[2] == Convert.ToInt32(bytes[1]))
                        {
                            counter++;
                            sum += splitted[1];
                        }
                    }
                }
                if(counter > 0)
                {
                    avg = sum / counter;

                    // Transform double to string then to bytes and send to client
                    string toReturn = avg.ToString();
                    listener.Send(Encoding.ASCII.GetBytes(toReturn), ep);
                    return;
                }
                else
                {
                    listener.Send(Encoding.ASCII.GetBytes("none"), ep);
                    return;
                }
            }
            if (bytes[0] == 5)
            {
                Console.WriteLine("got 5");

                int first = -1, second = -1;
                using (StreamReader sr = File.OpenText(pathToFile))
                {
                    sr.ReadLine(); // First row is a title (skip)
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        var splitted = s.Split(',').Select(x => int.Parse(x)).ToList();
                        if (splitted[2] == Convert.ToInt32(bytes[1]))
                        {
                            if(splitted[1] > first)
                            {
                                second = first;
                                first = splitted[1];
                            }
                            else if(splitted[1] > second)
                            {
                                second = splitted[1];
                            }
                        }
                    }
                }
                byte firstByte, secondByte;
                if (first == -1)
                {
                    firstByte = Convert.ToByte(255);
                }
                else
                {
                    firstByte = Convert.ToByte(first);
                }

                if (second == -1)
                {
                    secondByte = Convert.ToByte(255);
                }
                else
                {
                    secondByte = Convert.ToByte(second);
                }

                // Send bytes to client
                byte[] toSend = new byte[2] { firstByte, secondByte };
                listener.Send(toSend, ep);
                return;
            }
        }

        private static void AddToFile(string objectAsString)
        {
            StudentQuery q = JsonConvert.DeserializeObject<StudentQuery>(objectAsString);
            if (File.Exists(pathToFile) == false) // File does not exist
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(pathToFile))
                {
                    // Write the title
                    var collection = q.GetType().GetProperties().Select(p => p.Name).ToList();
                    var output = string.Join(",", collection);
                    sw.WriteLine(output);

                    // Write the actual data
                    sw.WriteLine(q.ToString());
                }
            }
            else // File exists
            {
                using (StreamWriter sw = File.AppendText(pathToFile))
                {
                    sw.WriteLine(q.ToString()); // Write the actual data
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TestCourseWork_TcpServer_
{
    class Program
    {
        static TcpListener server = null;
        static Int32 port = 103;
        static void Main(string[] args)
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine(">> " + "Ожидание подключения первого игрока... ");
            while (true)
            {

                TcpClient client1 = server.AcceptTcpClient();
                Console.WriteLine(">> " + "Подключился первый игрок");
                Console.WriteLine(">> " + "Ожидание подключения второго игрока... ");
                TcpClient client2 = server.AcceptTcpClient();
                Console.WriteLine(">> " + "Подключился второй игрок");
                ClientObject clientObject = new ClientObject(client1, client2);
                Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                clientThread.Start();
                
            }
        }
    }
    public class ClientObject
    {
        public TcpClient client1;
        public TcpClient client2;
        public ClientObject(TcpClient tcpClient1, TcpClient tcpClient2)
        {
            client1 = tcpClient1;
            client2 = tcpClient2;
        }
        public void Process()
        {
            NetworkStream stream1 = null;
            NetworkStream stream2 = null;
            try
            {
                stream1 = client1.GetStream();
                stream2 = client2.GetStream();
                Byte[] bytes1 = new Byte[256];
                Byte[] bytes2 = new Byte[256];
                String data1 = null;
                String data2 = null;
                Random rnd = new Random(1);
                int HP1;
                int HP2;
                while (true)
                {
                    data1 = null;
                    data2 = null;
                    int i1;
                    int i2;
                    while ((i1 = stream1.Read(bytes1, 0, bytes1.Length)) != 0 && (i2 = stream2.Read(bytes2, 0, bytes2.Length)) != 0)
                    {
                        //Получаем изначальные значения Health Point
                        data1 = System.Text.Encoding.ASCII.GetString(bytes1, 0, i1);
                        Console.WriteLine(">> " + "Получено от первого игрока: {0}", data1);
                        HP1 = int.Parse(data1);
                        data2 = System.Text.Encoding.ASCII.GetString(bytes2, 0, i2);
                        HP2 = int.Parse(data2);
                        Console.WriteLine(">> " + "Получено от второго игрока: {0}", data2);
                        //Вызываем методы "ход" в зависимости от выбора первого шага
                        if (rnd.Next(2) == 0)
                        {
                            Step(stream1, stream2, data1, bytes1);
                        }
                        else
                        {
                            Step(stream2, stream1, data2, bytes2);
                        }
                    }
                    
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(">> " + "SocketException: {0}", e);
            }
            finally
            {
                client1.Close();
                client2.Close();
                stream1.Close();
                stream2.Close();
            }
            Console.WriteLine("\n>> " + "Hit enter to continue...");
            Console.Read();
        }
        static void Step(NetworkStream stream1, NetworkStream stream2, String data, Byte[] bytes) 
        {
            int i;
            int damage;
            //отправляем игроку сообщение "Твой ход"
            byte[] msg = System.Text.Encoding.ASCII.GetBytes("Your step");
            stream1.Write(msg, 0, msg.Length);
            Console.WriteLine(">> " + "Ход первого игрока");
            while ((i = stream1.Read(bytes, 0, bytes.Length)) != 0)
            {
                //получаем значение дэмеджа от игрока А
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine(">> " + "Получено от первого игрока: {0}", data);
                damage = int.Parse(data);
                //отправляем значение дэмеджа игроку Б
                msg = System.Text.Encoding.ASCII.GetBytes(damage.ToString());
                stream2.Write(msg, 0, msg.Length);
            }
            Step(stream2,stream1,data,bytes);
        }
    }
}

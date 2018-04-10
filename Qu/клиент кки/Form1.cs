using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace клиент_кки
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Рофлоданные
        int HP = 30;
        int HP2 = 30;
        bool Turn = true;

        public void SendMessageFromSocket(int port)
        {
            // Буфер для входящих данных
            byte[] bytes = new byte[1024];

            // Соединяемся с удаленным устройством
            // Устанавливаем удаленную точку для сокета

            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Соединяем сокет с удаленной точкой
            sender.Connect(ipEndPoint);

            //Console.WriteLine("Введите сообщение: ");
            label1.Text = HP.ToString();
            label2.Text = HP.ToString();
            //string message = Console.ReadLine();

            if (Turn==true)
            {
                HP = HP - Convert.ToInt32(textBox1.Text);
                Turn = false;
                label1.Text = HP.ToString();
            }
            else
            {
                HP2 = HP2 - Convert.ToInt32(textBox2.Text);
                Turn = true;
                label2.Text = HP2.ToString();
            }

                Console.WriteLine("Сокет соединяется с {0} ", sender.RemoteEndPoint.ToString());
            //byte[] msg = Encoding.UTF8.GetBytes(message);
             byte[] msg = Encoding.UTF8.GetBytes(HP.ToString());
            // Отправляем данные через сокет
            int bytesSent = sender.Send(msg);

            // Получаем ответ от сервера
            int bytesRec = sender.Receive(bytes);

            //Console.WriteLine("\nОтвет от сервера: {0}\n\n", Encoding.UTF8.GetString(bytes, 0, bytesRec));

            // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
            //if (message.IndexOf("<TheEnd>") == -1)
            //    SendMessageFromSocket(port);

            // Освобождаем сокет
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                SendMessageFromSocket(11000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                MessageBox.Show("Ку");
            }
        }
    }
}
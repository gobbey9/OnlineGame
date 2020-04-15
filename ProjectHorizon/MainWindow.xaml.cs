using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectHorizon
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public MainWindow()
        {
            InitializeComponent();
        }


        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            int attempts = 0;
            //while (!clientSocket.Connected)
            //{
                try
                {
                    attempts++;
                    clientSocket.Connect(IPAddress.Parse(TextBoxIP.Text), 8778);
                }
                catch (SocketException)
                {
                    Console.WriteLine("Connection attempt failed");
                    return;
                }
            //}
            //Thread.Sleep(100);
            SendServer("login " + TextBoxUserName.Text + " ServerTestPasswordOne");
        }

        private void SendServer(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            clientSocket.Send(buffer);
            byte[] recBuffer = new byte[4096];
            int receive = clientSocket.Receive(recBuffer);
            byte[] data = new byte[receive];
            Array.Copy(recBuffer, data, receive);
            //Console.WriteLine("Received: " + Encoding.ASCII.GetString(data));
            TextBlockOutput.Text = "Received: " + Encoding.ASCII.GetString(data) + "\n";
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            //Array.Copy(buffer, dataBuf, received);

            string text = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("Text received: " + text);

            string response = string.Empty;

            //SendText(response, socket);
            //socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        private void ButtonSendCommand_Click(object sender, RoutedEventArgs e)
        {
            SendServer(TextBoxCommands.Text);
        }
    }
}

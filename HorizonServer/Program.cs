using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HorizonServer
{
    class Program
    {
        private static byte[] buffer = new byte[4096];
        private static List<Socket> clientSockets = new List<Socket>();
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream, ProtocolType.Tcp);
        static void Main(string[] args)
        {
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8778));
            serverSocket.Listen(5);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

        }

        private static void AcceptCallback(IAsyncResult AR)
        {
            Console.WriteLine("Accepting connections...");
            Socket socket = serverSocket.EndAccept(AR);
            clientSockets.Add(socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            Console.WriteLine("Connection accepted from: " + socket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            Array.Copy(buffer,dataBuf, received);

            string text = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("Text received: " + text);

            string response = string.Empty;

            if (text.ToLower() == "get time")
            {
                response = DateTime.Now.ToLongTimeString();
            }
            else
            {
                response = "Invalid Response";
               
            }
            SendText(response, socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        private static void SendText(string text, Socket socket)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        }
        private static void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            socket.EndSend(asyncResult);
        }
    }
}

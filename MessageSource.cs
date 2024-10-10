using System.Net;
using System.Net.Sockets;
using System.Text;
using DZ_ChatApp_vSem6.Abstraction;

namespace DZ_ChatApp_vSem6
{
    internal class MessageSource : IMessageSource
    {
        private readonly UdpClient _udpClient;

        public MessageSource(int port = 12345)
        {
            _udpClient = new UdpClient(port);
        }

        public MessageUDP? RecieveMessage(ref IPEndPoint endPoint)
        {   
            byte[] receiveBytes = _udpClient.Receive(ref endPoint);            
            string receiveJsonString = Encoding.UTF8.GetString(receiveBytes);
            return MessageUDP.FromJson(receiveJsonString);
        }

        public void Send(MessageUDP message, IPEndPoint endPoint)
        {
            byte[] sendBytes = Encoding.UTF8.GetBytes(message.ToJson());
            _udpClient.Send(sendBytes, sendBytes.Length, endPoint); 
            //Console.WriteLine("Сообщение отправлено.");
        }

    }
}

using System.Net;

namespace DZ_ChatApp_vSem6.Abstraction
{
    public interface IMessageSource
    {
       void Send(MessageUDP message, IPEndPoint endPoint);
       MessageUDP? RecieveMessage(ref IPEndPoint endPoint);
    }
}

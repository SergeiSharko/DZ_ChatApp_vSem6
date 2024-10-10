using System.Net;
using System.Text;
using DZ_ChatApp_vSem6.Abstraction;
using DZ_ChatApp_vSem6.Models;

namespace DZ_ChatApp_vSem6
{
    public class Server
    {
        private readonly IMessageSource _messageSource;        
        private static Dictionary<String, IPEndPoint> clients = new Dictionary<string, IPEndPoint>();
        bool work = true;

        public Server(IMessageSource source)
        {
            _messageSource = source;           
        }


        public void Register(MessageUDP message, IPEndPoint fromEndPoint)
        {
            if (clients.TryAdd(message.FromName!, fromEndPoint))
            {
                using (var ctx = new ChatUdpContext())
                {
                    if (ctx.Users.FirstOrDefault(x => x.Name == message.FromName) != null)
                    {
                        Console.WriteLine($"Пользователь {message.FromName} уже зарегистрирован в чате");
                        return;
                    }
                    else
                    {
                        ctx.Add(new User { Name = message.FromName });
                        ctx.SaveChanges();
                        Console.WriteLine($"Пользователь {message.FromName} зарегистрирован в чате");
                    }
                }
            }

        }


        public void ConfirmMessageReceived(int? id)
        {
            using (var ctx = new ChatUdpContext())
            {
                var msg = ctx.Messages.FirstOrDefault(x => x.Id == id);
                if (msg != null)
                {
                    msg.isReceived = true;
                    ctx.SaveChanges();
                }
            }
        }


        public void ReplyMessage(MessageUDP message)
        {
            int? id = null;
            if (clients.TryGetValue(message.ToName!, out IPEndPoint? endPoint))
            {
                using (var ctx = new ChatUdpContext())
                {
                    var fromUser = ctx.Users.FirstOrDefault(x => x.Name == message.FromName);
                    var toUser = ctx.Users.FirstOrDefault(y => y.Name == message.ToName);
                    var msg = new Message
                    {
                        FromUserId = fromUser!.Id,
                        ToUserId = toUser!.Id,
                        isReceived = false,
                        Text = message.Text,
                    };

                    ctx.Messages.Add(msg);

                    ctx.SaveChanges();

                    id = msg.Id;
                }

                var forwardMessage = new MessageUDP()
                {
                    Id = id,
                    Command = Command.Message,
                    ToName = message.ToName,
                    FromName = message.FromName,
                    Text = message.Text
                };

                ConfirmMessageReceived(forwardMessage.Id);

                byte[] forwardBytes = Encoding.UTF8.GetBytes(forwardMessage.ToJson());
                _messageSource.Send(forwardMessage, endPoint);
            }
            else
            {
                Console.WriteLine("Пользователь не найден");
            }
        }


        public void ProcessMessage(MessageUDP message, IPEndPoint fromEndPoint)
        {
            if (message.Command == Command.Register)
            {
                Register(message, new IPEndPoint(fromEndPoint.Address, fromEndPoint.Port));
            }
            if (message.Command == Command.Message)
            {
                ReplyMessage(message);
            }
            if (message.Command == Command.Confirmation)
            {
                ConfirmMessageReceived(message.Id);
                Console.WriteLine($"Подтвержденное сообщение, от = {message.FromName} для = {message.ToName}, ему назначено id = {message.Id}");
            }
        }

        public void Work()
        {            
            
            Console.WriteLine("UDP Сервер ожидает сообщений...");
            Console.WriteLine("Нажмите любую клавишу для завершения работы сервера!");                        
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any,0);
            
            while (work)
            {                
                try
                {
                    var receiveMessage = _messageSource.RecieveMessage(ref remoteEndPoint);
                    if (receiveMessage!.Command == Command.Exit) return;                    
                    else
                    {
                        ProcessMessage(receiveMessage!, remoteEndPoint);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при обработке сообщения: " + ex.Message);
                }
            }
        }

        public void Stop()
        {
            work = false;
        }

    }
}
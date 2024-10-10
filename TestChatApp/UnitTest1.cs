using System.Net;
using DZ_ChatApp_vSem6;
using DZ_ChatApp_vSem6.Abstraction;
using DZ_ChatApp_vSem6.Models;
using NUnit.Framework;

namespace TestChatApp
{
    public class MockMessageSource : IMessageSource
    {
        private Queue<MessageUDP> messages = new();
        private Server? _server;
        private IPEndPoint _endPoint = new IPEndPoint(IPAddress.Any, 0);

        public MockMessageSource()
        {
            messages.Enqueue(new MessageUDP { Command = Command.Register, FromName = "Вася" });
            messages.Enqueue(new MessageUDP { Command = Command.Register, FromName = "Юля" });
            messages.Enqueue(new MessageUDP { Command = Command.Message, FromName = "Юля", ToName = "Вася", Text = "От Юли" });
            messages.Enqueue(new MessageUDP { Command = Command.Message, FromName = "Вася", ToName = "Юля", Text = "От Васи" });
        }

        public MessageUDP RecieveMessage(ref IPEndPoint iPEndPoint)
        {
            return messages.Peek();
        }

        public void Send(MessageUDP message, IPEndPoint iPEndPoint)
        {
            messages.Enqueue(message);
            //throw new NotImplementedException();
        }

        public void AddServer(Server srv)
        {
            _server = srv;
        }

        public MessageUDP? Receive(ref IPEndPoint ep)
        {
            ep = _endPoint;
            if (messages.Count == 0)
            {
                _server!.Stop();
                return null;
            }
            var msg = messages.Dequeue();
            return msg;
        }
    }


    public class Tests
    {
        IMessageSource? _source;
        IPEndPoint? _endPoint;

        [SetUp]
        public void Setup()
        {
            using (var ctx = new ChatUdpContext())
            {
                ctx.Messages.RemoveRange(ctx.Messages);
                ctx.Users.RemoveRange(ctx.Users);
                ctx.SaveChanges();
            }

            _source = new MockMessageSource();
            _endPoint = new IPEndPoint(IPAddress.Any, 0);
        }

        [TearDown]
        public void TearDown()
        {
            using (var ctx = new ChatUdpContext())
            {
                ctx.Messages.RemoveRange(ctx.Messages);
                ctx.Users.RemoveRange(ctx.Users);
                ctx.SaveChanges();
            }
        }


        [Test]
        public void TestRecieveMessage()
        {           
            var result = _source!.RecieveMessage(ref _endPoint!);
            Assert.IsNotNull(result);
            Assert.IsNull(result!.Text);
            Assert.IsNotNull(result.FromName);
            Assert.That(result.FromName, Is.EqualTo("Вася"));
            Assert.That(result.Command, Is.EqualTo(Command.Register));

        }

        [Test]
        public void Register()
        {
            _source = new MockMessageSource();

            var result = _source.RecieveMessage(ref _endPoint!);

            Assert.IsNotNull(result);
            Assert.IsNull(result!.Text);
            Assert.That(result.FromName, Is.EqualTo("Вася"));
            Assert.That(result.Command, Is.EqualTo(Command.Register));
            Assert.Pass();
        }

        [Test]
        public void Test1()
        {
            var mock = new MockMessageSource();
            var srv = new Server(mock);
            mock.AddServer(srv);
            srv.Work();
            using (var ctx = new ChatUdpContext())
            {
                Assert.IsTrue(ctx.Users.Count() == 2, "Пользователи не созданы");
                var user1 = ctx.Users.FirstOrDefault(x => x.Name == "Вася");
                var user2 = ctx.Users.FirstOrDefault(x => x.Name == "Юля");
                Assert.IsNotNull(user1, "Пользователь не созданы");
                Assert.IsNotNull(user2, "Пользователь не созданы");
                Assert.IsTrue(user1!.MessagesFromUser!.Count == 1);
                Assert.IsTrue(user2!.MessagesFromUser!.Count == 1);
                Assert.IsTrue(user1!.MessagesToUser!.Count == 1);
                Assert.IsTrue(user2!.MessagesToUser!.Count == 1);
                var msg1 = ctx.Messages.FirstOrDefault(x => x.FromUser == user1 && x.ToUser == user2);
                var msg2 = ctx.Messages.FirstOrDefault(x => x.FromUser == user2 && x.ToUser == user1);
                Assert.That(msg2!.Text, Is.EqualTo("От Юли"));
                Assert.That(msg1!.Text, Is.EqualTo("От Васи"));
            }
        }
    }
}
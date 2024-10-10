namespace DZ_ChatApp_vSem6
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var server = new Server(new MessageSource());
                server.Work();
            }            
            else if (args.Length == 3)
            {
                int port = int.Parse(args[2]);
                var client = new Client(args[0], args[1], port);
                client.StartClient();
                Environment.Exit(0);
            }
            else
            {
                //Console.WriteLine("Для запуска сервера введите ник-нейм как параметр запуска приложения");
                Console.WriteLine("Для запуска клиента введите <Имя>, <IP-сервера> и <Номер порта>, как параметры запуска приложения");
            }
        }
    }
}

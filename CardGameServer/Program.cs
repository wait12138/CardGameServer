namespace CardGameServer
{
    class Program
    {
        public static ServerSocket serverSocket;
        static void Main(string[] args)
        {
            serverSocket = new ServerSocket();
            serverSocket.StartServer("0.0.0.0", 8080, 4);
            while (true)
            {

            }
        }
    }
}
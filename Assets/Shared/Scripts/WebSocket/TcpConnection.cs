using System.Net.Sockets;

namespace Shared.WebSocket
{
    public class TcpConnection
    {
        public TcpClient Client { get; private set; }
        public NetworkStream Stream { get; private set; }

        public TcpConnection(TcpClient client)
        {
            Client = client;
            Stream = client.GetStream();
        }

        public void Close()
        {
            Client?.Close();
            Stream?.Close();

            Client = null;
            Stream = null;
        }
    }
}
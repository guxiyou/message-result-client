using System;
using NetMQ;

namespace Client
{
	class ClientProgram
	{
		static void Main(string[] args)
		{
			Console.Write("Set TcpKeepAlive (Y/N)? ");
			var response = Console.ReadLine();

			bool keepAlive = "Y".Equals(response, StringComparison.OrdinalIgnoreCase);
			using (NetMQContext context = NetMQContext.Create())
			using (NetMQSocket subscribeSocket = context.CreateSubscriberSocket())
			{
				subscribeSocket.Connect("tcp://evolution:5002");
				subscribeSocket.ReceiveReady += SubSocketOnReceiveReady;
				subscribeSocket.Subscribe(string.Empty); //Prefix of messages to receive. Empty string receives all messages

				if (keepAlive)
				{
					Console.WriteLine("TcpKeepAlive set to: true");
					subscribeSocket.Options.TcpKeepalive = true;
					subscribeSocket.Options.TcpKeepaliveIdle = TimeSpan.FromSeconds(5);
					subscribeSocket.Options.TcpKeepaliveInterval = TimeSpan.FromSeconds(1);
				}
				else
				{
					Console.WriteLine("TcpKeepAlive set to: false");
					subscribeSocket.Options.TcpKeepalive = false;
				}
				while (true)
				{
					NetMQMessage message = subscribeSocket.ReceiveMessage();
					Console.WriteLine("Message Received: " + DateTime.Now);
				}
			}
		}

		private static void SubSocketOnReceiveReady(object sender, NetMQSocketEventArgs args)
		{
			NetMQMessage message = args.Socket.ReceiveMessage();

			Console.WriteLine("Message Received: " + DateTime.Now);
		}
	}
}

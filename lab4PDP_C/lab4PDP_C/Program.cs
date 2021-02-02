using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace lab4PDP_C
{
   

// A C# program for Client 

	class Program
	{
		private static Socket sender;
		// Main Method 
		static void Main(string[] args)
		{
			ExecuteClient();
		}
		private static void endSendCallback(IAsyncResult ar)
		{
			try
			{
				SocketError errorCode;
				int result = sender.EndSend(ar, out errorCode);
				Console.WriteLine(errorCode == SocketError.Success ?
					"Successful! The size of the message sent was :" + result.ToString() :
					"Error with error code: " + errorCode.ToString() //you probably want to consider to resend if there is error code, but best practice is to handle the error one by one
				);
			}
			catch (Exception e)
			{ //exception
				Console.WriteLine("Unhandled EndSend Exception! " + e.ToString());
				//do something like retry or just report that the sending fails
				//But since this is an exception, it probably best NOT to retry
			}
		}
		static void loopConnect(int noOfRetry, int attemptPeriodInSeconds)
		{
			int attempts = 0;
			while (!sender.Connected && attempts < noOfRetry)
			{
				try
				{
					++attempts;
					IAsyncResult result = sender.BeginConnect(IPAddress.Parse("127.0.0.1"), 1234, endConnectCallback, null);
					result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(attemptPeriodInSeconds));
					System.Threading.Thread.Sleep(attemptPeriodInSeconds * 1000);
				}
				catch (Exception e)
				{
					Console.WriteLine("Error: " + e.ToString());
				}
			}
			if (!sender.Connected)
			{
				Console.WriteLine("Connection attempt is unsuccessful!");
				return;
			}
		}

		private const int BUFFER_SIZE = 4096;
		private static byte[] buffer = new byte[BUFFER_SIZE]; //buffer size is limited to BUFFER_SIZE per message
		private static void endConnectCallback(IAsyncResult ar)
		{
			try
			{
				sender.EndConnect(ar);
				if (sender.Connected)
				{
					sender.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), sender);
				}
				else
				{
					Console.WriteLine("End of connection attempt, fail to connect...");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("End-connection attempt is unsuccessful! " + e.ToString());
			}
		}
		const int MAX_RECEIVE_ATTEMPT = 10;
		static int receiveAttempt = 0;
		private static void receiveCallback(IAsyncResult result)
		{
            Socket socket = null;
			try
			{
				socket = (Socket)result.AsyncState;
				if (socket.Connected)
				{
					int received = socket.EndReceive(result);
					if (received > 0)
					{
						receiveAttempt = 0;
						byte[] data = new byte[received];
						Buffer.BlockCopy(buffer, 0, data, 0, data.Length); //There are several way to do this according to https://stackoverflow.com/questions/5099604/any-faster-way-of-copying-arrays-in-c in general, System.Buffer.memcpyimpl is the fastest
																		   //DO ANYTHING THAT YOU WANT WITH data, IT IS THE RECEIVED PACKET!
																		   //Notice that your data is not string! It is actually byte[]
																		   //For now I will just print it out
						Console.WriteLine("Server: " + Encoding.UTF8.GetString(data));
						socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
					}
					else if (receiveAttempt < MAX_RECEIVE_ATTEMPT)
					{ //not exceeding the max attempt, try again
						++receiveAttempt;
						socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
					}
					else
					{ //completely fails!
						Console.WriteLine("receiveCallback is failed!");
						receiveAttempt = 0;
						sender.Close();
					}
				}
			}
			catch (Exception e)
			{ // this exception will happen when "this" is be disposed...
				Console.WriteLine("receiveCallback is failed! " + e.ToString());
			}
		}

		// ExecuteClient() Method 
		static void ExecuteClient()
		{

			try
			{

				// Establish the remote endpoint 
				// for the socket. This example 
				// uses port 11111 on the local 
				// computer. 
				//IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
				IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
				IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 1234);


				// Creation TCP/IP Socket using 
				// Socket Class Costructor 
				sender = new Socket(ipAddr.AddressFamily,
						SocketType.Stream, ProtocolType.Tcp);
				loopConnect(3, 3);
				try
				{

					// Connect Socket to the remote 
					// endpoint using method Connect() 
					//sender.Connect(localEndPoint);

					// We print EndPoint information 
					// that we are connected 
					Console.WriteLine("Socket connected to -> {0} ",
								sender.RemoteEndPoint.ToString());

					// Creation of messagge that 
					// we will send to Server 
					byte[] messageSent = Encoding.ASCII.GetBytes("24 4 ");
					byte[] messageSent2 = Encoding.ASCII.GetBytes("4");
					sender.BeginSend(messageSent, 0, messageSent.Length, SocketFlags.None, endSendCallback, sender); //use async
					//sender.BeginSend(messageSent2, 0, messageSent.Length, SocketFlags.None, endSendCallback, sender); //use async

					// Data buffer 
					byte[] messageReceived = new byte[1024];

					// We receive the messagge using 
					// the method Receive(). This 
					// method returns number of bytes 
					// received, that we'll use to 
					// convert them to string 
					int byteRecv = sender.Receive(messageReceived);
					Console.WriteLine("Message from Server -> {0}",
						Encoding.ASCII.GetString(messageReceived,
													0, byteRecv));

					// Close Socket using 
					// the method Close() 
					sender.Shutdown(SocketShutdown.Both);
					sender.Close();
				}

				// Manage of Socket's Exceptions 
				catch (ArgumentNullException ane)
				{

					Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
				}

				catch (SocketException se)
				{

					Console.WriteLine("SocketException : {0}", se.ToString());
				}

				catch (Exception e)
				{
					Console.WriteLine("Unexpected exception : {0}", e.ToString());
				}
			}

			catch (Exception e)
			{

				Console.WriteLine(e.ToString());
			}
		}
	}
}

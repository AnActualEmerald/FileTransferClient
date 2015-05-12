/*
 * Created by SharpDevelop.
 * User: Burrito119
 * Date: 5/12/2015
 * Time: 9:47 AM
 * 
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileTransferServer
{
	/// <summary>
	/// Description of Server.
	/// </summary>
	public class Server
	{
		private Socket _server;
		private String currentDir;
		private bool isConnected = false;
		
		private byte[] buffer = new byte[2048];
				
		public Server()
		{
			_server = new Socket(SocketType.Stream, ProtocolType.Tcp);
			currentDir = "c:/";
			
		}
		
		public void Start()
		{
			_server.Bind(new IPEndPoint(IPAddress.Any, 28889));
			_server.Listen(5);
			_server.BeginAccept(new AsyncCallback(AcceptCall), null);
		}
		
		private void ProcessClientInput(String command)
		{
			if(command.StartsWith("com"))
			{
				String[] comparam = command.Split(';');
				string com = comparam[0].Split(':')[1];
				string param = comparam[1].Split(':')[1];
				
				if(com == "cd" || com == "chang-dir"){
					currentDir = param;
					SendText(currentDir);
				}
			}
		}
		
		private void SendText(String msg)
		{
			byte[] tmp = Encoding.ASCII.GetBytes(msg);
			_server.BeginSend(tmp, 0, tmp.Length, SocketFlags.None, new AsyncCallback(SendCall), null);
		}
		
		#region Callbacks
		private void AcceptCall(IAsyncResult r)
		{
			Socket client = _server.EndAccept(r);
			client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ClientRecCall), client);
		}
		
		private void ClientRecCall(IAsyncResult r)
		{
			Socket client = (Socket)r.AsyncState;
			int len = client.EndReceive(r);
			byte[] rec = new byte[len];
			Array.Copy(buffer, rec, len);
			
			ProcessClientInput(Encoding.ASCII.GetString(rec));
			
			client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ClientRecCall), client);
		}
		
		private void SendCall(IAsyncResult r)
		{
			_server.EndSend(r);
		}
		#endregion
	}
}

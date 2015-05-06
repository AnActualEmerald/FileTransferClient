/*
 * Created by SharpDevelop.
 * User: Burrito119
 * Date: 4/30/2015
 * Time: 2:01 PM
 * 
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileTransferClient
{
	/// <summary>
	/// Description of Client.
	/// </summary>
	public class Client
	{
		private ConsoleManager cm;
		private Socket _client;
		private byte[] buffer = new byte[1024];
		
		
		public Client()
		{
			cm= new ConsoleManager();
			_client = new Socket(SocketType.Stream, ProtocolType.Tcp);
		}
		
		public Client connect(IPEndPoint ep, int try_limit = 10)
		{
			begin();
			//return this;
			int tries = 0;
			do{
				tries++;
				try{
					cm.WriteLine("Try connect #"+tries);
					_client.Connect(ep);
				}catch(Exception e){
					cm.WriteLine("Couldn't connect with error: " + e.StackTrace);
				}
				
				if(_client.Connected)
					break;
				
			}while(tries < try_limit);
			
			if(_client.Connected)
			{
				begin();
			}else{
				cm.WriteLine("Unable to connect, try again");
				
			}
			
			return this;
		}
		
		private void begin()
		{
			cm.WriteLine("Connected to host: " + _client.Client.RemoteEndPoint);
			while(true){
				cm.Write("Enter command: ");
				String command = cm.GetInput();
				ProcessCommand(command);
			}
		}
		
		private void ProcessCommand(String command)
		{
			if(command.StartsWith("stop"))
			{
				cm.WriteLine("Stopping program...");
				Environment.Exit(0);
			}
			
			if(command.StartsWith("cd"))
			{
				string[] com = command.Split(' ');
				foreach(string c in com)
					c.Trim();
					
				SendText("com:"+cm[0]+";param:"+cm[1]);
			}
		}
		
		public void SendCommand(Command c)
		{
			
		}
		
		private void SendText(String msg)
		{
			byte[] tmp = Encoding.ASCII.GetBytes(msg);
			_client.BeginSend(tmp, 0, tmp.Length, SocketFlags.None, new AsyncCallback(SendCall), null);
		}
		
		#region callbacks
		private void SendCall(IAsyncResult r)
		{
			_client.EndSend(r);
		}
		
		#endregion
	}
	
	public struct Command
	{
		public String name;
		public String[] vars;
	}
	
}

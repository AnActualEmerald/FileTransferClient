/*
 * Created by SharpDevelop.
 * User: Burrito119
 * Date: 4/30/2015
 * Time: 2:01 PM
 * 
 */
using System;
using System.Collections.Generic;
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
		
		// disable once FunctionNeverReturns
		private void begin()
		{
			cm.WriteLine("Connected to host: " + _client.RemoteEndPoint);
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
			
			foreach(Command c in Command.avail_commands)
			{
				foreach(string al in c.aliases){
					if(command.StartsWith(al))
					{
						string[] com = command.Split(' ');
						foreach(string cc in com)
							cc.Trim();
					
						if(SendCommand(c, com))
							Listen();
						
						return;
					}
				}
			}
			
		}
		
		public void Listen()
		{
			_client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(RecCall), null);
		}
		
		public bool SendCommand(Command c, string[] com)
		{
			try{
				if(c.vars.Length == 0)
					SendText("com:"+com[0]+";param:none");
				else
					SendText("com:"+com[0]+";param:"+com[1]);
			}catch(IndexOutOfRangeException){
				cm.WriteLine(c.HelpString());
				return false;
			}
			return true;
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
		
		private void RecCall(IAsyncResult r)
		{
			int len = _client.EndReceive(r);
			byte[] rec = new byte[len];
			Array.Copy(buffer, rec, len);
			
			cm.WriteLine(Encoding.ASCII.GetString(rec));
		}
		
		#endregion
	}
	
	public class Command
	{	
		public static List<Command> avail_commands = new List<Command>();
	
		#region Commands
		
		public static readonly Command Directory = new Command("dir", new string[]{}, 
		                                                       new string[]{"dir", "directory"});
		public static readonly Command GetFile = new Command("get-file", new string[]{"path"}, 
		                                                     new string[]{"get-file", "gf"});
		public static readonly Command ChangeDir = new Command("cd", new string[]{"path"},
		                                                       new string[]{"cd", "change-dir"});
		public static readonly Command Stop = new Command("stop", new string[]{}, new string[]{});
		
		#endregion
				
		public String name;
		public String[] vars;
		public String[] aliases;
		
		public Command(string name, string[] vars, string[] aliases)
		{
			this.name = name;
			this.vars = vars;
			this.aliases = aliases;
			if(avail_commands == null)
				avail_commands = new List<Command>();
			avail_commands.Add(this);
		}
		
		public string HelpString()
		{
			String s = name;
			foreach(string ss in vars)
			{
				s += " ["+ss+"]";
			}
			
			return s;
		}
	}
	
}

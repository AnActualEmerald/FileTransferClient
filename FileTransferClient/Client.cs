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
using System.IO;

namespace FileTransferClient
{
	/// <summary>
	/// Description of Client.
	/// </summary>
	public class Client
	{
		private ConsoleManager cm;
		//private Socket client;
		private TcpClient client;
		private NetworkStream stream;
		private byte[] buffer = new byte[2048];
        private int num_gotten = 0;
		
		
		public Client()
		{
			cm= new ConsoleManager();
            if (!Directory.Exists("./Recieved Files"))
                Directory.CreateDirectory("./Recieved Files");
			//client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
		
		public Client connect(IPEndPoint ep, int try_limit = 10)
		{
			
			int tries = 0;
			client = new TcpClient();

			do{
				tries++;
				try{
					cm.WriteLine("Try connect #"+tries);
					client.Connect(ep);
				}catch(Exception e){
					cm.WriteLine("Couldn't connect with error: " + e.StackTrace);
				}
				
				if(client.Connected)
					break;
				
			}while(tries < try_limit);
			
			if(client.Connected)
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
			stream = client.GetStream();
			cm.WriteLine("Connected to host: " + client.Client.RemoteEndPoint);
			while(true){
				cm.Write("Enter command:");
				String command = cm.GetInput();
				ProcessCommand(command);
			}
		}
		
	
		private void ProcessCommand(String command)
		{
			if(command.StartsWith("stop"))
			{
				stream.Dispose();
				cm.WriteLine("Stopping program...");
				Environment.Exit(0);
			}

            if(command.StartsWith("help"))
            {
                foreach (Command c in Command.avail_commands)
                {
                    cm.Write(c.name+": ");
                    foreach (String al in c.aliases)
                        cm.Write(", " + al);
                    cm.Write(" [");
                    foreach (String v in c.vars)
                        cm.Write(" " + v);
                    cm.WriteLine(" ]");
                }
            }
			
			foreach(Command c in Command.avail_commands)
			{
				foreach(string al in c.aliases){
					if(command.StartsWith(al))
					{
						string[] com;
						if(command.Contains("\""))
						{
							int index = command.IndexOf('\"');
							string par = command.Substring(index);
							par = par.Replace(' ', '%');
							par = par.Replace('\"', ' ');
							par = par.Trim();
							string cmd = command.Split(' ')[0];
							com = new string[]{cmd, par};
						}else{
							com = command.Split(' ');
						}
                        foreach (string cc in com)
                            cc.Trim();

                        if (SendCommand(c, com))
                        {
                            if (c.name == "get-file")
                            {
                                try {
                                    RecFile(com[1]);
                                }
                                catch (IndexOutOfRangeException)
                                {
                                    cm.WriteLine("Filetype needed");
                                }
                            }
                            else
                                Listen();
                        }
						return;
					}
				}
			}
			
		}
		
		public void Listen()
		{
			do{
				int len = stream.Read(buffer, 0, buffer.Length);
				byte[] rec = new byte[len];
				Array.Copy(buffer, rec, len);
			
				cm.WriteLine(Encoding.ASCII.GetString(rec));	
			}while(stream.DataAvailable);
		}

        private void RecFile(String _t)
        {
            List<Byte> file_bits = new List<byte>();
            do
            {
				int len = stream.Read(buffer, 0, buffer.Length);
                byte[] rec = new byte[len];
                Array.Copy(buffer, rec, len);
                file_bits.AddRange(rec);
                cm.WriteLine("Got " + file_bits.Count + " of " + client.Available);
			} while (stream.DataAvailable);
            cm.WriteLine("File recieved");
            num_gotten = Directory.GetFiles("./Recieved Files/").Length;
            File.WriteAllBytes("./Recieved Files/file"+_t, file_bits.ToArray());
            cm.WriteLine("FIle saved at ./Recieved Files/" + _t);
            
        }

        public bool SendCommand(Command c, string[] com)
		{
			try{
				if(c.vars.Length == 0)
					SendText("com|"+com[0]+";param|none");
				else
					SendText("com|"+com[0]+";param|"+com[1]);
			}catch(IndexOutOfRangeException){
				cm.WriteLine(c.HelpString());
				return false;
			}
			return true;
		}
		
		private void SendText(String msg)
		{
			byte[] tmp = Encoding.ASCII.GetBytes(msg);
			stream.BeginWrite(tmp, 0, tmp.Length, new AsyncCallback(SendCall), null);
		}

        #region callbacks
        private void SendCall(IAsyncResult r)
		{
			stream.EndRead(r);
		}
		
		private void RecCall(IAsyncResult r)
		{
			int len = stream.EndRead(r);
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

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
			_server = new Socket(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp);
			currentDir = "c:/";
			
		}
		
		public void Start()
		{
			_server.Bind(new IPEndPoint(IPAddress.Any, 28889));
			_server.Listen(5);
			Console.Write("Listening for clients...");
			_server.BeginAccept(new AsyncCallback(AcceptCall), null);
		}
		
		private void ProcessClientInput(String command, Socket client)
		{
			Console.WriteLine("Client: " + command);
			
			if(command.StartsWith("com"))
			{
				String[] comparam = command.Split(';');
				string com = comparam[0].Split('|')[1];
				string param = comparam[1].Split('|')[1];

                if (com == "cd" || com == "chang-dir")
                {
                    if (param.Contains(":"))
                        currentDir = param;
                    else
                        if (currentDir.EndsWith("/") || param.StartsWith("/"))
                        currentDir += param + "/";
                    else
                        currentDir += "/" + param + "/";
                    if (!Directory.Exists(currentDir))
                    {
                        SendText("Server: NO SUCH DIRECTORY \"" + currentDir + "\"", client);
                    }
                    SendText("Server: " + currentDir, client);
                }
                else if (com == "dir")
                {
                    String dirString = currentDir + "\n";
                    if (!Directory.Exists(currentDir))
                        SendText("Server: NO SUCH DIRECTORY \"" + currentDir + "\"", client);
                    string[] dirs = Directory.GetDirectories(currentDir);
                    string[] files = Directory.GetFiles(currentDir);
                    foreach (string dir in dirs)
                    {
                        dirString += dir;
                        for (int i = 0; i < Console.WindowWidth - dir.Length - 10; i++)
                            dirString += " ";
                        dirString += "DIRECTORY";
                        dirString += "\n";
                    }

                    foreach (string fil in files)
                    {
                        dirString += fil;
                        for (int i = 0; i < Console.WindowWidth - fil.Length - 10; i++)
                            dirString += " ";
                        dirString += "FILE";
                        dirString += "\n";
                    }

                    SendText("Server: " + dirString, client);
                }
                else if (com == "get-file" || com == "gf")
                {
                    try
                    {
                        if (!currentDir.EndsWith("/"))
                            currentDir += "/";
                        SendFile(currentDir + param, client);
                    }
                    catch (FileNotFoundException f)
                    {
                        SendText("File not found at \"" + f.FileName + "\"", client);
                    }
                }
                else {
                    SendText("Unknown Command or Other Error", client);
                }


			}
		}
		
		private void SendText(String msg, Socket target)
		{
			byte[] tmp = Encoding.ASCII.GetBytes(msg);
			target.BeginSend(tmp, 0, tmp.Length, SocketFlags.None, new AsyncCallback(SendCall), target
			                );
		}

        private void SendFile(String path, Socket target)
        {
            try
            {
                target.BeginSendFile(path, new AsyncCallback(SendFileCall), target);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                SendText("Unable to access file", target);
            }

        }

		
		#region Callbacks
		private void AcceptCall(IAsyncResult r)
		{
			Socket client = _server.EndAccept(r);
			Console.Write("Client Connected!\n");
			client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ClientRecCall), client);
		}
		
		private void ClientRecCall(IAsyncResult r)
		{
			try{
			Socket client = (Socket)r.AsyncState;
			int len = client.EndReceive(r);
			byte[] rec = new byte[len];
			Array.Copy(buffer, rec, len);
			
			ProcessClientInput(Encoding.ASCII.GetString(rec), client);
			
			client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ClientRecCall), client);
			}catch(Exception e){
                Console.WriteLine("Client disconnected: " +e.StackTrace);
                Console.Write("Listening for clients...");
                _server.BeginAccept(new AsyncCallback(AcceptCall), null);
			}
		}
		
		private void SendCall(IAsyncResult r)
		{
			((Socket)r.AsyncState).EndSend(r);
		}

        private void SendFileCall(IAsyncResult r)
        {
            ((Socket)r.AsyncState).EndSendFile(r);
        }
		#endregion
	}
}

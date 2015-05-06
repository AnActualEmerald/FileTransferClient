/*
 * Created by SharpDevelop.
 * User: Burrito119
 * Date: 4/30/2015
 * Time: 1:51 PM
 * 
 */
using System;
using System.Threading;

namespace FileTransferClient
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
			// TODO: Implement Functionality Here
			
			new	Client().connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 25565));
		}
	}
}
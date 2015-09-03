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
			Console.WriteLine("Welcome to Burrito's File Transfer System!");
			
			// TODO: Implement Functionality Here
			
			Console.Write("Enter IP: ");
			String ip = Console.ReadLine();
			System.Net.IPAddress addr;
			if(ip.Contains("local"))
				addr = System.Net.IPAddress.Loopback;
			else
				addr = System.Net.IPAddress.Parse(ip);
			new	Client().connect(new System.Net.IPEndPoint(addr, 28889));
		}
	}
}
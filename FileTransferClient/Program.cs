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
			Console.WriteLine("Welcome to Burrito's file transfer system!");

            // TODO: Implement Functionality Here

            Console.WriteLine("Enter Desired IP: ");
            String ip = Console.ReadLine();
            if (ip.Contains("local"))
                new Client().connect(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 28889));
            else
                new	Client().connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), 28889));
		}
	}
}
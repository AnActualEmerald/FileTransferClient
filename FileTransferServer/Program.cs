/*
 * Created by SharpDevelop.
 * User: Burrito119
 * Date: 5/12/2015
 * Time: 9:45 AM
 * 
 */
using System;

namespace FileTransferServer
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
			// TODO: Implement Functionality Here
			
			new Server().Start();
			Console.ReadLine();
		}
	}
}
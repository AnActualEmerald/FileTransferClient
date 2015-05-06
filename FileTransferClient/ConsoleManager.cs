/*
 * Created by SharpDevelop.
 * User: Burrito119
 * Date: 4/30/2015
 * Time: 1:52 PM
 * 
 */
using System;

namespace FileTransferClient
{
	/// <summary>
	/// Description of ConsoleManager.
	/// </summary>
	public class ConsoleManager
	{
		public ConsoleManager()
		{
		}
		
		public void RewriteLine(Object f)
		{
			Console.Write("\r{0}", f);
		}
		
		public void WriteLine(Object f)
		{
			Console.Write("{0}\n", f);
		}
		
		public void Write(Object f)
		{
			Console.Write(f);
		}
		
		public String GetInput()
		{
			return Console.ReadLine();
		}
	}
}

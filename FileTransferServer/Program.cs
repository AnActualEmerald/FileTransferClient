/*
 * Created by SharpDevelop.
 * User: Burrito119
 * Date: 5/12/2015
 * Time: 9:45 AM
 * 
 */

using System.Windows.Forms;

namespace FileTransferServer
{
    class Program
	{
		public static void Main(string[] args)
		{
            //Console.WriteLine("Starting Server...");

            // TODO: Implement Functionality Here
            Application.EnableVisualStyles();
            Application.Run(new ServerOutput());
		}


	}
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FileTransferServer
{
    public partial class ServerOutput : Form
    {
        public ServerOutput()
        {
            InitializeComponent();
        }

        private void ServerOutput_Load(object sender, EventArgs e)
        {
           // Console.SetOut(new DebugWriter(Text1));
            Console.WriteLine("Starting Server");
            new Server().Start();
            TrayIcon.Visible = true;
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TrayIcon.Visible = false;
            this.WindowState = FormWindowState.Normal;
        }

        private void ServerOutput_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                TrayIcon.Visible = true;
        }

        private void TrayIcon_MouseMove(object sender, MouseEventArgs e)
        {
            TrayIcon.ShowBalloonTip(3);
        }
    }

    partial class DebugWriter : TextWriter
    {
        private RichTextBox _parent;

        public DebugWriter(RichTextBox parent)
        {
            _parent = parent;
        }

        public override void Write(string value)
        {
            _parent.AppendText(value);
        }

        public override void WriteLine()
        {
            _parent.AppendText(Environment.NewLine);
        }

        public override void WriteLine(string value)
        {
            _parent.AppendText(value + Environment.NewLine);
        }

        public override Encoding Encoding
        {
            get
            {
                return new ASCIIEncoding();
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace 服务器客户端之间通信
{
    public partial class 服务器 : Form
    {
        public 服务器()
        {
            InitializeComponent();
        }
        private bool bConnected = false;
        //侦听
        private Thread tAcceptMsg = null;


        private IPEndPoint IPP = null;

        private Socket socket = null;

        private Socket clientSocket = null;

        private NetworkStream nstream = null;

        private TextReader treader = null;
        private TextWriter wreader = null;
        private void 服务器_Load(object sender, EventArgs e)
        {

        }

        private void AcceptMessage()
        {
            clientSocket = socket.Accept();
            if (clientSocket != null)
            {
                bConnected = true;
                this.label1.Text = "与客户" + clientSocket.RemoteEndPoint.ToString() + "成功建立连接";
            }
            nstream = new NetworkStream(clientSocket);
            treader = new StreamReader(nstream);
            string sTemp;
            while (bConnected)
            {
                try
                {
                    sTemp = treader.ReadLine();
                    if (sTemp.Length != 0)
                    {
                        lock (this)
                        {
                            richTextBox1.Text = "客户机" + sTemp + "\n" + richTextBox1.Text;
                        }
                    }
                }
                catch
                {
                    tAcceptMsg.Abort();
                    MessageBox.Show("无法与客户机通信");
                }
            }
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPP = new IPEndPoint(IPAddress.Any, 65535);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(IPP);
            socket.Listen(0);
            tAcceptMsg = new Thread(new ThreadStart(this.AcceptMessage));
            tAcceptMsg.Start();
            button1.Enabled = false;
        }

        private void richTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (bConnected)
                {
                    try
                    {
                        lock (this)
                        {
                            richTextBox1.Text = "服务器" + richTextBox2.Text + richTextBox1.Text;
                            wreader.WriteLine(richTextBox2.Text);
                            wreader.Flush();
                            richTextBox2.Text = "";
                            richTextBox2.Focus();
                        }
                    }

                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("未与客户机建立连接，不能通信");
                }

            }
        }

        private void 服务器_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                socket.Close();
                tAcceptMsg.Abort();
            }
            catch
            { }
        }
    }

}

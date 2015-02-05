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

namespace 客户机
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private bool bConnected = false;
        //侦听
        private Thread tAcceptMsg = null;


        private IPEndPoint IPP = null;

        private Socket socket = null;

      //  private Socket clientSocket = null;

        private NetworkStream nstream = null;

        private TextReader treader = null;
        private TextWriter wreader = null;
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        public void AcceptMessage()
        {
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
                            richTextBox1.Text = "服务器" + sTemp + "\n" + richTextBox1.Text;
                        }
                    }
                }
                catch
                {
                    tAcceptMsg.Abort();
                    MessageBox.Show("无法与服务器通信");
                }
            }
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                IPP = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(IPP);
                if (socket.Connected)
                {
                    nstream = new NetworkStream(socket);
                    treader = new StreamReader(nstream);
                    wreader = new StreamWriter(nstream);
                    tAcceptMsg = new Thread(new ThreadStart(this.AcceptMessage));
                    tAcceptMsg.Start();
                    bConnected = true;
                    button1.Enabled = false;
                    MessageBox.Show("与服务器成功建立连接");
                }
            }
            catch
            {
                MessageBox.Show("123123");
            }
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
                            richTextBox1.Text = "客户机" + richTextBox2.Text + richTextBox1.Text;
                            wreader.WriteLine(richTextBox2.Text);
                            wreader.Flush();
                            richTextBox2.Text = "";
                            richTextBox2.Focus();
                        }
                    }

                    catch
                    {
                        MessageBox.Show("false");
                    }
                }
                else
                {
                    MessageBox.Show("未与客户机建立连接，不能通信");
                }

            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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

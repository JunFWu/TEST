using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace OPCserverInterface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private bool gonext = false;
        private Int32 a = 0;
        private Int32 qua = 0;
        private Int32 reserved = 0;
        private string time = "";
        private string type = "";
        private string hclient = "";
        private string hashcode = "";
        private Boolean kill = false;
        private Thread thread;
        private void button1_Click(object sender, EventArgs e)
        {
            
     
            OPCRaW.connOPcserver("localhost", "KEPware.KEPServerEx.V4");
            OPCRaW.OPCServerADDgroup("one", "Channel1.Device1.123");
            gonext = false;
            timer1.Enabled = true;
     
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            textBox1.Text = Convert.ToString(a);
            textBox2.Text = Convert.ToString(qua);
            textBox3.Text = Convert.ToString(reserved);
            textBox4.Text = Convert.ToString(time);
            textBox5.Text = Convert.ToString(type);
            textBox6.Text = Convert.ToString(hclient);
            textBox7.Text = Convert.ToString(hashcode);

            if (gonext == false)
            {
                gonext = true;
            }
            else
            {
                return;
            }
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            //ThreadStart threadstart = new ThreadStart(treadtest);
            //Thread thread = new Thread(threadstart);
            //thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void treadtest()
        {
            OPCRaW.ReadRunTimeData(ref a, ref qua, ref reserved, ref time, ref type, ref hclient, ref hashcode);
            gonext = false;
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
           
            OPCRaW.ReadRunTimeData(ref a, ref qua, ref reserved, ref time, ref type, ref hclient, ref hashcode);
            gonext = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            timer1.Enabled = false;
            ThreadStart threadstart = new ThreadStart(disconn);
            Thread thread = new Thread(threadstart);
            thread.Start();
           
        }
        private void charge()
        {
            this.Cursor = Cursors.Default;
            textBox1.Text = "---";
            textBox2.Text = "---";
            textBox3.Text = "---";
            textBox4.Text = "---";
            textBox5.Text = "---";
            textBox6.Text = "---";
            textBox7.Text = "---";
        }
        private void disconn()
        {
            
            kill = true;
            ///停止后台线程触发
            while (backgroundWorker1.IsBusy)
            {
                //backgroundWorker1.CancelAsync();
                Thread.Sleep(3000);
            }
            ///等待线程处理完成 
            OPCRaW.DisConnectOPCServerOfBCNetS7();      ///执行服务器断开操作
            kill = false;
 
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            textBox1.Text = "---";
            textBox2.Text = "---";
            textBox3.Text = "---";
            textBox4.Text = "---";
            textBox5.Text = "---";
            textBox6.Text = "---";
            textBox7.Text = "---";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            return;
            if (kill == true)
            {
                OPCRaW.DisConnectOPCServerOfBCNetS7();      ///执行服务器断开操作
                textBox1.Text = "---";
                textBox2.Text = "---";
                textBox3.Text = "---";
                textBox4.Text = "---";
                textBox5.Text = "---";
                textBox6.Text = "---";
                textBox7.Text = "---";
                this.Cursor = Cursors.Default;
                kill = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OPCserverfind.findopcserver("192.168.1.100");
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IND880.Advance.APIClient;
using IND880.Advance.APIClient.Events;
using System.Configuration;
 
namespace 称重量提取
{
  
    public partial class 测试主程序 : Form
    {
       
        public 测试主程序()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ConfigurationManager.RefreshSection("appSettings");
                global.IP = ConfigurationManager.AppSettings["ip"];
                global.IP_pro = 19002;
                global.IOdata[1, 0] = "-------";
                global.IOdata[1, 1] = "-------";
                global.IOdata[1, 2] = "-------";
                global.IOdata[1, 3] = "-------";
                global.IOdata[1, 4] = "-------";
                global.IOdata[0, 0] = "-------";
                global.IOdata[0, 1] = "-------";
                global.IOdata[0, 2] = "-------";
                global.IOdata[0, 3] = "-------";
                global.IOdata[0, 4] = "-------";
                label22.Text = "";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            getweightwindows.ReleasegetwaitResourse();
            getwait.ReleasegetwaitResourse();
            getforIO.ReleaseIOResourse();
            connect.disconnect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getwait.pushweight();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
         
            if (global.mm == null)
            {
                label2.Text = "-------";
                label3.Text = "-------";
                label4.Text = "-------";
                label9.Text = "未连接";
                label22.Text = "";
                return;
            }
            if (!global.mm.IsConnected)
            {
                label2.Text = "-------";
                label3.Text = "-------";
                label4.Text = "-------";
                label9.Text = "未连接";
                label22.Text = "";
                return;
            }
            label9.Text = "已连接";
            //重量
            label2.Text = global.strdata[0];
            label3.Text = global.strdata[1];
            label4.Text = global.strdata[2];
            label21.Text = global.strdata[3];
            label19.Text = global.IOyes;
            label22.Text = global.UI;
            //IO状态IN
            label11.Text = global.IOdata[0, 0];
            label12.Text = global.IOdata[0, 1];
            label13.Text = global.IOdata[0, 2];
            label14.Text = global.IOdata[0, 3];
            label15.Text = global.IOdata[0, 4];
            //IO状态out
            label16.Text = global.IOdata[1, 0];
            label17.Text = global.IOdata[1, 1];
            label18.Text = global.IOdata[1, 2];
        }
        private void button2_Click(object sender, EventArgs e)
        {
            getwait.ReleasegetwaitResourse();
            label2.Text = "-------";
            label3.Text = "-------";
            label4.Text = "-------";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            getweightwindows.ReleasegetwaitResourse();
            getwait.ReleasegetwaitResourse();
            getforIO.ReleaseIOResourse();
            connect.disconnect();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!connect.connectserver())
            {
                return;
            }
                getwait.pushweight();
                getforIO.Initoushmode();
            getweightwindows.buildwindowsevent();
        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private    void button7_Click(object sender, EventArgs e)
        {
            getweightwindows.setNetweightBig();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            getweightwindows.setNetweightsmall(comboBox1.Text);
        }
        private void button8_Click(object sender, EventArgs e)
        {
            getwait.tarezero();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            getwait.gettare();
        }
    }
}

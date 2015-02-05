using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace OPC_C
{
    public partial class Form1 : Form
    {
        private bool next = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // OPCread.disconnect();
            dataGridView1.Rows.Clear();
           string[,] a =   OPCread.pubconn(textBox1.Text);
           for (int i = 0; i < a.Length / 6; i++)
           {
               dataGridView1.Rows.Add();
               int inta = dataGridView1.Rows.Count;
               dataGridView1.Rows[inta - 1].Cells["OPC服务器"].Value = a[i, 0];
               dataGridView1.Rows[inta - 1].Cells["标签"].Value = a[i, 1];
               dataGridView1.Rows[inta - 1].Cells["quantity"].Value = a[i, 2];
               dataGridView1.Rows[inta - 1].Cells["时间"].Value = a[i, 3];
               dataGridView1.Rows[inta - 1].Cells["value"].Value = a[i, 4];
           }
          timer1.Enabled = true;
    
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "127.0.0.1";
            next = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            OPCread.disconnect();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            OPCread.disconnect();
        }

       
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (next == false)
            {
                next = true;
                string[,] a = OPCread.roundread();
                for (int i = 0; i < a.Length / 6; i++)
                {
                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        if (Convert.ToString(dataGridView1.Rows[j].Cells["标签"].Value) == a[i, 1])
                        {
                            dataGridView1.Rows[j].Cells["quantity"].Value = a[i, 2];
                            dataGridView1.Rows[j].Cells["时间"].Value = a[i, 3];
                            dataGridView1.Rows[j].Cells["value"].Value = a[i, 4];
                            dataGridView1.Rows[j].Cells["i"].Value = a[i, 5];
                        }
                    }
                }
                next = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (next == false)
            {
                next = true;
                string[,] a = OPCread.roundread();
                for (int i = 0; i < a.Length / 6; i++)
                {
                    for (int j = 0; j < dataGridView1.Rows.Count; j++)
                    {
                        if (Convert.ToString(dataGridView1.Rows[j].Cells["标签"].Value) == Convert.ToString(a[i, 1]))
                        {
                            dataGridView1.Rows[j].Cells["quantity"].Value = a[i, 2];
                            dataGridView1.Rows[j].Cells["时间"].Value = a[i, 3];
                            dataGridView1.Rows[j].Cells["value"].Value = a[i, 4];
                            dataGridView1.Rows[j].Cells["i"].Value = a[i, 5];
                        }
                    }
                }
                next = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                return;
            }
            if (dataGridView1.SelectedRows.Count == 0)
            {
                return;
            }
            if (dataGridView1.CurrentRow.Cells["i"].Value != null)
            {
                OPCread.write(Convert.ToInt32(dataGridView1.CurrentRow.Cells["i"].Value), textBox2.Text);
            }
        }
    }
}

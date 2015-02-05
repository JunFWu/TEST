using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace sqlupdate
{
    class Program
    {
        static void Main(string[] args)
        {

            string IP = "";
            string pwd = "";
            string connectsql = "";
         //   Console.WriteLine("开始");
          //  Console.Write("IP地址是：");
          //  IP = Console.ReadLine();
          //  Console.Write("sa密码地址是：");
          //  pwd = Console.ReadLine();
            IP = "localhost";
            pwd = "sa";
            connectsql = "Data Source = " + IP + ";Database = test;uid =sa;pwd = " + pwd;
            Console.WriteLine(connectsql);


            SqlConnection strconn = new SqlConnection(connectsql);
            strconn.Open();
            SqlTransaction Transaction = strconn.BeginTransaction();
            Transaction.Commit();
            try
            {
                Transaction = strconn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Transaction = Transaction;
                cmd.Connection = strconn;
                cmd.CommandText = "insert into autoId values (1)";
                cmd.ExecuteNonQuery();
                Transaction.Commit();
                Console.WriteLine("1");
               
                
                Transaction = strconn.BeginTransaction();
                cmd.Transaction = Transaction;
                //cmd.Connection = strconn;
                cmd.CommandText = "insert into autoId values (1)";
                cmd.ExecuteNonQuery();
                Transaction.Commit();
                Console.WriteLine("2");
                Console.WriteLine("处理结束，按任意键结束程序");
                Console.ReadKey();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                try
                {
                    Transaction.Rollback();
                }
                catch { }
            }

           
          
        }
    }
}

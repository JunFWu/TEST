using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPCAutomation;
using System.Windows.Forms;
using System.Collections;

namespace OPC_C
{
    public class OPCread
    {
        private static OPCServer OPC_server = new OPCServer();
        private static Array allservers;
        private static ArrayList allitem = new ArrayList();
        private static OPCBrowser opcb;
       private static string[,] allitemvalue;
        private static OPCGroup opc_group;
        private static bool connected = false;
        private static bool groupadd = false;
       private static OPCItem OPC_item;
        /// <summary>
        /// 获取OPC服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool connectserver(string ip)
        {
            try
            {
                if (connected == false)
                {
                    object x = OPC_server.GetOPCServers(ip);  //取得OPC服务器列表
                    allservers = (Array)x;
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                connected = false;
                MessageBox.Show("获取OPC服务器列表失败\n" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 连接OPC服务器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool connectserveropc(string ip)
        {
            try
            {
                if (connected == false)
                {
                    foreach (var a in allservers)
                        OPC_server.Connect((string)a, ip);
                    connected = true;
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                connected = false;
                MessageBox.Show("连接OPC服务器列表失败\n" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 读取所有的标签值
        /// </summary>
        /// <returns></returns>
        private static bool readallitem()
        {
            try
            {
                if (connected == true)
                {
                    opcb = OPC_server.CreateBrowser();
                    opcb.ShowLeafs(true);
                    for (int i = 1; i < opcb.Count + 1; i++)
                    {
                        allitem.Add(opcb.Item(i));

                    }
                    return true;
                }
                allitem = null;
                return false;
            }
            catch (Exception ex)
            {
                connected = false;
                MessageBox.Show("获取标签列表失败\n" + ex.Message);
                return false;
            }
        }
        ////组名
        private static bool readgroup()
        {
            try
            {
                if (connected == true && groupadd == false)
                {
                    opc_group = OPC_server.OPCGroups.Add("First one");
                    opc_group.IsActive = true;
                    opc_group.IsSubscribed = false;
                    opc_group.DeadBand = 0;
                    opc_group.UpdateRate = 10;
                    int i = 0;
                    foreach (var a in allitem)
                    {
                        i++;
                        opc_group.OPCItems.AddItem((string)a, i);
                    }
                    groupadd = true;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                connected = false;
                groupadd = false;
                MessageBox.Show("写入标签列表失败\n" + ex.Message);
                return false;
            }
        }

        ////读取标签值
        private static string[,] readitemvalue()
        {
            OPCItem OPC_item;
            try
            {
                int a = opc_group.OPCItems.Count;
                string[,] itemvalue = new string[a, 6];
              //   if (groupadd == true)
                {

                    for (int i = 1; i < opc_group.OPCItems.Count; i++)
                    {
                         OPC_item = opc_group.OPCItems.Item(i);
                        string strID = OPC_item.ItemID;
                        //if (strID == "CPU224.Input.I00")
                        {

                            object quantity;
                            object time;
                            object dya;
                            OPC_item.Read(1, out dya, out quantity, out time);
                            foreach (var aaa in allservers)
                            itemvalue[i - 1, 0] = (string)aaa;
                            itemvalue[i - 1, 1] = strID;
                            itemvalue[i - 1, 2] = quantity.ToString();
                            itemvalue[i - 1, 3] = Convert.ToDateTime(time).AddHours(8).ToString("yyyy-MM-dd HH:mm");
                            itemvalue[i - 1, 4] = dya.ToString();
                            itemvalue[i - 1, 5] = i.ToString();
                        }
                    }

                }
                return itemvalue;
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取标签值失败\n" + ex.Message);
                OPC_item = null;
                opc_group = null;
                OPC_server.Disconnect();
                connected = false;
                groupadd = false;
                string[,] a = new string[1, 6];
                a[0, 0] = "";
                a[0, 1] = "";
                a[0, 2] = "";
                a[0, 3] = "";
                a[0, 4] = "";
                a[0, 5] = "";
                return a;
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        private static void disconn()
        {
            if (connected == true)
            {
                OPC_server.Disconnect();
                connected = false;
            }
            OPC_item = null;
            opc_group = null;
            opcb = null;
            OPC_server = null;
          
            groupadd = false;
            allservers = null;
            allitemvalue = null;
            allitem = null;
        }


        public static string[,] pubconn(string ip)
        {
            connectserver(ip); //获取opcserver
            connectserveropc(ip);  //连接OPCserver
            readallitem();
            readgroup();
            return readitemvalue();
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns></returns>
        public static string[,] roundread()
        {
            return readitemvalue();
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public static void disconnect()
        {
            disconn();
        }


       public  static void write(int i,string value)
        {
            if (i > opc_group.OPCItems.Count)
            {
                MessageBox.Show("无法找到标签，写入失败"); 
                return;
            }
            OPCItem OPC_item = opc_group.OPCItems.Item(i);
            OPC_item.Write(value);
        }
    }
}

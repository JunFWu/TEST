using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IND880.Advance.APIClient;
using System.Windows.Forms;

namespace 称重量提取
{
    public class connect
    {

        private static EventHandler mm_connectfaid;
        private static EventHandler mm_serverstop;
        public static bool connectserver()
        {
            
            if (global.mm == null)
            {
                global.mm = new APIClient(global.IP, global.IP_pro);
            }
            if (global.mm.IsConnected) return true;
            if (!global.mm.ConnectToServer("this is the test client"))
            {
                MessageBox.Show("", "连接失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            buildconnectfaildevent();
            buildserverstopdevent();
            return true;
        }
        /// <summary>
        /// 连接失败
        /// </summary>
        public  static void buildconnectfaildevent()
        {
            if (mm_connectfaid == null)
            {
                mm_connectfaid = new EventHandler(connectfaild);
                global.mm.ServerConnectionFailed += mm_connectfaid;
            }
        }
        private  static void connectfaild(object sender, EventArgs e)
        {
            MessageBox.Show("与API服务器连接断开，请重新连接");
            getwait.ReleasegetwaitResourse();
            getforIO.ReleaseIOResourse();
            connect.disconnect();
        }
        /// <summary>
        /// 服务停止
        /// </summary>
        /// 
        public  static void buildserverstopdevent()
        {
            if (mm_serverstop == null)
            {
                mm_serverstop = new EventHandler(serverstop);
                global.mm.ServerStopping += mm_serverstop;
            }
        }
        private static  void serverstop(object sender, EventArgs e)
        {
            MessageBox.Show("APIServer服务停止，将断开连接");
            getwait.ReleasegetwaitResourse();
            getforIO.ReleaseIOResourse();
            connect.disconnect();
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public static void disconnect()
        {
            if (global.mm == null)
            {
                mm_connectfaid = null;
                mm_serverstop = null;
                return;
            }
            if (mm_connectfaid != null)
            {
                global.mm.ServerConnectionFailed -= mm_connectfaid;
                mm_connectfaid = null;
            }
            if (mm_serverstop != null)
            {
                global.mm.ServerStopping -= mm_serverstop;
                mm_serverstop = null;
            }
            if (global.mm == null) return;
           
            if (!global.mm.IsConnected)
            {
                global.mm = null;
                return;
            }
            global.mm.DisconnectFromServer();
            global.mm = null;
            if (mm_connectfaid != null)
            {
                global.mm.ServerConnectionFailed -= mm_connectfaid;
            }
            if (mm_serverstop != null)
            {
                global.mm.ServerStopping -= mm_serverstop;
            }
        }


    }
}

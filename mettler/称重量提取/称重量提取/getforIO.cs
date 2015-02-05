using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IND880.Advance.APIClient.Events;

namespace 称重量提取
{
    /// <summary>
    /// 获取和设置仪表的IO状态
    /// </summary>
    public class getforIO
    {
        private static EventHandler<InputStatusEventArgs> mm_input;
        private static EventHandler<OutputStatusEventArgs> mm_output;

        public static void Initoushmode()
        {
            if (global.mm == null)
            {
                if (!connect.connectserver())
                {
                    return;
                }
            }
            else
            {
                if (!global.mm.IsConnected)
                {
                    if (!connect.connectserver())
                    {
                        return;
                    }
                }
            }

            global.mm.IOManager.SetNeedIOStatus(true);

            if (global.mm.IOManager.LocalIOBoards.Count == 0)
            {
                global.IOyes = "无扩展IO设备";
                return;
            }
            if (mm_input == null)
            {
                mm_input = new EventHandler<InputStatusEventArgs>(in_put);
                global.mm.IOManager.LocalIOBoard(3).InputPortStatusChanged += mm_input;
            }

            if (mm_output == null)
            {
                mm_output = new EventHandler<OutputStatusEventArgs>(out_put);
                global.mm.IOManager.LocalIOBoard(3).OutputPortStatusChanged += out_put;
            }
        }

        /// <summary>
        /// 显示输入信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void in_put(object sender, InputStatusEventArgs e)
        {
            global.IOdata[0, 0] = e.SlotNo.ToString();
            global.IOdata[0, 1] = e.PortNo.ToString();
            global.IOdata[0, 2] = e.Status.HaveFallingEdgeEvent.ToString();
            global.IOdata[0, 3] = e.Status.HaveRisingEdgeEvent.ToString();
            global.IOdata[0, 4] = e.Status.ElectricalLevel.ToString();
        }

        /// <summary>
        /// 显示输出信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void out_put(object sender, OutputStatusEventArgs e)
        {
            global.IOdata[1, 0] = e.SlotNo.ToString();
            global.IOdata[1, 1] = e.PortNo.ToString();
            global.IOdata[1, 2] = e.Status.ToString();
            global.IOdata[1, 3] = "";
            global.IOdata[1, 4] = "";
        }

        public  static void ReleaseIOResourse()
        {

            if (global.mm == null)
            {
                mm_input = null;
                mm_output = null;
                return;
            }
            if (mm_input != null)
            {
                global.mm.IOManager.LocalIOBoard(3).InputPortStatusChanged -=
                                    mm_input;
                mm_input = null;
            }
            if (mm_output != null)
            {
                global.mm.IOManager.LocalIOBoard(3).OutputPortStatusChanged -=
                                     mm_output;
                mm_output = null;
            }
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
           
        }
    }
}

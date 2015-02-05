using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IND880.Advance.APIClient.Events;
using IND880.Advance.APIClient;
using System.Windows.Forms;
using IND880.Advance.APIClient.APIServiceReference;

namespace 称重量提取
{
    /// <summary>
    /// 得到重量
    /// </summary>
    public class getwait
    {
        // private   static string[] strdata = new string[3];
        private static EventHandler<ScaleWeightDataEventArgs> mm_getwaitevent;
        public static void pushweight()
        {
            if (mm_getwaitevent != null)
            {
                return;
            }
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

            global.mm.Terminal.SetNeedWeightData(true);
            mm_getwaitevent += new EventHandler<ScaleWeightDataEventArgs>(received);
            global.mm.Terminal.Scale(1).ScaleWeightReceived +=
                  mm_getwaitevent;
        }

        private static void received(object sender, ScaleWeightDataEventArgs e)
        {
            global.strdata[0] = e.Data.NetWeight.WeightValue.ToString();
            global.strdata[1] = e.Data.TareWeight.WeightValue.ToString();
            global.strdata[2] = e.Data.GrossWeight.WeightValue.ToString();
            global.strdata[3] = global.mm.Terminal.CurrentScale.Setting.CurrentUnit.ToString();

        }
        public static void ReleasegetwaitResourse()
        {
            if (global.mm == null)
            {
                mm_getwaitevent = null;
                return;
            }
            if (mm_getwaitevent != null)
            {
                global.mm.Terminal.Scale(1).ScaleWeightReceived -=
                                   mm_getwaitevent;
                mm_getwaitevent = null;
            }
            global.strdata[0] = "-------";
            global.strdata[1] = "-------";
            global.strdata[2] = "-------";
        }

        /// <summary>
        /// 皮重清零
        /// </summary>
        public static void tarezero()
    {
        global.mm.Terminal.CurrentScale.ClearTare();
    }
        public static void gettare()
        {
            Weight tareweight = new Weight();
            tareweight.Unit = global.mm.Terminal.CurrentScale.Setting.CurrentUnit;
            tareweight.Digitals = 3;
            tareweight.Status = enumWeightStatus.VALID_WEIGHT;
            tareweight.WeightValue = 200;
            global.mm.Terminal.CurrentScale.TareScale(tareweight);
        }
    }
}

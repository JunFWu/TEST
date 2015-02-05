using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IND880.Advance.APIClient;
using IND880.Advance.APIClient.Events;
using IND880.Advance.APIClient.APIServiceReference;
using System.Windows.Forms;

namespace 称重量提取
{
      public class getweightwindows
    {    
          
          ///仪表设置界面事件

          private static EventHandler<SetupUIStatusEventArgs> setUPUI;  

          public static void buildwindowsevent()
          {
              if (global.mm == null)
              {
                  return;
              }
              if (!global.mm.IsConnected) return;
              setUPUI = new EventHandler<SetupUIStatusEventArgs>(setUPUIcharge);
              global.mm.Terminal.SetupUIStatusChanged += setUPUI;
              if (global.mm.Terminal.SetupUIIsOpen == true)
              {
                  global.UI = "设置";
              }
              else
              {
                  global.UI = "运行";
              }
          }
          private static void setUPUIcharge(object sender, SetupUIStatusEventArgs e)
          {
             
              if (e.Status == true)
              {
                  global.UI = "设置";
             
                  getwait.ReleasegetwaitResourse();
                  getforIO.ReleaseIOResourse();
              }
              else
              {
                  global.UI = "运行";
                  getwait.pushweight();
                  getforIO.Initoushmode();
              }
          }
          ///设置窗体最小化
          public static void setNetweightsmall(string type)
          {
              if (global.mm == null) return;
              if (!global.mm.IsConnected) return;
              WeightWindow status = new WeightWindow();
              status.Size = enumWeighingWindowSize.SMALL_SIZE;
              switch (type)
              {
                  case "上方":
                      status.DockStyle = enumUIDockStyle.Up;
                      break;
                  case "下方":
                      status.DockStyle = enumUIDockStyle.Bottom;
                      break;
                  case "不停靠":
                      status.DockStyle = enumUIDockStyle.None;
                      break;
                  default:
                      status.DockStyle = enumUIDockStyle.Up;
                      break;
              }
              global.mm.Terminal.SetWeightWindowStatus(status);
          }
          /// <summary>
          /// 最大化
          /// </summary>
          public static void setNetweightBig()
          {
              WeightWindow status = new WeightWindow();
              status.Size = enumWeighingWindowSize.BIG_SIZE;
              global.mm.Terminal.SetWeightWindowStatus(status);
          }

          public static void ReleasegetwaitResourse()
          {
              if (global.mm == null)
              {
                  setUPUI = null;
                  return;
              }
              if (setUPUI != null)
              {
                  global.mm.Terminal.SetupUIStatusChanged -=
                                     setUPUI;
                  setUPUI = null;
              }
          }
    }
}

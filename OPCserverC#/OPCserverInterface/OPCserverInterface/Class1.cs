using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;           /* COM对象调用支持 */
using OpcRcw.Da;                                /* OPC内核驱动支持 */
using OpcRcw.Comn;
//using OPCSiemensDAAutomation;//引用连接库

namespace OPCserverInterface
{

    public class OPCRaW
    {
        private static OpcRcw.Da.IOPCServer Serverobj;//定义OPCserver
        private static bool connectedOK = false;
        internal const int LOCALE_ID = 0x407;
        private static int pSvrGroupHandle = 0;
        private static object MyobjGroup1 = null;
        private static OpcRcw.Da.IOPCAsyncIO2 IOPCAsyncIO2Obj = null; //异步读写对象
        private static OpcRcw.Da.IOPCGroupStateMgt IOPCGroupStateMgtObj = null;//组管理对象
        private static IConnectionPointContainer pIConnectionPointContainer = null;
        private static IConnectionPoint pIConnectionPoint = null;
        private static Int32 dwCookie = 0;
       // private static object[] pobjGroup;           /* OPC组对象数组 */
  //    private int[] pSvrGroupHandle;        /* 保存每个OPC组在服务器内部的句柄（索引） */
        private static int[,] ItemSvrHandleArray;    /* 保存每个OPC标签（变量）在服务器内部的句柄（索引），[GroupIndex, ItemServerHandle] */
        private static int[] tItemSvrHandleArray;    /* 单个组内的OPC标签的服务器句柄 */
        private static IOPCSyncIO[] pIOPCSyncIO ;

         /* 构造函数 */
        private static void readAllOPcserver(string ip)
        {

        }
        //取得OPC服务器的列表
        /// <summary>
        /// 连接OPC服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="name"></param>
        public static void connOPcserver(string ip, string name)
        {
            if (connectedOK == true)
            {
                return;
            }
            try
            {
                Type svrComponentType;
                if (name == "" || name == null)
                {
                    svrComponentType = Type.GetTypeFromProgID(ip);
                }
                else
                {
                    svrComponentType = Type.GetTypeFromProgID(name, ip);
                }
                Serverobj = (IOPCServer)Activator.CreateInstance(svrComponentType);
                connectedOK = true;
            }
            catch (System.Exception error)
            {
                connectedOK = false;
                MessageBox.Show("连接OPCserver失败\n" + error.Message);
            }
            finally { }
        }

        /// <summary>
        /// 添加组和标签异步读取
        /// </summary>
        /// <param name="name"></param>
        public static void OPCServerADDgroup(string name,string itemname)
        {
            Int32 dwRequestedUpdateRate = 1000;
            Int32 hClientGroup = 1;
            Int32 pRevUpdateRate;


            float deadband = 0;
            int TimeBias = 0;
            GCHandle hTimeBias, hDeadband;
            hTimeBias = GCHandle.Alloc(TimeBias, GCHandleType.Pinned);
            hDeadband = GCHandle.Alloc(deadband, GCHandleType.Pinned);
            Guid iidRequiredInterface = typeof(IOPCItemMgt).GUID;
            if (connectedOK == false)
            {
                MessageBox.Show("OPC服务器没有连接，读取数据失败");
                return;
            }
            IntPtr pResults = IntPtr.Zero;                                                  /* 保存AddItems函数的执行结果 */
            IntPtr pErrors = IntPtr.Zero;                                                   /* 保存AddItems函数的执行错误 */
                                                       /* 执行结果信息 */
            try
            {
                Serverobj.AddGroup(name,
                    0, dwRequestedUpdateRate,
                    hClientGroup,
                    hTimeBias.AddrOfPinnedObject(),
                    hDeadband.AddrOfPinnedObject(),
                    LOCALE_ID,
                    out pSvrGroupHandle,
                    out pRevUpdateRate,
                    ref iidRequiredInterface,
                    out MyobjGroup1); //增加相应的组
                IOPCAsyncIO2Obj = (IOPCAsyncIO2)MyobjGroup1;  //为组同步读写定义句柄
                IOPCGroupStateMgtObj = (IOPCGroupStateMgt)MyobjGroup1;   //组管理对象
                pIConnectionPointContainer = (IConnectionPointContainer)MyobjGroup1;//增加特定组的异步调用连接

                Guid iid = typeof(IOPCDataCallback).GUID;  //为所有的异步调用创建回调
                pIConnectionPointContainer.FindConnectionPoint(ref iid, out pIConnectionPoint);
                //为OPC server 的连接点与客户端接收点之间建立连接
              //  pIConnectionPoint.Advise(this, out dwCookie);
                tItemSvrHandleArray = new  int[1];
                OPCITEMDEF[] itemarray = new OPCITEMDEF[4];
                itemarray[0].szAccessPath = "";                                      /* 访问路径"" */
                itemarray[0].szItemID = itemname; /* OPC标签名称 */
                itemarray[0].hClient = 1;
                tItemSvrHandleArray[0] = 1;/* 客户机的标签索引 */
                itemarray[0].dwBlobSize = 0;
                itemarray[0].pBlob = IntPtr.Zero;
                itemarray[0].vtRequestedDataType = 2;
                ItemSvrHandleArray = new int[1, 1];       
                ((IOPCItemMgt)MyobjGroup1).AddItems(1, itemarray, out pResults, out pErrors); //将标签加入到组中
                int[] errors = new int[1];                               /* 每个标签的错误信息 */
                IntPtr pos = pResults;
                Marshal.Copy(pErrors, errors, 0, 1);                     /* pErrors->errors */
                for (int i = 0; i < 1; i++)                              /* 检查每个标签的error */
                {
                    if (errors[i] == 0)                                                     /* 如果无错误=0 */
                    {
                        if (i != 0)                                                         /* 如果不是第一个标签则指针移动一个OPCITEMRESULT */
                            pos = new IntPtr(pos.ToInt32() + Marshal.SizeOf(typeof(OPCITEMRESULT)));
                        OPCITEMRESULT result = (OPCITEMRESULT)Marshal.PtrToStructure(pos, typeof(OPCITEMRESULT));   /* IntPtr->OPCITEMRESULT */
                        ItemSvrHandleArray[0,0] = result.hServer;                 /* 保存该标签在服务器中的句柄，以后读写标签数值时要用到 */
                    }
                    else
                    {                                                                       /* 如果有错误（向服务器添加标签失败） */
                        connectedOK = false;                                                /* 连接失败 */
                       // String strError;
                      //  Serverobj.GetErrorString(errors[i], LOCALE_ID, out strError);   /* 获取错误信息 */
                        // OutputExceptionToDatabase(.......)                               /* 将异常信息输出到数据库 */
                        throw new Exception("添加标签：" + itemarray[i].szItemID + " 错误！" //+ strError
                            );  /* 抛出异常 */
                    }
                }

            }
            catch (System.Exception error)
            {
             
                MessageBox.Show("group组添加失败失败\n" + error.Message);
            }
            finally
            {
                if (pResults != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pResults);
                    pErrors = IntPtr.Zero;
                }
                if (pErrors != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pErrors);
                    pErrors = IntPtr.Zero;
                }
            }
           
        }

        /* 初始化OPC同步IO接口 */
        /// <summary>
        /// 初始化OPC同步IO接口
        /// </summary>
        /// <returns></returns>
        private  static Boolean InitReqIOInterfaces()
        {
            Boolean InitReqIO = true;                                                                   /* 返回结果 */
            try
            {  pIOPCSyncIO = new IOPCSyncIO[1];   
                for (int GroupIndex = 0; GroupIndex <1; GroupIndex++)                  /* 每个组转换为OPC同步IO接口 */
                    pIOPCSyncIO[GroupIndex] = (IOPCSyncIO)MyobjGroup1;
            }
            catch (System.Exception error)                                                              /* 异常处理 */
            {
                InitReqIO = false;                                                                      /* 返回false */
                // OutputExceptionToDatabase(.......)                                                   /* 将异常信息输出到数据库 */
                throw new Exception(String.Format("初始化同步IO接口错误！"+error.Message));         /* 抛出异常 */
            }
            return InitReqIO;                                                                           /* 返回结果 */
        }

        /* 读取所有设备的数据，由上层应用调用，传递全局DeviceData[]引用 */
        /* 这个函数必须在ConnectOPCServerOfBCNetS7函数执行成功后才能调用 */
        /// <summary>
        /// 读取标签值
        /// </summary>
        /// <param name="value"></param>
        public static  void ReadRunTimeData(ref Int32 value,
                                            ref Int32 qua,
                                            ref Int32 reserved ,
                                            ref string time,
                                            ref string type,
                                            ref string hclient,
                                            ref string hashcode)
        {   
            if (connectedOK == false)
            {
                return;
            }
            if (InitReqIOInterfaces() == false)
                return;
            IntPtr pItemValues = IntPtr.Zero;                                                           /* 标签值指针 */
            IntPtr pErrors = IntPtr.Zero;                                                               /* 错误值指针 */
            int[] errors = new int[1];                                               /* 保存所有错误 */
            OPCITEMSTATE pItemState;                                                                    /* 标签状态 */
            for (int GroupIndex = 0; GroupIndex < 1; GroupIndex++)                      /* 读取每个组的所有标签值 */
            {
                if (pIOPCSyncIO[GroupIndex] != null)                                                    /* OPC同步接口有效 */
                {
                    try
                    {
                        for (int i = 0; i < 1; i++)                                  /* 获得该组的所有标签的服务器句柄 */
                            tItemSvrHandleArray[i] = ItemSvrHandleArray[GroupIndex, i];
                        pIOPCSyncIO[GroupIndex].Read(OPCDATASOURCE.OPC_DS_DEVICE, 1, tItemSvrHandleArray, out pItemValues, out pErrors); /* 读取标签值 */
                        IntPtr pos = pItemValues;                                                       /* 定位ItemValues的指针 */
                        Marshal.Copy(pErrors, errors, 0, 1);                         /* pErrors->errors */
                        if (pErrors != IntPtr.Zero)                                                     /* 释放pErrors的内存块 */
                        {
                            Marshal.FreeCoTaskMem(pErrors);
                            pErrors = IntPtr.Zero;
                        }
                        Boolean CheckAllBadQuality = true;                                              /* 检查所有标签是否为BAD的标记 */
                        for (int i = 0; i < 1; i++)                                  /* 从pItemState中获得每个标签的值 */
                        {
                            if (errors[i] == 0)                                                         /* 如果无错误 */
                            {
                                if (i != 0)                                                             /* 如果非第一个标签则指针移动一个OPCITEMSTATE */
                                    pos = new IntPtr(pos.ToInt32() + Marshal.SizeOf(typeof(OPCITEMSTATE)));
                                pItemState = (OPCITEMSTATE)Marshal.PtrToStructure(pos, typeof(OPCITEMSTATE));   /* pos->OPCITEMSTATE */
                                CheckAllBadQuality = CheckAllBadQuality && (pItemState.wQuality != Qualities.OPC_QUALITY_GOOD); /* 如果所有标签质量都为非GOOD则为true */
                                /*测试标签的其他字段具体的值*/
                                if (pItemState.vDataValue == null)
                                {
                                    value = 0;
                                }
                                else
                                {
                                    value = Int32.Parse(String.Format("{0}", pItemState.vDataValue));
                                }
                               qua = Int32.Parse(String.Format("{0}", pItemState.wQuality));
                               reserved = Int32.Parse(String.Format("{0}", pItemState.wReserved));
                               time = String.Format("{0}", pItemState.ftTimeStamp);
                               type = pItemState.GetType().ToString();
                               hclient = String.Format("{0}", pItemState.hClient);
                               hashcode = pItemState.GetHashCode().ToString();
                              
                            }
                            else
                            {
                                String strResult = "";
                                Serverobj.GetErrorString(errors[i], LOCALE_ID, out strResult);
                                // OutputExceptionToDatabase(.......)                                                   /* 将异常信息输出到数据库 */
                            }
                        }
       
                        /* 回收非托管资源 */
                        if (pItemValues != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(pItemValues);
                            pItemValues = IntPtr.Zero;
                        }
                    }
                    catch (System.Exception error)                                                                      /* IOPCSyncIO::Read函数执行异常 */
                    {
                        MessageBox.Show(error.Message);                                                   /* 将异常信息输出到数据库 */
                    }
                    finally                                                                                             /* 回收非托管资源 */
                    {
                        if (pItemValues != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(pItemValues);
                            pItemValues = IntPtr.Zero;
                        }
                    }
                }
            }
            if (pItemValues != IntPtr.Zero)                                                                             /* 回收非托管资源 */
            {
                Marshal.FreeCoTaskMem(pItemValues);
                pItemValues = IntPtr.Zero;
            }
        }

    
        /* 断开与OPC.BCNet.S7连接，由上层应用调用 */
        /* 这个函数必须在ConnectOPCServerOfBCNetS7函数执行成功后才能调用 */
        public  static void DisConnectOPCServerOfBCNetS7()
        {
            /* 释放所有OPC资源 */
            try
            {
                for (int i = 0; i <1; i++)
                {
                    Marshal.ReleaseComObject(pIOPCSyncIO[i]);
                    pIOPCSyncIO[i] = null;

                }
                Marshal.ReleaseComObject(IOPCAsyncIO2Obj);
                Serverobj.RemoveGroup(pSvrGroupHandle, 0);
                for (int i = 0; i < 1; i++)
                {
                    Marshal.ReleaseComObject(MyobjGroup1);
                    MyobjGroup1 = null;
                }
                Marshal.ReleaseComObject(Serverobj);
                Serverobj = null;
                connectedOK = false;
            }
            catch (Exception ex)                                                                              /* 断开OPC服务器异常 */
            {
                MessageBox.Show(ex.Message);                                                             /* 将异常信息输出到数据库 */
            }
        }


    }
}

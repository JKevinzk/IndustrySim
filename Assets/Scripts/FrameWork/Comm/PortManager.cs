using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.Win32;
using UnityEngine;

namespace FrameWork.Comm
{
    public sealed class PortManager
    {
        [CanBeNull] private System.IO.Ports.SerialPort _sp;

        private readonly List<byte> _receive = new List<byte>(); // 接收到的所有消息
        private readonly List<byte> _message = new List<byte>(); // 接收到的一条消息
        private static string[] _text; //处理后的消息
        private bool _readTextState; //读取状态

        private readonly ManualResetEvent _threadEvent; // 线程阻塞事件

        public PortManager()
        {
            _threadEvent = new ManualResetEvent(false);

            Thread dataReceivedThread = new Thread(DataReceived); // 创建线程
            Thread dataProcessorThread = new Thread(DataProcessor);
            dataReceivedThread.Start();
            dataProcessorThread.Start();
        }


        #region 扫描端口

        //使用API扫描
        public string[] ScanPorts_API()
        {
            string[] portList = System.IO.Ports.SerialPort.GetPortNames();
            return portList;
        }

        //使用注册表信息扫描
        public string[] ScanPorts_Regedit()
        {
            RegistryKey keyCom = Registry.LocalMachine.OpenSubKey("Hardware\\DeviceMap\\SerialComm");
            string[] SubKeys = keyCom.GetValueNames();
            string[] portList = new string[SubKeys.Length];
            for (int i = 0; i < SubKeys.Length; i++)
            {
                portList[i] = (string)keyCom.GetValue(SubKeys[i]);
            }

            return portList;
        }

        //试错方式扫描
        public string[] ScanPorts_TryFail()
        {
            List<string> tempPost = new List<string>();
            bool mark = false;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort("COM" + (i + 1).ToString());
                    sp.Open();
                    sp.Close();
                    tempPost.Add("COM" + (i + 1).ToString());
                    mark = true;
                }
                catch (System.Exception)
                {
                    continue;
                }
            }

            if (mark)
            {
                string[] portList = tempPost.ToArray();
                return portList;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 打开串口/关闭串口

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="portName">端口号</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">校验位</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopbits">停止位</param>
        public void OpenSerialPort(string portName, int baudRate, Parity parity, int dataBits, StopBits stopbits)
        {
            try
            {
                _sp = new System.IO.Ports.SerialPort(portName, baudRate, parity, dataBits, stopbits); //绑定端口
                _sp.ReadTimeout = 400;
                _sp.Open();
                //使用委托
                //sp.DataReceived += DataReceived;
                _threadEvent.Set();
            }
            catch (Exception ex)
            {
                _sp = new System.IO.Ports.SerialPort();
                Debug.Log(ex);
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void CloseSerialPort()
        {
            try
            {
                _threadEvent.Reset();

                _sp.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        #endregion

        #region 接收数据

        /// <summary>
        /// 接收数据 线程
        /// </summary>
        private void DataReceived()
        {
            byte[] buffer = new byte[1];
            int bytes = 0;
            while (true)
            {
                _threadEvent.WaitOne();
                lock (_receive)
                {
                    if (_sp != null && _sp.IsOpen)
                    {
                        try
                        {
                            int tmp = _sp.ReadBufferSize;
                            for (int i = 0; i < tmp; i++)
                            {
                                bytes = _sp.Read(buffer, 0, 1); //接收字节
                                if (bytes == 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    _receive.Add(buffer[0]);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() != typeof(ThreadAbortException))
                            {
                            }
                        }
                    }
                }

                Thread.Sleep(20);
            }
        }

        #endregion


        /// <summary>
        /// 数据处理
        /// </summary>
        /// <param name="data">字节数组</param>
        private void DataProcessor()
        {
            while (true)
            {
                _threadEvent.WaitOne();
                if (_receive.Count > 0)
                {
                    if (_receive[0] == id)
                    {
                        if (_receive.Count >= 3 && !_readTextState)
                        {
                            int index = 0;
                            _message.Add(_receive[index++]);
                            _message.Add(_receive[index++]);
                            _message.Add(_receive[index++]);
                            switch (_message[1])
                            {
                                case 0x01:
                                case 0x03:
                                    int length = Convert.ToInt32(Convert.ToString(_message[2], 10));
                                    if (_receive.Count >= length + 5)
                                    {
                                        while (index < length + 3)
                                        {
                                            _message.Add(_receive[index++]);
                                        }
                                    }
                                    else
                                    {
                                        goto case 0x03;
                                    }

                                    break;
                                case 0x05:
                                    if (_receive.Count >= 7)
                                    {
                                        _message.Add(_receive[index++]);
                                        _message.Add(_receive[index++]);
                                    }
                                    else
                                    {
                                        goto case 0x05;
                                    }

                                    break;
                                case 0x06:
                                case 0x0f:
                                case 0x10:
                                    if (_receive.Count >= 8)
                                    {
                                        _message.Add(_receive[index++]);
                                        _message.Add(_receive[index++]);
                                        _message.Add(_receive[index++]);
                                    }
                                    else
                                    {
                                        goto case 0x10;
                                    }

                                    break;
                                case 0x81:
                                case 0x83:
                                case 0x85:
                                case 0x86:
                                case 0x8F:
                                case 0x90:
                                default:
                                    if (_receive.Count >= 5)
                                    {
                                        _message.Add(_receive[index++]);
                                        _message.Add(_receive[index++]);
                                        _message.Add(_receive[index++]);
                                    }
                                    else
                                    {
                                        goto case 0x90;
                                    }

                                    break;
                            }

                            _message.Add(_receive[index++]);
                            _message.Add(_receive[index++]);
                            _receive.RemoveRange(0, _message.Count);
                            byte[] data = new byte[_message.Count - 2];
                            _message.CopyTo(0, data, 0, data.Length);
                            uint crc16 = Crc16_Modbus(data, (uint)data.Length);
                            byte crcH = Convert.ToByte(crc16 & 0xFF);
                            byte crcL = Convert.ToByte(crc16 / 0x100);
                            //crc验证，如果通过，代表收到的数据无误，使用text接收，不通过就自动重发
                            if ((_message[_message.Count - 2].Equals(crcH) &&
                                 _message[_message.Count - 1].Equals(crcL)) ||
                                (_message[_message.Count - 1].Equals(crcH) &&
                                 _message[_message.Count - 2].Equals(crcL)))
                            {
                                lock (_text = new string[_message.Count])
                                {
                                    try
                                    {
                                        for (int i = 0; i < _message.Count; i++)
                                        {
                                            _text[i] = Convert.ToString(_message[i], 10);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                        _text = null;
                                        _message.Clear();
                                        WriteData(sendData);
                                        continue;
                                    }
                                }

                                _readTextState = true;
                            }
                            else
                            {
                                WriteData(sendData);
                            }

                            _message.Clear();
                        }
                    }
                    else
                    {
                        _receive.RemoveAt(0); //*移除多余的数据*
                    }
                }

                //Thread.Sleep(20);
            }
        }


        #region 发送数据

        private byte[] sendData; //上一次发送的数据，暂存，用于通信失败后自动重发

        private void WriteData(byte[] dataStr)
        {
            if (!(_sp is { IsOpen: true })) return;
            _readTextState = false;
            _text = null;
            _sp.Write(dataStr, 0, dataStr.Length);
        }

        /// <summary>
        /// 以Modbus协议发送数据
        /// </summary>
        /// <param name="id">从机地址</param>
        /// <param name="fc">功能码</param>
        /// <param name="addrH">寄存器地址高位或起始地址高位</param>
        /// <param name="addrL">寄存器地址低位或起始地址低位</param>
        /// <param name="dinumH">目标地址高位或线圈数高位或地址数高位</param>
        /// <param name="dinumL">目标地址低位或线圈数低位或地址数低位</param>
        /// <param name="value">传输的数据，可空，仅更新数据时用，首位表示数据长度</param>
        public void SendData(byte id, byte fc, byte addrH, byte addrL, byte dinumH, byte dinumL, params byte[] value)
        {
            sendData = new byte[0];
            uint crc16;
            switch (fc)
            {
                case 0x01:
                case 0x03:
                case 0x05:
                case 0x06:
                    // 预置单寄存器
                    sendData = new byte[8];
                    sendData[0] = id; //从机地址
                    sendData[1] = fc; //功能码
                    sendData[2] = addrH; //寄存器地址高字节
                    sendData[3] = addrL; //寄存器地址低字节
                    sendData[4] = dinumH; //读取的寄存器数高字节
                    sendData[5] = dinumL; //寄存器数低字节
                    crc16 = Crc16_Modbus(sendData, 6);
                    sendData[6] = Convert.ToByte(crc16 & 0xFF);
                    sendData[7] = Convert.ToByte(crc16 / 0x100);
                    break;
                case 0x0F:
                    // 强置多线圈
                    sendData = new byte[8 + value.Length];
                    sendData[0] = id; //'站号
                    sendData[1] = fc; //'功能码
                    sendData[2] = addrH; //'
                    sendData[3] = addrL; //'起始地址
                    sendData[4] = dinumH;
                    sendData[5] = dinumL; //'线圈数量
                    sendData[6] = value[0]; //字节数
                    for (int i = 1; i < value.Length; i++)
                    {
                        sendData[6 + i] = value[i]; //输入值
                    }

                    crc16 = Crc16_Modbus(sendData,
                        (uint)(5 + value.Length)); // 因为value的第一个字节保存了长度，所以value.length比实际数据多一个字节
                    sendData[6 + value.Length] = Convert.ToByte(crc16 & 0xFF);
                    sendData[7 + value.Length] = Convert.ToByte(crc16 / 0x100);
                    break;
                case 0x10:
                    // 预置多寄存器
                    sendData = new byte[8 + value.Length];
                    sendData[0] = id; //'站号
                    sendData[1] = fc; //'功能码
                    sendData[2] = addrH; //'
                    sendData[3] = addrL; //'起始地址
                    sendData[4] = dinumH;
                    sendData[5] = dinumL; //'
                    sendData[6] = value[0]; //字节数
                    for (int i = 1; i < value.Length; i++)
                    {
                        sendData[6 + i] = value[i++]; //输入值
                        sendData[6 + i] = value[i];
                    }

                    crc16 = Crc16_Modbus(sendData, (uint)(5 + value.Length));
                    sendData[6 + value.Length] = Convert.ToByte(crc16 & 0xFF);
                    sendData[7 + value.Length] = Convert.ToByte(crc16 / 0x100);
                    break;
                default:
                    break;
            }

            this.id = sendData[0];
            WriteData(sendData);
        }

        #endregion

        #region 发送协议封装

        public byte id = 0x01; //从机地址

        public byte Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 发送重启指令
        /// </summary>
        public void Reset()
        {
            byte[] instruction = { 0x55, 0xAA, 0xBE, 0x9F };
            _sp.Write(instruction, 0, instruction.Length);
        }

        /// <summary>
        /// 读可读写数字量寄存器
        /// </summary>
        /// <param name="start">起始寄存器地址</param>
        /// <param name="numb">读取的寄存器数</param>
        public void ReadDR0x01(ushort start, ushort numb)
        {
            byte startH, startL, numbH, numbL;
            UShortToTwoByte(start, out startH, out startL);
            UShortToTwoByte(numb, out numbH, out numbL);
            SendData(id, 0x01, startH, startL, numbH, numbL);
        }

        /// <summary>
        /// 写可读写数字量寄存器
        /// </summary>
        /// <param name="start">需下置的寄存器地址</param>
        /// <param name="status">下置的数据</param>
        public void WriteDR0x05(ushort start, ushort status)
        {
            byte startH, startL, statusH, statusL;
            UShortToTwoByte(start, out startH, out startL);
            UShortToTwoByte(status, out statusH, out statusL);
            SendData(id, 0x05, startH, startL, statusH, statusL);
        }

        /// <summary>
        /// 读可读写模拟量寄存器
        /// </summary>
        /// <param name="start">起始寄存器地址</param>
        /// <param name="numb">读取的寄存器数</param>
        public void ReadAR0x03(ushort start, ushort numb)
        {
            byte startH, startL, numbH, numbL;
            UShortToTwoByte(start, out startH, out startL);
            UShortToTwoByte(numb, out numbH, out numbL);
            SendData(id, 0x03, startH, startL, numbH, numbL);
        }

        /// <summary>
        /// 写可读写模拟量寄存器
        /// </summary>
        /// <param name="start">需下置的寄存器地址</param>
        /// <param name="status">下置的数据</param>
        public void WriteAR0x06(ushort start, ushort status)
        {
            byte startH, startL, statusH, statusL;
            UShortToTwoByte(start, out startH, out startL);
            UShortToTwoByte(status, out statusH, out statusL);
            SendData(id, 0x06, startH, startL, statusH, statusL);
        }

        /// <summary>
        /// 写多个可读写数字量寄存器
        /// </summary>
        /// <param name="start">修改起始寄存器地址</param>
        /// <param name="numb">修改的寄存器数</param>
        /// <param name="value">修改的值，需注意该数组长度需正确</param>
        public void WriteDR0x0F(ushort start, ushort numb, params byte[] value)
        {
            byte startH, startL, numbH, numbL;
            UShortToTwoByte(start, out startH, out startL);
            UShortToTwoByte(numb, out numbH, out numbL);
            ushort lenght;
            lenght = (ushort)(numb >> 3);
            if ((numb & 0x07) != 0)
                lenght++;
            if (lenght != value.Length)
            {
                Debug.Log("输入有误，重新输入");
                return;
            }
            else
            {
                lenght++;
                byte[] valu = new byte[lenght];
                valu[0] = (byte)(lenght - 1);
                for (ushort i = 1; i <= lenght; i++)
                {
                    valu[i] = value[i - 1];
                }

                SendData(id, 0x0F, startH, startL, numbH, numbL, valu);
            }
        }

        /// <summary>
        /// 写多个可读写模拟量寄存器
        /// </summary>
        /// <param name="start">修改起始寄存器地址</param>
        /// <param name="numb">修改的寄存器数</param>
        /// <param name="value">修改的值，需注意该数组长度需正确</param>
        public void WriteAR0x10(ushort start, ushort numb, params byte[] value)
        {
            byte startH, startL, numbH, numbL;
            UShortToTwoByte(start, out startH, out startL);
            UShortToTwoByte(numb, out numbH, out numbL);
            ushort lenght;
            lenght = (ushort)(numb << 1);
            if (lenght != value.Length)
            {
                Debug.Log("输入有误，重新输入");
                return;
            }
            else
            {
                lenght++;
                byte[] valu = new byte[lenght];
                valu[0] = (byte)(lenght - 1);
                for (ushort i = 1; i <= lenght; i++)
                {
                    valu[i] = value[i - 1];
                }

                SendData(id, 0x10, startH, startL, numbH, numbL, valu);
            }
        }

        #endregion

        /// <summary>
        /// crc验证
        /// </summary>
        /// <returns></returns>
        private uint Crc16_Modbus(byte[] modebusdata, uint length) //length为modbusdata的长度
        {
            uint i, j;
            uint crc16 = 0xFFFF;

            for (i = 0; i < length; i++)
            {
                crc16 ^= modebusdata[i]; //CRC = BYTE xor CRC（^=取反）
                for (j = 0; j < 8; j++)
                {
                    if ((crc16 & 0x01) == 1) //如果CRC最后一位为1，右移一位后carry=1，则将CRC右移一位后，再与POLY16=0xA001进行xor运算
                    {
                        crc16 = (crc16 >> 1) ^ 0xA001;
                    }
                    else
                    {
                        crc16 = crc16 >> 1; //如果CRC最后一位为0，则只将CRC右移一位
                    }
                }
            }

            return crc16;
        }

        /// <summary>
        /// 16位拆分位两个8位
        /// </summary>
        private void UShortToTwoByte(ushort s, out byte high, out byte low)
        {
            high = (byte)((s >> 8) & 0xff); //高8位
            low = (byte)(s & 0xff); //低8位
        }

        /// <summary>
        /// 为外界提供获取数据的方法
        /// </summary>
        /// <returns>获取是否成功</returns>
        public bool GetData(out string result)
        {
            result = null;
            try
            {
                if (!_readTextState) return false;
                lock (_text)
                {
                    string[] data = new string[2];

                    switch (_text[1])
                    {
                        case "3":
                            for (var i = 3; i < _text.Length - 2; i++)
                            {
                                data[i - 3] = _text[i];
                            }

                            break;
                        case "5":
                        case "6":
                            data[0] = _text[5];
                            data[1] = _text[4];
                            break;
                    }

                    var temp = Convert.ToInt32(data[1]);
                    result += Convert.ToString(temp, 2).PadLeft(8, '0');
                    temp = Convert.ToInt32(data[0]);
                    result += Convert.ToString(temp, 2).PadLeft(8, '0');
                    //Debug.Log("signal:" + result);


                    _text = null;
                }

                _readTextState = false;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
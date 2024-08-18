using System.IO.Ports;
using UnityEditor;
using UnityEngine;

namespace FrameWork.Comm
{
    public sealed class SerialPort
    {
        public string[] ports; //所有串口
        public string port;
        public int baudRate;
        public int numBit;
        public Parity checkBit;
        public StopBits stopBit;

        public bool portState;
        

        public readonly PortManager portManager = new PortManager();
        // void Start()
        // {
        //     portManager = new PortManager();
        //     // 获取当前机器所有串口
        //     Ports = portManager.ScanPorts_API();
        //
        //     port = Ports[0];
        //     baudRate = Int32.Parse(transform.Find("OptionPanel/Options/BaudRate/BRDropdown/Label").GetComponentInChildren<Text>().text);
        //     numBit = Int32.Parse(transform.Find("OptionPanel/Options/NumBit/NBDropdown/Label").GetComponentInChildren<Text>().text);
        //     checkBit = (Parity)Int32.Parse(transform.Find("OptionPanel/Options/CheckBit/CBDropdown/Label").GetComponentInChildren<Text>().text);
        //     stopBit = (StopBits)Int32.Parse(transform.Find("OptionPanel/Options/StopBit/SBDropdown/Label").GetComponentInChildren<Text>().text);
        // }

        // 在程序退出时，关闭串口
        // private void OnApplicationQuit()
        // {
        //     portManager.CloseSerialPort();
        // }
        //
        // public void PortChanged()
        // {
        //     port = transform.Find("OptionPanel/Options/COM/COMDropdown/Label").GetComponent<Text>().text;
        // }
        //
        // public void BaudRateChanged()
        // {
        //     baudRate = Int32.Parse(transform.Find("OptionPanel/Options/BaudRate/BRDropdown/Label").GetComponentInChildren<Text>().text);
        // }
        //
        // public void NumBitChanged()
        // {
        //     numBit = Int32.Parse(transform.Find("OptionPanel/Options/NumBit/NBDropdown/Label").GetComponentInChildren<Text>().text);
        // }
        //
        // public void CheckBitChanged()
        // {
        //     checkBit = (Parity)Int32.Parse(transform.Find("OptionPanel/Options/CheckBit/CBDropdown/Label").GetComponentInChildren<Text>().text);
        // }
        //
        // public void StopBitChanged()
        // {
        //     stopBit = (StopBits)Int32.Parse(transform.Find("OptionPanel/Options/StopBit/SBDropdown/Label").GetComponentInChildren<Text>().text);
        // }

        public void OpenPort()
        {
            portManager.OpenSerialPort(port, baudRate, checkBit, numBit, stopBit);
            portState = true;
        }

        public void ClosePort()
        {
            portManager.CloseSerialPort();
            portState = false;
        }
    }
}
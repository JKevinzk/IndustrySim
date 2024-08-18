using System;
using System.Collections.Generic;
using System.Threading;
using Components;
using TMPro;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace FrameWork.Comm
{
    public class SignalManager : MonoBehaviour
    {
        // 信号相关的所有信息
        public SerialPort sp;
        private int _sensorId;
        private int _actuatorId;

        // PLC地址
        // M10,M11 作为传感器信号区域，M100~M107中存储低8位，M110~M117中存储高8位
        // M12,M13 作为执行器信号区域，M120~M127中存储低8位，M130~M137中存储高8位
        private const ushort ActuatorAddress = 58;
        private const ushort SensorAddress = 56;

        /// <summary>
        /// 传感器id-物体id映射关系
        /// </summary>
        public Dictionary<int, int> sensorObjDic;

        /// <summary>
        /// 执行器id-物体id映射关系
        /// </summary>
        public Dictionary<int, int> actuatorObjDic;

        /// <summary>
        /// 传感器信号id-位数映射
        /// </summary>
        public Dictionary<int, int> sensorIdBitDic;

        /// <summary>
        /// 执行器信号id-位数映射
        /// </summary>
        public Dictionary<int, int> actuatorIdBitDic;

        /// <summary>
        /// 执行器信号
        /// </summary>
        public List<bool> inputs;

        /// <summary>
        /// 传感器信号
        /// </summary>
        public List<bool> coils; // 传感器

        /// <summary>
        /// 传感器指令队列，(BIT,VALUE)
        /// </summary>
        public Queue<ValueTuple<ushort, ushort>> sensorInstructionQueue;

        private bool _isReceived = true; // 发送指令锁
        private string _data; // 数据内容
        private Thread _getActuatorStateThread; // 获取执行器状态线程
        private ManualResetEvent _getDataManualResetEvent; // 获取数据线程手动事件
        private ManualResetEvent _getActuatorStateResetEvent; // 获取执行器状态手动事件

        // 延迟测试相关变量
        private int _number;
        private float _timer;
        private const int Interval = 1;
        public float? responseTime;


        public static SignalManager Instance { get; private set; }

        public int SensorId => ++_sensorId;

        public int ActuatorId => ++_actuatorId;

        private void Awake()
        {
            Instance = this;
            _sensorId = 0;
            _actuatorId = 0;
            ResetDic();
        }

        public void ResetDic()
        {
            sensorObjDic = new Dictionary<int, int>();
            actuatorObjDic = new Dictionary<int, int>();
            sensorIdBitDic = new Dictionary<int, int>();
            actuatorIdBitDic = new Dictionary<int, int>();
        }

        private void Start()
        {
            inputs = new List<bool>(16);
            coils = new List<bool>(16);
            sp = new SerialPort();
            sensorInstructionQueue = new Queue<(ushort, ushort)>();
            _getDataManualResetEvent = new ManualResetEvent(false);
            for (var i = 0; i < 16; i++)
            {
                coils.Add(false);
                inputs.Add(false);
            }

            _getActuatorStateResetEvent = new ManualResetEvent(false);
            _getActuatorStateThread = new Thread(GetActuatorState);
            _getActuatorStateThread.Start();

            _number = 0;
            _timer = 0;
        }

        /// <summary>
        /// 将固定间隔返回的数据进行处理，并存储到input\coil信号区
        /// </summary>
        /// <param name="data"></param>
        private void SetSignals(string data)
        {
            if (data == null || data.Length < 16) return;
            // 倒序存储
            for (var i = 0; i < 16; i++)
            {
                inputs[15 - i] = data[i] != '0';
            }
        }

        private void FixedUpdate()
        {
            if (sp != null && RunScene.Instance.isRun)
            {
                _getActuatorStateResetEvent.Set();
                // 响应延迟测试
                try
                {
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= Interval)
                    {
                        responseTime = 1000f / _number;
                        GameObject.Find("ResponseTime").GetComponentInChildren<TMP_Text>().text = "response time:" +
                            responseTime + "ms";
                        _number = 0;
                        _timer = 0;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            else
            {
                _getActuatorStateResetEvent.Reset();
                GameObject.Find("ResponseTime").GetComponentInChildren<TMP_Text>().text = "";
                _number = 0;
                _timer = 0;
            }
                
        }

        private void GetActuatorState()
        {
            while (true)
            {
                _getActuatorStateResetEvent.WaitOne();
                // 每个固定的时间间隔都要查询其运动状态，以对场景进行更新
                if (!sp.portState) continue;
                // 当前是否已经接收了有效数据
                if (_isReceived)
                {
                    _isReceived = !_isReceived;
                    sp.portManager.ReadAR0x03(ActuatorAddress, 1);
                    if (GetData())
                    {
                        SetSignals(_data);
                        //Debug.Log(_data);
                    }
                        
                    _isReceived = !_isReceived;
                }

                _data = null;
                if (sensorInstructionQueue.Count > 0)
                {
                    var valueTuple = sensorInstructionQueue.Dequeue();
                    ChangeSensorSignal(valueTuple.Item1, valueTuple.Item2);
                }
            }
        }


        public void ClearSignalConfig()
        {
            foreach (var idPair in actuatorObjDic)
            {
                ComponentManager.Instance.compoInSceneDic[idPair.Value].GetComponent<IBaseSignalOperation>()
                    .ResetSignalBit();
            }

            sensorIdBitDic.Clear();
            actuatorIdBitDic.Clear();
        }

        /// <summary>
        /// 将信号配置信息写入组件脚本
        /// </summary>
        public void SetSignalBit()
        {
            // 扫描已经配置的执行器列表，根据执行器-物体ID映射，找到物体，执行物体的信号写入命令
            foreach (var idPair in actuatorIdBitDic)
            {
                ComponentManager.Instance.compoInSceneDic[actuatorObjDic[idPair.Key]]
                    .GetComponent<IBaseSignalOperation>()
                    .SetSignalBit();
            }

            foreach (var idPair in sensorIdBitDic)
            {
                ComponentManager.Instance.compoInSceneDic[sensorObjDic[idPair.Key]].GetComponent<IBaseSignalOperation>()
                    .SetSignalBit();
            }
        }

        /// <summary>
        /// 更改传感器区信号
        /// </summary>
        /// <param name="bit">更改的位数</param>
        /// <param name="signal">更改的值</param>
        private void ChangeSensorSignal(int bit, ushort signal)
        {
            if (!sp.portState) return;
            coils[bit] = signal != 0;
            ushort address = (ushort)(Convert.ToInt16(bit / 8) + SensorAddress);
            ushort value = 0;

            // 将8位信号数据转化为十进制数
            if (bit - 8 < 0)
            {
                for (var i = 7; i > -1; i--)
                {
                    value = (ushort)(value * 2 + (coils[i] ? 1 : 0));
                }
            }
            else
            {
                for (var i = 15; i > 7; i--)
                {
                    value = (ushort)(value * 2 + (coils[i] ? 1 : 0));
                }
            }

            // 写入位变量时，M10所在双字节区，高字节区是实际值，低字节区全部置0
            value = (ushort)(value << 8);
            if (_isReceived)
            {
                _isReceived = !_isReceived;
                //Debug.Log("发送传感器修改指令");
                //Debug.Log("发送数据：" + Convert.ToString(value, 2).PadLeft(16, '0'));
                sp.portManager.WriteAR0x06(address, value);
                if (!GetData())
                {
                    _isReceived = !_isReceived;
                    return;
                }

                if (_data.Equals(Convert.ToString(value, 2).PadLeft(16, '0')))
                {
                    _data = null;
                    //Debug.Log("传感器修改成功");
                    _isReceived = !_isReceived;
                    return;
                }

                _isReceived = !_isReceived;
            }
        }


        // 数据获取函数
        bool GetData()
        {
            Thread getDataThread = new Thread(GetDataThread);
            _getDataManualResetEvent.Reset();
            getDataThread.Start();
            bool isGet = _getDataManualResetEvent.WaitOne(1000); // 设置超时时间
            if (!isGet) Debug.LogWarning("数据获取超时");
            return isGet;
        }

        // 数据获取线程
        void GetDataThread()
        {
            while (_data == null && sp != null)
            {
                sp.portManager.GetData(out _data);
                if (_data == null) continue;
                _number++;
                _getDataManualResetEvent.Set();
                return;
            }
        }

        public void OnApplicationQuit()
        {
            sp?.portManager.CloseSerialPort();
            _getDataManualResetEvent.Reset();
            _getActuatorStateResetEvent.Reset();
        }
    }
}
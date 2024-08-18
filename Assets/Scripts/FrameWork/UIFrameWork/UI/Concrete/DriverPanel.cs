using System.Collections;
using System.IO.Ports;
using FrameWork.Comm;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class DriverPanel : BasePanel
    {
        private const string Path = "Prefabs/UI/Panel/DriverPanel";
        private bool _isConnect;

        private readonly SignalManager _signalManager = SignalManager.Instance;

        //开始主面板
        public DriverPanel() : base(new UIType(Path))
        {
        }

        // 用于非Mono类中开启协程的空类
        private class MonoTool : MonoBehaviour
        {
        }


        public override void onEnter()
        {
            // 参数对应UI
            Dropdown baudRateDropdown = UITool.GetComponentInChildren<Dropdown>("BRDropdown");
            Dropdown numBitDropdown = UITool.GetComponentInChildren<Dropdown>("NBDropdown");
            Dropdown checkBitDropdown = UITool.GetComponentInChildren<Dropdown>("CBDropdown");
            Dropdown stopBitDropdown = UITool.GetComponentInChildren<Dropdown>("SBDropdown");
            //绑定通信脚本
            _signalManager.sp.ports = _signalManager.sp.portManager.ScanPorts_API();
            //设置串口默认参数
            _signalManager.sp.port = _signalManager.sp.ports[0];
            _signalManager.sp.baudRate = int.Parse(baudRateDropdown.options[baudRateDropdown.value].text);
            _signalManager.sp.numBit = int.Parse(numBitDropdown.options[numBitDropdown.value].text);
            _signalManager.sp.checkBit = (Parity)int.Parse(checkBitDropdown.options[checkBitDropdown.value].text);
            _signalManager.sp.stopBit = (StopBits)int.Parse(stopBitDropdown.options[stopBitDropdown.value].text);


            // 确定串口连接状态
            _isConnect = _signalManager.sp.portState;
            if (_isConnect)
                UITool.GetComponentInChildren<Button>("ConnectBtn").GetComponentInChildren<TextMeshProUGUI>()
                    .text = "disconnect";
            else
            {
                UITool.GetComponentInChildren<Button>("ConnectBtn").GetComponentInChildren<TextMeshProUGUI>()
                    .text = "connect";
            }

            // 根据串口扫描结果添加串口
            Dropdown comDd = UITool.GetComponentInChildren<Dropdown>("COMDropdown");

            // 使用协程方式，等到扫描完可用串口后更新到面板
            IEnumerator WaitPorts()
            {
                yield return new WaitUntil(() =>
                    _signalManager.sp.ports != null
                );
                if (_signalManager.sp.ports.Length > 0)
                {
                    comDd.ClearOptions();
                    foreach (var port in _signalManager.sp.ports)
                    {
                        comDd.options.Add(new Dropdown.OptionData(port));
                    }

                    comDd.captionText.text = _signalManager.sp.ports[0];
                }
            }

            UITool.GetOrAddComponent<MonoTool>().StartCoroutine(WaitPorts());

            //添加点击事件
            //获取零件的监听
            UITool.GetComponentInChildren<Button>("BackBtn").onClick.AddListener(Pop);

            UITool.GetComponentInChildren<Button>("ConfigBtn").onClick.AddListener(() =>
            {
                UIManager.UIHide(UIType);
                Push(new SignalSetPanel.SignalSetPanel());
            });
            UITool.GetComponentInChildren<Button>("ConnectBtn").onClick.AddListener(() =>
            {
                if (!_isConnect)
                {
                    Debug.Log("连接串口");
                    UITool.GetComponentInChildren<Button>("ConnectBtn").GetComponentInChildren<TextMeshProUGUI>()
                        .text = "disconnect";
                    _isConnect = true;
                    lock (_signalManager.sp)
                    {
                        // _signalManager.sp.portManager.OpenSerialPort(_signalManager.sp.port, _signalManager.sp.baudRate,
                        // _signalManager.sp.checkBit, _signalManager.sp.numBit, _signalManager.sp.stopBit);
                        _signalManager.sp.OpenPort();
                        _signalManager.sp.portManager.Reset(); // 连接串口后重启PLC
                        var index = 0;
                        foreach (var coil in _signalManager.coils)
                        {
                            if (coil)
                                _signalManager.sensorInstructionQueue.Enqueue(((ushort)index, 0));
                            index++;
                        }
                    }
                }
                else
                {
                    Debug.Log("关闭串口");
                    _signalManager.sp.ClosePort();
                    UITool.GetComponentInChildren<Button>("ConnectBtn").GetComponentInChildren<TextMeshProUGUI>()
                        .text = "connect";
                    _isConnect = false;
                }
            });

            UITool.GetComponentInChildren<Dropdown>("COMDropdown").onValueChanged.AddListener(
                index => { _signalManager.sp.port = _signalManager.sp.ports[index]; }
            );
            baudRateDropdown.onValueChanged.AddListener(index =>
            {
                _signalManager.sp.baudRate = int.Parse(baudRateDropdown.options[index].text);
            });
            numBitDropdown.onValueChanged.AddListener(index =>
            {
                _signalManager.sp.numBit = int.Parse(numBitDropdown.options[index].text);
            });
            checkBitDropdown.onValueChanged.AddListener(index =>
            {
                _signalManager.sp.checkBit =
                    (Parity)int.Parse(checkBitDropdown.options[index].text);
            });
            stopBitDropdown.onValueChanged.AddListener(index =>
            {
                _signalManager.sp.stopBit =
                    (StopBits)int.Parse(stopBitDropdown.options[index].text);
            });
        }


        // public override void OnExit()
        // {
        //     //Debug.Log("27" + UIType.Path);
        //     UIManager.UIHide(UIType);
        // }
    }
}
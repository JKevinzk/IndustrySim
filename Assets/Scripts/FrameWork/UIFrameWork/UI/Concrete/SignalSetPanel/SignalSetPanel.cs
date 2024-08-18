using FrameWork.Comm;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace FrameWork.UIFrameWork.UI.Concrete.SignalSetPanel
{
    public class SignalSetPanel : BasePanel
    {
        private const string Path = "Prefabs/UI/Panel/SignalSetPanel";
        private readonly SignalManager _signalManager = SignalManager.Instance;
        private readonly ComponentManager _componentManager = ComponentManager.Instance;

        public SignalSetPanel() : base(new UIType(Path))
        {
        }

        public override void onEnter()
        {
            // 遍历信号管理器中信号字典，显示在配置界面
            foreach (var sensorPair in _signalManager.sensorObjDic)
            {
                GameObject sensorPrefab = UITool.FindChildGameObject("sensor");
                GameObject sensorBtn = Object.Instantiate(sensorPrefab, sensorPrefab.transform.parent);
                GameObject sensorParent = _componentManager.compoInSceneDic[sensorPair.Value];
                sensorBtn.transform.Find("Text").GetComponent<Text>().text =
                    sensorParent.name + " " + sensorParent.GetComponent<ObjectInfo>().sensorDic[sensorPair.Key] + "(id:" +
                    sensorPair.Key + ")";
                sensorBtn.GetComponentInChildren<SensorButton>().sensorId = sensorPair.Key;
                sensorBtn.SetActive(true);
            }

            foreach (var actuatorPair in _signalManager.actuatorObjDic)
            {
                GameObject actuatorPrefab = UITool.FindChildGameObject("actuator");
                GameObject actuatorBtn = Object.Instantiate(actuatorPrefab, actuatorPrefab.transform.parent);
                GameObject actuatorParent = _componentManager
                    .compoInSceneDic[actuatorPair.Value];
                actuatorBtn.transform.Find("Text").GetComponent<Text>().text =
                    actuatorParent.name + " " + actuatorParent.GetComponent<ObjectInfo>().actuatorDic[actuatorPair.Key] +
                    "(id:" + actuatorPair.Key + ")";
                actuatorBtn.GetComponentInChildren<ActuatorButton>().actuatorId = actuatorPair.Key;
                actuatorBtn.SetActive(true);
            }


            // 如果先前配置过，把配置结果显示在界面上
            if (_signalManager.actuatorIdBitDic.Count != 0)
                foreach (var actuatorPair in _signalManager.actuatorIdBitDic)
                {
                    var objID = _signalManager.actuatorObjDic[actuatorPair.Key];
                    var ap = _componentManager.compoInSceneDic[objID];
                    GameObject input = GameObject.Find("Input" + actuatorPair.Value);
                    input.transform.GetChild(2).GetComponentInChildren<Text>().text =
                        ap.name + " " + ap.GetComponent<ObjectInfo>().actuatorDic[actuatorPair.Key] + "(id:" +
                        actuatorPair.Key + ")";
                    input.GetComponentInChildren<InputButton>().actuatorId = actuatorPair.Key;
                    // todo 
                    // GameObject.Find("Input" + actuatorPair.Value + "/InputField/Placeholder").GetComponent<Text>().text =
                    //     actuatorPair.Key.ToString();
                }

            if (_signalManager.sensorIdBitDic.Count != 0)
                foreach (var sensorPair in _signalManager.sensorIdBitDic)
                {
                    var objID = _signalManager.sensorObjDic[sensorPair.Key];
                    var sp = _componentManager.compoInSceneDic[objID];
                    var coil = GameObject.Find("Coil" + sensorPair.Value);
                    coil.GetComponentInChildren<Text>().text =
                        sp.name + " " + sp.GetComponent<ObjectInfo>().sensorDic[sensorPair.Key] + "(id:" +
                        sensorPair.Key + ")";
                    coil.GetComponentInChildren<CoilButton>().sensorId = sensorPair.Key;
                    // todo
                    // GameObject.Find("Coil" + sensorPair.Value + "/InputField/Placeholder").GetComponent<Text>().text =
                    //     sensorPair.Key.ToString();
                }

            UITool.GetComponentInChildren<Button>("BackBtn").onClick.AddListener(() =>
            {
                Pop();
                UITool.IsPanelInUIRoot("DriverPanel", out var panel);
                UIManager.UIShow(panel);
            });

            // 遍历传感器和执行器配置canvas，并保存到信号管理器
            UITool.GetComponentInChildren<Button>("SaveBtn").onClick.AddListener(() =>
            {
                // 清空组件内部的信号记录和信号管理器的信号记录
                _signalManager.ClearSignalConfig();

                // 在信号管理器中记录面板上的信号
                InputButton[] inputButtons = GameObject.Find("InputCanvas").GetComponentsInChildren<InputButton>();
                int index = 0;

                foreach (var inputButton in inputButtons)
                {
                    if (inputButton.actuatorId != -1)
                        _signalManager.actuatorIdBitDic.Add(inputButton.actuatorId, index);
                    index++;
                }

                CoilButton[] coilButtons = GameObject.Find("CoilCanvas").GetComponentsInChildren<CoilButton>();
                index = 0;
                foreach (var coilButton in coilButtons)
                {
                    if (coilButton.sensorId != -1)
                        _signalManager.sensorIdBitDic.Add(coilButton.sensorId, index);
                    index++;
                }

                // 存储到组件中
                _signalManager.SetSignalBit();
                Push(new SignalChangedPanel());
            });

            // 清空界面
            UITool.GetComponentInChildren<Button>("ClearBtn").onClick.AddListener(() =>
            {
                GameObject canvas;

                void Reset()
                {
                    foreach (Transform input in canvas.transform)
                    {
                        input.Find("InputField").GetComponent<InputField>().text = "";
                        input.Find("InputField/Placeholder").GetComponent<Text>().text = "";
                    }
                }

                canvas = GameObject.Find("InputCanvas");
                Reset();
                canvas = GameObject.Find("CoilCanvas");
                Reset();
            });
        }
    }
}
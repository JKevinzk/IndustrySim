using System;
using System.Collections.Generic;
using System.Linq;
using FrameWork.Comm;
using UnityEngine;
using static Components.Stations.Palletizer.PalletizerName;

namespace Components.Stations.Palletizer
{
    // 堆垛机相关名称
    public static class PalletizerName
    {
        // 动画
        public static readonly int PushStateNameHash = Animator.StringToHash("Push");
        public static readonly int PushDirectionHash = Animator.StringToHash("PushDirection");
        public static readonly int PlateStateNameHash = Animator.StringToHash("Plate");
        public static readonly int PlateDirectionHash = Animator.StringToHash("PlateDirection");
        public static readonly int ClampStateHash = Animator.StringToHash("Clamp");
        public static readonly int ClampDirectionHash = Animator.StringToHash("ClampDirection");
        public static readonly int ElevatorStateHash = Animator.StringToHash("Elevator");
        public static readonly int ElevatorDirectionHash = Animator.StringToHash("ElevatorDirection");
        
        // 执行器
        public const string APush = "(Push)";
        public const string ATurn = "(Turn)";
        public const string AClamp = "(Clamp)";
        public const string ABeltInput = "Belt(+)";
        public const string ABeltOutput = "Belt(-)";
        public const string AChainInput = "Chain(+)";
        public const string AChainOutput = "Chain(-)";
        public const string AOpenPlate = "OpenPlate";
        public const string AElevatorUp = "Elevator+";
        public const string AElevatorDown = "Elevator-";
        public const string AElevatorLimit = "ElevatorMoveToLimit";
        
        // 传感器
        public const string SClamped = "(Clamped)";
        public const string SPlateLimit = "(Plate Limit)";
        public const string SPusherLimit = "(Pusher Limit)";
        public const string SElevatorMoving = "(Elevator Moving)";
        public const string SElevatorBackLimit = "Elevator (Back Limit)";
        public const string SElevatorFrontLimit = "Elevator (Front Limit)";
        
    }
    
    public interface IPalletizerPart
    {
        void SetBit(Palletizer palletizer);
    }
    
    public class Palletizer : BaseComponent,IBaseSignalOperation
    {
        /// <summary>
        /// 执行器名字-id
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, int> _actuatorNameIdDic;
        /// <summary>
        /// 执行器名字-位数
        /// </summary>
        private Dictionary<string, int> _actuatorNameBitDic;
        /// <summary>
        /// 传感器名字-id
        /// </summary>
        private Dictionary<string, int> _sensorNameIdDic;
        /// <summary>
        /// 传感器名字-位数
        /// </summary>
        private Dictionary<string, int> _sensorNameBitDic;

        private Animator _animator;

        // 部件状态
        private bool _clampState;
        private bool _plateState;

        private bool _turner;
        private bool _stripperPlate;
        
        // 部件管理
        // private Belt _loadBelt;
        // private Push _push;
        // private Plate _plate;
        // private Clamp _clamp;
        // private Elevator _elevator;
        // 部件集合
        // private HashSet<IPalletizerPart> _palletizerParts;
        private IPalletizerPart[] _palletizerParts;

        private readonly SignalManager _signalManager = SignalManager.Instance;

        // 对外查询接口
        public int GetActuatorBit(string actuatorName)
        {
            return _actuatorNameBitDic[actuatorName];
        }

        public int GetSensorBit(string sensorName)
        {
            return _sensorNameBitDic[sensorName];
        }

        void Awake()
        {
            _actuatorNameBitDic = new Dictionary<string, int>();
            _actuatorNameIdDic = new Dictionary<string, int>();
            _sensorNameBitDic = new Dictionary<string, int>();
            _sensorNameIdDic = new Dictionary<string, int>();
        }

        protected override void Start()
        {
            base.Start();
            _animator = GetComponent<Animator>();
            _palletizerParts = GetComponentsInChildren<IPalletizerPart>();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            // 执行器信号注册
            _actuatorNameIdDic.Add(APush, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(ATurn, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(AClamp, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(ABeltInput, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(ABeltOutput, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(AChainInput, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(AChainOutput, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(AOpenPlate, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(AElevatorUp, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(AElevatorDown, _signalManager.ActuatorId);
            _actuatorNameIdDic.Add(AElevatorLimit, _signalManager.ActuatorId);
            var info = gameObject.GetComponent<ObjectInfo>();
            foreach (var actuatorPair in _actuatorNameIdDic)
            {
                // 组件信号注册
                info.actuatorDic.Add(actuatorPair.Value, actuatorPair.Key);
                // 信号管理器信号注册
                _signalManager.actuatorObjDic.Add(actuatorPair.Value, info.ObjId);
                // 信号位数对应关系初始化
                _actuatorNameBitDic.Add(actuatorPair.Key, -1);
            }

            // 传感器信号注册
            _sensorNameIdDic.Add(SClamped, _signalManager.SensorId);
            _sensorNameIdDic.Add(SPlateLimit, _signalManager.SensorId);
            _sensorNameIdDic.Add(SPusherLimit, _signalManager.SensorId);
            _sensorNameIdDic.Add(SElevatorMoving, _signalManager.SensorId);
            _sensorNameIdDic.Add(SElevatorBackLimit, _signalManager.SensorId);
            _sensorNameIdDic.Add(SElevatorFrontLimit, _signalManager.SensorId);
            foreach (var sensorPair in _sensorNameIdDic)
            {
                info.sensorDic.Add(sensorPair.Value, sensorPair.Key);
                _signalManager.sensorObjDic.Add(sensorPair.Value, info.ObjId);
                _sensorNameBitDic.Add(sensorPair.Key, -1);
            }
        }

        public override void OnExit()
        {
            // 删除码垛机物体时删除信号
            foreach (var actuatorPair in _actuatorNameIdDic)
            {
                _signalManager.actuatorObjDic.Remove(actuatorPair.Value);
                _signalManager.actuatorIdBitDic.Remove(actuatorPair.Value);
            }

            foreach (var sensorPair in _sensorNameIdDic)
            {
                _signalManager.sensorObjDic.Remove(sensorPair.Value);
                _signalManager.sensorIdBitDic.Remove(sensorPair.Value);
            }

            base.OnExit();
        }


        protected override void Update()
        {
            base.Update();
            #region componentTest
            
            if (Input.GetKeyDown(KeyCode.T))
            {
                _stripperPlate = !_stripperPlate;
                _animator.SetBool("StripperPlate", _stripperPlate);
                if (_stripperPlate)
                    Debug.Log("StripperPlate Open");
                else
                    Debug.Log("StripperPlate Close");
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _turner = !_turner;
                _animator.SetBool("Turner", _turner);
                if (_turner)
                    Debug.Log("Turner Open");
                else
                    Debug.Log("Turner Close");
            }

            #endregion
        }

        public  void SetSignalBit()
        {
            foreach (var actuatorPair in _actuatorNameIdDic)
            {
                try
                {
                    _actuatorNameBitDic[actuatorPair.Key] = _signalManager.actuatorIdBitDic[actuatorPair.Value];
                    Debug.Log(actuatorPair.Key + "信号已配置");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(actuatorPair.Key + "执行器组件未配置信号");
                }
            }

            foreach (var sensorPair in _sensorNameIdDic)
            {
                try
                {
                    _sensorNameBitDic[sensorPair.Key] = _signalManager.sensorIdBitDic[sensorPair.Value];
                    Debug.Log(sensorPair.Key + "信号已配置");
                }
                catch (Exception e)
                {
                    Debug.LogWarning(sensorPair.Key + "传感器组件未配置信号");
                }
            }

            // 对部件进行信号配置
            foreach (var palletizerPart in _palletizerParts)
            {
                palletizerPart.SetBit(this);
            }
        }

        // 重置信号
        public void ResetSignalBit()
        {
            var keys = _actuatorNameBitDic.Select(actuatorPair => actuatorPair.Key).ToList();

            foreach (var key in keys)
            {
                _actuatorNameBitDic[key] = -1;
            }

            keys = _sensorNameBitDic.Select(sensorPair => sensorPair.Key).ToList();
            foreach (var key in keys)
            {
                _sensorNameBitDic[key] = -1;
            }

            foreach (var palletizerPart in _palletizerParts)
            {
                palletizerPart.SetBit(this);
            }
        }
    }

   
}
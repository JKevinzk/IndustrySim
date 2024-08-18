using System.Collections;
using FrameWork.Comm;
using UnityEngine;
using static Components.Stations.Palletizer.PalletizerName;

namespace Components.Stations.Palletizer
{
    public class Elevator : LoadBelt, IPalletizerPart
    {
        private bool _elevatorState;
        private bool _movingSent;
        private bool _stopSent;
        private bool _upSent;
        private bool _downSent;
        private int _elevatorUpBit = -1;
        private int _elevatorDownBit = -1;
        private int _elevatorLimitBit = -1;
        private int _chainInputBit = -1;
        private int _chainOutputBit = -1;
        private int _movingBit = -1;

        private Animator _animator;
        private const int LayerNumber = 1;

        private readonly SignalManager _signalManager = SignalManager.Instance;
        private readonly RunScene _runScene = RunScene.Instance;

        public override void Start()
        {
            base.Start();
            _elevatorState = false;
            _movingSent = false;
            _stopSent = false;
            _upSent = false;
            _downSent = false;
            GetComponent<Rigidbody>().mass = 90;
        }

        private void FixedUpdate()
        {
            if (!_runScene.isRun)
            {
                _upSent = false;
                _downSent = false;
                return;
            }


            #region 传送逻辑

            // todo 添加反转逻辑
            if (_chainInputBit != -1 && _signalManager.inputs[_chainInputBit])
            {
                MoveObj(gameObject);
                //Debug.Log("下层传送带开始运行");
            }

            #endregion

            if (_elevatorUpBit != -1 && _elevatorDownBit != -1 &&
                !(_signalManager.inputs[_elevatorUpBit] ^ _signalManager.inputs[_elevatorDownBit])) return;

            #region 上升逻辑

            // todo 要添加步进逻辑
            if (_elevatorUpBit != -1 && _elevatorLimitBit != -1 &&
                !_elevatorState && _signalManager.inputs[_elevatorUpBit] && _signalManager
                    .inputs[_elevatorLimitBit] && !_upSent)
            {
                _elevatorState = !_elevatorState;
                StartCoroutine(Up());
            }

            #endregion

            #region 下降逻辑

            // todo 添加步进逻辑
            if (_elevatorDownBit != -1 && _elevatorLimitBit != -1 &&
                _elevatorState && _signalManager.inputs[_elevatorDownBit] && _signalManager
                    .inputs[_elevatorLimitBit] && !_downSent)
            {
                _elevatorState = !_elevatorState;
                float time = _animator.GetCurrentAnimatorStateInfo(LayerNumber).normalizedTime;
                time = Mathf.Min(time, 1);
                time = Mathf.Max(time, 0); // 动画偏移规范到0~1，实现运行中的打断动画
                if (!_elevatorState)
                {
                    _animator.SetFloat(ElevatorDirectionHash, -.3f);
                    _animator.Play(ElevatorStateHash, LayerNumber, time);
                }

                _downSent = true;
                _upSent = false;
                Debug.Log("电梯下降");
            }

            #endregion

            #region sensor逻辑

            if (_movingBit == -1) return;
            float runTime = _animator.GetCurrentAnimatorStateInfo(LayerNumber).normalizedTime;

            if (runTime > 0 && runTime < 1 && !_movingSent)
            {
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_movingBit, 1));
                _movingSent = true;
                _stopSent = false;
                Debug.Log("电梯运行中");
            }

            if ((runTime < 0 || runTime > 1) && !_stopSent)
            {
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_movingBit, 0));
                _movingSent = false;
                _stopSent = true;
                Debug.Log("电梯停止运行");
            }

            #endregion
        }

        private IEnumerator Up()
        {
            yield return new WaitForSeconds(1f);
            float time = _animator.GetCurrentAnimatorStateInfo(LayerNumber).normalizedTime;
            time = Mathf.Min(time, 1);
            time = Mathf.Max(time, 0); // 动画偏移规范到0~1，实现运行中的打断动画
            if (_elevatorState)
            {
                _animator.SetFloat(ElevatorDirectionHash, 1f);
                _animator.Play(ElevatorStateHash, LayerNumber, time);
            }

            _upSent = true;
            _downSent = false;
            Debug.Log("电梯上升");
        }


        public void SetBit(Palletizer palletizer)
        {
            _elevatorUpBit = palletizer.GetActuatorBit(AElevatorUp);
            _elevatorDownBit = palletizer.GetActuatorBit(AElevatorDown);
            _elevatorLimitBit = palletizer.GetActuatorBit(AElevatorLimit);
            _chainInputBit = palletizer.GetActuatorBit(AChainInput);
            _chainOutputBit = palletizer.GetActuatorBit(AChainOutput);
            _movingBit = palletizer.GetSensorBit(SElevatorMoving);
            _animator = palletizer.GetComponent<Animator>();
        }
    }
}
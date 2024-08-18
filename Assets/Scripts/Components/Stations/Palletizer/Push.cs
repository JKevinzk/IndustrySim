using FrameWork.Comm;
using UnityEngine;
using static Components.Stations.Palletizer.PalletizerName;

namespace Components.Stations.Palletizer
{
    public class Push : MonoBehaviour, IPalletizerPart
    {
        private AnimatorStateInfo _pusherStateInfo;
        private Animator _animator;

        private bool _limitedSent; // 到达极限指令是否已发送
        private bool _unlimitedSent; // 未到达极限指令是否已发送
        private bool _pushState; // 推杆当前状态
        
        private readonly SignalManager _signalManager = SignalManager.Instance;
        private readonly RunScene _runScene = RunScene.Instance;
        private const int LayerNumber = 0;
        
        private int _limitBit;
        private int _pushBit;

        private void Start()
        {
            _limitedSent = false;
            _unlimitedSent = false;
            _pushState = false;
            _limitBit = _pushBit = -1;
        }

        private void FixedUpdate()
        {
            if (!_runScene.isRun) return;

            #region actuator

            // 当pusher状态与期望不符，更改状态
            if (_pushBit != -1 && _pushState ^ _signalManager.inputs[_pushBit])
            {
                _pushState = !_pushState;

                // 动画播放
                float time = _animator.GetCurrentAnimatorStateInfo(LayerNumber).normalizedTime;
                time = Mathf.Min(time, 1);
                time = Mathf.Max(time, 0); // 动画偏移规范到0~1，实现运行中的打断动画
                // 正放\倒放
                if (_pushState)
                {
                    _animator.SetFloat(PushDirectionHash, 1f);
                    _animator.Play(PushStateNameHash, LayerNumber, time);
                }
                else
                {
                    _animator.SetFloat(PushDirectionHash, -1f);
                    _animator.Play(PushStateNameHash, LayerNumber, time);
                }
            }

            #endregion

            #region sensor

            if (_limitBit == -1) return;
            _pusherStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (_pusherStateInfo.normalizedTime >= 1.0f || _pusherStateInfo.normalizedTime <= 0f)
            {
                if (_limitedSent) return;
                // 发送极限指令
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_limitBit, 1));
                _limitedSent = true;
                _unlimitedSent = false;
                Debug.Log("PUSH极限指令已发送");
            }
            else
            {
                if (_unlimitedSent) return;
                // 发送未到达极限指令
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_limitBit, 0));
                _unlimitedSent = true;
                _limitedSent = false;
                Debug.Log("PUSH未到达极限指令已发送");
            }

            #endregion
        }

        public void SetBit(Palletizer palletizer)
        {
            _limitBit = palletizer.GetSensorBit(SPusherLimit);
            _pushBit = palletizer.GetActuatorBit(APush);
            _animator = palletizer.GetComponent<Animator>();
        }
    }
}
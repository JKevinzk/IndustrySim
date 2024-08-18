using System;
using FrameWork.Comm;
using UnityEngine;
using static Components.Stations.Palletizer.PalletizerName;

namespace Components.Stations.Palletizer
{
    public class Clamp : MonoBehaviour, IPalletizerPart
    {
        private AnimatorStateInfo _clampStateInfo;
        private Animator _animator;

        private bool _limitedSent; // 已夹紧指令是否已发送
        private bool _unlimitedSent; // 未夹紧指令是否已发送
        private bool _clampState; // 推杆当前状态


        private readonly SignalManager _signalManager = SignalManager.Instance;
        private readonly RunScene _runScene = RunScene.Instance;
        private const int LayerNumber1 = 3;
        private const int LayerNumber2 = 4;


        private int _clampedBit;
        private int _clampBit;

        // Start is called before the first frame update
        void Start()
        {
            _limitedSent = false;
            _unlimitedSent = false;
            _clampState = false;
            _clampBit = _clampedBit = -1;
        }

        private void FixedUpdate()
        {
            if (!_runScene.isRun) return;

            #region actuator

            // 当clamp状态与期望不符，更改状态
            if (_clampBit != -1 && _clampState ^ _signalManager.inputs[_clampBit])
            {
                _clampState = !_clampState;

                // 动画播放
                float time = _animator.GetCurrentAnimatorStateInfo(LayerNumber1).normalizedTime;
                time = Mathf.Min(time, 1);
                time = Mathf.Max(time, 0); // 动画偏移规范到0~1，实现运行中的打断动画
                // 正放\倒放
                if (_clampState)
                {
                    _animator.SetFloat(ClampDirectionHash, 1f);
                    _animator.Play(ClampStateHash, LayerNumber1, time);
                    _animator.Play(ClampStateHash, LayerNumber2, time);
                    Debug.Log("clamp夹紧");
                }
                else
                {
                    _animator.SetFloat(ClampDirectionHash, -1f);
                    _animator.Play(ClampStateHash, LayerNumber1, time);
                    _animator.Play(ClampStateHash, LayerNumber2, time);
                    Debug.Log("夹具回位");
                }
            }

            #endregion

            #region sensor

            if (_clampedBit == -1) return;
            _clampStateInfo = _animator.GetCurrentAnimatorStateInfo(LayerNumber1);
            if (_clampStateInfo.normalizedTime >= 1.0f)
            {
                if (_limitedSent) return;
                // 发送极限指令
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_clampedBit, 1));
                _limitedSent = true;
                _unlimitedSent = false;
                Debug.Log("Clamp夹紧指令已发送");
            }
            else
            {
                if (_unlimitedSent) return;
                // 发送未到达极限指令
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_clampedBit, 0));
                _unlimitedSent = true;
                _limitedSent = false;
                Debug.Log("Clamp未夹紧指令已发送");
            }

            #endregion
        }


        public void SetBit(Palletizer palletizer)
        {
            _clampedBit = palletizer.GetSensorBit(SClamped);
            _clampBit = palletizer.GetActuatorBit(AClamp);
            _animator = palletizer.GetComponent<Animator>();
        }
    }
}
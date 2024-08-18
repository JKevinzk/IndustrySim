using FrameWork.Comm;
using UnityEngine;
using static Components.Stations.Palletizer.PalletizerName;

namespace Components.Stations.Palletizer
{
    public class Plate : MonoBehaviour, IPalletizerPart
    {
        private AnimatorStateInfo _plateStateInfo;
        private Animator _animator;

        private bool _limitedSent; // 到达极限指令是否已发送
        private bool _unlimitedSent; // 未到达极限指令是否已发送
        private bool _plateState; // 平台当前状态


        private readonly SignalManager _signalManager = SignalManager.Instance;
        private readonly RunScene _runScene = RunScene.Instance;
        private const int LayerNumber1 = 5;
        private const int LayerNumber2 = 6;


        private int _limitBit ;
        private int _plateBit ;

        private void Start()
        {
            _limitedSent = false;
            _unlimitedSent = false;
            _plateState = false;
            _limitBit = _plateBit = -1;
        }

        private void FixedUpdate()
        {
            if (!_runScene.isRun)
            {
                _limitedSent = false;
                return;
            }

            #region actuator

            // 当plate状态与期望不符，更改状态
            if (_plateBit != -1 && _plateState ^ _signalManager.inputs[_plateBit])
            {
                _plateState = !_plateState;

                // 动画播放
                float time = _animator.GetCurrentAnimatorStateInfo(LayerNumber1).normalizedTime;
                time = Mathf.Min(time, 1);
                time = Mathf.Max(time, 0); // 动画偏移规范到0~1，实现运行中的打断动画
                // 正放\倒放
                if (_plateState)
                {
                    _animator.SetFloat(PlateDirectionHash, 1f);
                    _animator.Play(PlateStateNameHash, LayerNumber1, time);
                    _animator.Play(PlateStateNameHash, LayerNumber2, time);
                }
                else
                {
                    _animator.SetFloat(PlateDirectionHash, -1f);
                    _animator.Play(PlateStateNameHash, LayerNumber1, time);
                    _animator.Play(PlateStateNameHash, LayerNumber2, time);
                }
            }

            #endregion

            #region sensor

            if (_limitBit == -1) return;
            _plateStateInfo = _animator.GetCurrentAnimatorStateInfo(LayerNumber1);
            if (_plateStateInfo.normalizedTime >= 1.0f || _plateStateInfo.normalizedTime <= 0f)
            {
                if (_limitedSent) return;
                // 发送极限指令
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_limitBit, 1));
                _limitedSent = true;
                _unlimitedSent = false;
                Debug.Log("Plate极限指令已发送");
            }
            else
            {
                if (_unlimitedSent) return;
                // 发送未到达极限指令
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_limitBit, 0));
                _unlimitedSent = true;
                _limitedSent = false;
                Debug.Log("Plate未到达极限指令已发送");
            }

            #endregion
        }

        public void SetBit(Palletizer palletizer)
        {
            _limitBit = palletizer.GetSensorBit(SPlateLimit);
            _plateBit = palletizer.GetActuatorBit(AOpenPlate);
            _animator = palletizer.GetComponent<Animator>();
        }
    }
}
using System;
using FrameWork.Comm;
using UnityEngine;
using static Components.Stations.Palletizer.PalletizerName;

namespace Components.Stations.Palletizer
{
    public class Belt : LoadBelt, IPalletizerPart
    {
        private int _beltInputBit = -1;
        private int _beltOutputBit = -1;

        private SignalManager _signalManager = SignalManager.Instance;
        private RunScene _runScene = RunScene.Instance;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
        }

        private void FixedUpdate()
        {
            if (!_runScene.isRun) return;
            // todo 添加反转逻辑
            if (_beltInputBit != -1 && _signalManager.inputs[_beltInputBit])
                // 上层进料传送
            {
                // foreach (var loadObject in loadSet)
                // {
                //     if (loadObject.bearObject == gameObject)
                //
                //         MoveObj(loadObject.gameObject);
                // }
                MoveObj(gameObject);
                //Debug.Log("上层传送带开始运行");
            }
        }

        public void SetBit(Palletizer palletizer)
        {
            _beltInputBit = palletizer.GetActuatorBit(ABeltInput);
            _beltOutputBit = palletizer.GetActuatorBit(ABeltOutput);
        }
    }
}
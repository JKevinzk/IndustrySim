using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.Comm;
using UnityEngine;

namespace Components.Stations.Palletizer
{
    /// <summary>
    /// 电梯的传感器感应逻辑
    /// </summary>
    public class ElevatorLimit : MonoBehaviour
    {
        protected int bit = -1;
        protected bool state; // 是否有货物处于其中
        private bool _hitSent;
        private bool _emptySent;

        private float _time;
        private bool _timeStart;

        private readonly RunScene _runScene = RunScene.Instance;
        private readonly SignalManager _signalManager = SignalManager.Instance;

        private Dictionary<string, PlacedObjectTypeSO> _componentManagerDic;

        private GameObject currentGameObject;

        protected void Start()
        {
            _hitSent = false;
            _emptySent = false;
            state = false;
            _componentManagerDic = ComponentManager.Instance.compoDict;
            _time = 0f;
            _timeStart = false;
        }
        

        protected virtual void OnTriggerEnter(Collider other)
        {
            string objName = other.gameObject.name;
            if (!_componentManagerDic.ContainsKey(objName) ||
                _componentManagerDic[objName].componentType != ComponentManager.ComponentClasses.ItemComponent)
                return;
            // 如果当前物体重复进入，排除再次进入的情况
            if (other.gameObject == currentGameObject) return;
            // 如果不处于运行态、未配置信号、已经发送过探测指令，直接返回
            if (!_runScene.isRun || bit == -1 || _hitSent) return;
            _signalManager.sensorInstructionQueue.Enqueue(((ushort)bit, 1));
            currentGameObject = other.gameObject;
            state = true;
            _hitSent = true;
            _emptySent = false;
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            string objName = other.gameObject.name;
            if (!_componentManagerDic.ContainsKey(objName) ||
                _componentManagerDic[objName].componentType != ComponentManager.ComponentClasses.ItemComponent)
                return;
            if (!_runScene.isRun || bit == -1 || _emptySent) return;
            _timeStart = true;
            _time = 0f;
        }

        private void FixedUpdate()
        {
            if (_time > 0.2)
            {
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)bit, 0));
                state = false;
                _hitSent = false;
                _emptySent = true;
                _time = 0;
                _timeStart = false;
            }

            if (_timeStart)
                _time += Time.fixedDeltaTime;
        }
    }
}
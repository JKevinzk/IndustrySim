using System;
using System.Collections.Generic;
using Components.Items;
using FrameWork.Comm;
using UnityEngine;

namespace Components
{
    public class BearComponent : BaseComponent,IBaseSignalOperation
    {
        private int _actuatorId;
        private int _bit = -1;

        private HashSet<ItemComponent> _loadSet;
        private Rigidbody _rigidbody;
        public float moveSpeed = 0.6f; // 移动速度
        private bool _isMove;
        private readonly SignalManager _signalManager = SignalManager.Instance;
        private readonly RunScene _runScene = RunScene.Instance;

        //public Direct direction = Direct.Forward;
        //Vector3 Dir = Vector3.forward;

        /// <summary>
        /// 设置运动方向
        /// right Vector3.right,left Vector3.left,forward Vector3.forward ,back Vector3.back
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _isMove = false;
            _loadSet = new HashSet<ItemComponent>();
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _rigidbody.mass = 100;
            
        }

        public void OnCollisionStay(Collision collisionInfo)
        {
            GameObject loadObject = collisionInfo.gameObject;
            if (collisionInfo.gameObject != null)
            {
                // 确保只提取传送带与item物体之间的碰撞
                if (collisionInfo.gameObject.layer == LayerMask.NameToLayer("Drag") &&
                    ComponentManager.Instance.compoDict[collisionInfo.gameObject.name].componentType ==
                    ComponentManager.ComponentClasses.ItemComponent &&
                    loadObject.GetComponent<ItemComponent>().bearObject == null
                   )
                {
                    // 加入负载池
                    _loadSet.Add(loadObject.GetComponent<ItemComponent>());
                    // 接管负载对象运动权限
                    loadObject.GetComponent<ItemComponent>().bearObject = gameObject;
                    //Debug.Log("有物体进入" + _actuatorId + "号传送带");
                }
            }
        }

        public void OnCollisionExit(Collision other)
        {
            ItemComponent loadComponent = other.gameObject.GetComponent<ItemComponent>();
            if (!_loadSet.Contains(loadComponent)) return;
            if (loadComponent.bearObject == gameObject)
                loadComponent.bearObject = null;
            _loadSet.Remove(loadComponent);
            //Debug.Log("有物体离开" + _actuatorId + "号传送带");
        }

        private void FixedUpdate()
        {
            // 编写组件运行逻辑，根据信号表查找信号值，进行对应的运动逻辑
            try
            {
                if (_bit != -1 && _runScene.isRun)
                {
                    _isMove = _signalManager.inputs[_bit];
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                Debug.LogWarning("组件" + _actuatorId + "未获取到信号");
            }

            if (_isMove && _runScene.isRun)
            {
                var position = _rigidbody.position;
                _rigidbody.position  += Vector3.back * moveSpeed * Time.fixedDeltaTime;
                _rigidbody.MovePosition(position);
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _actuatorId = _signalManager.ActuatorId;
            var info = GetComponent<ObjectInfo>();
            info.actuatorDic.Add(_actuatorId, "");
            _signalManager.actuatorObjDic.Add(_actuatorId, info.ObjId);
        }

        /// <summary>
        /// 删除物体
        /// </summary>
        public override void OnExit()
        {
            ClearLoadCmp();
            _signalManager.actuatorObjDic.Remove(_actuatorId);
            _signalManager.actuatorIdBitDic.Remove(_actuatorId);
            base.OnExit();
        }

        public override void OnFreeze()
        {
            base.OnFreeze();
        }

        public override void UnFreeze()
        {
            base.UnFreeze();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// 将负载组件的负载对象池清空。
        /// </summary>
        private void ClearLoadCmp()
        {
            foreach (var load in _loadSet)
            {
                if (load.bearObject == gameObject)
                    load.bearObject = null;
            }

            _loadSet.Clear();
        }

        public void DeleteLoadCmp(ItemComponent item)
        {
            _loadSet.Remove(item);
        }

        public void SetSignalBit()
        {
            _bit = _signalManager.actuatorIdBitDic[_actuatorId];
        }

        public void ResetSignalBit()
        {
            _bit = -1;
        }
    }
}
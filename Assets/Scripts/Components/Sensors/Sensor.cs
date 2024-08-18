using System;
using FrameWork.Comm;
using UnityEngine;

namespace Components.Sensors
{
    public class Sensor : BaseComponent, IBaseSignalOperation
    {
        private int _bit = -1;
        [SerializeField] private int sensorId;

        private Ray _ray;
        private RaycastHit _hitInfo;
        private Vector3 _rayDir;

        private readonly SignalManager _signalManager = SignalManager.Instance;
        private readonly RunScene _runScene = RunScene.Instance;

        private GameObject _hitObj;

        private bool _hitSent; // 碰撞信息是否已发送
        private bool _emptySent; // 未探测到信息是否已发送

        private float _time;

        private Renderer _renderer;


        protected override void Start()
        {
            base.Start();
            _emptySent = false;
            _hitSent = false;
            _time = 0;
            _renderer = GetComponentInChildren<Renderer>();
        }

        private void FixedUpdate()
        {
            if (!_runScene.isRun) return;
            // 探测物体
            _hitObj = SensorDetect();
            if (_hitObj != null)
            {
                // 向指定位置发送信号
                if (_bit == -1 || _hitSent) return;
                //Debug.Log(sensorId + "号感应器感应物体经过");
                _signalManager.sensorInstructionQueue.Enqueue(((ushort)_bit, 1));
                _hitSent = true;
                _emptySent = false;
            }
            else
            {
                if (_bit == -1 || _emptySent) return;

                if (_time > 0.1) // 等待一物理帧再发送离开,为了防止传送带会把物体往反方向带一物理帧
                {
                    _signalManager.sensorInstructionQueue.Enqueue(((ushort)_bit, 0));
                    _emptySent = true;
                    _hitSent = false;
                    _time = 0f;
                    Debug.Log(sensorId + "号感应器感应物体离开");
                }

                _time += Time.fixedDeltaTime;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            sensorId = _signalManager.SensorId;
            var info = GetComponent<ObjectInfo>();
            info.sensorDic.Add(sensorId, "");
            _signalManager.sensorObjDic.Add(sensorId, info.ObjId);
        }

        public override void OnExit()
        {
            _signalManager.sensorObjDic.Remove(sensorId);
            _signalManager.sensorIdBitDic.Remove(sensorId);
            base.OnExit();
        }

        public void SetSignalBit()
        {
            _bit = _signalManager.sensorIdBitDic[sensorId];
        }

        public void ResetSignalBit()
        {
            _bit = -1;
        }


        /// <summary>
        /// 检测物体
        /// </summary>
        /// <returns></returns>
        private GameObject SensorDetect()
        {
            _rayDir = transform.rotation * new Vector3(1, 0, 0);
            var startPoint = transform.position;
            startPoint.y += _renderer.bounds.extents.y * 1.2f;
            startPoint.z -= _renderer.bounds.extents.z * 0.3f;
            _ray = new Ray(startPoint, _rayDir);
            Debug.DrawRay(startPoint, _ray.direction, Color.red);
            if (Physics.Raycast(_ray, out _hitInfo, 10000, 1 << 7))
            {
                try
                {
                    if (_hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Drag")) return null;
                    return ComponentManager.Instance.compoDict[_hitInfo.collider.gameObject.name].componentType ==
                           ComponentManager.ComponentClasses.ItemComponent
                        ? _hitInfo.collider.gameObject
                        : null;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Debug.Log(_hitInfo.collider.gameObject.name);
                    throw;
                }
            }

            return null;
        }
    }
}
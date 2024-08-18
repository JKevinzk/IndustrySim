using System.Collections.Generic;
using FactoryClass.Utils;
using UnityEngine;

namespace Components
{
    public class BaseComponent : MonoBehaviour
    {
        protected struct link
        {
            public SenserComponent senser;
            public string eventType;
        }

        #region 组件基本信息

        public bool isMoveY;
        public float mouseOriginY; // 垂直移动鼠标原始位置
        public Vector3 objectOriginPos; // 垂直移动物体原始位置

        protected string _name;
        protected GameObject _gameObject;
        protected Transform _transform;
        protected int width;
        protected int height;
        protected bool isFreeze;
        protected bool isStart;
        protected Dictionary<string, link> links;
        protected bool rotX, rotY, rotZ; // 是否可以在指定方向进行旋转
        public bool isEmit;

        private Quaternion _tarRot; // 目标旋转方向
        private bool _isRotate; // 是否发生旋转
        private const float Speed = 6f; // 旋转速度
        private float? _minY, _maxY, _y; // 高度调整边界鼠标值, 物体实际高度极限值

        #endregion


        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
            if (_isRotate)
                gameObject.transform.rotation = Quaternion.Lerp(gameObject.transform.rotation, _tarRot,
                    0.01f * Speed);
            if (gameObject.transform.rotation == _tarRot)
                _isRotate = false;
            if (isMoveY)
                MoveY();
        }


        /// <summary>
        /// 将组件置于场景中时调用。
        /// </summary>
        public virtual void OnEnter()
        {
            links = new Dictionary<string, link>();
            isFreeze = false;
            isStart = false;
            rotX = rotY = rotZ = true;
            _tarRot = transform.rotation;
            _isRotate = false;
            isMoveY = false;
            isEmit = false;
        }

        /// <summary>
        /// 将组件从场景中移除时调用。
        /// </summary>
        public virtual void OnExit()
        {
            //Destroy(gameObject);
        }

        /// <summary>
        /// 当组件被设置冻结（不可移动）时调用。
        /// </summary>
        public virtual void OnFreeze()
        {
            isFreeze = true;
        }

        /// <summary>
        /// 当组件被解除冻结（不可移动）时调用。
        /// </summary>
        public virtual void UnFreeze()
        {
            isFreeze = false;
        }

        /// <summary>
        /// 当组件被开启时调用。
        /// </summary>
        public virtual void OnStart()
        {
            OnFreeze();
            isStart = true;
        }

        /// <summary>
        /// 当组件被关闭时调用。
        /// </summary>
        public virtual void OnPause()
        {
            UnFreeze();
            isStart = false;
        }

        /// <summary>
        /// 负责链接组件和传感器
        /// </summary>
        /// <param name="senser"></param>
        /// <param name="evt"></param>
        public void OnLink(SenserComponent senser, string evt, string del_name, EventModule.CallBack del)
        {
            link lk;
            lk.senser = senser;
            lk.eventType = evt;

            EventModule.AddListener(evt, del);

            if (links.ContainsKey(del_name))
            {
                links[del_name] = lk;
            }
            else
            {
                links.Add(del_name, lk);
            }
        }

        /// <summary>
        /// 负责断开链接组件和传感器
        /// </summary>
        /// <param name="senser"></param>
        /// <param name="evt"></param>
        public void DisLink(string del_name, EventModule.CallBack del)
        {
            if (links.ContainsKey(del_name))
            {
                string evt = links[del_name].eventType;

                EventModule.RemoveListener(evt, del);

                links.Remove(del_name);
            }
        }

        /// <summary>
        /// 物体发生旋转
        /// </summary>
        /// <param name="isRotateX">是否在X轴方向发生旋转</param>
        /// <param name="isRotateY">是否在Y轴方向发生旋转</param>
        /// <param name="isRotateZ">是否在Z轴方向发生旋转</param>
        /// <param name="flag">默认true为正转，false为反转</param>
        public void Rotate(bool isRotateX = false, bool isRotateY = false, bool isRotateZ = false, bool flag = true)
        {
            if (flag)
            {
                if (isRotateX || isRotateY || isRotateZ)
                {
                    if (isRotateX)
                        _tarRot = Quaternion.AngleAxis(90, new Vector3(1, 0, 0)) * _tarRot;
                    if (isRotateY)
                        _tarRot = Quaternion.AngleAxis(90, new Vector3(0, 1, 0)) * _tarRot;
                    if (isRotateZ)
                        _tarRot = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * _tarRot;
                    _isRotate = true;
                }
            }
            else
            {
                if (isRotateX || isRotateY || isRotateZ)
                {
                    if (isRotateX)
                        _tarRot = Quaternion.AngleAxis(-90, new Vector3(1, 0, 0)) * _tarRot;
                    if (isRotateY)
                        _tarRot = Quaternion.AngleAxis(-90, new Vector3(0, 1, 0)) * _tarRot;
                    if (isRotateZ)
                        _tarRot = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1)) * _tarRot;
                    _isRotate = true;
                }
            }
        }

        // 实现物体竖直方向的移动,当发生碰撞时，不能向下移动
        private void MoveY()
        {
            float mouse_y; //鼠标在Y轴上的偏移
            float moveSpeed = 0.01f;
            Vector3 pos = objectOriginPos; // 鼠标移动目标位置
            UtilsClass.SetCursor(UtilsClass.CursorType.HAND);
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            mouse_y = Input.mousePosition.y;
            Collision collision = GetComponent<ObjectInfo>().collisionInfo;
            // 发生碰撞时，保持当前位置
            // if (collision != null)
            // {
            //     // 第一次发生碰撞
            //     if (_y == null)
            //         // 下边界碰撞
            //         if (mouse_y < mouseOriginY)
            //         {
            //             _y = collision.GetContact(0).point.y;
            //             _minY = mouse_y;
            //         }
            //         // 上边界碰撞
            //         else
            //         {
            //             _y = collision.GetContact(0).point.y - GetComponent<ObjectBound>().DeltaY;
            //             _maxY = mouse_y;
            //         }
            //     else
            //     {
            //         // 对于刚开始就碰撞的物体，更新鼠标边界值
            //         if (_minY != null)
            //             _minY = mouseOriginY;
            //         if (_maxY != null)
            //             _maxY = mouseOriginY;
            //     }
            // }
            // // 无碰撞发生，初始化各边界值
            // else
            // {
            //     _minY = _maxY = _y = null;
            // }
            //
            // // 发生碰撞，高度取极限
            //
            // if (_y != null && (mouse_y < _minY || mouse_y > _maxY))
            // {
            //     pos.y = (float)_y;
            // }
            // else
            //     pos.y += (mouse_y - mouseOriginY) * moveSpeed;
            pos.y += (mouse_y - mouseOriginY) * moveSpeed;
            pos.y = Mathf.RoundToInt(pos.y / 0.125f) * 0.125f;

            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
                UtilsClass.FixVerticalPos(gameObject.transform, pos), Time.deltaTime * Speed);

            //左击取消垂直移动
            if (Input.GetMouseButtonDown(0))
            {
                isMoveY = false;
                gameObject.transform.position = UtilsClass.FixVerticalPos(gameObject.transform, pos);
                UtilsClass.SetCursor(UtilsClass.CursorType.DEFAULT);
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                GetComponent<ObjectInfo>().Y_Value = gameObject.transform.position.y;
                DragObject.isMovingY = false; // 解除垂直移动模式
                // 更新物体位置
                ComponentManager.Instance.UpdateObjInfo();
            }
        }


        // public virtual void SetSignalBit(){}
        //
        //
        // public virtual void ResetSignal(){}
    }

    public interface IBaseSignalOperation
    {
        void SetSignalBit();
        void ResetSignalBit();
    }
}
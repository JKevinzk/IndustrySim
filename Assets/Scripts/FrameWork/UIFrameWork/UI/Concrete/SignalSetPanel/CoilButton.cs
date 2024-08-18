using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete.SignalSetPanel
{
    public class CoilButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        private bool _leftMouseButtonUp; // 鼠标左键-抬起.

        private enum MouseButton
        {
            LEFT = 0,
        }

        private SensorButton _mDestroy; //获取跟随鼠标移动的组件

        private GameObject _mSensorCreate; //点击之后新创建的sensor物体

        private GameObject _mCreate; //获取预制体

        private GameObject _mCloneCoil; //获取标签为CloneCoil的对象

        private bool _mFlag; //鼠标是否在按钮里

        public int sensorId;

        void Start()
        {
            _mCreate = Resources.Load<GameObject>("Prefabs/UI/Component/sensor"); //获取预制体
        }

        // 根据标签获取组件
        void LateUpdate()
        {
            UpdateInput();

            if (_mSensorCreate)
            {
                _mSensorCreate.transform.position = Input.mousePosition;
                // if (_mFlag && _leftMouseButtonUp)
                // {
                //     GetComponentInChildren<Text>().text =
                //         GameObject.FindWithTag("CloneCoil").GetComponentInChildren<Text>().text; //获取文本信息
                //     Destroy(_mSensorCreate);
                // }
            }


            if (_leftMouseButtonUp)
            {
                // 处理从sensor面板拖到PLC接口的情况
                if (_mDestroy != null)
                {
                    if (_mDestroy.Get_m_MouseState() && _mFlag)
                    {
                        GetComponentInChildren<Text>().text = _mDestroy.GetComponentInChildren<Text>().text; //获取文本信息
                        sensorId = _mDestroy.sensorId;
                        _mDestroy.Set_m_MouseState(false);
                    }

                    Destroy(_mDestroy.gameObject); //销毁由senor产生的物体
                }

                // 处理coil接口到其他coil接口的情况
                if (_mCloneCoil != null)
                {
                    if (_mFlag)
                    {
                        var cloneCoil = GameObject.FindWithTag("CloneCoil");
                        GetComponentInChildren<Text>().text =
                            cloneCoil.GetComponentInChildren<Text>().text; //获取文本信息
                        sensorId = cloneCoil.GetComponentInChildren<SensorButton>().sensorId;
                    }

                    Destroy(_mCloneCoil); //销毁由coil产生的物体
                }
            }
        }


        private void UpdateInput()
        {
            _leftMouseButtonUp = Input.GetMouseButtonUp((int)MouseButton.LEFT); // 鼠标左键-抬起
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Debug.Log("进去了");

            _mFlag = true;
            if (GameObject.FindWithTag("CloneSensor") != null)
            {
                _mDestroy = GameObject.FindWithTag("CloneSensor").GetComponent<SensorButton>();
                _mDestroy.Set_m_MouseState(true); //进入coil按钮
            }

            if (GameObject.FindWithTag("CloneCoil"))
            {
                _mCloneCoil = GameObject.FindWithTag("CloneCoil");
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GetComponentInChildren<Text>().text != "")
            {
                Transform grandParentTrs = transform.parent.parent;
                grandParentTrs.GetComponent<GridLayoutGroup>().enabled = false; //避免在跟随鼠标位置之前，根据布局移动位置
                _mSensorCreate = Instantiate(_mCreate, grandParentTrs);

                _mSensorCreate.transform.position = Input.mousePosition;

                _mSensorCreate.GetComponent<GraphicRaycaster>().enabled = false; //避免生成的物体进行UI事件检测
                _mSensorCreate.tag = "CloneCoil";
                _mSensorCreate.GetComponentInChildren<Text>().text = GetComponentInChildren<Text>().text;
                _mSensorCreate.GetComponentInChildren<SensorButton>().sensorId = sensorId;
                sensorId = -1;
                GetComponentInChildren<Text>().text = null;

                _mCloneCoil = GameObject.FindWithTag("CloneCoil");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_mDestroy)
            {
                _mDestroy.Set_m_MouseState(false);
            }

            _mFlag = false;
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete.SignalSetPanel
{
    public class InputButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
    {
        private bool _leftMouseButtonUp; // 鼠标左键-抬起.

        private enum MouseButton
        {
            LEFT = 0,
        }

        private ActuatorButton _mDestroy; //获取跟随鼠标移动的组件

        private GameObject _mActuatorCreate; //点击之后新创建的actuator物体

        private GameObject _mCreate; //获取预制体

        private GameObject _mCloneInput; //获取标签为CloneCoil的对象

        private bool _mFlag;

        public int actuatorId;

        void Start()
        {
            _mCreate = Resources.Load<GameObject>("Prefabs/UI/Component/actuator"); //获取预制体
        }

        void LateUpdate()
        {
            UpdateInput();

            if (_mActuatorCreate)
            {
                _mActuatorCreate.transform.position = Input.mousePosition;
                // if (_mFlag && _leftMouseButtonUp)
                // {
                //     this.GetComponentInChildren<Text>().text =
                //         GameObject.FindWithTag("CloneInput").GetComponentInChildren<Text>().text; //获取文本信息
                //     Destroy(_mActuatorCreate);
                // }
            }

            if (_leftMouseButtonUp)
            {
                if (_mDestroy != null)
                {
                    if (_mDestroy.Get_m_MouseState() && _mFlag)
                    {
                        GetComponentInChildren<Text>().text =
                            _mDestroy.GetComponentInChildren<Text>().text; //获取文本信息
                        actuatorId = _mDestroy.actuatorId;
                        _mDestroy.Set_m_MouseState(false);
                    }

                    Destroy(_mDestroy.gameObject); //销毁由senor产生的物体
                }

                if (_mCloneInput != null)
                {
                    if (_mFlag)
                    {
                        var cloneInput = GameObject.FindWithTag("CloneInput");
                        GetComponentInChildren<Text>().text =
                            cloneInput.GetComponentInChildren<Text>().text; //获取文本信息
                        actuatorId = cloneInput.GetComponentInChildren<ActuatorButton>().actuatorId;
                    }

                    Destroy(_mCloneInput); //销毁由coil产生的物体
                }
            }
        }

        private void UpdateInput()
        {
            _leftMouseButtonUp = Input.GetMouseButtonUp((int)MouseButton.LEFT); // 鼠标左键-抬起
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mFlag = true;
            if (GameObject.FindWithTag("CloneActuator") != null)
            {
                _mDestroy = GameObject.FindWithTag("CloneActuator").GetComponent<ActuatorButton>();
                _mDestroy.Set_m_MouseState(true); //进入Input按钮
            }

            if (GameObject.FindWithTag("CloneInput"))
            {
                _mCloneInput = GameObject.FindWithTag("CloneInput");
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GetComponentInChildren<Text>().text != "")
            {
                Transform grandParentTrs = transform.parent.parent;
                grandParentTrs.GetComponent<GridLayoutGroup>().enabled = false;
                _mActuatorCreate = Instantiate(_mCreate, grandParentTrs);

                _mActuatorCreate.transform.position = Input.mousePosition;
                
                _mActuatorCreate.GetComponent<GraphicRaycaster>().enabled = false;
                _mActuatorCreate.tag = "CloneInput";
                _mActuatorCreate.GetComponentInChildren<Text>().text = GetComponentInChildren<Text>().text;
                _mActuatorCreate.GetComponentInChildren<ActuatorButton>().actuatorId = actuatorId;
                
                actuatorId = -1;
                GetComponentInChildren<Text>().text = null;

                _mCloneInput = GameObject.FindWithTag("CloneInput");
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
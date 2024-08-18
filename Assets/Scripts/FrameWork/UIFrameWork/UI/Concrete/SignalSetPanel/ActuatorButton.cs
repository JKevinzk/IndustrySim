using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete.SignalSetPanel
{
    public class ActuatorButton : MonoBehaviour, IPointerDownHandler
    {
        private bool _leftMouseButtonUp; // 鼠标左键-抬起.

        private ActuatorButton _mActuatorButton;

        private enum MouseButton
        {
            LEFT = 0,
        }

        private Vector3 _rectOffset;
        private bool _mMouseState; // 表示当前鼠标是否已点击sensor按钮且未松开

        public int actuatorId;

        private void LateUpdate()
        {
            UpdateInput();
            if (_mActuatorButton != null)
            {
                _mActuatorButton.transform.position = Input.mousePosition - _rectOffset;
                if (_leftMouseButtonUp && !_mActuatorButton._mMouseState)
                {
                    Destroy(_mActuatorButton.gameObject);
                }
            }
        }

        private void UpdateInput()
        {
            _leftMouseButtonUp = Input.GetMouseButtonUp((int)MouseButton.LEFT); // 鼠标左键-抬起
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
            _rectOffset = Input.mousePosition - transform.position;
            _mActuatorButton = Instantiate(this, transform.parent);
            _mActuatorButton.transform.position = Input.mousePosition - _rectOffset;
            _mActuatorButton.gameObject.GetComponent<GraphicRaycaster>().enabled = false;
            _mActuatorButton._mMouseState = false; //表示生成的sensors不在coil中，而是无效区或空白区
            _mActuatorButton.tag = "CloneActuator"; //设置标签用于查询获取
        }

        public void Set_m_MouseState(bool flag)
        {
            _mMouseState = flag;
        }

        public bool Get_m_MouseState()
        {
            return _mMouseState;
        }
    }
}
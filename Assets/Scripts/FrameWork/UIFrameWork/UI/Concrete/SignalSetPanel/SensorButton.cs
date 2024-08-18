using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FrameWork.UIFrameWork.UI.Concrete.SignalSetPanel
{
    public class SensorButton: MonoBehaviour,IPointerDownHandler
    {
        private bool _leftMouseButtonUp;                                       // 鼠标左键-抬起.
        private SensorButton _mSensorButton;
        private enum MouseButton
        {
            LEFT = 0,
            RIGHT,
            MIDDLE
        }

        private Vector3 _rectOffset;
        private bool _mMouseState;// 表示当前鼠标是否已点击sensor按钮且未松开

        public int sensorId;
    
        private void LateUpdate()
        {
            UpdateInput();
            if (_mSensorButton != null )
            {
                _mSensorButton.transform.position = Input.mousePosition-_rectOffset;
                if (_leftMouseButtonUp && !_mSensorButton._mMouseState )
                {
                    Destroy(_mSensorButton.gameObject);
                }
            }
        }
        //获取鼠标输入
        private void UpdateInput()
        {
            _leftMouseButtonUp = Input.GetMouseButtonUp((int)MouseButton.LEFT);             // 鼠标左键-抬起
        }

        //鼠标点击后生成新sensor组件
        public void OnPointerDown(PointerEventData eventData)
        {
            this.transform.parent.GetComponent<GridLayoutGroup>().enabled = false;
            _rectOffset = Input.mousePosition-this.transform.position ;
        
            _mSensorButton = Object.Instantiate(this, this.transform.parent);
        
            _mSensorButton.transform.position = Input.mousePosition-_rectOffset;
            _mSensorButton.gameObject.GetComponent<GraphicRaycaster>().enabled = false;
            _mSensorButton._mMouseState = false;//表示生成的sensors不在coil中，而是无效区或空白区
            _mSensorButton.tag = "CloneSensor";//设置标签用于查询获取

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
    

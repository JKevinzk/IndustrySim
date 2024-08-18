using Components.Project.Emitter;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class PartObjectPanel : BasePanel
    {
       private const string Path = "Prefabs/UI/Panel/PartObjectPanel";
        private readonly Vector3 _position;
        private GameObject _objectGroup;
        private const float Height = 175f; // 面板高度
        private const float Width = 200f; // 面板宽度
        private const float RightContextWidth = 300f; // 右键菜单宽度
        
        private Emitter _emitter;

        public PartObjectPanel(Vector3 position) : base(new UIType(Path))
        {
            _position = position;
        }

        public override void onEnter()
        {
            base.onEnter();
            _objectGroup = UITool.FindChildGameObject("ObjectGroup");
            _emitter = DragObject.Instance.ObjectInfo.GetComponent<Emitter>();
            SetPosition();
            CreateBase();
        }

        /// <summary>
        /// 设置面板的位置
        /// </summary>
        private void SetPosition()
        {
            // 初始位置
            var v = _position;
            v.y = _position.y + Height-Screen.height;
            v.x += RightContextWidth;
            
            // 垂直方向超出界面
            if (v.y - Height < -Screen.height)
            {
                v.y += v.y + Height - Screen.height;
            }
            
            // 水平方向超出界面
            if (v.x + Width > Screen.width)
            {
                v.x -= RightContextWidth + Width;
            } 
            _objectGroup.transform.position = v;
        }

        /// <summary>
        /// 设置产生物体的逻辑，并写入发射器属性
        /// </summary>
        private void CreateBase()
        {
            Toggle[] toggles = _objectGroup.GetComponentsInChildren<Toggle>();
            foreach (var toggle in toggles)
            {
                string objectName = toggle.transform.parent.name;
                if (_emitter.partObjects.Contains(objectName))
                    toggle.isOn = true;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn) _emitter.partObjects.Add(objectName);
                    else _emitter.partObjects.Remove(objectName);
                });
            }
        }
    }
}

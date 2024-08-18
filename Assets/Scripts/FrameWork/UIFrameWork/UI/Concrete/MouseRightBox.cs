using System;
using System.Linq;
using System.Net.Mime;
using Components;
using FactoryClass.Utils;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class MouseRightBox : BasePanel
    {
        private const string Path = "Prefabs/UI/Panel/MouseRightPanel";

        public MouseRightBox(GameObject dragObj) : base(new UIType(Path))
        {
            _dragObj = dragObj;
        }

        private readonly GameObject _dragObj;


        private float _offSetX; //UI偏移
        private float _offSetY;
        private bool canCopy = true;
        private GameObject copyInistateObj;
        private bool isRotate_x, isRotate_y, isRotate_z, isCopy, isMoveY;

        private bool isUI;
        private GameObject rightBox;
        private Button[] rightButton;
        private RectTransform rightRect;
        private BaseComponent _baseComponent;


        // 为支持旋转动画，需要开启协程而创建的空类
        private class MonoStub : MonoBehaviour
        {
        }

        public override void onEnter()
        {
            // 初始化操作物体信息
            base.onEnter();
            //rightBox = UITool.GetOrAddComponent<RectTransform>().gameObject;
            rightBox = UITool.FindChildGameObject("ButtonGroup");
            rightButton = rightBox.GetComponentsInChildren<Button>();
            _baseComponent = _dragObj.GetComponent<BaseComponent>();

            // 偏移加10px 防止手型图标遮挡
            _offSetX = rightBox.GetComponent<RectTransform>().rect.width;
            _offSetY = rightBox.GetComponent<RectTransform>().rect.height;

            UtilsClass.SetCursor(UtilsClass.CursorType.DEFAULT);

            // 设置右键菜单位置
            MouseRight();
            // 添加按钮监听
            PanelFunction();
            // 设置按钮功能显示
            ButtonShow();
        }


        private void MouseRight()
        {
            if (!RunScene.Instance.isRun)
            {
                /*设定UI位置*/
                //ChoosePivot();
                var position = Input.mousePosition;
                position.x += 30; // 增加偏移以防止鼠标图标遮挡
                position.y -= _offSetY;
                // 超出屏幕范围移动菜单位置
                if (position.x + _offSetX > Screen.width)
                    position.x -= _offSetX;
                if (position.y < 0)
                    position.y += _offSetY;
                rightBox.transform.position = position;
            }
        }

        private void ChoosePivot()
        {
            float width = Screen.width / 2f;
            float height = Screen.height / 2f;
            if (Input.mousePosition.x < width)
                rightBox.GetComponent<RectTransform>().pivot =
                    new Vector2(0, rightBox.GetComponent<RectTransform>().pivot.y);
            else
                rightBox.GetComponent<RectTransform>().pivot =
                    new Vector2(1, rightBox.GetComponent<RectTransform>().pivot.y);

            if (Input.mousePosition.y < height * 1.5)
                rightBox.GetComponent<RectTransform>().pivot =
                    new Vector2(rightBox.GetComponent<RectTransform>().pivot.x, 0);
            else
                rightBox.GetComponent<RectTransform>().pivot =
                    new Vector2(rightBox.GetComponent<RectTransform>().pivot.x, 1);
        }

        #region 面板功能

        private void PanelFunction()
        {
            for (var i = 0; i < rightButton.Length; i++)
            {
                if (rightButton[i] == null) return;
                var name = rightButton[i].name;
                switch (name)
                {
                    case "YawAdd":
                        rightButton[i].onClick.AddListener(() => { _baseComponent.Rotate(isRotateY: true); });
                        break;
                    case "RollAdd":
                        rightButton[i].onClick.AddListener(() => { _baseComponent.Rotate(isRotateX: true); });
                        break;
                    case "PitchAdd":
                        rightButton[i].onClick.AddListener(() => { _baseComponent.Rotate(isRotateZ: true); });
                        break;
                    case "YawDel":
                        rightButton[i].onClick.AddListener(() =>
                        {
                            _baseComponent.Rotate(isRotateY: true, flag: false);
                        });
                        break;
                    case "RollDel":
                        rightButton[i].onClick.AddListener(() =>
                        {
                            _baseComponent.Rotate(isRotateX: true, flag: false);
                        });
                        break;
                    case "PitchDel":
                        rightButton[i].onClick.AddListener(() =>
                        {
                            _baseComponent.Rotate(isRotateZ: true, flag: false);
                        });
                        break;
                    case "MoveHorizon":
                        rightButton[i].onClick.AddListener(MoveHorizon);
                        break;
                    case "HorizanTransButton":
                        rightButton[i].onClick.AddListener(MoveHorizon);
                        break;
                    case "VerticalTransButton":
                        rightButton[i].onClick.AddListener(IsMoveY);
                        break;
                    case "CopyButton":
                        rightButton[i].onClick.AddListener(Copy);
                        break;
                    case "DeleteButton":
                        rightButton[i].onClick.AddListener(Delete);
                        break;
                    case "BaseButton":
                        rightButton[i].onClick.AddListener(BaseToEmit);
                        break;
                    case "PartButton":
                        rightButton[i].onClick.AddListener(PartToEmit);
                        break;
                }
            }
        }


        private void Copy()
        {
            PanelManger.Pop();

            Vector3 point;


            var Y_Value = _dragObj.GetComponent<ObjectInfo>().Y_Value;
            point = UtilsClass.GetGridWordPositionWithYPlane(Y_Value);


            if (canCopy)
            {
                copyInistateObj = Object.Instantiate(_dragObj);
                copyInistateObj.name = ComponentManager.ProcessName(copyInistateObj.name);
                canCopy = false;
            }

            if (copyInistateObj != null)
            {
                copyInistateObj.transform.position = Vector3.Lerp(copyInistateObj.transform.position,
                    UtilsClass.FixAxisToBottomLeft(copyInistateObj, point), Time.deltaTime * 10);

                if (Math.Abs((copyInistateObj.transform.position - point).magnitude
                             - UtilsClass.GetSize(copyInistateObj).magnitude / 2) < 0.1)
                {
                    //存储COPY对象的信息
                    copyInistateObj.name = ComponentManager.ProcessName(copyInistateObj.name);
                    ComponentManager.Instance.SaveObjInfo(copyInistateObj);

                    isCopy = false;
                    canCopy = true;
                    copyInistateObj = null;
                }
            }
        }

        private void Delete()
        {
            PanelManger.Pop();

            for (var i = 0; i < ComponentManager.Instance.compoInSceneDic.Count; i++)
            {
                var obj = ComponentManager.Instance.compoInSceneDic.ElementAt(i).Value;
                if (obj.transform.position == _dragObj.transform.position)
                {
                    ComponentManager.Instance.DelObjInfoByIndex(i);
                    _dragObj.GetComponent<BaseComponent>().OnExit();
                }
            }

            Object.Destroy(_dragObj);
        }

        private void IsMoveY()
        {
            PanelManger.Pop();
            _baseComponent.mouseOriginY = Input.mousePosition.y;
            _baseComponent.objectOriginPos = _dragObj.transform.position;
            _baseComponent.isMoveY = true;
            DragObject.isMovingY = true; // 开启垂直移动模式
        }


        private void MoveHorizon()
        {
            PanelManger.Pop();
        }

        //翻转
        private void IsRotateX()
        {
            _baseComponent.Rotate(isRotateX: true, flag: true);
        }

        //正转
        private void IsRotateY()
        {
            _baseComponent.Rotate(isRotateY: true, flag: true);
        }

        //侧转
        private void IsRotateZ()
        {
            _baseComponent.Rotate(isRotateZ: true, flag: true);
        }

        // 托盘发射面板
        private void BaseToEmit()
        {
            if (UITool.IsPanelInUIRoot("BaseObjectPanel", out _)) return;
            if (UITool.IsPanelInUIRoot("PartObjectPanel", out _)) Pop();
            Push(new BaseObjectPanel(rightBox.transform.position)); // 显示货物发射选项面板
        }

        // 货物发射面板
        private void PartToEmit()
        {
            if (UITool.IsPanelInUIRoot("PartObjectPanel", out _)) return;
            if (UITool.IsPanelInUIRoot("BaseObjectPanel", out _)) Pop();
            Push(new PartObjectPanel(rightBox.transform.position)); // 显示货物发射选项面板
        }

        #endregion

        /// <summary>
        /// 特定物品，按钮功能显示
        /// </summary>
        private void ButtonShow()
        {
            // 旋转
            
            // 发射
            if (!_baseComponent.isEmit)
            {
                Button baseButton = UITool.GetComponentInChildren<Button>("BaseButton");
                baseButton.interactable = false;
                baseButton.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.5f);
                Button partButton = UITool.GetComponentInChildren<Button>("PartButton");
                partButton.interactable = false;
                partButton.GetComponentInChildren<Text>().color = new Color(1, 1, 1, 0.5f);
            }
        }
    }
}
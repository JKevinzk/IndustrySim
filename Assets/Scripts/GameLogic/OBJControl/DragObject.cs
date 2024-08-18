using System;
using System.Collections;
using System.Collections.Generic;
using FactoryClass.Utils;
using FrameWork.UIFrameWork.UI.Concrete;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = System.Object;

public class DragObject : MonoBehaviour
{
    //只对指定的层级进行拖动
    [SerializeField] private LayerMask uiLayerMask = (1 << 5);
    [SerializeField] private LayerMask dragLayerMask = 1 << 7;
    [SerializeField] private LayerMask groundLayerMask = 1 << 6;


    //public GameObject dragObject; //拖动物体的位置
    public bool isDragging; // 当前是否有物体被拖动
    public static bool isMovingY; // 当前是否处于垂直移动模式
    public static bool isViewing; // 当前是否处于视角改变模式

    private bool isRun;

    //物体的信息
    public ObjectInfo ObjectInfo { get; private set; }

    //当前需要拖动对象的坐标相对于鼠标在世界空间坐标中的偏移量
    private int DragOffsetX;
    private int DragOffsetZ;


    //存储当前需要拖动的对象在屏幕空间中的坐标
    private Vector3 screenPos = Vector3.zero;
    private int OriginMousePosX;
    private int OriginMousePosZ;
    private int OriginObjPosX;
    private int OriginObjPosZ;

    public static DragObject Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isDragging = false;
        isMovingY = false;
        isViewing = false;
    }

    private void Update()
    {
        if (UtilsClass.IsPointerOverUIObject() || isMovingY || isViewing)
        {
            return;
        }

        Choose();
    }

    private void FixedUpdate()
    {
        if (UtilsClass.IsPointerOverUIObject() || isMovingY || isViewing)
        {
            return;
        }

        Move();
    }


    private void Choose()
    {
        // 鼠标按下，选中物体
        if (Input.GetMouseButtonDown(0))
        {
            if (UITool.IsPanelInUIRoot("FileCanvas", out _))
            {
                GameRoot.Instance.Pop();
            }
            // 如果右键面板打开，点击其他地方时关闭面板
            StartCoroutine(PopRightPanel());
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //如果当前对象与指定的层级发生碰撞，表示当前对象可以被拖动
            if (Physics.Raycast(ray, out var hitInfo, 1000f, dragLayerMask))
            {
                // 在选中移动物体前，进行判断是否当前有选中物体，有的话取消其包围盒显示
                if (ObjectInfo) ObjectInfo.IsChoose = false;
                // 获取选中物体信息
                ObjectInfo = hitInfo.transform.gameObject.GetComponentInParent<ObjectInfo>();
                ObjectInfo.IsChoose = true; //物体被选中
                ObjectInfo.IsDragging = true;

                //获取物体原先网格位置，以左下为锚点
                GridBuildSystem.Instance.grid.GetXZ(
                    UtilsClass.FixAxisToBottomLeft(ObjectInfo.gameObject, ObjectInfo.gameObject.transform.position),
                    out OriginObjPosX, out OriginObjPosZ);

                // 获取鼠标与物体所在平面交点
                var point = UtilsClass.FixAxisToBottomLeft(ObjectInfo.gameObject,
                    UtilsClass.GetGridWordPositionWithYPlane(ObjectInfo.Y_Value));
                GridBuildSystem.Instance.grid.GetXZ(point, out OriginMousePosX, out OriginMousePosZ);

                //取得网格偏差
                DragOffsetX = OriginObjPosX - OriginMousePosX;
                DragOffsetZ = OriginObjPosZ - OriginMousePosZ;


                GridBuildSystem.ShowGrid();
                isDragging = true;
            }
            else
            {
                // 点击右键菜单会导致UI界面射线阻挡，所以这里要处理其特殊情况
                CancelDrag();
            }
        }

        // 鼠标抬起，结束拖拽
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            if (ObjectInfo)
            {
                ObjectInfo.IsDragging = false; // 结束拖拽状态
            }

            GridBuildSystem.HideGrid(); // 隐藏网格
        }

        //鼠标右键选中
        if (Input.GetMouseButtonUp(1))
        {
            if (RunScene.Instance.isRun) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 1000, dragLayerMask))
            {
                if (ObjectInfo) ObjectInfo.IsChoose = false;
                ObjectInfo = hitInfo.collider.gameObject.GetComponentInParent<ObjectInfo>();
                ObjectInfo.IsChoose = true; //物体被选中
                StartCoroutine(PopRightPanel(true));
                GameRoot.Instance.Push(new MouseRightBox(ObjectInfo.gameObject)); // 显示右键菜单
            }
            else
            {
                CancelDrag();
                StartCoroutine(PopRightPanel());
            }
        }
    }

    private void Move()
    {
        if (Input.GetMouseButton(0) && isDragging)
        {
            //运行状态的拖拽
            if (RunScene.Instance.isRun)
            {
                var currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);
                // var currentPos = Camera.main.ScreenToWorldPoint(currentScreenPos) + RunOffset;
                var currentPos = Camera.main.ScreenToWorldPoint(currentScreenPos);
                //dragTransform.position = UtilsClass.FixAxisToBottomLeft(dragTransform.gameObject, currentPos);
            }
            //放置状态的拖拽
            else
            {
                var yValue = ObjectInfo.Y_Value;

                // 获取鼠标发出的射线到物体所在平面交点，并修正到物体左下角
                var mousePoint = UtilsClass.FixAxisToBottomLeft(ObjectInfo.gameObject,
                    UtilsClass.GetGridWordPositionWithYPlane(yValue));
                // 获取网格坐标
                GridBuildSystem.Instance.grid.GetXZ(mousePoint, out var gridX, out var gridZ);
                // 获取世界坐标
                var point = GridBuildSystem.Instance.grid.GetWorldPosition(ObjectInfo.gameObject, gridX + DragOffsetX,
                    gridZ + DragOffsetZ) + new Vector3(0, yValue, 0);
                ObjectInfo.gameObject.transform.position = Vector3.Lerp(ObjectInfo.gameObject.transform.position,
                    point, Time.deltaTime * 10);
            }
        }
    }

    public void CancelDrag()
    {
        if (!ObjectInfo) return;
        ObjectInfo.IsChoose = false;
        ObjectInfo = null;
        isDragging = false;
    }


    private IEnumerator PopRightPanel(bool flag=false)
    {
        int rightPanelNumber = 0;
        while (UITool.IsPanelInUIRoot("MouseRightPanel", out GameObject panel))
        {
            GameRoot.Instance.Pop();
            if (panel.name == "MouseRightPanel") rightPanelNumber++;
            yield return null;
        }

        if (flag && rightPanelNumber > 1)
            GameRoot.Instance.Push(new MouseRightBox(ObjectInfo.gameObject));
    }
}
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using FactoryClass.Utils;

public class SelectObjManager : MonoBehaviour
{
    public static SelectObjManager Instance
    {
        get { return _instance; }
    }

    private static SelectObjManager _instance;

    private float _zDistance = 50f; //物体z轴距摄像机的长度
    //private float _scaleFactor = 1f;//对象的缩放系数

    private bool isDragging;
    private bool isPlaceSuccess = false; //是否是有效的放置（如果放置在地面上返回True,否则为False）
    [SerializeField] private LayerMask _groundLayerMask; //地面层级
    public GameObject currentPlaceObj = null; //当前要被放置的对象
    public PlacedObjectTypeSO currentPlaceTypeSO;

    void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        isDragging = false;
    }


    void Update()
    {
        if (currentPlaceObj == null) return;

        if (isDragging && Input.GetMouseButton(0)) MoveCurrentPlaceObj();
        if (isDragging && !Input.GetMouseButton(0)) CheckIfPlaceSuccess(); // 拖拽方式创建的物体
    }

    //让当前对象跟随鼠标移动

    void MoveCurrentPlaceObj()
    {
        isDragging = true;
        Vector3 point;
        Vector3 screenPosition;
        screenPosition = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, _groundLayerMask))
        {
            GridBuildSystem.Instance.grid.GetXZ(hitInfo.point, out int x, out int z);

            point = GridBuildSystem.Instance.grid.GetWorldPosition(x, z);

            isPlaceSuccess = true;
        }

        else
        {
            point = ray.GetPoint(_zDistance);
            isPlaceSuccess = false;
        }

        currentPlaceObj.GetComponent<DrawLine>().ClearPoints();
        currentPlaceObj.GetComponent<ObjectBound>().AddPoints();
        currentPlaceObj.GetComponent<DrawLine>().ShowGrid();
        currentPlaceObj.transform.position = Vector3.Lerp(currentPlaceObj.transform.position,
            UtilsClass.FixAxisToBottomLeft(currentPlaceObj, point), Time.deltaTime * 10);
    }

    //在指定位置实例化一个对象
    void CreatePlaceObj()
    {
        string createName = ComponentManager.ProcessName(currentPlaceObj.name);
        //GameObject obj = ComponentManager.Instance.CreatComponentByName(createName);
        GameObject obj = ComponentManager.Instance.CreatComponentByTypeSO(currentPlaceTypeSO);
        obj.transform.position = currentPlaceObj.transform.position;
        Destroy(currentPlaceObj);
        //存储信息
        ComponentManager.Instance.SaveObjInfo(obj);
    }

    /// <summary>
    ///检测是否放置成功
    /// </summary>
    void CheckIfPlaceSuccess()
    {
        if (isPlaceSuccess)
        {
            CreatePlaceObj();
        }

        isDragging = false;
        currentPlaceObj.SetActive(false);
        currentPlaceObj = null;
    }

    /// <summary>
    /// 将要创建的对象传递给当前对象管理器
    /// </summary>
    /// <param name="newObject"></param>
    public void AttachNewObject(GameObject newObject)
    {
        // 获取新创建对象
        if (currentPlaceObj)
        {
            currentPlaceObj.SetActive(false);
        }

        currentPlaceObj = newObject;
        currentPlaceObj.AddComponent<DrawLine>();
        currentPlaceObj.AddComponent<ObjectBound>();
        isDragging = true;
    }

    public void AttachNewObject(PlacedObjectTypeSO placedObjectTypeSo)
    {
        // 获取新创建对象
        if (currentPlaceObj)
        {
            currentPlaceObj.SetActive(false);
        }

        currentPlaceTypeSO = placedObjectTypeSo;
        currentPlaceObj = Instantiate(placedObjectTypeSo.prefab);

        currentPlaceObj.name = ComponentManager.ProcessName(currentPlaceObj.name);
        ComponentUtil.SetComponentSize(currentPlaceObj);
        currentPlaceObj.AddComponent<DrawLine>();
        currentPlaceObj.AddComponent<ObjectBound>();

        isDragging = true;
    }
}
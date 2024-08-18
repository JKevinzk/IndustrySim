using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Components;
using UnityEngine;
using FactoryClass.Utils;

public class GridBuildSystem : MonoBehaviour
{
    private static GridBuildSystem _instance;
    public static GridBuildSystem Instance { get { return _instance; } }

    public GridXZ<GridObject> grid;
    [SerializeField] private Transform testTranform;
    [SerializeField] private LayerMask colliderLayerMask;
    [SerializeField] private List<PlacedObjectTypeSO> placedObjectTypeSOList;
    
    //地板大小60 * 36 m，12.5cm * 12.5cm 为 1个格子，大格子 2m = 12.5 * 16, 总格子数 = 288 * 480
    //由于精度限制，将cellSize放大4倍，格子数宽高缩小4倍
    // int gridWidth = 288 / 4;
    private const int GridWidth = 288;

    // int gridHeight = 480 / 4;
    private const int GridHeight = 480;

    // float cellSize = 0.5f;
    public const float CellSize = 0.125f;
    int gridBoldLineNum = 4;

    public GameObject floor;
    public Material matFloorWithGrid;
    public Material matFloor;
    
    private List<Vector3[]> m_linePoints = new List<Vector3[]>();//线段的起始点
    private static GameObject _gridGameObject;
   
    
    private void Awake()
    {
        _instance = this;
        
        grid = new GridXZ<GridObject>(GridWidth, GridHeight, CellSize, gridBoldLineNum, 
            new Vector3 (-18, 0, -30), 
            (GridXZ<GridObject> g, int x, int z) => new GridObject(g, x, z));
        
        m_linePoints = grid.m_linePoints;
        UITool uiTool = new UITool(GameObject.Find("GridInsideGroup"));
        _gridGameObject = GameObject.Find("Grid");
        GameObject gridInside = uiTool.GetComponentInChildren<Transform>("GridInside").gameObject;
        float x = gridInside.transform.position.x;
        float z = gridInside.transform.position.z;
        for (var i = 0; i < 36; i+=2)
        {
            for (var j = 0; j < 60; j+=2)
            {
                GameObject gridClone = Instantiate(gridInside,gridInside.transform.parent);
                gridClone.transform.position =new Vector3(x+i, 0.01f, z +j);
                gridClone.SetActive(true);
            }
        }
        HideGrid();
        //GetComponent<DrawLine>().Init(m_linePoints);
    }
    
    public class GridObject
    {
        private GridXZ<GridObject> grid;
        private int x;
        private int z;
        private Transform transform;
        //private PlacedObject placedObject;
        public GridObject(GridXZ<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(Transform transform)
        {
            this.transform = transform;
            grid.TriggerGridObjectChanged(x, z);
        }

        public Transform GetPlacedObject()
        {
            return transform;
        }
        public void ClearPlacedObject()
        {
            transform = null;
        }
        public bool CanBuild()
        {
            return transform == null;
        }
        public override string ToString()
        {
            return x + "," + z + "\n" + transform;
        }
    }

    public static void ShowGrid()
    {
        _gridGameObject.SetActive(true);
    }

    public static void HideGrid()
    {
        _gridGameObject.SetActive(false);
    }
    
    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycast, 1000f, colliderLayerMask))
        {
            return raycast.point;
        }
        else { return Vector3.zero; }
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryClass.Utils;


public class GridXZ<TGridObject> 
{

    public event EventHandler<OnGridObjectChangedEventArgs> OnGridObjectChanged;
    public class OnGridObjectChangedEventArgs : EventArgs
    {

        public int x;
        public int z;
    }
    public TGridObject[,] gridArray;
    public int width;
    public int height;
    public float cellSize;
    public int gridBoldLineNum;
    private Color gridColor = new Color(216 / 256f, 191 / 256f, 216 / 256f, 0.1f);
    private Color gridBoldColor = new Color(252 / 256f, 252 / 256f, 245 / 256f, 0.2f);
    //[JsonIgnore]
    public TextMesh[,] DebugTextArray { get; private set; }
    private Vector3 originPosition;
    
    //线段的起始点
    public List<Vector3[]> m_linePoints = new List<Vector3[]>();


    public GridXZ(int width, int height, float cellSize, int gridBoldLineNum, Vector3 originPosition, Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.gridBoldLineNum = gridBoldLineNum;

        gridArray = new TGridObject[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                gridArray[x, z] = createGridObject(this, x, z);
            }
        }
        
        InitializeDebugTextArray(width, height, cellSize, gridBoldLineNum);

    }

    public void InitializeDebugTextArray(int width, int height, float cellSize, int gridBoldLineNum)
    {
        DebugTextArray = new TextMesh[width, height];
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {

                if (x % gridBoldLineNum == 0 && z % gridBoldLineNum == 0)
                {
                    // Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + gridBoldLineNum), gridBoldColor, 200f);
                    // Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + gridBoldLineNum, z), gridBoldColor, 200f);
                    m_linePoints.Add(new []{GetWorldPosition(x, z), GetWorldPosition(x, z + gridBoldLineNum)});
                    m_linePoints.Add(new []{GetWorldPosition(x, z), GetWorldPosition(x + gridBoldLineNum, z)});
                }
                else
                {
                    // Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), gridColor, 200f);
                    // Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), gridColor, 200f);
                    m_linePoints.Add(new []{GetWorldPosition(x, z), GetWorldPosition(x, z + 1)});
                    m_linePoints.Add(new []{GetWorldPosition(x, z), GetWorldPosition(x + 1, z)});
                }
            }
        }
        
        

    }

    public void SetDebugText(int x, int z, string text)
    {
        DebugTextArray[x, z].text = text;
    }

    public Vector3 GetWorldPosition(GameObject gameObject, int x, int z)
    {
        var offsetX = ComponentManager.Instance.compoDict[gameObject.name].width/2;
        var offsetZ = ComponentManager.Instance.compoDict[gameObject.name].length/2;
        var point = new Vector3(x, 0, z) * cellSize + originPosition + new Vector3(offsetX, 0, offsetZ);
        // 设定位置区间
        if (point.x > 18) point.x = 18;
        if (point.x < -18) point.x = -18;
        if (point.z > 30) point.z = 30;
        if (point.z < -30) point.z = -30;
        return point;
    } 
    public Vector3 GetWorldPosition( int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    public void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        x = Mathf.RoundToInt((worldPosition - originPosition).x / cellSize);
        z = Mathf.RoundToInt((worldPosition - originPosition).z / cellSize);
    }

    public void SetGridObject(int x, int z, TGridObject value)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            gridArray[x, z] = value;
            TriggerGridObjectChanged(x, z);
        }
    }

    public void TriggerGridObjectChanged(int x, int z)
    {
        OnGridObjectChanged?.Invoke(this, new OnGridObjectChangedEventArgs { x = x, z = z });
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXZ(worldPosition, out int x, out int z);
        SetGridObject(x, z, value);
    }

    //根据网格坐标获取对象
    public TGridObject GetGridObject(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < height)
        {
            return gridArray[x, z];
        }
        else
        {
            return default(TGridObject);
        }
    }

    //根据世界坐标获取对象
    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        int x, z;
        GetXZ(worldPosition, out x, out z);
        return GetGridObject(x, z);
    }
}
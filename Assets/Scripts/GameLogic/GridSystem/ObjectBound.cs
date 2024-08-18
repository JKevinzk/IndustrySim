using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;

[RequireComponent(typeof(DrawLine))]
public class ObjectBound : MonoBehaviour
{
    public bool isMove;
    private Vector3[] _points;
    private Vector3[] _corners;
    private Bounds _bounds;
    private Quaternion _originRotation;
    private float _deltaX;
    private float _deltaY;
    private float _deltaZ;

    public float DeltaY => _deltaY;
    private Dictionary<string, PlacedObjectTypeSO> _comDictionary;

    public void Awake()
    {
        _points = new Vector3[8];
        _corners = new Vector3[8];
        _bounds = GetBounds(gameObject);
        _originRotation = transform.rotation;
        _deltaX = Mathf.Abs(_bounds.extents.x);
        _deltaY = Mathf.Abs(_bounds.extents.y);
        _deltaZ = Mathf.Abs(_bounds.extents.z);
        isMove = false;
        UpdateCorner(_bounds.center);
    }

    private void Start()
    {
        _comDictionary = ComponentManager.Instance.compoDict;
        var x = _comDictionary[gameObject.name].width;
        var z = _comDictionary[gameObject.name].length;
        if (x == 0 || z == 0)
            Debug.LogWarning("当前设备未设置长或宽,设备名称：" + gameObject.name);
        else
        {
            _deltaX = x / 2;
            _deltaZ = z / 2;
        }
    }


    /// <summary>
    /// 获取所有子物体整体的包围盒
    /// </summary>
    /// <returns></returns>
    public static Bounds GetBounds(GameObject component)
    {
        Bounds bounds = default;
        if (component.GetComponent<Renderer>())
        {
            bounds = component.GetComponent<Renderer>().bounds;
            return bounds;
        }

        bool isEmpty = true;
        foreach (Transform trChild in component.GetComponentsInChildren<Transform>())
        {
            if (trChild.GetComponent<Renderer>())
                if (isEmpty)
                {
                    bounds = trChild.GetComponent<Renderer>().bounds;
                    isEmpty = false;
                }
                else
                    bounds.Encapsulate(trChild.GetComponent<Renderer>().bounds);
        }

        return bounds;
    }

    private void UpdateCorner(Vector3 center)
    {
        _corners[0] = center + new Vector3(-_deltaX, _deltaY, -_deltaZ); // 上前左（相对于中心点）
        _corners[1] = center + new Vector3(_deltaX, _deltaY, -_deltaZ); // 上前右
        _corners[2] = center + new Vector3(_deltaX, _deltaY, _deltaZ); // 上后右
        _corners[3] = center + new Vector3(-_deltaX, _deltaY, _deltaZ); // 上后左

        _corners[4] = center + new Vector3(-_deltaX, -_deltaY, -_deltaZ); // 下前左
        _corners[5] = center + new Vector3(_deltaX, -_deltaY, -_deltaZ); // 下前右
        _corners[6] = center + new Vector3(_deltaX, -_deltaY, _deltaZ); // 下后右
        _corners[7] = center + new Vector3(-_deltaX, -_deltaY, _deltaZ); // 下后左
        for (int i = 0; i < _corners.Length; i++)
        {
            _points[i] = _corners[i];
        }
    }

    public void Update()
    {
        if (!isMove) return;
        _bounds = GetBounds(gameObject);
        Vector3 center = _bounds.center;

        UpdateCorner(center);

        // 根据物体旋转修改点的坐标
        for (int i = 0; i < _corners.Length; i++)
        {
            Vector3 dis = _corners[i] - center;
            _points[i] = center + Quaternion.Euler(transform.rotation.eulerAngles - _originRotation.eulerAngles) * dis;
        }
    }

    public void AddPoints()
    {
        DrawLine drawLine = GetComponent<DrawLine>();
        drawLine.AddPoints(new[] { _points[0], _points[1] });
        drawLine.AddPoints(new[] { _points[1], _points[2] });
        drawLine.AddPoints(new[] { _points[2], _points[3] });
        drawLine.AddPoints(new[] { _points[3], _points[0] });
        drawLine.AddPoints(new[] { _points[4], _points[5] });
        drawLine.AddPoints(new[] { _points[5], _points[6] });
        drawLine.AddPoints(new[] { _points[6], _points[7] });
        drawLine.AddPoints(new[] { _points[7], _points[4] });
        drawLine.AddPoints(new[] { _points[0], _points[4] });
        drawLine.AddPoints(new[] { _points[1], _points[5] });
        drawLine.AddPoints(new[] { _points[2], _points[6] });
        drawLine.AddPoints(new[] { _points[3], _points[7] });
        isMove = true;
    }
}
using System;
using System.Collections.Generic;
using Components;
using FactoryClass.Utils;
using UnityEngine;
using UnityEngine.Rendering;


public class ObjectInfo : MonoBehaviour
{
    // 物品相关信息
    private int objId;
    public Dictionary<int, string> sensorDic; // id-传感器名称字典
    public Dictionary<int, string> actuatorDic; // id-执行器名称字典

    private bool _isDragging; // 是否被拖拽
    public float Y_Value { get; set; } // 垂直移动时离地面的值
    public Collision collisionInfo; // 碰撞信息

    // 通用组件
    private Rigidbody _rigidbody;
    private DrawLine _drawLine;
    private Transform objTF;

    public int ObjId
    {
        get => objId;
        set => objId = value;
    }

    public bool IsChoose { get; set; }

    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            _isDragging = value;
            if (value)
            {
                GetComponentInChildren<Rigidbody>().constraints =
                    ~ (RigidbodyConstraints.FreezePositionX |
                       RigidbodyConstraints.FreezePositionZ); // 碰撞相关，选中的物体只能在水平面上移动，而不会产生旋转        
                GetComponentInChildren<Rigidbody>().isKinematic = false;
            }
            else
            {
                GetComponent<ObjectInfo>().GetComponentInChildren<Rigidbody>().constraints =
                    RigidbodyConstraints.FreezeAll;
                GetComponent<ObjectInfo>().GetComponentInChildren<Rigidbody>().isKinematic = true;
            }
        }
    }

    private void Awake()
    {
        objTF = GetComponent<Transform>();
        sensorDic = new Dictionary<int, string>();
        actuatorDic = new Dictionary<int, string>();
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _drawLine = GetComponent<DrawLine>();
        if (gameObject.transform.position.y != 0)
        {
            Y_Value = objTF.position.y;
        }

        if (!RunScene.Instance.isRun)
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            _rigidbody.isKinematic = true;
            _rigidbody.useGravity = false;
        }
        else
        {
            _rigidbody.constraints = RigidbodyConstraints.None;
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
        }

        GetComponent<BaseComponent>()?.OnEnter();
    }

    private void Update()
    {
        if (IsChoose)
        {
            _drawLine.ShowGrid();
        }
        else
        {
            _drawLine.HideGrid();
        }
    }

    // 碰撞检测
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Drag"))
        {
            collisionInfo = collision;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        collisionInfo = null;
    }
}
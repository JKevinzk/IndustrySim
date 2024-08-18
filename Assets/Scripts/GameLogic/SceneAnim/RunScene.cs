using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components;
using UnityEngine;
using UnityEngine.UI;

public class RunScene : MonoBehaviour
{
    private static RunScene _instance;

    public static RunScene Instance
    {
        get { return _instance; }
    }

    private ComponentManager compManager;

    public bool isRun = false;

    public bool isFreeze = false;
    //public bool isReStart = false;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        compManager = ComponentManager.Instance;
    }


    void Update()
    {
        //����ʱ���ò�������
        // if (isRun && !isFreeze)
        // {
        // foreach (var comp in compManager.compoInSceneList)
        // {
        // comp.GetComponent<BaseComponent>().OnStart();
        // }
        // }
    }

    private void ResetObjPos()
    {
        for (int i = 0; i < compManager.compoInSceneDic.Count; i++)
        {
            var obj = compManager.compoInSceneDic.ElementAt(i).Value;
            Vector3 pos = new Vector3((float)compManager.createdObjPosXList[i],
                (float)compManager.createdObjPosYList[i], (float)compManager.createdObjPosZList[i]);
            obj.transform.position = pos;
            Vector3 rot = new Vector3((float)compManager.createdObjRotXList[i],
                (float)compManager.createdObjRotYList[i], (float)compManager.createdObjRotZList[i]);
            obj.transform.rotation = Quaternion.Euler(rot);
        }
    }

    public void Restart()
    {
        Debug.Log("ִ��restart");
        //运行时才能触发restart，效果是场景重新运行动画
        if (isRun)
        {
            // for (int i = 0; i < compManager.compoInSceneList.Count; i++)
            // {
            //     Vector3 pos = new Vector3((float) compManager.createdObjPosXList[i],
            //         (float) compManager.createdObjPosYList[i], (float) compManager.createdObjPosZList[i]);
            //     compManager.compoInSceneList[i].transform.position = pos;
            // }
            ResetObjPos();
        }
    }

    public void OnFreeze()
    {
        isFreeze = !isFreeze;
        Debug.Log($"isFreeze???{isFreeze}");

        foreach (var comp in compManager.compoInSceneDic)
        {
            comp.Value.GetComponent<BaseComponent>().OnFreeze();
        }
    }

    //为false时需要复位 
    public void OnStart()
    {
        isRun = !isRun;
        // 开始运行
        if (isRun)
        {
            // 更新component位置
            GameObject tem = new GameObject("TemporaryObject");
            compManager.UpdateObjInfo();
        }
        // 结束运行
        else
        {
            // 复位
            for (int i = 0; i < compManager.compoInSceneDic.Count; i++)
            {
                ResetObjPos();
            }
            Destroy(GameObject.Find("TemporaryObject"));
        }
        
        //执行物体运行动画                                                                                                                                  
        foreach (var comp in compManager.compoInSceneDic)
        {
            comp.Value.GetComponent<BaseComponent>().OnStart();
        }
    }
}
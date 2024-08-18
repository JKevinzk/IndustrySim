using System;
using System.Collections.Generic;
using System.Linq;
using Components;
using UnityEngine;


public class ComponentManager : MonoBehaviour
{
    private int _objId;
    public List<PlacedObjectTypeSO> allComponents;

    public Dictionary<string, PlacedObjectTypeSO> compoDict;

    public enum ComponentClasses
    {
        All,
        ItemComponent,
        BearComponent,
        SensorComponent,
        StationComponent,
        WarningComponent,
        WalkWayComponent,
        ProjectComponent,
        Unfinished,
        
    }

    //需要存档的信息------------------------------------
    public Dictionary<int, GameObject> compoInSceneDic;
    public List<string> createdObjName; // 组件名称
    public List<double> createdObjPosXList;
    public List<double> createdObjPosYList;
    public List<double> createdObjPosZList;

    public List<double> createdObjRotXList;
    public List<double> createdObjRotYList;

    public List<double> createdObjRotZList;
    //------------------------------------------------

    public static ComponentManager Instance { get; private set; }

    public int ObjId => ++_objId;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _objId = 0;
        compoDict = new Dictionary<string, PlacedObjectTypeSO>();
        foreach (var component in allComponents)
        {
            compoDict.Add(component.name, component);
            //Debug.Log($"ComponentName: {component.name}");
        }

        createdObjPosXList = new List<double>();
        createdObjPosYList = new List<double>();
        createdObjPosZList = new List<double>();
        createdObjRotXList = new List<double>();
        createdObjRotYList = new List<double>();
        createdObjRotZList = new List<double>();
        compoInSceneDic = new Dictionary<int, GameObject>();
    }

    //删除实例化后名字带的(clone)
    public static string ProcessName(string objName)
    {
        if (objName.Length > 7)
        {
            objName = objName.PadRight(7);
            if (objName.EndsWith("(Clone)") || objName.EndsWith("(clone)"))
                objName = objName.Remove(objName.Length - 7, 7);
        }

        return objName;
    }

    public void DestroyAllCreatedObj()
    {
        if (compoInSceneDic.Count != 0)
        {
            for (int i = compoInSceneDic.Count - 1; i >= 0; i--)
            {
                compoInSceneDic.ElementAt(i).Value.GetComponent<BaseComponent>()?.OnExit();
                Destroy(compoInSceneDic.ElementAt(i).Value);
                DelObjInfoByIndex(i);
            }
        }
    }

    /// <summary>
    /// 通过名字实例化零件并配置好相关组件
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public GameObject CreatComponentByName(string objName)
    {
        //此时返回的是预制体对象，由于不可删除，需要进行clone，最后放入SceneList的是克隆后的对象，便于操作
        IComponentModel compo = ComponentFactory.GetModelTypeByName(objName);
        GameObject obj = compo.Creat(objName);
        GameObject objClone = Instantiate(obj);
        objClone.name = ProcessName(obj.name);
        ComponentUtil.SetComponentSize(objClone);
        compo.AddComponent(ref objClone, objName);
        return objClone;
    }

    /// <summary>
    /// 通过物体类型信息进行创建
    /// </summary>
    /// <param name="placedObjectTypeSo">物体类型信息</param>
    /// <returns>返回创建好的物体</returns>
    public GameObject CreatComponentByTypeSO(PlacedObjectTypeSO placedObjectTypeSo)
    {
        IComponentModel compo = ComponentFactory.GetModelTypeByName(placedObjectTypeSo.prefab.name);
        GameObject obj = compo.Creat(placedObjectTypeSo.prefab.name);

        GameObject objClone = Instantiate(obj);
        objClone.name = ProcessName(obj.name);
        ComponentUtil.SetComponentSize(objClone);
        compo.AddComponent(ref objClone, placedObjectTypeSo.prefab.name);
        return objClone;
    }

    //更新模型位置的变动，因为SaveObjInfo只存储了刚创建时的信息
    public void UpdateObjInfo()
    {
        for (int i = compoInSceneDic.Count - 1; i >= 0; i--)
        {
            var obj = compoInSceneDic.ElementAt(i).Value;
            createdObjPosXList[i] = obj.transform.position.x;
            createdObjPosYList[i] = obj.transform.position.y;
            createdObjPosZList[i] = obj.transform.position.z;
            createdObjRotXList[i] = obj.transform.rotation.eulerAngles.x;
            createdObjRotYList[i] = obj.transform.rotation.eulerAngles.y;
            createdObjRotZList[i] = obj.transform.rotation.eulerAngles.z;
        }
    }

    public void SaveObjInfo(GameObject obj)
    {
        compoInSceneDic.Add(obj.GetComponent<ObjectInfo>().ObjId, obj);
        createdObjName.Add(obj.name);
        var position = obj.transform.position;
        createdObjPosXList.Add((position.x));
        createdObjPosYList.Add(position.y);
        createdObjPosZList.Add(position.z);
        var rotation = obj.transform.rotation;
        createdObjRotXList.Add(rotation.x);
        createdObjRotYList.Add(rotation.y);
        createdObjRotZList.Add(rotation.z);
    }

    public void DelObjInfoByIndex(int index)
    {
        // 从组件管理器中删除
        compoInSceneDic.Remove(compoInSceneDic.ElementAt(index).Key);
        createdObjName.RemoveAt(index);
        createdObjPosXList.RemoveAt(index);
        createdObjPosYList.RemoveAt(index);
        createdObjPosZList.RemoveAt(index);
        createdObjRotXList.Remove(index);
        createdObjRotYList.Remove(index);
        createdObjRotZList.Remove(index);

        // 从信号管理器中删除
        Debug.Log("场景中还剩下 " + compoInSceneDic.Count + " 个零件");
    }
}
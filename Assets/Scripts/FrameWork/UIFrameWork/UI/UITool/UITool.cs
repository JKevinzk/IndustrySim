using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;


//UI管理工具
public class UITool
{
    public readonly GameObject activePanel;

    public UITool(GameObject panel)
    {
        activePanel = panel;
    }

    /// <summary>
    /// 判断要推入的面板是否已存在
    /// </summary>
    /// <param name="panelName"></param>
    /// <param name="obj"></param>
    /// <returns>当查询面板已存在时返回真，否则返回假</returns>
    public static bool IsPanelInUIRoot(String panelName, out GameObject obj)
    {
        GameObject uiRoot = GameObject.Find("UIRoot");
        RectTransform[] parent = uiRoot.GetComponentsInChildren<RectTransform>();
        foreach (var item in parent)
        {
            if (item.name == panelName)
            {
                obj = item.gameObject;
                return true;
            }
        }

        obj = null;
        return false;
    }

    /// <summary>
    /// 给当前的活动面板获取或者添加一个组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetOrAddComponent<T>() where T : Component
    {
        if (activePanel.GetComponent<T>() == null) activePanel.AddComponent<T>();
        return activePanel.GetComponent<T>();
    }

    /// <summary>
    /// 根据名称查找子对象
    /// </summary>
    /// <param name="name">对象名称</param>
    /// <param name="includeInactive">是否查找隐藏对象，默认为true</param>
    /// <returns>返回找到的对象</returns>
    public GameObject FindChildGameObject(string name, bool includeInactive = true)
    {
        Transform[] trans = activePanel.GetComponentsInChildren<Transform>(includeInactive);

        foreach (Transform item in trans)
        {
            if (item.name == name)
            {
                return item.gameObject;
            }
        }
        Debug.LogWarning($"{activePanel.name}中找不到{name}子对象");
        return null;
    }

    /// <summary>
    /// 返回所有指定组件
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> FindComponentsInChild<T>(bool includeInactive = true) where T : Component
    {
        T[] trans = activePanel.GetComponentsInChildren<T>(includeInactive);
        if (trans != null)
            return trans;
        Debug.LogWarning($"{activePanel.name}中无子对象");
        return null;
    }

    /// <summary>
    /// 根据名称获取子对象的组件
    /// </summary>
    /// <param name="name">查找对象名称</param>
    /// <param name="includeInactive">是否搜索隐藏对象</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>返回获取的泛型T</returns>
    public T GetComponentInChildren<T>(string name,bool includeInactive = true) where T : Component
    {
        GameObject child = FindChildGameObject(name, includeInactive);

        if (child != null)
        {
            return child.GetComponent<T>() == null ? null : child.GetComponent<T>();
        }

        return null;
    }
    
}
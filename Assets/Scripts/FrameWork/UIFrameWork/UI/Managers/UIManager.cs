using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 存储所有UI信息，并能创建或销毁UI
/// </summary>
public class UIManager
{
    private Dictionary<UIType, GameObject> dictUI { get; }

    public UIManager()
    {
        dictUI = new Dictionary<UIType, GameObject>();
    }


    /// <summary>
    /// 获取UI对象
    /// </summary>
    /// <param name="type">面板类型，包含了路径和名称</param>
    /// <returns>场景中的面板对象</returns>
    public GameObject GetSingleUI(UIType type)
    {
        GameObject parent = GameObject.Find("UIRoot");

        if (parent == null)
        {
            Debug.LogError("Canvas不存在");

            return null;
        }

        if (dictUI.ContainsKey(type))
        {
            return dictUI[type];
        }

        GameObject ui = GameObject.Instantiate(Resources.Load<GameObject>(type.Path), parent.transform);
        ui.name = type.Name;
        ui.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        ui.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        dictUI.Add(type, ui);
        return ui;
    }

    // 销毁UI
    public void DestroyUI(UIType type)
    {
        if (dictUI.ContainsKey(type))
        {
            Object.Destroy(dictUI[type]);
            dictUI.Remove(type);
        }
    }


    //这两个方法需添加canvas group组件
    public void UIShow(GameObject panel)
    {
        panel.GetComponent<CanvasGroup>().alpha = 1;
        panel.GetComponent<CanvasGroup>().interactable = true;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void UIHide(UIType type)
    {
        if (dictUI.ContainsKey(type))
        {
            GameObject panel;
            panel = dictUI[type];
            panel.GetComponent<CanvasGroup>().alpha = 0;
            panel.GetComponent<CanvasGroup>().interactable = false;
            panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void UIHide(GameObject panel)
    {
        if (panel == null) return;
        panel.GetComponent<CanvasGroup>().alpha = 0;
        panel.GetComponent<CanvasGroup>().interactable = false;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
    }
}
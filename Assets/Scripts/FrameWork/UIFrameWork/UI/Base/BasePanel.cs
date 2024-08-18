using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//UI面板的父类，包括UI状态信息
public class BasePanel
{
    //UI信息
    public UIType UIType { get; private set; }

    //UI管理工具
    public UITool UITool { get; private set; }

    //面板管理器
    public PanelManger PanelManger { get; private set; }

    //UI管理器
    public UIManager UIManager { get; private set; }

    public BasePanel(UIType type)
    {
        this.UIType = type;
        //Debug.Log("path = " + UIType.Path);
    }

    public void Initialize(UITool tool)
    {
        UITool = tool;
    }

    public void Initialize(PanelManger panelManger)
    {
        PanelManger = panelManger;
    }

    public void Initialize(UIManager uIManager)
    {
        UIManager = uIManager;
    }

    //UI进入时执行的操作，执行一次
    public virtual void onEnter()
    {
    }

    //UI暂停时执行的操作
    public virtual void OnPause()
    {
        //UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = false;
    }
    //UI继续时执行的操作
    public virtual void OnResume()
    {
        //UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = true;
    }
    //UI 退出时执行的操作
    public virtual void OnExit()
    {
        UIManager.DestroyUI(UIType);
    }

    //显示一个面板
    public void Push(BasePanel panel) => PanelManger?.Push(panel);

    //关闭一个面板
    public void Pop() => PanelManger?.Pop();

}
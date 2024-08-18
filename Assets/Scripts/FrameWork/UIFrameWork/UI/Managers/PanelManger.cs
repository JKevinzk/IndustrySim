using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//面板管理器，存储UI
public class PanelManger 
{

    public Stack<BasePanel> stackPanel { get; }
    private UIManager uiManager;
    private BasePanel panel;

    public PanelManger()
    {
        stackPanel = new Stack<BasePanel>(); 
        uiManager = new UIManager();
    }

    /// <summary>
    /// 入栈，该操作显示一个面板
    /// </summary>
    /// <param name="nextPanel">需要显示的面板</param>
    public void Push(BasePanel nextPanel)
    {
        if ( stackPanel.Count > 0 )
        {
            panel = stackPanel.Peek();

            panel.OnPause();

        }

        stackPanel.Push(nextPanel);
        GameObject panelGo = uiManager.GetSingleUI(nextPanel.UIType);
        nextPanel.Initialize(new UITool(panelGo));
        nextPanel.Initialize(this);
        nextPanel.Initialize(uiManager);
        nextPanel.onEnter();
    } 

    //出栈，执行面板的OnExit方法
    public void Pop()
    {
        if ( stackPanel.Count > 0 )
        {
            //弹出栈顶面板
            stackPanel.Pop().OnExit();
        }
        if ( stackPanel.Count > 0 )
        {
            stackPanel.Peek().OnResume();
        }
    }

    //执行所有面板的OnExit
    public void PopAll()
    {
        while ( stackPanel.Count > 0)
        {
            stackPanel.Pop().OnExit();
        }
    }
}
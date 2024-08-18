using System;
using System.Collections;
using System.Collections.Generic;
using FrameWork.UIFrameWork.UI.Concrete;
using UnityEngine;
using UnityEngine.SceneManagement;

//开始场景的管理
public class StartScene : SceneState
{
    //场景名称
    readonly string sceneName = "Start";
    PanelManger panelManger;
    public override void OnEnter()
    {
        panelManger = new PanelManger();
        if ( SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
            SceneManager.sceneLoaded += SceneLoaded; //场景加载完执行的事件
        }
        else
        {
            panelManger.Push(new StartPanel());
            GameRoot.Instance.SetAction(panelManger.Push, panelManger.Pop);
        }
    }

    public override void OnExit()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        //panelManger.PopAll();
    }

    //场景加载完毕后执行的方法
    private void SceneLoaded(Scene scene, LoadSceneMode load)
    {
        panelManger.Push(new StartPanel());
        GameRoot.Instance.SetAction(panelManger.Push, panelManger.Pop);
        Debug.Log($"{sceneName}场景加载完毕!");
    }
}

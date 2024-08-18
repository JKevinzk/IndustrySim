using System;
using System.Collections;
using System.Collections.Generic;
using FactoryClass.Utils;
using UnityEngine;

//场景的状态管理系统
public class SceneSystem 
{
    //场景状态类
    public SceneState sceneState;

    public String sceneName;
        
    /// <summary>
    /// 设置当前场景并进行当前场景
    /// </summary>
    /// <param name="state"></param>
    public void SetScene(SceneState state,String name)
    {
        if (sceneState != null)
        {
            //场景有更换，则执行前一个场景的退出操作
            sceneState.OnExit();
        }
        sceneState = state; //场景赋值给传入场景
        if (sceneState != null)
        {
            sceneName = name;
            //传入场景不为空，执行进入操作
            sceneState.OnEnter();

        }

        //sceneState?.OnExit();
        //sceneState = state;
        //sceneState?.OnEnter();
    }
    
}

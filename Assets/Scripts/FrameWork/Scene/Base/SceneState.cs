using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//场景状态，定义场景必须要实现的抽象方法
public abstract class SceneState
{
    //场景名称
    // public string sceneName;
    
    //场景进入时执行的操作
    public abstract void OnEnter();

    //场景退出时执行的操作
    public abstract void OnExit();
}

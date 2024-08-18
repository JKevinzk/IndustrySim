using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//此类中定义需要保存的东西
[System.Serializable]
public class Save
{
    public int sceneId = 0;
    public string sceneName = "Default";
    public string sceneInfo = "";
    public string sceneShot = "";
    public class Position
    {
        public double x;
        public double y;
        public double z;
    }

    public List<string> usingGameObjString = new List<string>();//放置的模型名称

    //放置的模型位置
    public List<double> usingPlacedObjPositionX = new List<double>();
    public List<double> usingPlacedObjPositionY = new List<double>();
    public List<double> usingPlacedObjPositionZ = new List<double>();
    
    //放置的模型的旋转属性
    public List<double> usingPlacedOjbRotationX = new List<double>();
    public List<double> usingPlacedOjbRotationY = new List<double>();
    public List<double> usingPlacedOjbRotationZ = new List<double>();
}

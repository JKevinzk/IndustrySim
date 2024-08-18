using System.Collections;
using System.Collections.Generic;
using FrameWork.UIFrameWork.UI.Concrete;
using UnityEngine;

public class StartManger : MonoBehaviour
{
    PanelManger panelManger;
    BasePanel panel;
   
    //static readonly string path = "Prefabs/UI/Panel/WelcomCanvas";

    private void Awake()
    {
        panelManger = new PanelManger();
        
    }

    private void Start()
    {
        panelManger.Push(new StartPanel());
    }
}

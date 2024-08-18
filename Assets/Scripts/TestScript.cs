using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FactoryClass.Utils;
public class TestScript : MonoBehaviour
{
    private bool isOn;
    public Vector3 vector1;
    public Vector3 vector2;
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        //IComponentModel component = ComponentFactory.GetModelTypeByName("Tray");
        //obj = component.Creat("Tray");
        //Instantiate(obj);
        
        //Debug.Log(obj.name);
        //Debug.Log(gameObject.name+" size:"+UtilsClass.GetSize(gameObject));
        
    }

    // Update is called once per frame
    void Update()
    {
        //isOn = RunScene.Instance.isRun;
        //print("TestScript: " + isOn);
        //vector1 = 
        //Debug.Log(UtilsClass.IsPointerOverUIObject());
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIType
{
    //UI名字
    public string Name { get; private set; }
    //UI路径
    public string Path { get; private set; }
    public UIType ( string path)
    {
        Path = path;
        
        Name = path.Substring(path.LastIndexOf('/') + 1);
    }
   
}

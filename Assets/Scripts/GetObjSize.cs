using System.Collections;
using System.Collections.Generic;
using FactoryClass.Utils;
using UnityEngine;

public class GetObjSize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.name+":");
        Debug.Log(UtilsClass.GetSize(gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    private void OnRenderObject()
    {
        // transform.position = SceneView.lastActiveSceneView.camera.transform.position;
        // transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewTransparent : MonoBehaviour
{
    public GameObject factory3;
    private Camera m_Camera;
    
    // Start is called before the first frame update
    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Camera.transform.position.y >= 20)
        {
            factory3.SetActive(false);
        }
        else
        {
            factory3.SetActive(true);
        }
    }
}

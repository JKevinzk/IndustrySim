using System.Collections;
using System.Collections.Generic;
using GameLogic.SaveAndLoad;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoadBtn : MonoBehaviour
{
    SaveManager saveManager = SaveManager.Instance;
    public void OnClick()
    {
        var SceneName = EventSystem.current.currentSelectedGameObject.name;
        saveManager.LoadScene(SceneName);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

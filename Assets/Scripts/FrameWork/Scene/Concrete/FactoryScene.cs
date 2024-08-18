using System;
using FactoryClass.Utils;
using FrameWork.UIFrameWork.UI.Concrete;
using UnityEngine;
using UnityEngine.SceneManagement;

//工作面板场景
public class FactoryScene : SceneState
{
    //场景名称
    public readonly string sceneName = "Factory";
    PanelManger panelManger;
    private String loadSceneName = null;
    
    public FactoryScene(String LoadSceneName= null)
    {
        loadSceneName = LoadSceneName;
        // if (LoadSceneName != null)
        //     SceneManager.sceneLoaded +=
        //         (Scene scene, LoadSceneMode load) => { SaveManager.Instance.LoadScene(LoadSceneName); };
    }

    public override void OnEnter()
    {
        panelManger = new PanelManger();
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
            UtilsClass.SetCursor(UtilsClass.CursorType.DEFAULT);

            SceneManager.sceneLoaded += SceneLoaded;
            // if (loadSceneName != null)
            // {
            //     SceneManager.sceneLoaded +=
            //     (Scene scene, LoadSceneMode load) => { SaveManager.Instance.LoadScene(loadSceneName); };
            // }
            
        }
        else
        {
            panelManger.Push(new StartPanel());
            GameRoot.Instance.SetAction(panelManger.Push, panelManger.Pop);
            //场景中弹窗可用
            //GameRoot.Instance.Push()
        }
    }

    public override void OnExit()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
        // if (loadSceneName != null)
        //     SceneManager.sceneLoaded -=
        //         (Scene scene, LoadSceneMode load) => { SaveManager.Instance.LoadScene(loadSceneName); };
        //panelManger.PopAll();
    }

    //场景加载完毕后执行的方法
    private void SceneLoaded(Scene scene, LoadSceneMode load)
    {
        panelManger.Push(new WorkPanel());
        GameRoot.Instance.SetAction(panelManger.Push, panelManger.Pop);
        Debug.Log($"{sceneName}场景加载完毕!");
    }
}
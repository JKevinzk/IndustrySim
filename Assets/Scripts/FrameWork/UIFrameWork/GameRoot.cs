using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FactoryClass.Utils;

//管理全局
public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance { get; private set; }

    //场景管理器
    public SceneSystem SceneSystem { get; private set; }

    //显示面板，在框架之外调用
    public UnityAction<BasePanel> Push { get; private set; }
    
    //销毁面板，在框架之外调用 
    public UnityAction Pop { get; private set; }

    private void Awake()
    {
        if ( Instance == null )
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        SceneSystem = new SceneSystem();
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        SceneSystem.SetScene(new StartScene(),"Start");
        
    }

    //设置Push
    public void SetAction(UnityAction<BasePanel> push, UnityAction pop)
    {
        Push = push;
        Pop = pop;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System;
using System.Collections;
using System.IO;
using FactoryClass.Utils;
using FrameWork.Comm;
using GameLogic.SaveAndLoad;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FrameWork.UIFrameWork.UI.Concrete.LoadScenePanel
{
    public class LoadScenePanel : BasePanel
    {
        private const string Path = "Prefabs/UI/Panel/LoadScenePanel";
        private readonly string _streamingFilePath = Application.streamingAssetsPath;
        private readonly SignalManager _signalManager = SignalManager.Instance;
        
        // 加载场景面板
        public LoadScenePanel() : base(new UIType(Path))
        {
        }

        public override void onEnter()
        {
            //扫描文件夹获得文件名
            DirectoryInfo directoryInfo = new DirectoryInfo(_streamingFilePath);
            FileInfo[] fileInfo = directoryInfo.GetFiles();
            foreach (var file in fileInfo)
            {
                string fileName = file.Name;
                if (!fileName.EndsWith(".json")) continue;
                if (file.Directory != null)
                {
                    var sceneFile = JsonUtility.FromJson<Save>(File.ReadAllText(file.FullName));
                    ShowLoadSceneButton(sceneFile.sceneName, sceneFile.sceneInfo,sceneFile.sceneShot);
                }
            }

            var transforms = UITool.FindComponentsInChild<Transform>();
            foreach (var child in transforms)
            {
                var childType = child.gameObject.GetComponent<Button>();
                if (childType != null)
                    UITool.GetComponentInChildren<Button>(childType.name).onClick.AddListener(() =>
                    {
                        // 添加对场景按钮的监听
                        Debug.Log("加载" + childType.name);
                        if (GameRoot.Instance.SceneSystem.sceneName == "Factory")
                        {
                            ComponentManager.Instance.DestroyAllCreatedObj();
                            SignalManager.Instance.ResetDic();
                            Debug.Log(_signalManager);
                        }
                        else
                        {
                            GameRoot.Instance.SceneSystem.SetScene(new FactoryScene(),"Factory");
                        
                        }
                        GameObject.Find("Square").GetComponent<Image>().color= Color.Lerp(GameObject.Find("Square").GetComponent<Image>().color,new Color(GameObject.Find("Square").GetComponent<Image>().color.r,GameObject.Find("Square").GetComponent<Image>().color.r,GameObject.Find("Square").GetComponent<Image>().color.r,150),1.5f*Time.deltaTime);
                        LoadSceneCoroutineTemp(childType.GetComponentInChildren<Text>().text);
                        if (GameRoot.Instance.SceneSystem.sceneName == "Factory")
                        {
                            try
                            {
                                while (PanelManger.stackPanel.Peek().UIType.Path != "Prefabs/UI/Panel/WorkCanvas")
                                {
                                    Pop();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                Debug.LogWarning("面板栈空");
                            }
                        
                        }
                        UtilsClass.SetCursor(UtilsClass.CursorType.DEFAULT);
                    });
            }
        }

        /// <summary>
        /// 显示保存的场景按钮
        /// </summary>
        /// <param name="name">场景名称</param>
        /// <param name="info">场景信息</param>
        /// <param name="pictureString">场景快照</param>
        private void ShowLoadSceneButton(string name, string info, string pictureString)
        {
            GameObject parent = GameObject.Find("SceneContent");
            if (parent == null)
            {
                Debug.LogError("ButtonPanel不存在");
                return;
            }
        
            GameObject sceneButton = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Panel/LoadSceneButton"),
                parent.transform);

            sceneButton.name = name;
            sceneButton.transform.Find("LoadSceneButton/SceneName").GetComponent<Text>().text = name;
            sceneButton.transform.Find("LoadSceneButton/SceneInfo").GetComponent<Text>().text = info;
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(Convert.FromBase64String(pictureString));
            sceneButton.transform.Find("LoadSceneButton/Image").GetComponent<Image>().sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }

        // 把加载场景的协程挂载到空类上，并开启
        private void LoadSceneCoroutineTemp(string loadSceneName)
        {
            var loadSceneCoroutineTemp = GameObject.Find("LoadSceneCoroutineTemp");
            if (loadSceneCoroutineTemp == null)
            {
                loadSceneCoroutineTemp = new GameObject
                {
                    name = "LoadSceneCoroutineTemp"
                };
                loadSceneCoroutineTemp.AddComponent<LoadSceneCoroutine>();
            }

            loadSceneCoroutineTemp.GetComponent<LoadSceneCoroutine>().StartCoroutine(LoadScene(loadSceneName));
        }

        // 加载场景的协程函数，为了直到场景切换完成才执行加载物品的步骤
        private static IEnumerator LoadScene(string loadSceneName)
        {
            yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Factory");
            Debug.Log("开始加载物品");
            SaveManager.Instance.LoadScene(loadSceneName);
        }

        // 为了可以在非MonoBehavior类中开启协程，创建的空类 
        private class LoadSceneCoroutine : MonoBehaviour
        {
            private void Awake()
            {
                if (GameObject.Find("LoadSceneCoroutineTemp") != null)
                    DontDestroyOnLoad(GameObject.Find("LoadSceneCoroutineTemp"));
            }
        }
    }
}
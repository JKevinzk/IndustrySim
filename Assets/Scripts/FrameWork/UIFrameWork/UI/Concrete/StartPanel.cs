using FactoryClass.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class StartPanel : BasePanel
    {
        private const string Path = "Prefabs/UI/Panel/WelcomCanvas";

        //public UIType uiType = new UIType(path);
        //开始主面板
        public StartPanel() : base(new UIType(Path))
        {
        }

        public override void onEnter()
        {
            UtilsClass.LoadCursorTexture();
            UtilsClass.SetCursor(UtilsClass.CursorType.DEFAULT);
            UITool.GetComponentInChildren<Button>("BackBtn").onClick.AddListener(() =>
            {
                //点击事件添加
                Debug.Log("点击backBtn");
                //PanelManger.Pop();
                //PanelManger.Push(new WorkPanel());
                if (GameRoot.Instance.SceneSystem.sceneName == "Factory")
                {
                    while (PanelManger.stackPanel.Peek().UIType != UIType)
                    {
                        Pop();
                    }

                    Pop();
                }
                else
                {
                    GameRoot.Instance.SceneSystem.SetScene(new FactoryScene(), "Factory");
                }
            });
            UITool.GetComponentInChildren<Button>("SceneBtn").onClick.AddListener(() =>
            {
                //点击事件添加
                Debug.Log("点击SceneBtn");
                if (UITool.IsPanelInUIRoot("LoadScenePanel", out _)) return;
                Push(new LoadScenePanel.LoadScenePanel()); //压栈，显示面板
            });
            UITool.GetComponentInChildren<Button>("NewBtn").onClick.AddListener(() =>
            {
                if (GameRoot.Instance.SceneSystem.sceneName == "Factory")
                {
                    ComponentManager.Instance.DestroyAllCreatedObj();
                    while (PanelManger.stackPanel.Peek().UIType.Path != "Prefabs/UI/Panel/WorkCanvas")
                    {
                        Pop();
                    }
                }
                else
                {
                    GameRoot.Instance.SceneSystem.SetScene(new FactoryScene(), "Factory");
                }
            });

            UITool.GetComponentInChildren<Button>("QuitBtn").onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false; //如果是在unity编译器中
#else
                Application.Quit();//否则在打包文件中
#endif
            });
        }

        public override void OnExit()
        {
            UIManager.DestroyUI(UIType);
        }

        public override void OnPause()
        {
            //UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = false;
        }

        public override void OnResume()
        {
            //UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class FilePanel : BasePanel
    {
        private const string Path = "Prefabs/UI/Panel/FileCanvas";


        //开始主面板
        public FilePanel() : base(new UIType(Path)) { }

        public override void onEnter()
        {
        
            UITool.GetComponentInChildren<Button>("New").onClick.AddListener(() =>
            {

                //点击事件添加
                Debug.Log("点击New");
            
            });

            // UITool.GetOrAddComponentInChildren<Button>("Open").onClick.AddListener(() =>
            // {
            //
            //     //点击事件添加
            //     saveManager.LoadScene();
            //     
            // });
        
            UITool.GetComponentInChildren<Button>("Save").onClick.AddListener(() =>
            {
                //点击事件添加
                Debug.Log("点击Save");
                Pop();
                UIManager.UIHide(GameObject.Find("WorkCanvas"));
                UITool.IsPanelInUIRoot("ComponentCanvas", out var componentPanel);
                if (componentPanel != null) UIManager.UIHide(componentPanel);
                Push(new SaveScenePanel());
            });
        }

        public override void OnPause()
        {
            //UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = false;

        }

        public override void OnResume()
        {
            //UITool.GetOrAddComponent<CanvasGroup>().blocksRaycasts = true;
        }


        //public override void OnExit()
        //{
        //    SceneManager.sceneLoaded -= SceneLoaded;
        //}

        ////场景加载完毕后执行的方法
        //private void SceneLoaded(Scene scene, LoadSceneMode load)
        //{
        //    panelManger.Push(new StartPanel());
        //    Debug.Log($"{sceneName}场景加载完毕!");
        //}
    }
}

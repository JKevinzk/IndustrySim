using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class WorkPanel : BasePanel
    {
        static readonly string path = "Prefabs/UI/Panel/WorkCanvas";
        // public bool _componentDisplay { get; set; }
        private bool _fileDisplay = false;
        private readonly RunScene _runScene = RunScene.Instance;
    
        //public UIType uiType = new UIType(path);
        //开始主面板
        public WorkPanel() : base(new UIType(path))
        {
            // _componentDisplay = false;
        }

        public override void onEnter()
        {

            UITool.GetComponentInChildren<Button>("BackToWelcomeBtn").onClick.AddListener(() =>
            {

                //点击事件添加
                Debug.Log("点击BackToWelcomeBtn");
                if (_runScene.isRun) return;
                
                Push(new StartPanel());
            });

            UITool.GetComponentInChildren<Button>("ComponentWindowBtn").onClick.AddListener(() =>
            {
            
                if (_runScene.isRun) return;
                //点击事件添加
                if ( !UITool.IsPanelInUIRoot("ComponentCanvas",out _) )
                {
                    Push(new ComponentPanel());
                }
                else
                {
                    Pop();
                }
            });
            UITool.GetComponentInChildren<Button>("FileBtn").onClick.AddListener(() =>
            {
                if (_runScene.isRun) return;
                //点击事件添加
                Debug.Log("FileBtn");

                if (!UITool.IsPanelInUIRoot("FileCanvas",out _))
                {
                    Push(new FilePanel());
                }
                else
                {
                    Pop();
                }
            });

            UITool.GetComponentInChildren<Button>("DriverBtn").onClick.AddListener(() =>
            {
                if (_runScene.isRun) return;
                DriverPanel driverPanel = new DriverPanel();
                if (UITool.IsPanelInUIRoot("ComponentCanvas",out _))
                {
                    Pop();
                }
                
                // bool flag = UITool.IsPanelInUIRoot("DriverPanel", out GameObject obj);
                // if (flag)
                // {
                //     UIManager.UIShow(obj);
                //     driverPanel.onEnter();
                // }
                // else
                // {
                //     Push(driverPanel);
                // }
                Push(driverPanel);
            });

            UITool.GetComponentInChildren<Button>("ResetBtn").onClick.AddListener(() =>
            {
                RunScene.Instance.Restart();
            });

            UITool.GetComponentInChildren<Button>("FreezeBtn").onClick.AddListener(() =>
            {
                RunScene.Instance.OnFreeze();
            });

            UITool.GetComponentInChildren<Button>("PlayBtn").onClick.AddListener(() =>
            {
                Debug.Log("点击playbtn");
                // GameObject tem = new GameObject("TemporaryObject");
                // GameObject.Instantiate(tem,GameObject.Find("UIRoot").transform.parent);
                Debug.Log("点击playbtn");
                //Debug.Log("点击playbtn");
                _runScene.OnStart();
                if (_runScene.isRun)
                {
                    UITool.FindChildGameObject("Stop").SetActive(true);
                    UITool.GetComponentInChildren<Button>("BackToWelcomeBtn").interactable = false;
                    UITool.GetComponentInChildren<Button>("FileBtn").interactable = false;
                    UITool.GetComponentInChildren<Button>("EditBtn").interactable = false;
                    UITool.GetComponentInChildren<Button>("ViewBtn").interactable = false;
                    UITool.GetComponentInChildren<Button>("DriverBtn").interactable = false;
                    UITool.GetComponentInChildren<Button>("ComponentWindowBtn").interactable = false;
                    if (UITool.IsPanelInUIRoot("ComponentCanvas", out _)) Pop();
                    // 开始运行时取消拖拽包围盒
                    DragObject.Instance.CancelDrag();
                }
                else
                {
                    UITool.FindChildGameObject("Stop").SetActive(false);
                    UITool.GetComponentInChildren<Button>("BackToWelcomeBtn").interactable = true;
                    UITool.GetComponentInChildren<Button>("FileBtn").interactable = true;
                    UITool.GetComponentInChildren<Button>("EditBtn").interactable = true;
                    UITool.GetComponentInChildren<Button>("ViewBtn").interactable = true;
                    UITool.GetComponentInChildren<Button>("DriverBtn").interactable = true;
                    UITool.GetComponentInChildren<Button>("ComponentWindowBtn").interactable = true;
                }
                // Debug.Log($"isRun状态为{RunScene.Instance.isRun}");
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
    }
}

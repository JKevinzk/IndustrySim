using GameLogic.SaveAndLoad;
using UnityEngine;
using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class SaveScenePanel : BasePanel
    {
        private const int Width = 420;
        private const int Height = 270;
        private const string Path = "Prefabs/UI/Panel/SaveInfoCanvas";
        private readonly SaveManager _saveManager = SaveManager.Instance;
        private readonly ScreenShot _screenShot = GameObject.Find("Main Camera").GetComponent<ScreenShot>();

        public SaveScenePanel() : base(new UIType(Path))
        {
        }
    

        public override void onEnter()
        {
            // 获取场景截图
            UITool.IsPanelInUIRoot("SaveInfoCanvas", out var savePanel);
            _screenShot.shouldHideObj.Add(savePanel);
            _screenShot.StartCapture(getPic);

            UITool.GetComponentInChildren<Button>("BackBtn").onClick.AddListener(() =>
            {
                Pop();
                UITool.IsPanelInUIRoot("WorkCanvas", out var panel);
                UIManager.UIShow(panel);
                UITool.IsPanelInUIRoot("ComponentCanvas", out var componentPanel);
                if (componentPanel != null)
                    UIManager.UIShow(componentPanel);
            });
            UITool.GetComponentInChildren<Button>("Submit").onClick.AddListener(() =>
            {
                // 确认提交
                Debug.Log("确认保存");
                string sceneName = UITool.GetComponentInChildren<InputField>("sceneName").text;
                string sceneInfo = UITool.GetComponentInChildren<InputField>("sceneInfo").text;
                _saveManager.SaveScene(sceneName, sceneInfo);
                Pop();
                UITool.IsPanelInUIRoot("WorkCanvas", out var panel);
                UIManager.UIShow(panel);
                UITool.IsPanelInUIRoot("ComponentCanvas", out var componentPanel);
                if (componentPanel != null)
                    UIManager.UIShow(componentPanel);
            });
        }

        public void getPic()
        {
            Image image = GameObject.Find("MainPanel/Image").GetComponent<Image>();
            Texture2D texture = new Texture2D(Width, Height);
            texture.LoadImage(GameObject.Find("Main Camera").GetComponent<ScreenShot>().PicByte);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, Width, Height), new Vector2(0, 1));
            image.sprite = sprite;
        }
    }
}
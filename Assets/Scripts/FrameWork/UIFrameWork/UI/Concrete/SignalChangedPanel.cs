using UnityEngine.UI;

namespace FrameWork.UIFrameWork.UI.Concrete
{
    public class SignalChangedPanel: BasePanel
    {
        private const string Path = "Prefabs/UI/Panel/SignalChangedPanel";
        public SignalChangedPanel() : base(new UIType(Path))
        {
        }

        public override void onEnter()
        {
            base.onEnter();
            UITool.GetComponentInChildren<Button>("OkBtn").onClick.AddListener(Pop);
        }
    }
}
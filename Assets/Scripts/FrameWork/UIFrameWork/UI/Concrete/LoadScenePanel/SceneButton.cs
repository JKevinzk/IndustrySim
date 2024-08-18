using FactoryClass.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FrameWork.UIFrameWork.UI.Concrete.LoadScenePanel
{
    public class SceneButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
    

        public void OnPointerEnter(PointerEventData eventData)
        {
            UtilsClass.SetCursor(UtilsClass.CursorType.HAND);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UtilsClass.SetCursor(UtilsClass.CursorType.DEFAULT);
        }
    }
}

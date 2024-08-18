using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine.EventSystems;
using UnityEngine;

namespace FactoryClass.Utils
{
    public static class UtilsClass
    {
        public const int sortingOrderDefault = 5000;


        //获取对象当前尺寸
        public static Vector3 GetSize(GameObject obj)
        {
            Vector3 size = new Vector3();
            Transform transform = obj.transform;
            Vector3 length = obj.GetComponentInChildren<MeshFilter>().mesh.bounds.size;
            float length_X = length.x * transform.lossyScale.x;
            float length_Y = length.y * transform.lossyScale.y;
            float length_Z = length.z * transform.lossyScale.z;
            size.x = length_X;
            size.y = length_Y;
            size.z = length_Z;
            return size;
        }

        #region UI控制

        public static void UIHide(GameObject obj)
        {
            obj.GetComponent<CanvasGroup>().alpha = 0;
            obj.GetComponent<CanvasGroup>().interactable = false;
            obj.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        //判断鼠标是否在UI上
        public static bool IsUI(RectTransform rect)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition);
        }


        public static bool IsUI(RectTransform[] rect)
        {
            foreach (var item in rect)
            {
                if (IsUI(item)) return true;
            }

            return false;
        }

        public static bool IsUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        public static bool IsPointerOverUIObject()

        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            return results.Count > 0;
        }

        /// <summary>
        /// 判断左键点击发射的射线是否在UI上
        /// </summary>
        /// <returns>当有触碰多于一个UI面板时返回真，否则返回假</returns>
        // bool CheckGuiRaycastObjects()
        // {
        //     PointerEventData eventData = new PointerEventData(eventSystem);
        //     eventData.pressPosition = Input.mousePosition;
        //     eventData.position = Input.mousePosition;
        //
        //     List<RaycastResult> list = new List<RaycastResult>();
        //     if (graphicRaycaster != null)
        //         graphicRaycaster.Raycast(eventData, list);
        //
        //     foreach (var raycastResult in list)
        //     {
        //         Debug.Log(raycastResult.gameObject.name);
        //     }
        //     return list.Count > 0;
        // }

        #endregion

        //修正轴点在中心的物体，使得物体水平值为0时刚好在地面
        public static Vector3 FixVerticalPos(Transform transform, Vector3 Position)
        {
            //float Y;
            //Vector3 length = transform.GetComponent<MeshFilter>().mesh.bounds.size;
            //Y = length.y * transform.localScale.y;
            // if (Position.y < Y / 2) Position.y = Y / 2;
            if (Position.y < 0) Position.y = 0;
            //最高至天花板
            if (Position.y > 24) Position.y = 24;
            return Position;
        }
        
        /// <summary>
        /// 使物体轴心位于左下角便于网格拖动
        /// </summary>
        /// <param name="gameObject">修正基准物体</param>
        /// <param name="position">需要修正的位置</param>
        /// <returns>修正后的位置</returns>
        public static Vector3 FixAxisToBottomLeft(GameObject gameObject,Vector3 position)
        {
            float X, Z;
            X = ComponentManager.Instance.compoDict[gameObject.name].width;
            Z = ComponentManager.Instance.compoDict[gameObject.name].length;
            
            position.x -= X / 2;
            position.z -= Z / 2;

            if (position.y < 0) position.y = 0;
            if (position.y > 24) position.y = 24;
            return position;
        }

        // 获取与特定层级的交点
        public static Vector3 GetMouseWorldPosition(LayerMask Layer)
        {
            Vector3 point = new Vector3(0, 0, 0);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 1000, Layer))
            {
                point = hitInfo.point;
            }

            return point;
        }


        #region 设置鼠标样式

        private static Texture2D handCursorTexture; // 小手.
        private static Texture2D eyeCursorTexture; // 眼睛.
        private static Texture2D magnifierCursorTexture; // 放大镜.
        private static Texture2D defaultTexture;
        private static Vector2 hotSpot = Vector2.zero;
        private static CursorMode cursorMode = CursorMode.Auto;

        public enum CursorType
        {
            //默认样式.
            DEFAULT = 0,

            //小手
            HAND,

            //眼镜
            EYE,

            //放大镜
            MAGNIFIER
        }

        public static void LoadCursorTexture()
        {
            handCursorTexture = Resources.Load<Texture2D>("Textures/HandCursor");
            eyeCursorTexture = Resources.Load<Texture2D>("Textures/EyeCursor");
            magnifierCursorTexture = Resources.Load<Texture2D>("Textures/Magnifier");
            defaultTexture = Resources.Load<Texture2D>("Textures/DefultCursor");
            
        }

        public static void SetCursor(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.DEFAULT:
                    Cursor.SetCursor(defaultTexture, hotSpot, cursorMode);
                    break;
                case CursorType.HAND:
                    Cursor.SetCursor(handCursorTexture, hotSpot, cursorMode);
                    break;
                case CursorType.EYE:
                    Cursor.SetCursor(eyeCursorTexture, hotSpot, cursorMode);
                    break;
                case CursorType.MAGNIFIER:
                    Cursor.SetCursor(magnifierCursorTexture, hotSpot, cursorMode);
                    break;
                default:
                    Debug.LogError("未知指针");
                    break;
            }
        }

        #endregion


        public static TextMesh CreateWorldText(string text, Transform parent = null,
            Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null,
            TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left,
            int sortingOrder = sortingOrderDefault)
        {
            if (color == null) color = Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment,
                sortingOrder);
        }


        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize,
            Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }

        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="point">直线上某一点</param>
        /// <param name="direct">直线的方向</param>
        /// <param name="planeNormal">平面的法线</param>
        /// <param name="planePoint">平面上的任意一点</param>
        /// <returns>交点</returns>
        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal,
            Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            return d * direct.normalized + point;
        }
        
        /// <summary>
        /// 计算鼠标发出的射线与平面交点
        /// </summary>
        /// <param name="planeNormal">平面法线</param>
        /// <param name="planePoint">平面上一点</param>
        /// <returns></returns>
        public static Vector3 GetIntersectWithMouseAndPlane(Vector3 planeNormal, Vector3 planePoint)
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 direct = ray.direction;
            return GetIntersectWithLineAndPlane(point, direct, planeNormal, planePoint);
        }

        /// <summary>
        /// 鼠标射线与y=n的平面相交并获取基于这个平面的网格世界坐标
        /// </summary>
        /// <param name="y"></param>
        /// <returns>基于这个平面的世界坐标</returns>
        public static Vector3 GetGridWordPositionWithYPlane(float y)
        {
            Vector3 point = GetIntersectWithMouseAndPlane(Vector3.up, new Vector3(0, y, 0));
            return point + new Vector3(0, y, 0);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Graphics = UnityEngine.Graphics;


public class ScreenShot : MonoBehaviour
{
    [Tooltip("是否启用截图后将图片保存到下面路径的功能")] public bool saveUseable = false;

    // [Tooltip("存储路径")]
    // public string savePath = "C:/Users/GOD/Desktop";

    public List<GameObject> shouldHideObj;

    private Camera _camera;

    /// <summary>
    /// 截图后的sprite
    /// </summary>
    private static byte[] _picBytes;


    public Sprite sprite;
    //action_End为截图完执行代码方法

    //byte获取方式
    public byte[] PicByte
    {
        get => _picBytes;
        set => _picBytes = value;
    }

    private void Start()
    {
        _camera = GetComponent<Camera>();
        //StartCapture();
        //Debug.Log("successful");
    }

    public void StartCapture(Action getPic)
    {
        Rect mRect = new Rect(0, 0, _camera.pixelWidth, _camera.pixelHeight);
        CaptureStart();
        StartCoroutine(CaptureByRect(mRect, getPic));
    }

    /// <summary>
    /// 根据一个Rect类型来截取指定范围的屏幕, 左下角为(0,0)
    /// 例：StartCoroutine(CaptureByRect(new Rect(100, 150, 1024, 768)));
    /// 从左下角的横坐标100,纵坐标150坐标开始截图，宽度1024，高度768
    /// </summary>
    private IEnumerator CaptureByRect(Rect mRect, Action actionEnd)
    {
        //等待渲染线程结束
        yield return new WaitForEndOfFrame();

        //初始化Texture2D, 大小可以根据需求更改
        Texture2D mTexture = new Texture2D((int)mRect.width, (int)mRect.height,
            TextureFormat.RGB24, false);
        //读取屏幕像素信息并存储为纹理数据
        mTexture.ReadPixels(mRect, 0, 0);
        mTexture = ScaleTexture(mTexture, 420, 270);
        //应用
        mTexture.Apply();
        // sprite = Sprite.Create(mTexture,
        //     new Rect(0, 0, mTexture.width, mTexture.height),
        //     new Vector2(0.5f, 0.5f));
        _picBytes = mTexture.EncodeToPNG();

        //ResizeImage(_picBytes, 420, 270);


        if (saveUseable)
        {
            _picBytes = mTexture.EncodeToPNG();

            //若没路径 创建
            // if (!Directory.Exists((savePath)))
            // {
            //     Directory.CreateDirectory(savePath);
            // }
            // //保存路径
            // string path_save = savePath + "/IMG_" + currentTime;
            // System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            // System.Drawing.Image img = System.Drawing.Image.FromStream(ms);

            //写入图片
            // File.WriteAllBytes(path_save, bytes);
        }

        CaptureOver();
        actionEnd?.Invoke();
    }

    void CaptureStart()
    {
        if (shouldHideObj.Count > 0)
        {
            foreach (var t in shouldHideObj)
            {
                t.SetActive(false);
            }
        }
    }

    void CaptureOver()
    {
        if (shouldHideObj.Count > 0)
        {
            foreach (var t in shouldHideObj)
            {
                t.SetActive(true);
            }

            shouldHideObj.Clear();
        }
    }

    Texture2D ScaleTexture(Texture2D source, float targetWidth, float targetHeight)
    {
        Texture2D result = new Texture2D((int)targetWidth, (int)targetHeight, source.format, false);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor = source.GetPixelBilinear((float)j / result.width, (float)i / result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }
}
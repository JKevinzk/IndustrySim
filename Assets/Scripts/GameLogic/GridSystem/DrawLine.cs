using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ObjectBound))]
public class DrawLine : MonoBehaviour
{
    /// <summary>
    /// 存储线段两端的坐标
    /// </summary>
    private List<Vector3[]> m_linePoints = new List<Vector3[]>();

    private bool isShowGrid = true; //是否显示

    /// <summary>
    /// 初始化线段的坐标
    /// </summary>
    /// <param name="linePoints"></param>
    public void Init(List<Vector3[]> linePoints)
    {
        m_linePoints = linePoints;
    }

    public void AddPoints(Vector3[] points)
    {
        m_linePoints.Add(new[]
            { transform.InverseTransformPoint(points[0]), transform.InverseTransformPoint(points[1]) });
    }

    public void ClearPoints()
    {
        m_linePoints.Clear();
    }

    /// <summary>
    /// 隐藏网格
    /// </summary>
    public void HideGrid()
    {
        isShowGrid = false;
    }

    /// <summary>
    /// 显示包围盒,会先将缓存中的点清空再显示包围盒和竖线
    /// </summary>
    public void ShowGrid()
    { 
        ClearPoints();
        var position = transform.position;
        AddPoints(new[]
        {
            new Vector3(position.x, position.y, position.z),
            new Vector3(position.x, 0, position.z)
        }); //添加竖线
        GetComponent<ObjectBound>().AddPoints();
        isShowGrid = true;
    }

    static Material lineMaterial;

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    /// <summary>
    /// 使用GL进行绘图
    /// </summary>
    public void OnRenderObject()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);
        GL.MultMatrix(transform.localToWorldMatrix);

        GL.PushMatrix();
        GL.Begin(GL.LINES);

        //GL.MultMatrix(transform.localToWorldMatrix);

        GL.Color(new Color(1, 1, 1, 0.8f));

        if (isShowGrid)
        {
            foreach (var points in m_linePoints)
            {
                GL.Vertex(points[0]);
                GL.Vertex(points[1]);
            }
        }

        GL.End();
        GL.PopMatrix();
    }
}
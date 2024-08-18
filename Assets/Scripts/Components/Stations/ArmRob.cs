using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRob : MonoBehaviour
{
    private GameObject axis1;
    private GameObject axis2;
    private GameObject axis3;
    private GameObject axis4;
    private GameObject axis5;
    private GameObject axis6;
    
    static int[] point1 = new int[] { -90, -90, -90, 5, 25, 11 };
    static int[] point2 = new int[] { 12, -90, -90, -90, 9, 2 };
    static int[] point3 = new int[] { -90, -90, -90, 7, -90, -90 };
    static int[] point4 = new int[] { -90, -90, -90, 6, 23, 11 };
    static int[] point5 = new int[] { 9, -90, -90, -90, -90, -90 };

    private ArrayList pointList1; 
    private ArrayList pointList2;
    private ArrayList pointList3;
    private ArrayList pointList4;
    private ArrayList pointList5;
    //private ArrayList arrayList = new ArrayList();
    private List<ArrayList> points = new List<ArrayList>();
    

    private void Awake()
    {
        pointList1 = new ArrayList(point1);
        pointList2 = new ArrayList(point2);
        pointList3 = new ArrayList(point3);
        pointList4 = new ArrayList(point4);
        pointList5 = new ArrayList(point5);
        points.Add(pointList1);
        points.Add(pointList2);
        points.Add(pointList3);
        points.Add(pointList4);
        points.Add(pointList5);
        axis1 = GameObject.Find("Axis1");
        axis2 = GameObject.Find("Axis2");
        axis3 = GameObject.Find("Axis3");
        axis4 = GameObject.Find("Axis4");
        axis5 = GameObject.Find("Axis5");
        axis6 = GameObject.Find("Axis6");
    }
    // Start is called before the first frame update
    void Start()
    {
       
        //for (var i = 0; i < points.Count; i++)
        //{
        //    for (var j = 0; j < points[i].Count; j++)
        //    {
        //        Debug.Log(points[i][j]);
        //    }
        //}
    }

    float t = 0;
    int value = 0;
    bool flag = true;
    // Update is called once per frame
    void Update()
    {
        t += 0.5f * Time.deltaTime;
        if (t > 1) t = 1;

        if (flag)
        {
            for (var i = 0; i < 4; i++)
            {
                int[] startPoint = (int[])points[i].ToArray(typeof(int));
                int[] endPoint = (int[])points[i + 1].ToArray(typeof(int));
                int rotate1 = endPoint[0] - startPoint[0];
                int rotate2 = endPoint[1] - startPoint[1];
                int rotate3 = endPoint[2] - startPoint[2];
                int rotate4 = endPoint[3] - startPoint[3];
                int rotate5 = endPoint[4] - startPoint[4];
                int rotate6 = endPoint[5] - startPoint[5];
                //Debug.Log(rotate2 + "rotate2");
                axis1.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Lerp(0, rotate1, t), 0));
                axis2.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(0, rotate2, t)));
                axis3.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(0, rotate3, t)));

                if (rotate4 > value)
                {
                    axis4.transform.Rotate(1, 0, 0);
                    value++;
                }
                
                
                axis5.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(0, rotate5, t)));
                axis6.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Lerp(0, rotate6, t)));

                if (axis6.transform.rotation.z == -0.8) flag = false; //终止条件，旋转全部完成 

                Debug.Log(axis6.transform.rotation + "axis6");
                Debug.Log($"已经到达第{i + 2}点");
            }
        }
            
            
        

    }

}

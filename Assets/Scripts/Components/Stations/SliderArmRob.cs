using UnityEngine;
using UnityEngine.UI;

public class SliderArmRob : MonoBehaviour
{
    public Slider slider1;
    public Slider slider2;
    public Slider slider3;
    public Slider slider4;
    public Slider slider5;
    public Slider slider6;
    public Text axis1_angle;
    public Text axis2_angle;
    public Text axis3_angle;
    public Text axis4_angle;
    public Text axis5_angle;
    public Text axis6_angle;
    private GameObject axis1;
    private GameObject axis2;
    private GameObject axis3;
    private GameObject axis4;
    private GameObject axis5;
    private GameObject axis6;
    
    private int value;
    
    private void Awake()
    {
        axis1 = GameObject.Find("Axis1");
        axis2 = GameObject.Find("Axis2");
        axis3 = GameObject.Find("Axis3");
        axis4 = GameObject.Find("Axis4");
        axis5 = GameObject.Find("Axis5");
        axis6 = GameObject.Find("Axis6");
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        Quaternion origin_axis1 = axis1.transform.localRotation;
        Quaternion origin_axis2 = axis2.transform.localRotation;
        Quaternion origin_axis3 = axis3.transform.localRotation;
        Quaternion origin_axis5 = axis5.transform.localRotation;
        Quaternion origin_axis6 = axis6.transform.localRotation;
        float rotateSpeed = Time.deltaTime * 5;
        axis1.transform.localRotation = Quaternion.Slerp(origin_axis1, Quaternion.Euler(slider1.value, -90, -90), rotateSpeed);
        axis2.transform.localRotation = Quaternion.Slerp(origin_axis2,Quaternion.Euler(0.076f, 0.282f, slider2.value), rotateSpeed);
        axis3.transform.localRotation = Quaternion.Slerp(origin_axis3, Quaternion.Euler(5.702f, 0.802f, slider3.value), rotateSpeed);
        if (slider4.value > value)
        {
            axis4.transform.Rotate(1, 0, 0);
            value++;
        }

        if (slider4.value < value)
        {
            axis4.transform.Rotate(-1, 0, 0);
            value--;
        }

        axis5.transform.localRotation = Quaternion.Slerp(origin_axis5,Quaternion.Euler(0.121f, -0.178f, slider5.value), rotateSpeed);
        axis6.transform.localRotation = Quaternion.Slerp(origin_axis6,Quaternion.Euler(slider6.value, 0.0f, 0.0f),rotateSpeed);
        axis1_angle.text = slider1.value.ToString();
        axis2_angle.text = slider2.value.ToString();
        axis3_angle.text = slider3.value.ToString();
        axis4_angle.text = slider4.value.ToString();
        axis5_angle.text = slider5.value.ToString();
        axis6_angle.text = slider6.value.ToString();
    }
}
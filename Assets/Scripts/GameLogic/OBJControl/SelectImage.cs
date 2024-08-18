using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectImage : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    
    public PlacedObjectTypeSO placedObjectTypeSO;//需要被实例化的预制
    //private GameObject inistateObj;//实例化后的对象
    private Button m_btn;
    // public Color initalColor;
    // public Color highlightedColor;

    void Awake()
    {

        //Debug.Log(placedObjectTypeSOArray.Length + "Length");
        //Debug.Log(placedObjectTypeSOArray[0].name + "name");
    }
    void Start()
    {
        placedObjectTypeSO = ComponentManager.Instance.compoDict[this.name];
        
        
        //实例化预制
        //inistateObj = Instantiate(placedObjectTypeSO.prefab);
        //inistateObj.SetActive(false);
        m_btn = this.GetComponent<Button>();
        // m_btn.image.sprite = transform.GetChild(0).image;
        // 可交互 --> 点击检测是否可用
        // m_btn.interactable = true;
        // 设置过渡模式
        // m_btn.transition = Selectable.Transition.ColorTint;
        //
        //
        ColorBlock cb = new ColorBlock();
        cb.normalColor = Color.white;
        cb.highlightedColor = Color.gray;
        cb.pressedColor = Color.white;
        cb.disabledColor = new Color(1, 0.5f, 0, 1);
        cb.selectedColor = Color.red;
        // m_btn.colors = cb;

    }
    //检测鼠标对UI的按下
    public void OnPointerDown(PointerEventData eventData)
    {
        GridBuildSystem.ShowGrid();
        SelectObjManager.Instance.AttachNewObject(placedObjectTypeSO);
    }
    //在鼠标移入时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        // GetComponent<Renderer>().material.SetColor( highlightedColor);
        // m_btn.DoStateTransition(SelectionState.Normal, true);
        // Debug.Log("child     "+transform.GetChild(0).gameObject);
        // Debug.Log("GameA.activeSelf:"+GetComponent<UnityEngine.UI.Button>().activeSelf);
        // Debug.Log("GameA.activeInHierarchy:" + GetComponent<UnityEngine.UI.Button>().activeInHierarch);
        try
        {
            GameObject.Find("ComponentName").GetComponent<TextMeshProUGUI>().SetText(placedObjectTypeSO.nameString);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
       
    }
    //在鼠标移出时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        // base.OnPointerExit(eventData);
        // DoStateTransition(SelectionState.Normal, true);
        // GetComponent<UnityEngine.UI.Button>().Select(Normal);
        try
        {
            GameObject.Find("ComponentName").GetComponent<TextMeshProUGUI>().SetText("");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }


}

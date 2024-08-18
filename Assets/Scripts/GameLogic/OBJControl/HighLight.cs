using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HighLight : MonoBehaviour,  IPointerEnterHandler,IPointerExitHandler
{	
	private Graphic m_graphics ;
    // Start is called before the first frame update
    void Awake()
    {

       
    }
    void Start()
    {
      	m_graphics = gameObject.GetComponent<Graphic>();
        

    }
  
    //在鼠标移入时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
		m_graphics.color =new  Color(0.4274f,0.4274f,0.4274f,0.3921f);
        
    }
    //在鼠标移出时触发
    public void OnPointerExit(PointerEventData eventData)
    {
      
    	m_graphics.color = new  Color(0.2196f,0.2196f,0.2196f,0.3921f);
    }

}

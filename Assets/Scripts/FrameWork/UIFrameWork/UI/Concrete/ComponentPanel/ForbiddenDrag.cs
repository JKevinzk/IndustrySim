using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Audio;
using UnityEngine.UI;

public class ForbiddenDrag : MonoBehaviour,  IDragHandler, IEndDragHandler
{
    
    public UnityEvent leftDrag;

    public UnityEvent leftEndDrag;

    private ScrollRect _scrollRect;

    private void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    void Update()
    {
        leftDrag.AddListener(ForbiddenVertical);
        leftEndDrag.AddListener(EnableVertical);
    }

    private void EnableVertical()
    {
        _scrollRect.vertical = true;
    }

    private void ForbiddenVertical()
    {
       _scrollRect.vertical = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.dragging) leftDrag.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        leftEndDrag.Invoke();
    }
}

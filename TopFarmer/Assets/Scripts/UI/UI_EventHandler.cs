using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_EventHandler : MonoBehaviour,IPointerEnterHandler, IPointerClickHandler ,IBeginDragHandler,IDragHandler,IEndDragHandler,IDropHandler
{
    public Action<PointerEventData> OnPointerEnterHandler = null;
    public Action<PointerEventData> OnPointerClickHandler = null;
    public Action<PointerEventData> OnBeginDragHandler= null;
    public Action<PointerEventData> OnDragHandler = null;
    public Action<PointerEventData> OnEndDragHandler = null;
    public Action<PointerEventData> OnDropHandler= null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(OnPointerEnterHandler != null)
            OnPointerEnterHandler.Invoke(eventData);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnPointerClickHandler != null)
            OnPointerClickHandler.Invoke(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(OnBeginDragHandler != null)
            OnBeginDragHandler.Invoke(eventData);
    }
    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position;
        if (OnDragHandler != null)
            OnDragHandler.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (OnEndDragHandler != null)
            OnEndDragHandler.Invoke(eventData);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (OnDropHandler != null)
            OnDropHandler.Invoke(eventData);
    }

   
}

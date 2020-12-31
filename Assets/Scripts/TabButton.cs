using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerClickHandler
{
    public TabGroup TabGroup;
    public Image Background;
    public Sprite Idle;
    public Sprite Selected;
    public UnityEvent OnTabSelected;
    public UnityEvent OnTabDeselected;

    public void Awake()
    {
        Background = GetComponent<Image>();
        TabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TabGroup.OnTabSelected(this);
    }

    public void Select()
    {
        if (OnTabSelected!=null)
        {
            OnTabSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if (OnTabDeselected != null)
        {
            OnTabDeselected.Invoke();
        }
    }
}

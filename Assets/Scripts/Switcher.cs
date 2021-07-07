using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class Switcher : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _switcherBar;
    [SerializeField] private Image _switcherValue;
    public int Value;
    [SerializeField] protected int _maxValue = 3;
    public UnityEvent OnValueChanged;

    void Start()
    {
        if (_maxValue == 0)
        {
            _maxValue = 2;
        }
        UpdatePosition();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        SetValue();
    }
    public virtual void SetValue()
    {
        if (Value + 1 > _maxValue)
        {
            Value = 0;
            OnValueChanged.Invoke();
        }
        else
        {
            Value += 1;
            OnValueChanged.Invoke();
        }
        UpdatePosition();
    }
    public virtual void UpdatePosition()
    {
        if (_switcherBar != null && _switcherValue != null)
        {
            float barLength = _switcherBar.rectTransform.rect.width - _switcherValue.rectTransform.rect.width;
            float valuePosition = _switcherValue.rectTransform.localPosition.x;
            valuePosition = barLength / Convert.ToSingle(_maxValue) * Convert.ToSingle(Value);
            valuePosition = valuePosition - barLength / 2;
            _switcherValue.rectTransform.localPosition = new Vector3(valuePosition, _switcherValue.rectTransform.localPosition.y, _switcherValue.rectTransform.localPosition.z);
        }
    }
}

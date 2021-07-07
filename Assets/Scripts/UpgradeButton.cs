using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UpgradeButton : Switcher
{
    public UnityEvent LackOfMoney;
    public override void SetValue()
    {
        if (_maxValue > 1)
        {
            if (Value + 1 <= _maxValue)
            {
                if (Value == 0 && Player.Instance.Stars - Garage.Instance.FirstGradePrice < 0)
                {
                    LackOfMoney.Invoke();
                    return;
                }
                else if (Value == 1 && Player.Instance.Stars - Garage.Instance.SecondGradePrice < 0)
                {
                    LackOfMoney.Invoke();
                    return;
                }
                else if (Value == 2 && Player.Instance.Stars - Garage.Instance.ThirdGradePrice < 0)
                {
                    LackOfMoney.Invoke();
                    return;
                }
                Value += 1;
                OnValueChanged.Invoke();
            }
        }
        else
        {
            if (Value + 1 <= _maxValue)
            {
                if (Value == 0 && Player.Instance.Stars - Garage.Instance.IndicatorsPrice < 0)
                {
                    LackOfMoney.Invoke();
                    return;
                }
                Value += 1;
                OnValueChanged.Invoke();
            }
        }
        UpdatePosition();
    }
    public override void UpdatePosition()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < Value; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}

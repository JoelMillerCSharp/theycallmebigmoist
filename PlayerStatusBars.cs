using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusBars : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    public void SetToMaxValue(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }
    public void SetToCurrentValue(float currentValue)
    {
        slider.value = currentValue;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ScrollController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private Scrollbar _scrollBar;

    [SerializeField] private int _amountItems;

    private float _scroolBarValue;

    // Start is called before the first frame update
    void Start()
    {
        _scrollBar.onValueChanged.AddListener(MoveToPosition);
    }

    private void MoveToPosition(float value)
    {
        _scroolBarValue = value;
    }

    public void MoveToPosition()
    {
        DOTween.Kill(true);

        var fraction = 1f / (_amountItems-1);
        var result = 0f;
        var clamped = Mathf.Clamp(_scroolBarValue, 0f, 1f);
        float dist = float.MaxValue;

        for (int i = 0; i < _amountItems; i++)
        {
            if (Math.Abs((1f / (_amountItems-1) * i)-clamped) <dist)
            {
                dist = Math.Abs((1f / (_amountItems - 1) * i) - clamped);
                result = (fraction * i);
                GameManager.SINGLETON.TotalMaps = i;
            }
                
        }

        DOTween.To(() => _scrollBar.value, x => _scrollBar.value = x, result, 0.35f);

    }

    public void OnEndDrag(PointerEventData eventData)
    {
       MoveToPosition();
    }
}


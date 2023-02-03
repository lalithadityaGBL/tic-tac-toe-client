using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ButtonClick : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClick; 
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        var seq = DOTween.Sequence();
        transform.localScale = Vector3.one * .75f;
        seq.Append(transform.DOScale(1f, 0.2f).SetEase(Ease.OutBounce));
        seq.AppendCallback(() => {
            if (onClick != null)
                onClick.Invoke();
        });
  
    }
}

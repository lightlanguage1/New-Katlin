#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SelfDebug : MonoBehaviour
{

    private void Start()
    {
        Vector3 vec3 = GetComponent<RectTransform>().anchoredPosition;
        
        transform.DOLocalMove(new Vector3(-730, 315), 0.4f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                gameObject.SetActive(false);
                GetComponent<RectTransform>().anchoredPosition = vec3;
            });
            
        });
    }
}

#endif

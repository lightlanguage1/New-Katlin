using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgAction : MonoBehaviour
{
    private void OnEnable()
    {
        BattleManager.instance.EnemyAtkEvent += imageMove;
    }

    private void OnDestroy()
    {
        BattleManager.instance.EnemyAtkEvent -= imageMove;
    }

    private void imageMove()
    {
        Vector3 vec3 = new Vector3(-17f, 2.5f, 0);   //记录起始位置

        transform.DOLocalMove(new Vector3(-8.8f, 2.5f, 0), 0.5f).SetEase(Ease.InQuint).OnComplete(() =>
        {
            DOVirtual.DelayedCall(1.5f, () =>
            {
                transform.localPosition = vec3; //复位
            });
        });    //逐渐加速
    }
}

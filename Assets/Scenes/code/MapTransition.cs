using UnityEngine;
using System.Collections;

public class MapTransition : MonoBehaviour
{
    public Transform position2;
    public GameObject player;
    public GameObject blackoutScreen; // 参考你的黑屏UI元素
    public float transitionDelay = 1.0f;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning)
        {
            return;
        }

        if (!isTransitioning && other.CompareTag("Player"))
        {
            StartCoroutine(TransitionWithDelay(position2.position));
        }
    }

    private IEnumerator TransitionWithDelay(Vector3 targetPosition)
    {
        isTransitioning = true;

        // 激活黑屏UI元素
        blackoutScreen.SetActive(true);

        // 等待过渡延迟
        yield return new WaitForSeconds(transitionDelay);

        // 传送玩家
        player.transform.position = targetPosition;

        // 再次等待过渡延迟
        yield return new WaitForSeconds(transitionDelay);

        // 停用黑屏UI元素
        blackoutScreen.SetActive(false);

        isTransitioning = false;
    }
}

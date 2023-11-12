using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnBridge : MonoBehaviour
{
    public GameObject[] bridgeParts;
    public Vector3 targetScale = new Vector3(1, 1, 1);
    public float duration = 0.5f;
    public float shrinkDelay = 5f;
    public float shrinkDuration = 2f;
    public bool ReverseOrder = false;  // Control the order of spawning and shrinking

    // UnityEvents
    public UnityEvent onShrinkStart;
    public UnityEvent onShrinkEnd;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (GameObject bridgePart in bridgeParts)
        {
            bridgePart.transform.localScale = Vector3.zero;
            bridgePart.gameObject.SetActive(false);
        }
    }

    public void Spawn()
    {
        Init();
        StopAllCoroutines();
        StartCoroutine(SpawnAndScaleBridge());
    }

    public void Forward()
    {
        ReverseOrder = false;
    }

    public void Backward()
    {
        ReverseOrder = true;
    }

    IEnumerator SpawnAndScaleBridge()
    {
        if (ReverseOrder)
        {
            for (int i = bridgeParts.Length - 1; i >= 0; i--)
            {
                bridgeParts[i].gameObject.SetActive(true);
                yield return StartCoroutine(ScaleOverTime(bridgeParts[i], targetScale, duration));
            }
        }
        else
        {
            foreach (GameObject bridgePart in bridgeParts)
            {
                bridgePart.gameObject.SetActive(true);
                yield return StartCoroutine(ScaleOverTime(bridgePart, targetScale, duration));
            }
        }

        StartCoroutine(ShrinkBridgeAfterDelay());
    }

    IEnumerator ShrinkBridgeAfterDelay()
    {
        yield return new WaitForSeconds(shrinkDelay);

        onShrinkStart.Invoke();

        if (ReverseOrder)
        {
            for (int i = bridgeParts.Length - 1; i >= 0; i--)
            {
                yield return StartCoroutine(ScaleOverTime(bridgeParts[i], Vector3.zero, shrinkDuration));
                bridgeParts[i].gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject bridgePart in bridgeParts)
            {
                yield return StartCoroutine(ScaleOverTime(bridgePart, Vector3.zero, shrinkDuration));
                bridgePart.gameObject.SetActive(false);
            }
        }

        onShrinkEnd.Invoke();
    }

    IEnumerator ScaleOverTime(GameObject targetObject, Vector3 toScale, float duration)
    {
        float counter = 0;
        Vector3 startScaleSize = targetObject.transform.localScale;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            targetObject.transform.localScale = Vector3.Lerp(startScaleSize, toScale, counter / duration);
            yield return null;
        }
    }
}

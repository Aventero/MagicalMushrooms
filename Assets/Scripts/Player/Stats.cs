using System.Collections;
using TMPro;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public GameObject CoinCounter;
    private TMP_Text counterText;
    public int CoinsCollected = 0;
    public float popScaleFactor = 1.1f; // This determines how big the pop effect will be.
    public float popDuration = 0.001f; // This determines how long the pop effect will last.
    public static Stats Instance { get; private set; }
    private Vector3 originalScale;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        counterText = CoinCounter.GetComponentInChildren<TMP_Text>();
        counterText.SetText("Lights " + CoinsCollected.ToString());
        originalScale = counterText.transform.localScale;
    }

    public void IncreaseCoinsCollected(int value)
    {
        CoinsCollected += value;
        counterText.SetText("Lights " + CoinsCollected.ToString());

        // Start the pop effect
        StartCoroutine(PopTextEffect());
    }

    private IEnumerator PopTextEffect()
    {
        float elapsedTime = 0;
        float halfDuration = popDuration / 2f;

        // Scale up
        while (elapsedTime < halfDuration)
        {
            float percentage = elapsedTime / halfDuration;
            counterText.transform.localScale = Vector3.Lerp(originalScale, originalScale * popScaleFactor, percentage);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        counterText.transform.localScale = originalScale * popScaleFactor;

        elapsedTime = 0; 

        // Scale down
        while (elapsedTime < halfDuration)
        {
            float percentage = elapsedTime / halfDuration;
            counterText.transform.localScale = Vector3.Lerp(originalScale * popScaleFactor, originalScale, percentage);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        counterText.transform.localScale = originalScale; 
    }
}

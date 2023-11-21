using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Stats : MonoBehaviour
{
    [Header("Mushrooms")]
    public int MushroomValue = 25;
    public int MushroomsCollected = 0;
    public int MaxMushrooms { get; private set;}
    public TMP_Text CollectedText;

    [Header("Floating Text")]
    public Canvas UI;
    public GameObject textPrefab;
    public GameObject floatingTextOrigin;
    public float moveSpeed = 1f;
    public float fadeDuration = 1f;

    [Header("Coins")]
    public int CoinsCollected = 0;
    public GameObject CoinCounterGameObject;
    public int MaxCoins { get; private set; }
    public int StartMaxCoins { get; private set; }
    private TMP_Text counterText;

    [Header("Coloration")]
    public MagicLiquid StaffColoration;

    [Header("Pop")]
    private bool IsPopping = false;
    public static Stats Instance { get; private set; }
    private Vector3 originalScale;
    private Color originialColor;
    private bool missingCoinsRunning = false;

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
        StartMaxCoins = MushroomValue;
        MaxCoins = StartMaxCoins;
        counterText = CoinCounterGameObject.GetComponentInChildren<TMP_Text>();
        counterText.SetText("Magic " + CoinsCollected.ToString() + "/" + MaxCoins);
        originalScale = counterText.transform.localScale;
        originialColor = counterText.color;
        MaxMushrooms = FindObjectsOfType<MushroomCollectable>().Length;
        CollectedText.SetText("0" + " / " + MaxMushrooms);

    }

    public void IncreaseMushroomsCollected()
    {
        MushroomsCollected++;
        MaxCoins += MushroomValue;
        counterText.SetText("Magic " + CoinsCollected.ToString() + "/" + MaxCoins);
        CollectedText.SetText(Stats.Instance.MushroomsCollected + " / " + Stats.Instance.MaxMushrooms);
        CreateFloatingText("+" + MushroomValue + " Max", floatingTextOrigin.transform.position, Color.yellow, 2f, 1.5f);
    }

    public void IncreaseCoinsCollected(int value)
    {
        counterText.color = Color.white;
        CoinsCollected += value;

        if (CoinsCollected > MaxCoins)
            CoinsCollected = MaxCoins;

        counterText.SetText("Magic " + CoinsCollected.ToString() + "/" + MaxCoins);
        CreateFloatingText("+" + value, floatingTextOrigin.transform.position, Color.cyan, 0.1f, 1.1f);
        StaffColoration.MagicReachedOrigin();
    }

    public void DecreaseCoinsCollected(int value)
    {
        CoinsCollected -= value;

        if (CoinsCollected <= 0)
        {
            StartCoroutine(PopEffect(0.5f, 1.2f, Color.red));
            CoinsCollected = 0;
        }

        counterText.SetText("Magic " + CoinsCollected.ToString() + "/" + MaxCoins);
        CreateFloatingText("-" + value, floatingTextOrigin.transform.position, new Color(0.7f, 0f, 0f, 1f), 0.2f, 0.9f);
        StaffColoration.MagicReachedOrigin();
    }

    public float GetNormalizedCoins()
    {
        return (float)CoinsCollected / MaxCoins;
    }

    public void MissingCoinsEffect()
    {
        if (!missingCoinsRunning)
        {
            missingCoinsRunning = true;
            StartCoroutine(PopEffect(0.5f, 1.2f, Color.red));
        }
    }

    private IEnumerator PopEffect(float popDuration, float popScale, Color? color = null)
    {
        float elapsedTime = 0;
        float halfDuration = popDuration / 2f;
        if (color.HasValue)
        {
            counterText.color = color.Value;
        }

        // Scale up
        while (elapsedTime < halfDuration)
        {
            float percentage = elapsedTime / halfDuration;
            counterText.transform.localScale = Vector3.Lerp(originalScale, originalScale * popScale, percentage);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        counterText.transform.localScale = originalScale * popScale;

        elapsedTime = 0;

        // Scale down
        while (elapsedTime < halfDuration)
        {
            float percentage = elapsedTime / halfDuration;
            counterText.transform.localScale = Vector3.Lerp(originalScale * popScale, originalScale, percentage);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        counterText.transform.localScale = originalScale;
        counterText.color = originialColor;
        missingCoinsRunning = false;
        IsPopping = false;
    }

    public void CreateFloatingText(string text, Vector3 position, Color color, float popDuration, float popScale)
    {
        GameObject textObj = Instantiate(textPrefab, position, Quaternion.identity, UI.transform);
        TMP_Text txt = textObj.GetComponent<TMP_Text>();
        txt.SetText(text);
        txt.color = color;
        StartCoroutine(MoveAndFade(textObj, color, popDuration, popScale));
    }

    private IEnumerator MoveAndFade(GameObject textObj, Color color, float popDuration, float popScale)
    {
        float elapsed = 0;
        CanvasGroup canvasGroup = textObj.GetComponent<CanvasGroup>();
        Vector3 startPosition = textObj.transform.position;

        float arcHeight = 50f;
        float arcWidth = 30f;

        // Random starting direction: 1 for right, -1 for left
        float directionMultiplier = Random.Range(-3f, 3f);

        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;

            float x = Mathf.Lerp(0, arcWidth * directionMultiplier, t);
            float y = Mathf.Sin(Mathf.PI * t) * arcHeight;

            textObj.transform.position = startPosition + new Vector3(x, y, 0);
            canvasGroup.alpha = 1 - t;

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!IsPopping)
        {
            IsPopping = true;
            StartCoroutine(PopEffect(popDuration, popScale, color));
        }
        Destroy(textObj);
    }

}

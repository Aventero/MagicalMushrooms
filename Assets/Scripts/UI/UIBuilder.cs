using UnityEngine;
using UnityEngine.UI;

public class UIBuilder : MonoBehaviour
{
    public static GameObject[] CreateIcons(Transform parent, Sprite icon, string displayName, int numberOfIcons, Vector2 anchor, Vector2 pivot, float scale, float gapSize)
    {
        GameObject[] iconArray = new GameObject[numberOfIcons];
        float width = icon.rect.width * scale + gapSize;
        
        int direction = (anchor.x == 0 ? 1 : -1);

        for (int i = 0; i < numberOfIcons; i++)
        {
            GameObject newIcon = CreateSpriteOnScreen(parent, new Vector2(i * width * direction, 0), icon, displayName, anchor, pivot, scale);
            iconArray[i] = newIcon;
        }

        return iconArray;
    }

    public static GameObject[] CreateIcons(Transform parent, Sprite[] icon, string displayName, Vector2 anchor, Vector2 pivot, Vector2 size)
    {
        GameObject[] iconArray = new GameObject[icon.Length];
        float width = size.x;
        int direction = (anchor.x == 0 ? 1 : -1);

        for (int i = 0; i < icon.Length; i++)
        {
            GameObject newIcon = CreateSpriteOnScreen(parent, new Vector2(i * width * direction, 0), icon[i], displayName, anchor, pivot, size);
            iconArray[i] = newIcon;
        }

        return iconArray;
    }

    public static GameObject CreateSpriteOnScreen(Transform parent, Vector2 position, Sprite icon, string displayName, Vector2 anchor, Vector2 pivot, Vector2 size)
    {
        // Spawn new health icon
        GameObject newIcon = new GameObject(displayName);
        newIcon.transform.parent = parent;

        // position the icon
        RectTransform rectTransform = newIcon.AddComponent<RectTransform>();

        // Set anchor point and the pivot to the top left corner
        rectTransform.sizeDelta = size;
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = pivot;

        rectTransform.anchoredPosition = position;

        // Add the sprite
        Image image = newIcon.AddComponent<Image>();
        image.sprite = icon;

        return newIcon;
    }

    public static GameObject CreateSpriteOnScreen(Transform parent, Vector2 position, Sprite icon, string displayName, Vector2 anchor, Vector2 pivot, float scale)
    {
        // Spawn new health icon
        GameObject newIcon = new GameObject(displayName);
        newIcon.transform.parent = parent;

        // position the icon
        RectTransform rectTransform = newIcon.AddComponent<RectTransform>();

        // Set anchor point and the pivot to the top left corner
        //rectTransform.sizeDelta = size;
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = pivot;
        rectTransform.localScale = new Vector3(scale, scale);

        rectTransform.anchoredPosition = position;

        // Add the sprite
        Image image = newIcon.AddComponent<Image>();
        image.sprite = icon;

        return newIcon;
    }
}
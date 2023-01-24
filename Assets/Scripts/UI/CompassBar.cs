using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    [Header("Variables")]
    public float SpriteScale = 0.3f;
    public float MinDistance = 2f;
    public GameObject IconPrefab;
    public RectTransform BackgroundTransform;

    private GameObject referenceNorth;
    private RectTransform maskTransform;

    // First: Target | Second: Compass Object
    private List<(GameObject, GameObject)> itemList;
    private List<GameObject> itemObjects;

    void Start()
    {
        maskTransform = this.GetComponent<RectTransform>();

        Item[] items = GameObject.FindObjectsOfType<Item>();
        itemList = new List<(GameObject, GameObject)>();
        itemObjects = ItemObjectsToGameObjects(items);

        CreateReferenceNorth();
    }

    void Update()
    {
        ShiftCompassBackground();
        UpdateIcons();
    }

    private void CreateReferenceNorth()
    {
        referenceNorth = new GameObject("ReferenceNorth for CompassBar");
        referenceNorth.transform.position = new Vector3(100, 0, 0);
    }

    private List<GameObject> ItemObjectsToGameObjects(Item[] items)
    {
        List<GameObject> gameObjects = new();

        for (int i = 0; i < items.Length; i++)
            gameObjects.Add(items[i].gameObject);

        return gameObjects;
    }

    private void ShiftCompassBackground()
    {
        Vector3 targetDirection = (referenceNorth.transform.position - Camera.main.transform.position);

        // Angle beetween -180 and 180
        float angle = Vector2.SignedAngle(new Vector2(targetDirection.x, targetDirection.z), new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z));
        angle = Mathf.InverseLerp(-180, 180, angle);

        float shiftAmount = Mathf.Lerp(-200, 200, angle);
        BackgroundTransform.anchoredPosition = new Vector2(shiftAmount, 0f);
    }

    private void UpdateIcons()
    {
        foreach (GameObject itemObject in itemObjects)
        {
            // Get the tuple with the item object
            (GameObject, GameObject) currentTuple = itemList.Find(tuple => tuple.Item1 == itemObject);

            // Item Object has been destroyed
            if (itemObject == null)
            {
                // Remove sprite
                if (currentTuple.Item2 != null)
                    Destroy(currentTuple.Item2);

                // Remove Item Object 
                itemList.Remove(currentTuple);
                itemObjects.Remove(itemObject);
                return;
            }

            // Calculate Distance from player to item
            float distance = Vector3.Distance(Camera.main.transform.position, itemObject.transform.position);

            if (distance > MinDistance)
            {
                if (currentTuple.Item1 == null)
                {
                    // Create Sprite if not already in list
                    GameObject itemSprite = CreateIcon(itemObject);
                    itemList.Add((itemObject, itemSprite));
                }
                else
                {
                    UpdateIconPosition(currentTuple.Item1, currentTuple.Item2.GetComponent<RectTransform>());
                    UpdateItemDistance(distance, currentTuple.Item2);
                }
            }
            else
            {
                // Sprite is out of range -> Remove Sprite
                if (currentTuple.Item2 != null)
                    Destroy(currentTuple.Item2);

                itemList.Remove(currentTuple);
            }
        }
    }

    private void UpdateItemDistance(float distance, GameObject iconObject)
    {
        TMP_Text distanceText = iconObject.GetComponentInChildren<TMP_Text>();
        distanceText.text = distance.ToString("F1") + " m";
    }

    private void UpdateIconPosition(GameObject target, RectTransform marker)
    {
        Vector3 targetDirection = (target.transform.position - Camera.main.transform.position);
        float angle = Vector2.SignedAngle(new Vector2(targetDirection.x, targetDirection.z), new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z));
        float markerPosition = Mathf.Clamp(2 * angle / Camera.main.fieldOfView, -1, 1);

        marker.anchoredPosition = new Vector2(maskTransform.rect.width / 2 * markerPosition, 0);
    }

    private GameObject CreateIcon(GameObject itemGameobject)
    {
        // Create new Icon Game Object
        ItemData item = itemGameobject.GetComponent<Item>().item;
        GameObject newItemGameObject = Instantiate(IconPrefab, maskTransform);

        // Set the item sprite
        Image newItemImage = newItemGameObject.GetComponent<Image>();
        newItemImage.sprite = item.Icon;

        return newItemGameObject;
    }
}

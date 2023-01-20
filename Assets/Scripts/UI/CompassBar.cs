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

    [Header("Marker Objects")]
    public GameObject MarkerPrefab;
    public List<GameObject> Markers;

    private RectTransform compassTransform;

    // First: Target | Second: Compass Object
    private List<(GameObject, GameObject)> markerObjects;
    private List<(GameObject, GameObject)> itemList;
    private List<GameObject> itemObjects;

    void Start()
    {
        compassTransform = this.GetComponent<RectTransform>();

        itemList = new List<(GameObject, GameObject)>();
        
        Item[] items = GameObject.FindObjectsOfType<Item>();
        itemObjects = ItemObjectsToGameObjects(items);

        markerObjects = new List<(GameObject, GameObject)>();
        CreateAllMarker();
    }

    void Update()
    {
        ShiftCompassBackground();
        UpdateIcons();
        UpdateMarkers();
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
        Vector3 targetDirection = (Markers[0].transform.position - Camera.main.transform.position);

        // Angle beetween -180 and 180
        float angle = Vector2.SignedAngle(new Vector2(targetDirection.x, targetDirection.z), new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z));

        //  800 = -180
        // 1200 = 0
        // 1600 = 180
        angle = Mathf.InverseLerp(-180, 180, angle);
        float shiftAmount = Mathf.Lerp(-200, 200, angle);

        //float shiftAmount = Mathf.Abs(angle) * 0.9f;

        //int movingDirectino = angle <= 0 ? -1 : 1;

        //compassTransform.anchoredPosition = new Vector2(shiftAmount , 0);
        compassTransform.anchoredPosition = new Vector2(shiftAmount, 0f);

        //Debug.Log("AngleToNorth: " + angle + " ShiftAmount: " + shiftAmount);

        // Make copy of the current Background and add the new Background at the end of the current background
    }

    private void UpdateMarkers()
    {
        float compassWidth = compassTransform.rect.width / 2;

        foreach ((GameObject, GameObject) marker in markerObjects)
        {
            GameObject target = marker.Item1;
            RectTransform markerObjectTransform = marker.Item2.GetComponent<RectTransform>();

            UpdateMarkerPosition(target, markerObjectTransform);

            float x = markerObjectTransform.anchoredPosition.x;

            bool iconNotAtEndOfCompass = (x != compassWidth && x != -compassWidth);
            marker.Item2.SetActive(iconNotAtEndOfCompass);
        }
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
                    UpdateMarkerPosition(currentTuple.Item1, currentTuple.Item2.GetComponent<RectTransform>());
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

    private void UpdateMarkerPosition(GameObject target, RectTransform marker)
    {
        Vector3 targetDirection = (target.transform.position - Camera.main.transform.position);
        float angle = Vector2.SignedAngle(new Vector2(targetDirection.x, targetDirection.z), new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z));
        float markerPosition = Mathf.Clamp(2 * angle / Camera.main.fieldOfView, -1, 1);

        marker.anchoredPosition = new Vector2(compassTransform.rect.width / 2 * markerPosition, 0);
    }

    private void CreateAllMarker()
    {
        foreach (GameObject marker in Markers)
        {
            CompassBarMarker compassMarker = marker.GetComponent<CompassBarMarker>();
            GameObject newMarker = CreateMarker(compassMarker, compassMarker.name);
            markerObjects.Add((marker, newMarker));
        }
    }

    private GameObject CreateMarker(CompassBarMarker marker, string name)
    {
        GameObject markerObject = Instantiate(MarkerPrefab, this.transform);
        markerObject.name = name;
        markerObject.GetComponentInChildren<TMP_Text>().text = marker.DisplaySign.ToString();

        return markerObject;
    }

    private GameObject CreateIcon(GameObject itemGameobject)
    {
        // Create new Icon Game Object
        ItemData item = itemGameobject.GetComponent<Item>().item;
        GameObject newItemGameObject = Instantiate(IconPrefab, this.transform);

        // Set the item sprite
        Image newItemImage = newItemGameObject.GetComponent<Image>();
        newItemImage.sprite = item.Icon;

        return newItemGameObject;
    }
}

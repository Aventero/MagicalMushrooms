using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    public float SpriteScale = 0.3f;
    public float MinDistance = 2f;
    public GameObject iconPrefab;

    private RectTransform compassTransform;
    private List<(GameObject, GameObject)> itemList;
    private List<GameObject> itemObjects;

    void Start()
    {
        compassTransform = this.GetComponent<RectTransform>();

        itemList = new List<(GameObject, GameObject)>();
        itemObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Item"));
    }

    void Update()
    {
        UpdateIcons();
    }


    private void UpdateIcons()
    {
        foreach(GameObject itemObject in itemObjects)
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

            if (distance < MinDistance)
            {
                if (currentTuple.Item1 == null)
                {
                    // Create Sprite if not already in list
                    GameObject itemSprite = CreateIcon(itemObject);
                    itemList.Add((itemObject, itemSprite));
                }
                else
                {
                    UpdateMarker(currentTuple.Item1, currentTuple.Item2.GetComponent<RectTransform>());
                }
            }
            else
            {
                // Sprite is out of range -> Remove Sprite
                if(currentTuple.Item2 != null)
                    Destroy(currentTuple.Item2);

                itemList.Remove(currentTuple);
            }
        }
    }

    private GameObject CreateIcon(GameObject itemGameobject)
    {
        // Create new Icon Game Object
        Item item = itemGameobject.GetComponent<Item>();
        GameObject newItemGameObject = Instantiate(iconPrefab, this.transform);

        // Set the item sprite
        Image newItemImage = newItemGameObject.GetComponent<Image>();
        newItemImage.sprite = item.Icon;

        return newItemGameObject;
    }

    private void UpdateMarker(GameObject target, RectTransform marker)
    {
        Vector3 targetDirection = (target.transform.position - Camera.main.transform.position).normalized;
        float angle = Vector2.SignedAngle(new Vector2(targetDirection.x, targetDirection.z), new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z));
        float markerPosition = Mathf.Clamp(angle / Camera.main.fieldOfView, -1, 1);

        marker.anchoredPosition = new Vector2(compassTransform.rect.width / 2 * markerPosition, 0);
    }
}

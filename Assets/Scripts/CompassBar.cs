using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    public float SpriteScale = 0.3f;
    public float MinDistance = 2f;

    private RectTransform compassTransform;
    private List<(GameObject, GameObject)> itemList;
    private GameObject[] itemObjects;

    void Start()
    {
        compassTransform = this.GetComponent<RectTransform>();

        itemList = new List<(GameObject, GameObject)>();
        itemObjects = GameObject.FindGameObjectsWithTag("Item");
        CreateIcons(itemObjects);
    }

    void Update()
    {
        UpdateIcons();
    }

    private void CreateIcons(GameObject[] itemGameObjects)
    {
        for(int i = 0; i < itemGameObjects.Length; i++)
        {
            GameObject newItemGameObject = CreateIcon(itemGameObjects[i]);
            itemList.Add((itemGameObjects[i], newItemGameObject));
        }
    }

    private GameObject CreateIcon(GameObject itemGameobject)
    {
        Item item = itemGameobject.GetComponent<Item>();
        GameObject newItemGameObject = new(item.Name);
        newItemGameObject.transform.parent = this.transform;

        RectTransform rectTransform = newItemGameObject.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = compassTransform.anchoredPosition;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale = new Vector3(SpriteScale, SpriteScale);

        Image newItemImage = newItemGameObject.AddComponent<Image>();
        newItemImage.sprite = item.Icon;

        return newItemGameObject;
    }

    private void UpdateIcons()
    {
        foreach((GameObject, GameObject) itemTuple in itemList)
        {
            GameObject target = itemTuple.Item1;
            GameObject spriteGameObject = itemTuple.Item2;
            float distance = Vector3.Distance(target.transform.position, Camera.main.transform.position);

            if (target == null)
            {
                Destroy(spriteGameObject);
                itemList.Remove(itemTuple);
                return;
            }
            else if( distance > MinDistance)
            {
                if (spriteGameObject != null)
                    Destroy(spriteGameObject);
                return;
            }
            else if(distance < MinDistance && spriteGameObject == null && target != null)
            {
                itemList.Remove(itemTuple);
                itemList.Add((target, CreateIcon(target)));
                return;
            }

            UpdateMarker(target, spriteGameObject.GetComponent<RectTransform>());
        }
    }

    private void UpdateMarker(GameObject target, RectTransform marker)
    {
        Vector3 targetDirection = (target.transform.position - Camera.main.transform.position).normalized;
        float angle = Vector2.SignedAngle(new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z), new Vector2(targetDirection.x, targetDirection.z));
        float markerPosition = Mathf.Clamp(angle / Camera.main.fieldOfView, -1, 1);

        marker.anchoredPosition = new Vector2(compassTransform.rect.width / 2 * markerPosition, 0);
    }
}

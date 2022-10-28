using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    public float SpriteScale = 0.3f;
    public RectTransform FirstMarker;
    public GameObject target;

    private RectTransform compassTransform;
    private List<(GameObject, GameObject)> itemList;

    // Start is called before the first frame update
    void Start()
    {
        itemList = new List<(GameObject, GameObject)>();
        compassTransform = this.GetComponent<RectTransform>();

        CreateIcons(GameObject.FindGameObjectsWithTag("Item"));
    }

    // Update is called once per frame
    void Update()
    {
        UpdateIcons();
    }

    private void CreateIcons(GameObject[] itemGameObjects)
    {
        for(int i = 0; i < itemGameObjects.Length; i++)
        {
            Item item = itemGameObjects[i].GetComponent<Item>();
            GameObject newItemGameObject = new(item.Name);
            newItemGameObject.transform.parent = this.transform;

            RectTransform rectTransform = newItemGameObject.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = compassTransform.anchoredPosition;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.localScale = new Vector3(SpriteScale, SpriteScale);

            Image newItemImage = newItemGameObject.AddComponent<Image>();
            newItemImage.sprite = item.Icon;

            itemList.Add((itemGameObjects[i], newItemGameObject));
        }
    }

    private void UpdateIcons()
    {
        foreach((GameObject, GameObject) itemTuple in itemList)
        {
            GameObject target = itemTuple.Item1;
            GameObject spriteGameObject = itemTuple.Item2;

            if(target == null)
            {
                Destroy(spriteGameObject);
                itemList.Remove((target, spriteGameObject));
                return;
            }

            updateMarker(target, spriteGameObject.GetComponent<RectTransform>());
        }
    }

    private void updateMarker(GameObject target, RectTransform marker)
    {
        Vector3 targetDirection = target.transform.position - Camera.main.transform.position;
        float angle = Vector2.SignedAngle(new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z), new Vector2(targetDirection.x, targetDirection.z));
        float markerPosition = Mathf.Clamp(angle / Camera.main.fieldOfView, -1, 1);

        marker.anchoredPosition = new Vector2(compassTransform.rect.width / 2 * markerPosition, 0);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EvenUISpacing : MonoBehaviour
{
    public float ButtonHeight;
    public float ButtonWidth;

    public Vector2 Pivot;

    private float childCount;
    private float spaceBetweenButtons;
    private float totalHeight;

    private void Start()
    {
        childCount = transform.childCount;
        totalHeight = GetComponent<RectTransform>().sizeDelta.y;

        spaceBetweenButtons = (totalHeight - (childCount * ButtonHeight)) / childCount;

        for (int i = 0; i < childCount; i++)
        {
            RectTransform child = (RectTransform)transform.GetChild(i);
            child.pivot = Pivot;
            child.localPosition = new Vector3(0, -(i * (spaceBetweenButtons + ButtonHeight)),0);
            child.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);
        }
    }

    private void Update()
    {
        Start();
    }
}

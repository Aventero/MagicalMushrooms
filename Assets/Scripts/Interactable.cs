using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    [Header("Sprites:")]
    public Sprite InRangeSprite;
    public Sprite OutOfRangeSprite;

    public Image Image;

    [Header("Properties:")]
    public float interactionSize = 2f;

    private GameObject player;

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if(Image)
            Image.sprite = OutOfRangeSprite;

        CreateSphereCollider();
    }

    protected void Update()
    {
        if(Image)
            Image.transform.LookAt(player.transform);
    }

    protected virtual void CreateSphereCollider()
    {
        SphereCollider sphereCollider = this.gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = interactionSize;
        sphereCollider.isTrigger = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, interactionSize);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player"))
            return;

        Image.sprite = InRangeSprite;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player"))
            return;

        Image.sprite = OutOfRangeSprite;
    }
}
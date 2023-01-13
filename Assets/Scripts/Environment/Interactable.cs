using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour
{
    [Header("Sprites:")]
    public Sprite InRangeSprite;
    public Image Image;

    private GameObject player;

    protected void Start()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        player = GameObject.FindGameObjectWithTag("Player");

        if (Image)
        {
            Image.sprite = InRangeSprite;
            Image.gameObject.SetActive(false);
        }
    }

    protected void Update()
    {
        if(Image)
            Image.transform.LookAt(player.transform);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player"))
            return;

        //Image.gameObject.SetActive(true);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.tag.Equals("Player"))
            return;

        //Image.gameObject.SetActive(false);
    }

    public abstract void Interact();
    public abstract void InPlayerSight();
    public abstract void OutOfPlayerSight();
}

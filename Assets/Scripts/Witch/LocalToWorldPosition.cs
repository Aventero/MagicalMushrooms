using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalToWorldPosition : MonoBehaviour
{
    public GameObject LocalObject;

    // Start is called before the first frame update
    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = LocalObject.transform.position;
    }
}

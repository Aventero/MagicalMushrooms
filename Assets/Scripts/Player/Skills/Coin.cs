using UnityEngine;

class Coin : MonoBehaviour
{
    public float travelSpeed;

    private bool enableVacuum = false;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");    
    }

    public void ActivateVacuum()
    {
        enableVacuum = true;
    }

    private void Update()
    {
        if (!enableVacuum)
            return;

        float percentage = 0;

        Vector3.Lerp(this.transform.position, player.transform.position, percentage);
    }
}

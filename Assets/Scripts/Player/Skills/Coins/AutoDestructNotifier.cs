using UnityEngine;

public class AutoDestructNotifier : MonoBehaviour
{
    public SpawnZoneManager manager;

    private void OnDestroy()
    {
        manager.NotifyDestroyed(gameObject);
    }
}
using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class DespawnEffect : MonoBehaviour
{
    private float delay = 2.0f;

    void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(DespawnAfterDelay());
        }
    }

    private IEnumerator DespawnAfterDelay()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            yield return new WaitForSeconds(delay);
            NetworkObject networkObject = GetComponent<NetworkObject>();
            if (networkObject != null && networkObject.IsSpawned)
            {
                networkObject.Despawn();
            }
        }
    }
}

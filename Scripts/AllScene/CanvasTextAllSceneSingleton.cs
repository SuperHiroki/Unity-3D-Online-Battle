using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTextAllSceneSingleton : MonoBehaviour
{
    public static CanvasTextAllSceneSingleton Instance { get; private set; }

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}

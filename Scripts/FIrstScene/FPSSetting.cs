using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSSetting : MonoBehaviour
{
    void Awake()
    {
        #if UNITY_EDITOR
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
        #endif
    }
}

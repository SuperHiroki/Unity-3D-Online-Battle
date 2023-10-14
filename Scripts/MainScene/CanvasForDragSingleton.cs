using UnityEngine;

public class CanvasForDragSingleton : MonoBehaviour
{
    //Singletonでキャンバスにアクセスできるように
    public static CanvasForDragSingleton Instance { get; private set; }
    public RectTransform CanvasRectTransform { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CanvasRectTransform = GetComponent<RectTransform>();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


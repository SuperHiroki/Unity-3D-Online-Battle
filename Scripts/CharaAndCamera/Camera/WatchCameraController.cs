using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System.Collections.Generic;
using UnityEngine.UI;

public class WatchCameraController : MonoBehaviour
{
    //グローバル変数
    private bool isDead => GameManager.IsDead;
    private bool isDash => GameManager.IsDash;
    private bool isDragging => GameManager.IsDragging;
    
    //ローカル変数
    private float speed = 36.0f;
    private float dashMultiply = 1.8f;
    private float rotationSpeed = 2.0f;
    private Vector3 startDragPosition;
    private float vertical;
    private float horizontal;
    private Image DragArrowImage;
    private RectTransform canvasRectTransform;
    private Vector3 movementOneFrameBefore = Vector3.zero;

    void Awake()
    {
        AudioListener audioListener = GetComponent<AudioListener>();
        if (audioListener != null)
        {
            Destroy(audioListener);
        }
    }

    private void Start()
    {
        canvasRectTransform = CanvasForDragSingleton.Instance.CanvasRectTransform;
        DragArrowImage = RoomPlayerInfo.imagesMainScene["DragArrow"];
    }

    void Update()
    {
        if (!isDead)
        {
            return;
        }

        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        if (Input.GetMouseButtonDown(0) && IsWithinStartDragRange(Input.mousePosition))
        {
            StartDragging(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            PerformDragging(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopDragging();
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsWithinStartDragRange(Input.mousePosition))
                    {
                        StartDragging(touch.position);
                    }
                    break;
                case TouchPhase.Moved:
                    PerformDragging(touch.position);
                    break;
                case TouchPhase.Ended:
                    StopDragging();
                    break;
            }
        }

        Vector3 forwardMovement = gameObject.transform.forward * vertical;
        Vector3 rightMovement = gameObject.transform.right * horizontal;
        Vector3 movement = (forwardMovement + rightMovement) * speed * Time.deltaTime;
        movement.y = 0;
        transform.position += movement;

        //方向転換
        if (movement != Vector3.zero)
        {
            movementOneFrameBefore = movement;
        }
        else
        {
            if (movementOneFrameBefore != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementOneFrameBefore.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (Input.GetKey(KeyCode.K) || isDash)
        {
            movement *= dashMultiply;
        }
    }

    private void StartDragging(Vector3 startPosition)
    {
        GameManager.SetIsDragging(true);
        startDragPosition = startPosition;
    }

    private void PerformDragging(Vector3 currentPosition)
    {
        if (isDragging)
        {
            float referenceDistance = Screen.width * 0.1f;
            Vector3 movementNormalized = (currentPosition - startDragPosition) / referenceDistance;
            movementNormalized = Vector3.ClampMagnitude(movementNormalized, 1f);
            vertical = movementNormalized.y;
            horizontal = movementNormalized.x;
            UpdateArrow(startDragPosition, currentPosition);
        }
    }

    private void StopDragging()
    {
        GameManager.SetIsDragging(false);
        DeleteArrow();
    }

    private bool IsWithinStartDragRange(Vector3 dragStartPosScreen)
    {
        Vector2 dragStartPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, dragStartPosScreen, null, out dragStartPos);
        Vector2 dragAreaPos = GlobalDefine.ImagesDefineDictMainScene["DragBackground"].position;
        Vector2 dragAreaSize = GlobalDefine.ImagesDefineDictMainScene["DragBackground"].sizeDelta;
        Vector2 halfSize = dragAreaSize * 0.5f;
        Vector2 lowerLeft = dragAreaPos - halfSize;
        Vector2 upperRight = dragAreaPos + halfSize;
        return dragStartPos.x >= lowerLeft.x && dragStartPos.x <= upperRight.x && dragStartPos.y >= lowerLeft.y && dragStartPos.y <= upperRight.y;
    }

    private void UpdateArrow(Vector3 startPosition, Vector3 endPosition)
    {
        DragArrowImage.rectTransform.pivot = new Vector2(0, 0.5f);
        Vector3 direction = endPosition - startPosition;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        DragArrowImage.transform.position = startPosition;
        DragArrowImage.transform.eulerAngles = new Vector3(0, 0, angle);
        float distance = Vector3.Distance(startPosition, endPosition);
        float maxArrowSize = 100.0f;
        float adjustedDistance = Mathf.Min(distance, maxArrowSize);
        DragArrowImage.rectTransform.sizeDelta = new Vector2(adjustedDistance, DragArrowImage.rectTransform.sizeDelta.y);
    }

    private void DeleteArrow()
    {
        DragArrowImage.rectTransform.anchoredPosition = GlobalDefine.ImagesDefineDictMainScene["DragArrow"].position;
    }
}

using UnityEngine;

public class CameraController : MonoBehaviour
{
    //別スクリプトからtargetにキャラを設定してもらう
    [HideInInspector]//インスペクタで参照設定をする必要はない。
    public Transform target;

    //グローバル変数
    private bool isDragging => GameManager.IsDragging;

    //ローカル変数
    private float distanceBehind = 10f;
    private float cameraHeight = 4.0f;
    private float smoothSpeed_move = 0.1f;
    private float smoothSpeed_stop = 1.35f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 currentOffset;
    private bool wasInput = false;

    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (target == null) return;
        if (horizontal != 0 || vertical != 0 || isDragging == true)
        {
            if (!wasInput)
            {
                wasInput = true;
                currentOffset = transform.position - target.position;
            }
            Vector3 desiredPosition = target.position + currentOffset;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed_move);
        }
        else
        {
            if (wasInput)
            {
                wasInput = false;
            }
            /////////////////////////////////////////////////////////カメラの位置の調整
            //現在地から、y軸方向の座標を変更する（ターゲットと同じ高さにする）
            Vector3 adjustedTransformPosition = transform.position;
            adjustedTransformPosition.y = target.position.y;
            //目的地から、y軸方向の座標を変更する（ターゲットと同じ高さにする）
            Vector3 targetPositionBehind = target.position - target.forward * distanceBehind;
            Vector3 adjustedTargetPositionBehind = targetPositionBehind;
            adjustedTargetPositionBehind.y = target.position.y;
            //水平面での目的地（slerpPosition）を求める。
            Vector3 slerpPosition = Vector3.Slerp(adjustedTransformPosition - target.position, adjustedTargetPositionBehind - target.position, smoothSpeed_stop * Time.deltaTime) + target.position;
            //実際に動かす
            slerpPosition.y += cameraHeight;
            transform.position = slerpPosition;
        }
        transform.LookAt(target);
    }
}


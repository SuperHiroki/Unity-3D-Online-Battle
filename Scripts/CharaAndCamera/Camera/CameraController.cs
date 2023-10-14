using UnityEngine;

public class CameraController : MonoBehaviour
{
    //�ʃX�N���v�g����target�ɃL������ݒ肵�Ă��炤
    [HideInInspector]//�C���X�y�N�^�ŎQ�Ɛݒ������K�v�͂Ȃ��B
    public Transform target;

    //�O���[�o���ϐ�
    private bool isDragging => GameManager.IsDragging;

    //���[�J���ϐ�
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
            /////////////////////////////////////////////////////////�J�����̈ʒu�̒���
            //���ݒn����Ay�������̍��W��ύX����i�^�[�Q�b�g�Ɠ��������ɂ���j
            Vector3 adjustedTransformPosition = transform.position;
            adjustedTransformPosition.y = target.position.y;
            //�ړI�n����Ay�������̍��W��ύX����i�^�[�Q�b�g�Ɠ��������ɂ���j
            Vector3 targetPositionBehind = target.position - target.forward * distanceBehind;
            Vector3 adjustedTargetPositionBehind = targetPositionBehind;
            adjustedTargetPositionBehind.y = target.position.y;
            //�����ʂł̖ړI�n�islerpPosition�j�����߂�B
            Vector3 slerpPosition = Vector3.Slerp(adjustedTransformPosition - target.position, adjustedTargetPositionBehind - target.position, smoothSpeed_stop * Time.deltaTime) + target.position;
            //���ۂɓ�����
            slerpPosition.y += cameraHeight;
            transform.position = slerpPosition;
        }
        transform.LookAt(target);
    }
}


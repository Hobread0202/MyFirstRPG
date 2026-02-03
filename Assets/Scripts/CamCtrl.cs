using UnityEngine;
using UnityEngine.InputSystem;

public class CamCtrl : MonoBehaviour
{
    Transform _cameraArm;
    [SerializeField] PlayerCtrl _player;

    [SerializeField] float sensitivity = 100f;  //감도
    [SerializeField] float minY = -80f;
    [SerializeField] float maxY = 80f;
    float xRotation = 0f;   //위아래 각도확인용 변수

    private void Awake()
    {
        _cameraArm = GetComponent<Transform>();
    }

    public void OnCamMove(InputAction.CallbackContext ctx)
    {
        if (_player.IsDead) return;

        //ESC 누른상태면 리턴
        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 cameraMove = ctx.ReadValue<Vector2>();

        //마우스 좌우 이동값
        float mouseX = cameraMove.x * sensitivity * Time.deltaTime;
        //마우스 상하 이동값
        float mouseY = cameraMove.y * sensitivity * Time.deltaTime;

        // Clamp 범위 안에서 상하 회전 (X축)
        xRotation -= mouseY;    //위를 봐야하니까 -
        xRotation = Mathf.Clamp(xRotation, minY, maxY);

        // 좌우 회전 (Y축)
        _cameraArm.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //플레이어 좌우 회전
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 플레이어
    public float sensitivity = 200f;
    public float distance = 7f;

    [SerializeField] private float autoAlignSpeed = 6f;
    [SerializeField] private float autoAlignDelay = 0.25f;
    [SerializeField] private float mouseDeadZone = 0.05f;

    [SerializeField] private float softImpactDuration = 0.08f;
    [SerializeField] private float softImpactMagnitude = 0.04f;
    [SerializeField] private float softImpactDrop = 0.08f;

    [SerializeField] private float hardImpactDuration = 0.18f;
    [SerializeField] private float hardImpactMagnitude = 0.1f;
    [SerializeField] private float hardImpactDrop = 0.18f;

    [SerializeField] private float impactRecoverSpeed = 12f;

    private float yaw;
    private float pitch;
    private float lastMouseInputTime;

    private float currentShakeDuration = 0f;
    private float currentShakeMagnitude = 0f;
    private float currentDropOffset = 0f;
    private float shakeTimer = 0f;

    private PlayerInputController input;
    private PlayerMovement playerMovement;

    void Start()
    {
        input = FindObjectOfType<PlayerInputController>();
        playerMovement = target.GetComponent<PlayerMovement>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PlayLandingImpact(bool hardLanding)
    {
        if (hardLanding)
        {
            currentShakeDuration = hardImpactDuration;
            currentShakeMagnitude = hardImpactMagnitude;
            currentDropOffset = hardImpactDrop;
        }
        else
        {
            currentShakeDuration = softImpactDuration;
            currentShakeMagnitude = softImpactMagnitude;
            currentDropOffset = softImpactDrop;
        }
        shakeTimer = currentShakeDuration;
    }

    Vector3 GetImpactOffset()
    {
        Vector3 impactoffset = Vector3.zero;

        // 아래로 눌리는 효과 부드러운 복원
        currentDropOffset = Mathf.Lerp(currentDropOffset, 0f, impactRecoverSpeed * Time.deltaTime);
        impactoffset += Vector3.down * currentDropOffset;

        // 흔들림 효과
        if(shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;

            float x = Random.Range(-1f, 1f) * currentShakeMagnitude;
            float y = Random.Range(-1f, 1f) * currentShakeMagnitude;

            impactoffset += new Vector3(x, y, 0f);
        }

        return impactoffset;
    }

    private void LateUpdate()
    {
        Rotate();
        Follow();
    }

    void Rotate()
    {
        HandleMouseRotation();
        HandleAutoAlign();

        pitch = Mathf.Clamp(pitch, -30f, 70f);
    }

    void HandleMouseRotation()
    {
        float mouseX = input.MouseDelta.x;
        float mouseY = input.MouseDelta.y;

        if(Mathf.Abs(mouseX) > mouseDeadZone || Mathf.Abs(mouseY) > mouseDeadZone)
        {
            lastMouseInputTime = Time.time;
        }

        yaw += mouseX * sensitivity * Time.deltaTime;
        pitch -= mouseY * sensitivity * Time.deltaTime;
    }

    void HandleAutoAlign()
    {
        if(playerMovement == null)
        {
            return;
        }

        if(Time.time - lastMouseInputTime < autoAlignDelay)
        {
            return;
        }

        if(!playerMovement.HasMoveInput())
        {
            return;
        }

        Vector3 moveDir = playerMovement.GetHorizontalMoveDirection();
        moveDir.y = 0f;

        if (moveDir.sqrMagnitude < 0.001f)
        {
            return;
        }

        float targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
        yaw = Mathf.LerpAngle(yaw, targetYaw, autoAlignSpeed * Time.deltaTime);
    }

    void Follow()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        Vector3 impactOffset = GetImpactOffset();

        transform.position = target.position + offset + impactOffset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}

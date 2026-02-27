using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // 플레이어
    public float sensitivity = 200f;
    public float distance = 5f;

    private float yaw;
    private float pitch;

    private PlayerInputController input;

    void Start()
    {
        input = FindObjectOfType<PlayerInputController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        Rotate();
        Follow();
    }

    void Rotate()
    {
        yaw += input.MouseDelta.x * sensitivity * Time.deltaTime;
        pitch -= input.MouseDelta.y * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -30f, 70f);
    }

    void Follow()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void Update()
    {
        
    }
}

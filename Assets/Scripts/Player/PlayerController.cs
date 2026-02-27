using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform cameraTransform; // 카메라 연결

    private PlayerInputController input;
    private Rigidbody rb;

    public float moveSpeed = 5f;

    void Start()
    {
        input = FindObjectOfType<PlayerInputController>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        // 카메라 기준 방향 가져오기
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // y 제거 (기울어짐 방지)
        forward.y = 0; // 카메라 방향에 따라 캐릭터가 땅으로 파고 들거나 공중에 뜨는 현상 방지
        right.y = 0;

        forward.Normalize();
        right.Normalize(); // 방향 벡터 길이 통일 (없을 시 대각선 이동이 더 빨라짐)

        // 입력 기반 이동 방향
        Vector3 moveDir = forward * input.MoveInput.y + right * input.MoveInput.x;

        // 이동 적용
        rb.velocity = new Vector3(
            moveDir.x * moveSpeed,
            rb.velocity.y,
            moveDir.z * moveSpeed
            );
    }

    void Update()
    {
        
    }
}
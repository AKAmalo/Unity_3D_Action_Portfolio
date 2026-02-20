using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInputController input;
    private bool isGrounded;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float airControl = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInputController>();
    }

    void FixedUpdate()
    {
        isGrounded = CheckGrounded();
        Move();
        Jump();
    }

    // 이동
    private void Move()
    {
        Vector3 moveDir = new Vector3(input.MoveInput.x, 0, input.MoveInput.y).normalized;
        Vector3 targetVelocity = moveDir * moveSpeed;
        Vector3 velocity = rb.velocity;

        // 공중 방향전환 속도 제어
        if(isGrounded)
        {
            // 지상 정상 이동속도
            velocity.x = targetVelocity.x;
            velocity.z = targetVelocity.z;
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

            if (moveDir != Vector3.zero)
            {
                // 점프 중일 때 방향 전환 시 속도 변경
                Vector3 newVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, airControl);

                velocity.x = newVelocity.x;
                velocity.z = newVelocity.z;
            }
            else
            {
                // 방향 입력 없을 시 자연 감속
                horizontalVelocity *= 0.98f;

                velocity.x = horizontalVelocity.x;
                velocity.z = horizontalVelocity.z;
            }
        }
        rb.velocity = velocity;
    }

    // 점프
    private void Jump()
    {
        if (input.ConsumeJump() && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // 점프 입력 받는 즉시 후 입력 차단
        }
    }

    // 바닥 체크

    private bool CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, 1.2f, groundLayer);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

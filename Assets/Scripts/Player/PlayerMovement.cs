using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerInputController input;
    private bool isGrounded;
    private PlayerStateMachine stateMachine;
    private bool canRotate = true;
    private float rotateSpeed = 360f;
    private float moveSpeedMultiplier = 1f;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Animator animator;

    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float hardLandingThreshold = -7f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInputController>();
    }


    void Start()
    {
        stateMachine = new PlayerStateMachine();
        stateMachine.ChangeState(new IdleState(this));
    }

    void Update()
    {
        stateMachine.Update();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        isGrounded = CheckGrounded();
    }

    // === 상태에서 호출할 함수 ===
    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    public bool HasMoveInput()
    {
        return input.MoveInput != Vector2.zero;
    }

    public Vector2 GetMoveInput()
    {
        return input.MoveInput;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool ConsumeJump()
    {
        return input.ConsumeJump();
    }

    public bool IsRunning()
    {
        return input.RunPressed;
    }

    public void ChangeState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }

    // 점프
    public void Jump()
    {
        if (!isGrounded) return;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 기존 Y속도 제거
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false; // 점프 입력 받는 즉시 후 입력 차단
    }

    public void Rotate(Vector3 moveDir)
    {
        if(!canRotate)
        {
            return;
        }

        if (moveDir == Vector3.zero)    // 0일 때 회전 금지 (미입력 시 떨림 방지)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

        modelRoot.rotation = Quaternion.RotateTowards(      // 부드러운 회전으로 자연스러움
            modelRoot.rotation,
            targetRotation,
            rotateSpeed * Time.deltaTime   // 회전 속도 (deg/sec)
            );
    }

    public void SetCanRotate(bool value)
    {
        canRotate = value;
    }

    public bool CanRotate()
    {
        return canRotate;
    }

    public void SetRotateSpeed(float value)
    {
        rotateSpeed = value;
    }

    public void SetMoveSpeedMultiplier(float value)
    {
        moveSpeedMultiplier = value;
    }

    public float GetMoveSpeedMultiplier()
    {
        return moveSpeedMultiplier;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public Vector3 GetHorizontalMoveDirection()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(horizontalVelocity.sqrMagnitude > 0.01f)
        {
            return horizontalVelocity.normalized;
        }
        return modelRoot.forward;
    }

    public Transform GetModelRoot()
    {
        return modelRoot;
    }

    // 바닥 체크
    private bool CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, 1.2f, groundLayer);
    }

    private void UpdateAnimation()
    {
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float maxSpeed = IsRunning() ? runSpeed : moveSpeed;
        float normalizedSpeed = speed / maxSpeed;

        animator.SetFloat("Speed", normalizedSpeed, 0.1f, Time.deltaTime);

        animator.SetBool("IsGrounded", isGrounded);

        // 낙하 상태 자동 처리
        animator.SetBool("IsFalling", !isGrounded && rb.velocity.y < 0f);
    }
}
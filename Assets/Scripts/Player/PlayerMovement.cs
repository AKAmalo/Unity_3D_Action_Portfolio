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
    private LayerMask groundCheckMask;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask slopeLayer;
    [SerializeField] private LayerMask stairRampLayer;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Animator animator;

    // Slope Movement
    [SerializeField] private float maxSlopeAngle = 45f; // 최대 경사각
    private RaycastHit slopeHit;
    private bool isStepping = false; // 계단 오르기 중인지 여부

    // Step Climb 설정값
 /*   private CapsuleCollider capsuleCollider;
    [SerializeField] private float maxStepHeight = 0.15f; // 실제 올라갈 수 있는 최대 높이
    [SerializeField] private float stepCheckDistance = 0.5f; // 계단 감지 거리
    [SerializeField] private float maxStepAngle = 45f; // 벽 판정 각도 제한
  */

    public float MoveSpeed => moveSpeed;
    public float RunSpeed => runSpeed;
    public float hardLandingThreshold = -7f;

    // === Coyote Time ===
    [SerializeField] private float coyoteTime = 0.1f;
    private float coyoteCounter;

    // === Jump Buffer ===
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    // 공중 판정 확인 시간
    [SerializeField] private float fallGraceTime = 0.12f;
    private float airborneTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInputController>();
    //    capsuleCollider = GetComponent<CapsuleCollider>();
    }


    void Start()
    {
        groundCheckMask = groundLayer | slopeLayer | stairRampLayer;

        stateMachine = new PlayerStateMachine();
        stateMachine.ChangeState(new IdleState(this));
    }

    void Update()
    {
        isGrounded = CheckGrounded();

        // 공중 체공 시간 측정
        if(isGrounded)
            airborneTimer = 0f;
        else
            airborneTimer += Time.deltaTime;

        UpdateTimers();
        stateMachine.Update();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
   //     StepClimb();
        StickToSlope();
    }

    private void UpdateTimers()
    {
        // Coyote Time
        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
        {
            coyoteCounter -= Time.deltaTime;

            if (coyoteCounter < 0f) coyoteCounter = 0f;
        }

        // Jump Buffer
        if (input.ConsumeJump())
            jumpBufferCounter = jumpBufferTime;
        else
        {
            jumpBufferCounter -= Time.deltaTime;

            if (jumpBufferCounter < 0f) jumpBufferCounter = 0f;
        }
    }

    public bool CanJump()
    {
        return coyoteCounter > 0f && jumpBufferCounter > 0f;
    }

    public bool ShouldFall()
    {
        return airborneTimer >= fallGraceTime;
    }

    public void ConsumeJumpBuffer()
    {
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
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
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 기존 Y속도 제거
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false; // 점프 입력 받는 즉시 후 입력 차단

        coyoteCounter = 0f;
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

    public bool IsStepping()
    {
               return isStepping;
    }

    // 계단 Ramp 체크
    public bool IsOnStairRamp()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        bool hit = Physics.Raycast(origin, Vector3.down, 1.5f, stairRampLayer);

        return hit;
    }

    // 경사면 체크
    public bool OnSlope()
    {
        if (IsOnStairRamp())   // 계단 Ramp 위에서는 경사면 판정 비활성화
        {
            return false;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.5f, slopeLayer))
        {
            float angle  = Vector3.Angle(slopeHit.normal, Vector3.up);

            return angle > 0f && angle <= maxSlopeAngle;
        }
        return false;
    }

    // 현재 경사면 Normal 반환
    public Vector3 GetSlopeNormal()
    {
        return slopeHit.normal;
    }

    // 바닥 체크
    private bool CheckGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.2f;

        float sphereRadius = 0.3f;
        float checkDistance = 1.0f;

        RaycastHit hit;

        bool grounded = Physics.SphereCast(origin, sphereRadius, Vector3.down, out hit, checkDistance, groundCheckMask);


        return grounded;
    }

    // 자동 계단 오르기 기능
 /*  private void StepClimb()
    {
        if(!HasMoveInput())
        {
            isStepping = false;
            return;
        }

        isStepping = false; // 매 프레임마다 초기화

        if (!HasMoveInput() || !isGrounded)    // 이동 입력 없거나 공중에서는 실행 X
            return;

        if(rb.velocity.y > 0.1f)   // 점프 상승 중이면 실행 금지
            return;

        Vector3 moveDir = GetHorizontalMoveDirection();

        Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (horizontalVel.magnitude < 0.3f) // 너무 느리면 실행 X
        {
            isStepping = false;
            return;
        }

        // 캡슐 하단 위치 계산
        float colliderBottom = transform.position.y + capsuleCollider.center.y - (capsuleCollider.height * 0.5f);
        // 아래 감지 시작점
        Vector3 lowerOrigin = new Vector3(transform.position.x, colliderBottom + 0.05f, transform.position.z);
        // 위 감지 시작점
        Vector3 upperOrigin = lowerOrigin + Vector3.up * maxStepHeight;

        // 아래 레이 - 장애물 있는지 확인
        bool lowerHit = Physics.Raycast(lowerOrigin, moveDir, out RaycastHit lowerHitInfo, stepCheckDistance, groundLayer);
        // 위 레이 - 공간 비어있는지 확인
        bool upperHit = Physics.Raycast(upperOrigin, moveDir, stepCheckDistance, groundLayer);

        // 계단 판정 - 아래 막혀있고, 위는 비어있어야함.
        if (lowerHit && !upperHit)
        {
            Debug.Log("STEP LOG");
            // 벽 판정 방지 - 표면 각도 검사
            float surfaceAngle = Vector3.Angle(lowerHitInfo.normal, Vector3.up);

            // 너무 가파르면 벽으로 간주
            if (surfaceAngle > maxStepAngle)
                return;

            isStepping = true;

            Vector3 stepMove = (Vector3.up * maxStepHeight) + (moveDir * 0.08f);
            rb.MovePosition(rb.position + stepMove * Time.fixedDeltaTime * 8f);
        }
    }*/

    private void StickToSlope()
    {
        if(!isGrounded)
            return;

        if(IsOnStairRamp())   // 계단 Ramp 위에서는 경사면 밀착 기능 비활성화
            return;

        if (isStepping)   // 계단 오르는 중에는 경사면 밀착 기능 비활성화
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0f;
            rb.velocity = velocity;

            return;
        }

        if (!OnSlope())
            return;

        if(rb.velocity.y > 0f)
            return;

        rb.AddForce(Vector3.down * 30f, ForceMode.Acceleration);

        if(!HasMoveInput())
        {
            Vector3 velocity = rb.velocity;

            velocity.x = 0f;
            velocity.z = 0f;

            rb.velocity = velocity;
        }
    }

    private void UpdateAnimation()
    {
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float normalizedSpeed = Mathf.Clamp01(speed / runSpeed);

        // 항상 RunSpeed 기준으로 정규화 - Walk = 약 0.6 - Run = 약 1.0
        animator.SetFloat("Speed", normalizedSpeed, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);
        // 낙하 상태 자동 처리
        animator.SetBool("IsFalling", !isGrounded && rb.velocity.y < 0f);
    }
}
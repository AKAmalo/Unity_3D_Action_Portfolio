using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

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

    // Player Data
    [SerializeField] private PlayerData playerData;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask slopeLayer;
    [SerializeField] private LayerMask stairRampLayer;
    [SerializeField] private Transform modelRoot;
    [SerializeField] private Animator animator;

    // Dash
    private float dashCooldownTimer = 0f;

    // Slope Movement
    [SerializeField] private float maxSlopeAngle = 45f; // 최대 경사각
    private RaycastHit slopeHit;
    private bool isStepping = false; // 계단 오르기 중인지 여부

    // ScriptableObject 데이터 사용
    public float MoveSpeed => playerData.moveSpeed;
    public float RunSpeed => playerData.runSpeed;
    public float DashSpeed => playerData.dashSpeed;
    public float DashCooldown => playerData.dashCooldown;
    public float JumpForce => playerData.jumpForce;
    public float CoyoteTime => playerData.coyoteTime;
    public float JumpBufferTime => playerData.jumpBufferTime;
    public float FallGraceTime => playerData.fallGraceTime;
    public float HardLandingThreshold => playerData.hardLandingThreshold;

    // === Coyote Time ===
    private float coyoteCounter;

    // === Jump Buffer ===
    private float jumpBufferCounter;

    // 공중 판정 확인 시간
    private float airborneTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInputController>();

        // ScrptableObject 연결 체크
        if (playerData == null)
        {
            Debug.LogError("PlayerData가 연결되지 않았습니다.");
        }
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
        if (isGrounded)
            airborneTimer = 0f;
        else
            airborneTimer += Time.deltaTime;

        UpdateTimers();
        stateMachine.Update();
        UpdateAnimation();

        // Dash 쿨다운 타이머 업데이트
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        StickToSlope();
    }

    private void UpdateTimers()
    {
        // Coyote Time
        if (isGrounded)
            coyoteCounter = CoyoteTime;
        else
        {
            coyoteCounter -= Time.deltaTime;

            if (coyoteCounter < 0f) coyoteCounter = 0f;
        }

        // Jump Buffer
        if (input.ConsumeJump())
            jumpBufferCounter = JumpBufferTime;
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
        return airborneTimer >= FallGraceTime;
    }

    public void ConsumeJumpBuffer()
    {
        jumpBufferCounter = 0f;
        coyoteCounter = 0f;
    }

    public bool CanDash()  // 대쉬 가능 여부 체크
    {
        return dashCooldownTimer <= 0f;
    }

    public bool IsDashing()
    {
        return stateMachine.CurrentState is DashState;
    }

    public void StartDashCooldown()  // 쿨다운 시작 함수, 대쉬 사용 시 호출
    {
        dashCooldownTimer = DashCooldown;
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

    public bool IsRunning()
    {
        return input.RunPressed;
    }

    public bool DashPressed()   // 입력 접근
    {
        return input.ConsumeDash();
    }

    public void ChangeState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    // 점프
    public void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 기존 Y속도 제거
        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);

        isGrounded = false; // 점프 입력 받는 즉시 후 입력 차단

        coyoteCounter = 0f;

        // Event 발생
        GameEvent.OnPlayerJump?.Invoke(
            transform.position,
            transform.rotation);
    }

    public void Rotate(Vector3 moveDir)
    {
        if (!canRotate)
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

        if (horizontalVelocity.sqrMagnitude > 0.01f)
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
            float angle = Vector3.Angle(slopeHit.normal, Vector3.up);

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

    public void StickToSlope()
    {
        if (!isGrounded)
            return;

        if (IsOnStairRamp())   // 계단 Ramp 위에서는 경사면 밀착 기능 비활성화
            return;

        if (isStepping)   // 계단 오르는 중에는 경사면 밀착 기능 비활성화
        {
            Vector3 velocity = rb.velocity;
            velocity.y = 0f;
            rb.velocity = velocity;

            return;
        }

        if (!OnSlope() && !IsDashing())
            return;

        if (rb.velocity.y > 0f)
            return;

        float stickForce = IsDashing() ? 80f : 30f;
        rb.AddForce(Vector3.down * stickForce, ForceMode.Acceleration);

        if (!HasMoveInput() && !IsDashing())
        {
            Vector3 velocity = rb.velocity;

            velocity.x = 0f;
            velocity.z = 0f;

            rb.velocity = velocity;
        }
    }

    public void ResetSlopeRotation()
    {
        Vector3 forward = modelRoot.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
        {
            forward = transform.forward;
        }

        modelRoot.rotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
    }

    // Dash 중에도 경사면 기울기 유지
    public void AlignToGround()
    {
        // 공중이면 기울기 제거
        if (!IsGrounded())
        {
            Vector3 forward = modelRoot.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.001f)
            {
                forward = transform.forward;
            }

            Quaternion uprightRotation =
                Quaternion.LookRotation(forward.normalized, Vector3.up);

            modelRoot.rotation = Quaternion.Slerp(
                modelRoot.rotation,
                uprightRotation,
                12f * Time.deltaTime
                );

            return;
        }

        // 지면 위에서는 기존 경사면 정렬
        if (!TryGetGroundNormal(out Vector3 normal))
            return;

        Vector3 slopeForward = Vector3.ProjectOnPlane(modelRoot.forward, normal).normalized;

        Quaternion targetRotation = Quaternion.LookRotation(slopeForward, normal);

        modelRoot.rotation = Quaternion.Slerp(
            modelRoot.rotation,
            targetRotation,
            10f * Time.deltaTime
            );
    }

    public bool TryGetGroundNormal(out Vector3 normal)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 3f, groundCheckMask))
        {
            normal = hit.normal;
            return true;
        }
        normal = Vector3.up;
        return false;
    }

    private void UpdateAnimation()
    {
        float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        float normalizedSpeed = Mathf.Clamp01(speed / RunSpeed);

        // 항상 RunSpeed 기준으로 정규화 - Walk = 약 0.6 - Run = 약 1.0
        animator.SetFloat("Speed", normalizedSpeed, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);
    }

    // Debug HUD
    // 현재 상태 이름 반환
    public string GetCurrentStateName()
    {
        if (stateMachine.CurrentState == null)
            return "None";

        return stateMachine.CurrentState.GetType().Name;
    }

    // 현재 남은 Dash 쿨다운
    public float GetDashCooldownRemaining()
    {
        return Mathf.Max(0f, dashCooldownTimer);
    }

    // 자신의 Transform 반환
    public Transform GetTransform()
    {
        return transform;
    }
}
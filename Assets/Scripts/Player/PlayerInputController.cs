using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MouseDelta { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHold { get; private set; }
    public bool RunPressed { get; private set; }
    public bool DashPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    public bool ConsumeJump()
    {
        if (JumpPressed)
        {
            JumpPressed = false;
            return true;
        }
        return false;
    }

    public bool ConsumeDash()
    {
        if (DashPressed)
        {
            DashPressed = false;
            return true;
        }
        return false;
    }

    // 상호작용 입력 소비
    public void ConsumeInteract()
    {
        InteractPressed = false;
    }

    void Update()
    {
        // 이동 입력 제어
        MoveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
            );

        // 점프 입력 제어
        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpPressed = true;
        }

        JumpHold = Input.GetKey(KeyCode.Space);

        // 달리기 입력 제어
        RunPressed = Input.GetKey(KeyCode.LeftShift);

        // 마우스 입력 제어
        MouseDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
            );

        // 대쉬
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            DashPressed = true;
        }

        InteractPressed = Input.GetKeyDown(KeyCode.E);
    }
}

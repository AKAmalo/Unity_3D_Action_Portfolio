using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MouseDelta { get; private set; }
    public Vector2 MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }

    public bool ConsumeJump()
    {
        if (JumpPressed)
        {
            JumpPressed = false;
            return true;
        }
        return false;
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

        // 마우스 입력 제어
        MouseDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
            );
    }

    void Start()
    {

    }
}

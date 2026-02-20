using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public Vector2 MoveInput {get; private set;}
    public bool JumpPressed { get; private set;}

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
    }

    void Start()
    {

    }
}

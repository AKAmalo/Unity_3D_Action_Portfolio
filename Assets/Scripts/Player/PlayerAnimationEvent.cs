using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    // Animation Event 전용 스크립트
    // Animator가 붙어있는 Y Bot에서 Animation Event를 받을 수 있도록
    // 별도의 컴포넌트를 만들어준다.


    // Animation Event에서 호출할 함수
    // 반드시 public이어야 한다.
    // Animation Event에서는 매개변수가 없는 함수로 사용한다.
    public void Footstep()
    {
        // Y Bot의 부모 중에서 PlayerMovement를 찾는다.
        PlayerMovement player =
            GetComponentInParent<PlayerMovement>();

        // PlayerMovement를 찾지 못한 경우
        if (player == null)
        {
            Debug.LogWarning(
                "PlayerAnimationEvent : PlayerMovement를 찾을 수 없습니다.");
            return;
        }

        // 현재 플레이어가 지면에 있을 때만 발소리를 재생한다.
        if (!player.IsGrounded())
            return;

        if (!player.HasMoveInput())
            return;

        // 플레이어 위치를 전달해서 발소리 이벤트 발생
        GameEvent.OnPlayerFootstep?.Invoke(
            player.GetTransform().position);
    }
}
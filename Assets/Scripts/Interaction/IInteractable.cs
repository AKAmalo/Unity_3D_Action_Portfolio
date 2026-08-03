using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어와 상호작용 가능한 오브젝트가 구현해야 하는 인터페이스
/// </summary>

public interface IInteractable
{
    /// <summary>
    /// UI에 표시할 상호작용 문구
    /// ex) Open Door, Pull Lever
    /// </summary
    string GetInteractionText();

    /// <summary>
    /// 플레이어가 상호작용(E키)을 눌렀을 때 호줓
    /// </summary> 
    void Interact(PlayerMovement player);
}
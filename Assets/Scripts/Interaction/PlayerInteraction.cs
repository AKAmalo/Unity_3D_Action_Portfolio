using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 상호작용을 담당하는 클래스
/// </summary>

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask interactLayer;

    private PlayerMovement player;
    private PlayerInputController input;
    private IInteractable currentInteractable;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        input = GetComponent<PlayerInputController>();
    }

    private void Update()
    {
        DetectInteraction();

        // E키 입력
        if(input.InteractPressed)
        {
            TryInteract();
            input.ConsumeInteract();
        }
    }

    /// <summary>
    /// 디버그용 Ray 표시
    /// </summary>
    private void DetectInteraction()
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * interactDistance, Color.green);

        currentInteractable = null;

        Ray ray = new Ray(origin, direction);

        if(Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            currentInteractable = hit.collider.GetComponent<IInteractable>();
        }
    }

    /// <summary>
    /// 상호작용 시도
    /// </summary>
    private void TryInteract()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact(player);
    }

    /// <summary>
    /// 현재 바라보고 있는 상호작용 오브젝트 반환
    /// UI에서 사용할 예정
    /// </summary>
    public IInteractable GetCurrentInteractable()
    {
        return currentInteractable;
    }
}
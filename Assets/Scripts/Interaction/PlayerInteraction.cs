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
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * interactDistance, Color.green);
    }

    /// <summary>
    /// 상호작용 시도
    /// </summary>
    private void TryInteract()
    {
        Ray ray = new Ray(transform.position + Vector3.up, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
         IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if ( (interactable != null))
            {
                interactable.Interact(player);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


/// <summary>
/// 플레이어가 바라보고 있는 상호작용 오브젝트의 안내 UI
/// </summary>
public class InteractionUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private TextMeshProUGUI interactionText;

    private void Update()
    {
        if (playerInteraction == null || interactionText == null)
            return;

        IInteractable interactable = playerInteraction.GetCurrentInteractable();

        if (interactable == null)
        {
            interactionText.gameObject.SetActive(false);
            return;
        }

        interactionText.gameObject.SetActive(true);

        interactionText.text = $"[E] {interactable.GetInteractionText()}";
    }
}

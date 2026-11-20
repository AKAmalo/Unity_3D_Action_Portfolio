using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 다른 오브젝트를 활성화시키는 레버
public class Lever : MonoBehaviour, IInteractable
{
    [Header("Target")]
    [SerializeField] private List<MonoBehaviour> targets = new();

    [Header("Interaction")]
    [SerializeField] private string activateText = "Pull Lever";
    [SerializeField] private string deactivateText = "Reset Lever";

    [Header("Lever Movement")]
    [SerializeField] private Transform leverHandle;
    [SerializeField] private float activateAngle = -45f;
    [SerializeField] private float leverMoveSpeed = 8f;

    private readonly List<IActivatable> activatables = new();

    private bool isActivated;
    private bool isMoving;

    private Quaternion defaultRotation;
    private Quaternion activatedRotation;

    private void Awake()
    {
        // Target에서 IActivatable 구현 여부 확인
        foreach (MonoBehaviour target in targets)
        {
            if (target is IActivatable activatable)
            {
                activatables.Add(activatable);
            }
            else
            {
                Debug.LogError($"{name} : Target이 IActivatable을 구현하지 않았습니다.");
            }
        }

        // 레버 손잡이가 지정되어 있으면 초기 회전 저장
        if (leverHandle != null)
        {
            defaultRotation = leverHandle.localRotation;

            activatedRotation = defaultRotation * Quaternion.Euler(activateAngle, 0f, 0f);
        }
    }

    private void Update()
    {
        UpdateLeverAnimation();
    }

    public string GetInteractionText()
    {
        return isActivated ? deactivateText : activateText;
    }

    public void Interact(PlayerMovement player)
    {
        Toggle();
    }

    private void Toggle()
    {
        if (isMoving)
            return;

        isActivated = !isActivated;

        // 연결된 모든 대상 활성화
        foreach (IActivatable activatable in activatables)
        {
            activatable.Activate();
        }
    }

    // 레버 손잡이 회전
    private void UpdateLeverAnimation()
    {
        if (leverHandle == null)
            return;

        Quaternion targetRotation = isActivated ? activatedRotation : defaultRotation;

        leverHandle.localRotation = Quaternion.Slerp(
            leverHandle.localRotation,
            targetRotation,
            leverMoveSpeed * Time.deltaTime
            );

        if (Quaternion.Angle(
            leverHandle.localRotation,
            targetRotation) < 0.1f)
        {
            leverHandle.localRotation = targetRotation;
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }
}

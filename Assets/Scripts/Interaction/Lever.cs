using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [Header("Target")]
    [SerializeField] private List<MonoBehaviour> targets = new();

    [Header("Interaction")]
    [SerializeField] private string interactionText = "Pull Lever";

    private readonly List<IActivatable> activatables = new();

    private void Awake()
    {
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
    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    public void Interact(PlayerMovement player)
    {
        foreach (IActivatable activatable in activatables)
        {
            activatable.Activate();
        }
    }
}

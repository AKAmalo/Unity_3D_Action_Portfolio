using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;

    private bool isOpen;
    private Quaternion closedRotation;
    private Quaternion openedRotation;

    private void Awake()
    {
        closedRotation = transform.rotation;
        openedRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    public string GetInteractionText()
    {
        return isOpen ? "Close Door" : "Open Door";
    }

    public void Interact(PlayerMovement player)
    {
        StopAllCoroutines();
        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        isOpen = !isOpen;

        Quaternion target = isOpen ? openedRotation : closedRotation;

        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                target,
                Time.deltaTime * openSpeed);

            yield return null;
        }
        transform.rotation = target;
    }
}

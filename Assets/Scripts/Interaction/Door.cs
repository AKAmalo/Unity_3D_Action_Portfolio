using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Door")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float openSpeed = 2f;

    [Header("Interaction text")]
    [SerializeField] private string openText = "Open Door";
    [SerializeField] private string closeText = "Close Door";

    private bool isOpen;
    private bool isMoving;
    private Quaternion closedRotation;
    private Quaternion openedRotation;

    private void Awake()
    {
        closedRotation = transform.rotation;
        openedRotation = closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }

    public string GetInteractionText()
    {
        return isOpen ? closeText : openText;
    }

    public void Interact(PlayerMovement player)
    {
      if (isMoving)
            return;

        StartCoroutine(OpenDoor());
    }

    private IEnumerator OpenDoor()
    {
        isMoving = true;
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
        isMoving = false;
    }
}

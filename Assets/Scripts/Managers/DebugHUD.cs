using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private TextMeshProUGUI debugText;

    private void Update()
    {
        if (player == null || debugText == null)
            return;

        string stateName = player.GetCurrentStateName();

        float speed =
            new Vector3(
                player.GetRigidbody().velocity.x,
                0f,
                player.GetRigidbody().velocity.z).magnitude;

        string info =
            $"State: {stateName}\n" +
            $"Grounded : {player.IsGrounded()}\n" +
            $"Speed: {speed:F2}\n" +
            $"Dash CD : {player.GetDashCooldownRemaining():F2}\n" +
            $"Move Input : {player.GetMoveInput()}";

        debugText.text = info;
    }
}

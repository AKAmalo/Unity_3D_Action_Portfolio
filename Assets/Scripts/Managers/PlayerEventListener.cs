using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEventListener : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvent.OnPlayerJump += OnJump;
        GameEvent.OnPlayerDash += OnDash;
        GameEvent.OnPlayerLand += OnLand;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerJump -= OnJump;
        GameEvent.OnPlayerDash -= OnDash;
        GameEvent.OnPlayerLand -= OnLand;
    }

    private void OnJump()
    {
        Debug.Log("EVENT : Player Jump");
    }

    private void OnDash()
    {
        Debug.Log("EVENT : Player Dash");
    }

    private void OnLand()
    {
        Debug.Log("EVENT : Player Land");
    }
}

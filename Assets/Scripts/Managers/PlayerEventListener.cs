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

    private void OnJump(Vector3 position, Quaternion rotation)
    {
        Debug.Log("EVENT : Player Jump");
    }

    private void OnDash(Vector3 position, Quaternion rotation)
    {
        Debug.Log("EVENT : Player Dash");
    }

    private void OnLand(Vector3 position, Quaternion rotation, bool hardLanding)
    {
        Debug.Log(hardLanding ? "EVENT : Hard Land" : "EVENT : Soft Land");
    }
}

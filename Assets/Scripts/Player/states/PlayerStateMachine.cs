using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine
{
    public IPlayerState CurrentState {  get; private set; }

    public void ChangeState(IPlayerState newstate)
    {
        CurrentState?.Exit();
        CurrentState = newstate;
        CurrentState?.Enter();
    }

    public void Update()
    {
        CurrentState?.Update();
    }
}

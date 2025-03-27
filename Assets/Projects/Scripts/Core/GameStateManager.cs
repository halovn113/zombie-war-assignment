using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public GAME_STATE CurrentState { get; private set; }

    public event Action<GAME_STATE> OnStateChanged;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void ChangeState(GAME_STATE newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }
}

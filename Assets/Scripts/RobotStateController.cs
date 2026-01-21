using System;
using UnityEngine;

public class RobotStateController : MonoBehaviour
{
    public RobotState State;
    public event Action OnStateChanged;

    public void SetState(RobotState newState)
    {
        if (State == newState) return;

        State = newState;
        OnStateChanged?.Invoke();
    }
}

public enum RobotState { Moving, Winning, Dying, Undefined }



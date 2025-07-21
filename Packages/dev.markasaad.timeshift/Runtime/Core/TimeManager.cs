using System;
using System.Collections.Generic;

using UnityEngine;

public class TimeManager : MonoBehaviour {
    private static TimeManager s_instance;
    public static TimeManager Instance {
        get {
            if (s_instance == null) {
                s_instance = FindAnyObjectByType<TimeManager>();
            }
            return s_instance;
        }
    }

    [SerializeReference, SubclassSelector]
    public List<TimeState> States = new List<TimeState>();

    [SerializeReference, SubclassSelector]
    public TimeState CurrentState;

    private Action m_onSaveState;
    private Action m_onLoadNextState;
    private Action m_onLoadPreviousState;

    public void RegisterTimeControllable(ITimeControllable timeControllable) {
        m_onSaveState += timeControllable.SaveState;
        m_onLoadNextState += timeControllable.LoadNextState;
        m_onLoadPreviousState += timeControllable.LoadPreviousState;
    }

    public void UnregisterTimeControllable(ITimeControllable timeControllable) {
        m_onSaveState -= timeControllable.SaveState;
        m_onLoadNextState -= timeControllable.LoadNextState;
        m_onLoadPreviousState -= timeControllable.LoadPreviousState;
    }


    public void SetState(TimeState state) {
        if (CurrentState == state)
            return;
        CurrentState?.OnExitState();
        CurrentState = state;
        CurrentState?.OnEnterState();
    }

    public void SetStateToType<T>() where T : TimeState {
        if (CurrentState is T)
            return;

        foreach (var state in States) {
            if (state is T) {
                SetState(state);
                return;
            }
        }

        Debug.LogWarning($"No state of type {typeof(T).Name} found.");
    }

    private void Start() {
        if (States.Count == 0) {
            enabled = false;
            return;
        }

        SetState(States[0]);
    }

    private void Update() {
        CurrentState.Update(m_onSaveState, m_onLoadNextState, m_onLoadPreviousState);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace qt.AI
{
    public class StateMachine : MonoBehaviour
    {
        private Dictionary<string, BaseState> _states;

        private BaseState _currentState;
        
        public BaseState CurrentState { get; private set; }

        void Start()
        {
            GetStates();
        }

        public void SetState(string stateName)
        {
            if (!_states.TryGetValue(stateName.ToLower(), out var state))
            {
                Debug.LogError($@"State ""{stateName}"" not found.");
                return;
            }
            
            if(_currentState != null)
                _currentState.ExitState();

            _currentState = state;
            _currentState.EnterState();
        }

        private void GetStates()
        {
            // get all states of the current object.
            _states = new Dictionary<string, BaseState>();
            var states = gameObject.GetComponents<BaseState>();

            foreach (var state in states)
            {
                state.StateMachine = this;
                _states.Add(states.GetType().Name.ToLower(), state);
            }
        }

        private void Update()
        {
            if (_currentState == null) return;
            _currentState.Think();
            _currentState.Act();
        }
    }
}
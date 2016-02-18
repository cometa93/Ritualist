using System;
using System.Collections.Generic;

namespace DevilMind
{
    public delegate void TransitionActionCallback();
    public delegate void OnStateChangeCallback(int finishedState, Action onFinish);

    public class StateMachine
    {
#region Variables and Constructors
        private int _currentState = -1;
        private readonly Dictionary<string, StateTransition> _transitions;

        public string Name { get; private set; }
        public StateMachine( string name = "Default")
        {
            _transitions = new Dictionary<string, StateTransition>();
            Name = name;
        }
#endregion

        public void RegisterTransition(int state,int command,int finishState,TransitionActionCallback action)
        {
            if (state < 0 || command < 0 || finishState < 0)
            {
                return;
            }

            StateTransition transition = new StateTransition(state,command,finishState,action,SetState);
            if (_transitions.ContainsKey(transition.GetID()))
            {
                Log.Error(MessageGroup.Common, "Given transition is already registered in state machine named : " + Name);
                return;
            }
            _transitions[transition.GetID()] = transition;

        }

        public void Switch(int command)
        {
            if (_currentState == -1)
            {
                return;
            }
            if (command < 0)
            {
                Log.Error(MessageGroup.Common, "Given command is smaller than zero in state machine named : " + Name);
                return;
            }
            var transitionID = StateTransition.GenerateID(_currentState, command);
            if (!_transitions.ContainsKey(transitionID))
            {
                return;
            }
            _transitions[transitionID].MakeTransition();

        }

        private void SetState(int newState,Action onFinish)
        {
            _currentState = newState;
            onFinish();
        }

        public void InitializeMachineState(int firstState)
        {
            if (_currentState > 0)
            {
                Log.Error(MessageGroup.Common, "Current state machine named: " + Name + " is already initialized.");
                return;
            }
            _currentState = firstState;
        }

        public void Reset()
        {
            _currentState = -1;
            foreach (var keyValue in _transitions)
            {
                keyValue.Value.Destroy();
            }
            _transitions.Clear();
        }
    
    }
}
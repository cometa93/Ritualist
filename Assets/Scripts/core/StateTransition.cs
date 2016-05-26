namespace DevilMind
{
    public class StateTransition
    {
        private readonly int _state;
        private readonly int _command;
        private readonly int _nextState;
        private bool _waitForDestroy;

        private readonly TransitionActionCallback _transitionActionCallback;
        private readonly OnStateChangeCallback _onStateChangeCallback;

        public StateTransition(int state, int command, int finishState, TransitionActionCallback transitionActionCallback, OnStateChangeCallback onStateChangeCallback)
        {
            _state = state;
            _command = command;
            _transitionActionCallback = transitionActionCallback;
            _onStateChangeCallback = onStateChangeCallback;
            _nextState = finishState;
        }

        public void Destroy()
        {
            _waitForDestroy = true;
        }

        public void MakeTransition()
        {
            if (_waitForDestroy)
            {
                return;
            }

            if (_onStateChangeCallback != null && _waitForDestroy == false)
            {
                _onStateChangeCallback(_nextState, () =>
                {
                    if (_transitionActionCallback != null && _waitForDestroy == false)
                    {
                            _transitionActionCallback();
                    }
                });

            }
        }

        public string GetID()
        {
            return _state + "##" + _command;
        }

        public static string GenerateID(int state, int command)
        {
            return state + "##" + command;
        }
    }
}
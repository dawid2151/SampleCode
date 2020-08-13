using System;
using System.Collections.Generic;


namespace EdTools.Unity
{
    public class StateMachine
    {
        public IState CurrentState { get { return _currentState; } }

        private IState _currentState;
        private Dictionary<Type, List<Transition>> _allTransitions = new Dictionary<Type, List<Transition>>();
        private List<Transition> _currentStateTransitions = new List<Transition>();
        private List<Transition> _anyTransitions = new List<Transition>();
        private List<Transition> _emptyTransitions = new List<Transition>();

        public void Tick()
        {
            foreach (Transition t in _anyTransitions)
            {
                if (t.Condition())
                {
                    SetState(t.To);
                    break;
                }
            }

            foreach (Transition t in _currentStateTransitions)
            {
                if (t.Condition())
                {
                    SetState(t.To);
                    break;
                }
            }

            _currentState?.OnTick();
        }

        public void SetState(IState newState)
        {
            if (_currentState == newState)
                return;

            _currentState?.OnExit();

            //copy possible transitions to _currentStateTransitions (to limit loops in Tick())
            _allTransitions.TryGetValue(newState.GetType(), out _currentStateTransitions);
            if (_currentStateTransitions == null)
                _currentStateTransitions = _emptyTransitions;

            _currentState = newState;

            _currentState.OnEnter();
        }

        /// <summary>
        /// Add transition from "one given state" to "another given state" when condition is met.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="condition"></param>
        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            if (_allTransitions.ContainsKey(from.GetType()))
            {
                _allTransitions[from.GetType()].Add(new Transition(from, to, condition));
            }
            else
            {
                _allTransitions.Add(from.GetType(), new List<Transition> { new Transition(from, to, condition) });
            }
        }
        /// <summary>
        /// Add transition from "any" to "given" state when condition is met.
        /// </summary>
        /// <param name="to"></param>
        /// <param name="condition"></param>
        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            _anyTransitions.Add(new Transition(null, to, condition));
        }
    }
}

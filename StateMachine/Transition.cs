using System;

namespace EdTools.Unity
{
    public class Transition
    {
        public IState From;
        public IState To;
        public Func<bool> Condition;

        public Transition(IState from, IState to, Func<bool> condition)
        {
            From = from;
            To = to;
            Condition = condition;
        }
    }
}

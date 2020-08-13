namespace EdTools.Unity
{
    public interface IState
    {
        void OnEnter();
        void OnTick();
        void OnExit();
    }
}

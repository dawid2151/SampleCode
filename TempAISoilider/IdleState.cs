using EdTools.Unity;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : IState
{
    private NavMeshAgent _agent;
    private AnimationUpdater _animationUpdater;

    public IdleState(NavMeshAgent agent, AnimationUpdater updater)
    {
        _agent = agent;
        _animationUpdater = updater;
    }

    public void OnEnter()
    {
        _agent.ResetPath();
        _animationUpdater.SetWalkSpeed(0f);
        Debug.Log("Entered Idle");
    }

    public void OnExit()
    {
        
    }

    public void OnTick()
    {
        
    }

    private void SetTargetPoint(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }
}

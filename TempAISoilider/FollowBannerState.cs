using EdTools.Unity;
using UnityEngine;
using UnityEngine.AI;

public class FollowBannerState : IState
{
    private NavMeshAgent _agent;
    private AnimationUpdater _animationUpdater;
    private Transform _target;

    public FollowBannerState(NavMeshAgent agent, AnimationUpdater updater, Transform banner)
    {
        _agent = agent;
        _animationUpdater = updater;
        _target = banner;
    }

    public void OnEnter()
    {
        _animationUpdater.SetWalkSpeed(1f);
        Debug.Log("To the banner!");
        
    }

    public void OnExit()
    {
        _agent.ResetPath();
    }

    public void OnTick()
    {
        _agent.SetDestination(_target.position);
    }

}

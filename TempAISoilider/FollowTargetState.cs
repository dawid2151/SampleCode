using EdTools.Unity;
using UnityEngine;
using UnityEngine.AI;

public class FollowTargetState : IState
{
    private NavMeshAgent _agent;
    private AnimationUpdater _animationUpdater;
    private IAttacker _attacker;
    public FollowTargetState(NavMeshAgent agent, AnimationUpdater updater, IAttacker attacker)
    {
        _agent = agent;
        _animationUpdater = updater;
        _attacker = attacker;
    }

    public void OnEnter()
    {
        _agent.SetDestination((_attacker.Target as MonoBehaviour).transform.position);
        _animationUpdater.SetWalkSpeed(1f);
        Debug.Log("Follow enemy " + (_attacker.Target as MonoBehaviour).name);
    }

    public void OnExit()
    {
        _agent.ResetPath();
    }

    public void OnTick()
    {
        
    }
}

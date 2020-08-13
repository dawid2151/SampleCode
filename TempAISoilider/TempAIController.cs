using EdTools.Unity;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class TempAIController : MonoBehaviour, IAttacker
{
    public GroundOwner Owner { get { return GameManager.PlayerOwnership; } }
    public IAttackable Target { get; set; }

    [SerializeField] private AnimationUpdater _animationUpdater;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _commandBanner;
    [SerializeField] private float _closingDistance = 5f;
    [SerializeField] private float _attackRange = 2f;

    private StateMachine _stateMachine;
    void Awake()
    {
        _stateMachine = new StateMachine();

        var idleState = new IdleState(_agent, _animationUpdater);
        var followBannerState = new FollowBannerState(_agent, _animationUpdater, _commandBanner);
        var followTargetState = new FollowTargetState(_agent, _animationUpdater, this);
        var attackState = new AttackState(this, _animationUpdater);

        _stateMachine.AddAnyTransition(followBannerState, BannerNotInRange);
        _stateMachine.AddAnyTransition(followTargetState, TargetNotInRange);
        _stateMachine.AddAnyTransition(attackState, TargetInRange);

        _stateMachine.AddTransition(attackState, idleState, HasNoTarget);
        _stateMachine.AddTransition(followTargetState, idleState, HasNoTarget);
        _stateMachine.AddTransition(followBannerState, idleState, BannerInRange);

        _stateMachine.SetState(idleState);
    }

    void Update()
    {
        _stateMachine.Tick();
    }


    private bool BannerInRange()
    {
        if (transform.position.Distance(_commandBanner.position, Axis.Y) < _closingDistance)
            return true;

        return false;
    }
    private bool BannerNotInRange()
    {
        return !BannerInRange();
    }

    private bool HasTarget()
    {
        if (Target != null)
            return true;
        return false;
    }
    private bool HasNoTarget()
    {
        return !HasTarget();
    }
    private bool TargetInRange()
    {
        if (HasTarget() && (transform.position.Distance((Target as MonoBehaviour).transform.position, Axis.Y) < _attackRange))
            return true;

        return false;
    }
    private bool TargetNotInRange()
    {
        if (!TargetInRange() && HasTarget())
            return true;
        return false;
    }

}

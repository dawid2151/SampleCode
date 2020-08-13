using EdTools.Unity;
using System.Collections;
using UnityEngine;

public class AttackState : IState
{
    private IAttacker _attacker;
    private AnimationUpdater _animationUpdater;
    private float _attackRate = 0.5f;
    private int _damage = 20;
    private Coroutine _attackRoutine;
    public AttackState(IAttacker attacker, AnimationUpdater updater)
    {
        _attacker = attacker;
        _animationUpdater = updater;
    }

    public void OnEnter()
    {
        Debug.Log("Entered Attack");
        _attackRoutine = (_attacker as MonoBehaviour).StartCoroutine(Attack());
        _animationUpdater.SetWalkSpeed(0f);
    }

    public void OnExit()
    {
        (_attacker as MonoBehaviour).StopCoroutine(_attackRoutine);
    }

    public void OnTick()
    {
       
    }

    private IEnumerator Attack()
    {
        while (_attacker.Target.CanBeAttacked)
        {
            _animationUpdater.SetAttackTrigger();
            _attacker.Target.TakeDamage(_attacker, _damage);
            yield return new WaitForSeconds(1f / _attackRate);
        }
    }
}

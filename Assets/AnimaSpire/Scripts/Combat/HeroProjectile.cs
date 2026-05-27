using System;
using UnityEngine;

public class HeroProjectile : MonoBehaviour
{
    private EnemyUnit target;
    private Vector3 lastKnownTargetPosition;
    private float damage;
    private int stageToken;
    private float speed;
    private float hitDistance;
    private Func<int> getCurrentStageToken;
    private Action<HeroProjectile, EnemyUnit, float, int> resolveHit;
    private Action<HeroProjectile> returnToPool;

    public bool IsActive { get; private set; }

    public void Fire(
        Vector3 startPosition,
        EnemyUnit targetEnemy,
        float damageSnapshot,
        int combatStageToken,
        float projectileSpeed,
        float projectileHitDistance,
        Func<int> currentStageTokenProvider,
        Action<HeroProjectile, EnemyUnit, float, int> hitResolver,
        Action<HeroProjectile> poolReturn)
    {
        transform.position = startPosition;
        target = targetEnemy;
        damage = damageSnapshot;
        stageToken = combatStageToken;
        speed = Mathf.Max(projectileSpeed, 0f);
        hitDistance = Mathf.Max(projectileHitDistance, 0f);
        getCurrentStageToken = currentStageTokenProvider;
        resolveHit = hitResolver;
        returnToPool = poolReturn;
        lastKnownTargetPosition = target != null ? target.transform.position : startPosition;
        IsActive = true;
        gameObject.SetActive(true);
    }

    public void ForceReturn()
    {
        ReturnToPool();
    }

    private void Update()
    {
        if (!IsActive)
        {
            return;
        }

        if (getCurrentStageToken == null || getCurrentStageToken.Invoke() != stageToken)
        {
            ReturnToPool();
            return;
        }

        bool targetAlive = target != null && target.IsAlive;
        if (targetAlive)
        {
            lastKnownTargetPosition = target.transform.position;
        }

        float maxDistanceDelta = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, lastKnownTargetPosition, maxDistanceDelta);

        Vector3 distanceVector = lastKnownTargetPosition - transform.position;
        if (distanceVector.sqrMagnitude > hitDistance * hitDistance)
        {
            return;
        }

        if (targetAlive)
        {
            resolveHit?.Invoke(this, target, damage, stageToken);
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        target = null;
        damage = 0f;
        stageToken = 0;
        getCurrentStageToken = null;
        resolveHit = null;
        Action<HeroProjectile> poolReturn = returnToPool;
        returnToPool = null;
        gameObject.SetActive(false);
        poolReturn?.Invoke(this);
    }
}

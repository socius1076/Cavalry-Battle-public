//敵のステータス

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyStatus : ObjectStatus
{
    private Collider _collider = null;
    private NavMeshAgent navMeshAgent = null;
    private Appear appear = null;

    protected override void Start()
    {
        base.Start();
        LifeGaugeContainer.Instance.Add(this);
        _collider = GetComponent<Collider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        appear = GameObject.Find("AppearGate").GetComponent<Appear>();
    }

    private void Update()
    {
        animator.SetFloat("MoveSpeed",navMeshAgent.velocity.magnitude); //AnimatorのMoveSpeed値設定
    }

    protected override void Die()
    {
        _collider.enabled = false;
        base.Die();
        StartCoroutine(DestroyCoroutine());
    }

    private IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
        appear.EnemyCount--;
    }
}

using UnityEngine;
using Photon.Pun;

public abstract class ObjectStatus : MonoBehaviourPunCallbacks
{
    public enum StateEnum
    {
        Normal,
        Attack,
        Die,
        Idle
    }
    public float NowLife = 0.0f;
    public float MaxLife = 5.0f;
    public float Attack = 1.0f;
    protected Animator animator = null;
    //初期状態
    public StateEnum stateEnum = StateEnum.Normal;

    //移動可能かどうか
    public bool MoveAble => StateEnum.Normal == stateEnum;

    //攻撃可能かどうか
    public bool AttackAble => StateEnum.Normal == stateEnum;

    protected virtual void Start()
    {
        NowLife = MaxLife;
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Die()
    {

    }

    public void LifeGaugeDelete()
    {
        LifeGaugeContainer.Instance.Remove(this);
    }

    public virtual void Damage(float damage)
    {
        if(stateEnum == StateEnum.Die) return;
        NowLife -= damage;
        if(NowLife > 0) return;
        stateEnum = StateEnum.Die;
        animator.SetTrigger("Die");
        Die();
    }

    public virtual void GoAttack(int signal)
    {
        if(!AttackAble) return;
        //今後追加
        switch(signal)
        {
            case 0:
                stateEnum = StateEnum.Attack;
                animator.SetTrigger("Attack");
                break;
            default:
                break;
        }
    }

    public void GoNormal()
    {
        if(stateEnum == StateEnum.Die) return;
        stateEnum = StateEnum.Normal;
    }

    public void GoIdle()
    {
        if(stateEnum == StateEnum.Die) return;
        stateEnum = StateEnum.Idle;
    }
}
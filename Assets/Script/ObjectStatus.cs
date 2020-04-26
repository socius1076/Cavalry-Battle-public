//ステータスの親

using UnityEngine;
using Photon.Pun;

public abstract class ObjectStatus : MonoBehaviourPunCallbacks
{
    public enum StateEnum //状態の列挙型
    {
        Normal,
        Attack,
        Die,
        Idle
    }
    public float NowLife; //現在のライフ
    public float MaxLife = 5.0f; //最大ライフ
    public float Attack = 1.0f; //攻撃力
    protected Animator animator = null;
    public StateEnum stateEnum = StateEnum.Normal; //初期状態

    public bool MoveAble //移動可能かどうか
    {
        get
        {
            if(StateEnum.Normal == stateEnum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool AttackAble //攻撃可能かどうか
    {
        get
        {
            if(StateEnum.Normal == stateEnum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

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
        LifeGaugeContainer.Instance.Remove(this); //表示終了
    }

    public virtual void Damage(float damage)
    {
        if(stateEnum == StateEnum.Die)
        {
            return;
        }
        NowLife -= damage;
        if(NowLife > 0)
        {
            return;
        }
        stateEnum = StateEnum.Die;
        animator.SetTrigger("Die");
        Die();
    }

    public virtual void GoAttack(int signal)
    {
        if(AttackAble == false)
        {
            return;
        }
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
        if(stateEnum == StateEnum.Die)
        {
            return;
        }
        stateEnum = StateEnum.Normal;
    }

    public void GoIdle()
    {
        if(stateEnum == StateEnum.Die)
        {
            return;
        }
        stateEnum = StateEnum.Idle;
    }
}
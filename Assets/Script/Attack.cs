//攻撃

using System.Collections;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(ObjectStatus))]

public class Attack : MonoBehaviourPunCallbacks
{
    [SerializeField] private float AttackCoolDown = 0.0f;
    [SerializeField] private float IdelCoolDown = 0.0f;
    [SerializeField] private Collider OnRideColl = null;
    [SerializeField] private Collider SkillColl = null;
    [SerializeField] private Collider RiderColl = null;
    [SerializeField] private Collider HuntColl = null;
    [SerializeField] private AudioSource AttackSound = null;
    [SerializeField] private ParticleSystem _particleSystem = null;
    [SerializeField] private FlagStatus flagStatus = null;
    private ObjectStatus objectStatus = null;
    public int flagcount = 0;
    
    private void Start()
    {
        objectStatus = GetComponent<ObjectStatus>();
    }

    public void AttackIfPossible(int signal) //攻撃可能かどうか
    {
        if(objectStatus.AttackAble == false)
        {
            return;
        }
        objectStatus.GoAttack(signal);
        
    }
    
    public void AttackRangeEnter(Collider collider) //プレイヤーが攻撃範囲に入った場合
    {
        PlayerStatus playerStatus = collider.GetComponent<PlayerStatus>();
        if(playerStatus.NowLife <= 0.0f)
        {
            return;
        }
        if(objectStatus.AttackAble == false) //連続攻撃防止
        {
            return;
        }
        StartCoroutine(StartCooldown());
    }

    private void AttackStart() //攻撃開始
    {
        OnRideColl.enabled = true;
        if(AttackSound != null)
        {
            //AttackSound.pitch = Random.Range(0.9f, 1.0f);
            AttackSound.Play();
        }
    }

    private void FinishAttack() //攻撃終了
    {
        OnRideColl.enabled = false;
        StartCoroutine(CooldownCoroutine());
    }

    private void SkillStart() //スキル開始
    {
        _particleSystem.Play();
        SkillColl.enabled = true;
        if(AttackSound != null)
        {
            AttackSound.Play();
        }
    }

    private void SkillFinish() //スキル終了
    {
        SkillColl.enabled = false;
    }

     public void RiderAttackStart() //乗り手攻撃開始
    {
        RiderColl.enabled = true;
        if(AttackSound != null)
        {
            //AttackSound.pitch = Random.Range(0.9f, 1.0f);
            AttackSound.Play();
        }
    }

    public void FinishRiderAttack() //乗り手攻撃終了
    {
        RiderColl.enabled = false;
        StartCoroutine(CooldownCoroutine());
    }

    public void HitAttack(Collider collider) //攻撃が当たった場合
    {
        if(objectStatus.CompareTag("Player"))
        {
            PlayerStatus playerStatus = objectStatus.GetComponent<PlayerStatus>();
            if(playerStatus.pun.roomjudge == 0)
            {
                if(photonView.IsMine == false) //実行中のphotonviewが自分かどうか判定,hp同期のためfalse
                {
                    ObjectStatus Target = collider.GetComponent<ObjectStatus>();
                    if(null == Target)
                    {
                        return;
                    }
                    Target.Damage(objectStatus.Attack);
                }
            }
            else if(playerStatus.pun.roomjudge == 1)
            {
                ObjectStatus Target = collider.GetComponent<ObjectStatus>();
                if(null == Target)
                {
                    return;
                }
                Target.Damage(objectStatus.Attack);
            }
        }
        else
        {
            ObjectStatus Target = collider.GetComponent<ObjectStatus>();
            if(null == Target)
            {
                return;
            }
            Target.Damage(objectStatus.Attack);
        }
    }

    public void HuntStart() //Hunt開始
    {
        HuntColl.enabled = true;
    }

    public void FinishHunt() //Hunt終了
    {
        HuntColl.enabled = false;
        StartCoroutine(CooldownCoroutine());
    }

    public void HuntAttack(Collider collider) //Hunt成功した場合
    {
        if(collider.GetComponentInParent<ObjectStatus>().CompareTag("Enemy") == true)
        {
            flagStatus.FlagInc();
            flagcount++; //トレーニングモード用変数
            Destroy(collider.gameObject);
        }
        else
        {
            if(photonView.IsMine == true)
            {
                flagStatus.photonView.RPC("FlagInc", RpcTarget.All);
            }
            FlagStatus enemyflagStatus = collider.GetComponentInChildren<FlagStatus>();
            if(enemyflagStatus.photonView.IsMine == false)
            {
                enemyflagStatus.photonView.RPC("FlagDec", RpcTarget.All);
            }
        }
    }

    private IEnumerator StartCooldown() //敵の攻撃開始前処理
    {
        objectStatus.GoIdle();
        yield return new WaitForSeconds(IdelCoolDown);
        objectStatus.GoNormal();
        AttackIfPossible(0);
    }

    private IEnumerator CooldownCoroutine() //攻撃後クールダウン
    {
        yield return new WaitForSeconds(AttackCoolDown);
        objectStatus.GoNormal();
    }
}

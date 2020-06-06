using System.Collections;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(ObjectStatus))]

public class Attack : MonoBehaviourPunCallbacks
{
    [SerializeField] private float AttackCoolDown = 0.0f;
    [SerializeField] private float IdleCoolDown = 0.0f;
    [SerializeField] private Collider OnRideColl = null;
    [SerializeField] private Collider SkillColl = null;
    [SerializeField] private Collider RiderColl = null;
    [SerializeField] private Collider HuntColl = null;
    [SerializeField] private AudioSource AttackSound = null;
    [SerializeField] private ParticleSystem _particleSystem = null;
    [SerializeField] private FlagStatus flagStatus = null;
    private ObjectStatus objectStatus = null;
    //トレーニングモード用変数
    public int flagcount = 0;
    
    private void Start()
    {
        objectStatus = GetComponent<ObjectStatus>();
    }

    public void AttackIfPossible(int signal)
    {
        if(!objectStatus.AttackAble) return;
        objectStatus.GoAttack(signal);
    }
    
    public void AttackRangeEnter(Collider collider)
    {
        PlayerStatus playerStatus = collider.GetComponent<PlayerStatus>();
        if(playerStatus.NowLife <= 0.0f) return;
        if(!objectStatus.AttackAble) return;
        StartCoroutine(StartCooldown());
    }

    public void AttackStart()
    {
        OnRideColl.enabled = true;
        if(AttackSound != null)
        {
            AttackSound.Play();
        }
    }

    public void AttackFinish()
    {
        OnRideColl.enabled = false;
        StartCoroutine(FinishCooldown());
    }

    public void SkillStart()
    {
        SkillColl.enabled = true;
        _particleSystem.Play();
        if(AttackSound != null)
        {
            AttackSound.Play();
        }
    }

    public void SkillFinish()
    {
        SkillColl.enabled = false;
    }

    public void RiderAttackStart()
    {
        RiderColl.enabled = true;
        if(AttackSound != null)
        {
            AttackSound.Play();
        }
    }

    public void RiderAttackFinish()
    {
        RiderColl.enabled = false;
        StartCoroutine(FinishCooldown());
    }

    //ダメージ処理
    public void AttackHit(Collider collider)
    {
        //プレイヤーの場合
        if(objectStatus.CompareTag("Player"))
        {
            PlayerStatus playerStatus = objectStatus.GetComponent<PlayerStatus>();
            //オンラインマッチングモードの場合
            if(playerStatus.pun.roomjudge == 0)
            {
                //ラグを考慮し適切にダメージ処理が行われるよう、他プレイヤー側に表示されている自身のオブジェクトがダメージを受けた場合に処理を行う
                //他プレイヤー側のオブジェクトか判定
                if(!photonView.IsMine)
                {
                    ObjectStatus Target = collider.GetComponent<ObjectStatus>();
                    if(Target == null) return;
                    Target.Damage(objectStatus.AttackPower);
                }
            }
            //トレーニングモードの場合
            else if(playerStatus.pun.roomjudge == 1)
            {
                ObjectStatus Target = collider.GetComponent<ObjectStatus>();
                if(Target == null) return;
                Target.Damage(objectStatus.AttackPower);
            }
        }
        //敵の場合
        else
        {
            ObjectStatus Target = collider.GetComponent<ObjectStatus>();
            if(Target == null) return;
            Target.Damage(objectStatus.AttackPower);
        }
    }

    public void HuntStart()
    {
        HuntColl.enabled = true;
    }

    public void HuntFinish()
    {
        HuntColl.enabled = false;
        StartCoroutine(FinishCooldown());
    }

    //旗を奪うことに成功した場合に呼ばれる
    public void HuntAttack(Collider collider)
    {
        //トレーニングモードの場合
        if(collider.GetComponentInParent<ObjectStatus>().CompareTag("Enemy") == true)
        {
            flagStatus.FlagInc();
            flagcount++;
            Destroy(collider.gameObject);
        }
        //オンラインマッチングモードの場合
        else
        {
            //自身の生成したオブジェクトの場合
            if(photonView.IsMine)
            {
                //自身の旗を増やす
                flagStatus.photonView.RPC("FlagInc", RpcTarget.All);
            }
            FlagStatus enemyflagStatus = collider.GetComponentInChildren<FlagStatus>();
            //他プレイヤーが生成したオブジェクトの場合
            if(!enemyflagStatus.photonView.IsMine)
            {
                //取られたプレイヤーの旗を減らす
                enemyflagStatus.photonView.RPC("FlagDec", RpcTarget.All);
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        objectStatus.GoIdle();
        yield return new WaitForSeconds(IdleCoolDown);
        objectStatus.GoNormal();
        AttackIfPossible(0);
    }

    private IEnumerator FinishCooldown()
    {
        yield return new WaitForSeconds(AttackCoolDown);
        objectStatus.GoNormal();
    }
}

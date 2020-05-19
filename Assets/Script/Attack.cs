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
    public int flagcount = 0; //トレーニングモード用変数
    
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

    private void AttackStart()
    {
        OnRideColl.enabled = true;
        if(AttackSound != null)
        {
            AttackSound.Play();
        }
    }

    private void FinishAttack()
    {
        OnRideColl.enabled = false;
        StartCoroutine(CooldownCoroutine());
    }

    private void SkillStart()
    {
        SkillColl.enabled = true;
        _particleSystem.Play();
        if(AttackSound != null)
        {
            AttackSound.Play();
        }
    }

    private void SkillFinish()
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

    public void FinishRiderAttack()
    {
        RiderColl.enabled = false;
        StartCoroutine(CooldownCoroutine());
    }

    public void HitAttack(Collider collider)
    {
        if(objectStatus.CompareTag("Player"))
        {
            PlayerStatus playerStatus = objectStatus.GetComponent<PlayerStatus>();
            if(playerStatus.pun.roomjudge == 0)
            {
                if(!photonView.IsMine) //実行中のphotonviewが自分かどうか判定,hp同期のためfalse
                {
                    ObjectStatus Target = collider.GetComponent<ObjectStatus>();
                    if(Target == null) return;
                    Target.Damage(objectStatus.Attack);
                }
            }
            else if(playerStatus.pun.roomjudge == 1)
            {
                ObjectStatus Target = collider.GetComponent<ObjectStatus>();
                if(Target == null) return;
                Target.Damage(objectStatus.Attack);
            }
        }
        else
        {
            ObjectStatus Target = collider.GetComponent<ObjectStatus>();
            if(Target == null) return;
            Target.Damage(objectStatus.Attack);
        }
    }

    public void HuntStart()
    {
        HuntColl.enabled = true;
    }

    public void FinishHunt()
    {
        HuntColl.enabled = false;
        StartCoroutine(CooldownCoroutine());
    }

    public void HuntAttack(Collider collider)
    {
        if(collider.GetComponentInParent<ObjectStatus>().CompareTag("Enemy") == true)
        {
            flagStatus.FlagInc();
            flagcount++;
            Destroy(collider.gameObject);
        }
        else
        {
            if(photonView.IsMine)
            {
                flagStatus.photonView.RPC("FlagInc", RpcTarget.All);
            }
            FlagStatus enemyflagStatus = collider.GetComponentInChildren<FlagStatus>();
            if(!enemyflagStatus.photonView.IsMine)
            {
                enemyflagStatus.photonView.RPC("FlagDec", RpcTarget.All);
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        objectStatus.GoIdle();
        yield return new WaitForSeconds(IdelCoolDown);
        objectStatus.GoNormal();
        AttackIfPossible(0);
    }

    private IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(AttackCoolDown);
        objectStatus.GoNormal();
    }
}

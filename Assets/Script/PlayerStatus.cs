//ステータス

using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatus : ObjectStatus, IPunObservable
{
    [SerializeField] private Renderer drenderer = null;
    [SerializeField] private Renderer rrenderer = null;
    [SerializeField] private ParticleSystem _particleSystem = null;
    [SerializeField] protected Animator rideranimator = null;
    public bool LiveState = true;
    public bool Hit = false;
    public bool SkillState = false;
    public Pun pun = null;
    public float SkillCoolDown = 0.0f;

    protected override void Start()
    {
        base.Start();
        pun = GameObject.Find("Pun").GetComponent<Pun>();
        LifeGaugeAdd();
    }

    public void LifeGaugeAdd()
    {
        LifeGaugeContainer.Instance.Add(this); //インスタンスを代入
    }

    public override void Damage(float damage)
    {
        if(stateEnum == StateEnum.Die)
        {
            return;
        }
        if(Hit == true)
        {
            return;
        }
        if(pun.roomjudge == 0)
        {
            NowLife -= damage;
        }
        else if(pun.roomjudge == 1) //トレーニングの場合無敵状態
        {
            //NowLife -= damage;
        }
        if(photonView.IsMine == true)
        {
            photonView.RPC("DamageRPC", RpcTarget.All);
        }
        if(NowLife > 0.0f)
        {
            return;
        }
        LiveState = false;
        stateEnum = StateEnum.Die;
        animator.SetTrigger("Die");
        rideranimator.SetTrigger("Die");
        Die();
    }

    [PunRPC] private void DamageRPC()
    {
        StartCoroutine(DamageHit());
    }

    protected override void Die()
    {
        base.Die();
        StartCoroutine(Stan());
    }

    private IEnumerator DamageHit() //無敵状態
    {
        if(NowLife <= 0.0f)
        {
            yield break;
        }
        Hit = true;
        for(int i = 0; i < 5; i++)
        {
            drenderer.material.color = new Color(1.0f, 0.5f, 0.5f);
            rrenderer.material.color = new Color(1.0f, 0.75f, 0.25f);
            yield return new WaitForSeconds(0.1f);
            drenderer.material.color = new Color(1.0f, 0.0f, 0.0f);
            rrenderer.material.color = new Color(1.0f, 0.5f, 0.0f);
            yield return new WaitForSeconds(0.1f);
        }
        Hit = false;
        stateEnum = StateEnum.Normal;
    }

    private IEnumerator Stan() //スタン状態
    {
        //PlayerPrefs.DeleteAll();
        yield return new WaitForSeconds(3.0f);
        if(pun.roomjudge == 0)
        {
            stateEnum = StateEnum.Normal;
            LiveState = true;
            NowLife = MaxLife;
        }
        else if(pun.roomjudge == 1)
        {
            /*stateEnum = StateEnum.Normal;
            LiveState = true;
            NowLife = MaxLife;
            LifeGaugeContainer.Instance.Add(this);*/
        }
        //SceneManager.LoadScene("GameOverScene");
    }

    private void OnTriggerEnter(Collider coll)
    {
        if(coll.CompareTag("Item") == true)
        {
            _particleSystem.Play();
        }
    }

    public override void GoAttack(int signal)
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
            case 1:
                stateEnum = StateEnum.Attack;
                rideranimator.SetTrigger("Attack");
                break;
            case 2:
                stateEnum = StateEnum.Attack;
                rideranimator.SetTrigger("Hunt");
                break;
            case 3:
                if(SkillState == true)
                {
                    return;
                }
                SkillState = true;
                StartCoroutine(SkillStatus());
                stateEnum = StateEnum.Attack;
                animator.SetTrigger("Skill");
                break;
            default:
                break;
        }
    }

    private IEnumerator SkillStatus()
    {
        yield return new WaitForSeconds(1.5f);
        GoNormal();
        yield return new WaitForSeconds(SkillCoolDown);
        SkillState = false;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(NowLife);
        }
        else
        {
            NowLife = (float)stream.ReceiveNext();
        }
    }

    public void JumpAction()
    {
        if(LiveState == true)
        {
            photonView.RPC("Jump", RpcTarget.All);
        }
    }

    public void WallDamege(Collider collider)
    {
        if(collider.CompareTag("Wall"))
        {
            if(photonView.IsMine == true)
            {
                Damage(3.0f);
            }
        }
    }
}

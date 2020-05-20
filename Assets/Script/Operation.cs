using UnityEngine;
using UnityEngine.UI;

public class Operation : MonoBehaviour
{
    [SerializeField] private Button DoragonAttackButton = null;
    [SerializeField] private Button RiderAttackButton = null;
    [SerializeField] private Button RiderHuntButton = null;
    [SerializeField] private Button SkillButton = null;
    [SerializeField] private Button JumpButton = null;
    public Attack attack = null;
    public PlayerStatus playerStatus = null;
    
    private void Start()
    {
        DoragonAttackButton.onClick.AddListener(() => Action(0));
        RiderAttackButton.onClick.AddListener(() => Action(1));
        RiderHuntButton.onClick.AddListener(() => Action(2));
        SkillButton.onClick.AddListener(() => Action(3));
        JumpButton.onClick.AddListener(Jump);
    }

    private void Update()
    {
        if(playerStatus == null) return;
        if(playerStatus.SkillState) //スキルのクールダウン処理
        {
            SkillButton.interactable = false;
        }
        else
        {
            SkillButton.interactable = true;
        }
    }

    private void Action(int signal)
    {
        attack.AttackIfPossible(signal);
    }

    private void Jump()
    {
        playerStatus.JumpAction();
    }
}

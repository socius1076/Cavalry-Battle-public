using UnityEngine;

public class RiderAttack : MonoBehaviour //Animatorイベント呼び出し用
{
    private Attack attack = null;

    private void Start()
    {
        attack = GetComponentInParent<Attack>();
    }

    private void RiderAttackStart()
    {
        attack.RiderAttackStart();
    }

    private void FinishRiderAttack()
    {
        attack.FinishRiderAttack();
    }

    private void HuntStart()
    {
        attack.HuntStart();
    }

    private void FinishHunt()
    {
        attack.FinishHunt();
    }
}

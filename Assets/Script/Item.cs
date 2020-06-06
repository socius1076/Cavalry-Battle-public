using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]

public class Item : MonoBehaviour
{
    private enum ItemEnum
    {
        //未実装
        LifeUP,
        PowerUP,
        //未実装
        SpeedUP
    }
    [SerializeField] private ItemEnum type = ItemEnum.LifeUP;
    
    public void Initialize()
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;
        Transform _transform = transform;
        Vector3 DefaultScale = _transform.localScale;
        _transform.localScale = Vector3.zero;
        _transform.DOScale(DefaultScale, 0.5f);
        _transform.DOJump(transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)), 3.0f, 1, 0.5f)
            .OnComplete(() => collider.enabled = true);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(!collider.CompareTag("Player")) return;
        PlayerStatus playerStatus =  collider.GetComponent<PlayerStatus>();
        OnRideController onRideController = collider.GetComponent<OnRideController>();
        switch(type)
        {
            case ItemEnum.LifeUP:
                playerStatus.NowLife += 1.0f;
                break;
            case ItemEnum.PowerUP:
                playerStatus.AttackPower += 1.0f;
                break;
            case ItemEnum.SpeedUP:
                onRideController.Velocity += 10.0f;
                break;
            default:
                break;
        }
        Destroy(gameObject);
    }
}

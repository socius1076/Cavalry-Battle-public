//アイテム処理

using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Collider))]

public class Item : MonoBehaviour
{
    private enum ItemEnum //アイテムの種類列挙型
    {
        LifeUP,
        PowerUP,
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
        _transform.DOScale(DefaultScale, 0.5f); //0.5f時間でDefaultScaleにする
        _transform.DOJump(transform.position + new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)), 3.0f, 1, 0.5f). //ジャンプ
        //collider.enabled = true;
            //SetEase(Ease.OutBounce). //バウンスさせる
                OnComplete(() => CollEnable(collider)); //アニメーション終了後実行
    }

    private void CollEnable(Collider collider)
    {
        collider.enabled = true;
    }

    private void OnTriggerEnter(Collider collider) //Colliderが侵入したら
    {
        if(collider.CompareTag("Player") == false)
        {
            return;
        }
        PlayerStatus playerStatus =  collider.GetComponent<PlayerStatus>();
        OnRideController onRideController = collider.GetComponent<OnRideController>();
        switch(type)
        {
            case ItemEnum.LifeUP:
                playerStatus.NowLife += 1.0f;
                break;
            case ItemEnum.PowerUP:
                playerStatus.Attack += 1.0f;
                break;
            case ItemEnum.SpeedUP:
                onRideController.Velocity += 10.0f;
                break;
            default:
                break;
        }
        /*ItemData.Instance.Add(type); //アイテム追加
        ItemData.Instance.Save(); //セーブ
        foreach(ItemData.OwnedItem item in ItemData.Instance.OwnedItems) //所持アイテムログ
        {
            Debug.Log(item.Type + "を" + item.Number + "個所持");
        }*/
        Destroy(gameObject);
    }
}

using UnityEngine;
using DG.Tweening;
using Photon.Pun;

[RequireComponent(typeof(Collider))]

public class FlagDrop : MonoBehaviourPunCallbacks
{
    private Vector3 DropPosition = Vector3.zero;

    //旗を落とす
    public void Initialize(Transform playerPos)
    {
        //位置を保存
        Transform _transform = GetComponent<Transform>();
        //大きさを保存
        Vector3 DefaultScale = _transform.localScale;
        //大きさを0にする
        _transform.localScale = Vector3.zero;
        //落とす場所決め
        while(true)
        {
            Vector3 Distance = new Vector3(3.0f, 0.0f, 0.0f);
            Vector3 AnglePosition = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f) * Distance;
            //プレイヤー(y軸)を中心にx-z平面でDistanceだけ離れたランダムな位置
            DropPosition = playerPos.position + AnglePosition;
            Ray ray = new Ray(DropPosition, new Vector3(0.0f, -1.0f, 0.0f));
            //Rayを飛ばして落とせる場所か判定
            if(Physics.Raycast(ray, out RaycastHit hit, 5.0f))
            {
                //落とせる場所(床)の場合
                if(hit.collider.tag == "Floor") break;
            }
        }
        //0.5秒かけて元の大きさに戻る
        _transform.DOScale(DefaultScale, 0.5f);
        //DropPositionに3の力で1回0.5秒かけてジャンプする
        _transform.DOJump(DropPosition, 3.0f, 1, 0.5f).
            //ジャンプ処理が終了したら呼び出される
            OnComplete(() => photonView.RPC("CollEnable", RpcTarget.All));
    }

    //旗を落とす途中で拾われないように最初は当たり判定が無効化されているためRPCで当たり判定を有効化する
    [PunRPC] private void CollEnable()
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(!collider.CompareTag("Player")) return;
        FlagStatus flagStatus = collider.GetComponentInChildren<FlagStatus>();
        if(flagStatus.photonView.IsMine)
        {
            flagStatus.photonView.RPC("FlagInc", RpcTarget.All);
            photonView.RPC("DestroyRPC", RpcTarget.All);
        }
    }

    [PunRPC] private void DestroyRPC()
    {
        Destroy(gameObject);
    }
}

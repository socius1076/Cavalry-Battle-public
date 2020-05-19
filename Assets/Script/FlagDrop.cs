using UnityEngine;
using DG.Tweening;
using Photon.Pun;

[RequireComponent(typeof(Collider))]

public class FlagDrop : MonoBehaviourPunCallbacks
{
    private Vector3 DropPosition = Vector3.zero;

    public void Initialize(Transform playerPos)
    {
        Transform _transform = GetComponent<Transform>();
        Vector3 DefaultScale = _transform.localScale;
        _transform.localScale = Vector3.zero;
        while(true) //落とす場所決め
        {
            Vector3 Distance = new Vector3(3.0f, 0.0f, 0.0f);
            Vector3 AnglePosition = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f) * Distance;
            DropPosition = playerPos.position + AnglePosition;
            Ray ray = new Ray(DropPosition, new Vector3(0.0f, -1.0f, 0.0f));
            if(Physics.Raycast(ray, out RaycastHit hit, 5.0f))
            {
                if(hit.collider.tag == "Floor") break;
            }
        }
        _transform.DOScale(DefaultScale, 0.5f);
        _transform.DOJump(DropPosition, 3.0f, 1, 0.5f).
            OnComplete(() => photonView.RPC("CollEnable", RpcTarget.All));
    }

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

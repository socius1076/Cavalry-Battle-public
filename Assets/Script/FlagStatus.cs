using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Collider))]

public class FlagStatus : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject flag1 = null;
    [SerializeField] private GameObject flag2 = null;
    [SerializeField] private GameObject flag3 = null;
    [SerializeField] private GameObject flag4 = null;
    [SerializeField] private GameObject flag5 = null;
    private BoxCollider FlagBox = null; 
    public int number = 0;
    private Vector3 BoxSize = Vector3.zero;
    private PlayerStatus playerStatus = null;
    private bool dropjudge = false;
    private Transform playerPos = null;

    private void Start()
    {
        number = 1;
        FlagBox = GetComponent<BoxCollider>();
        playerStatus = GetComponentInParent<PlayerStatus>();
        playerPos = playerStatus.GetComponent<Transform>();
        BoxSize = FlagBox.size;
        flag2.SetActive(false);
        flag3.SetActive(false);
        flag4.SetActive(false);
        flag5.SetActive(false);
    }

    private void Update()
    {
        if(playerStatus.NowLife <= 0.0f)
        {
            Drop();
        }
        else
        {
            dropjudge = false;
        }
    }

    private void Drop()
    {
        if(dropjudge) return;
        dropjudge = true;
        if(photonView.IsMine)
        {
            int numcache = number;
            for(int i = 0; i < numcache; i++)
            {
                FlagDrop flag = PhotonNetwork.Instantiate("Flag", playerPos.position, Quaternion.identity).GetComponent<FlagDrop>();
                flag.Initialize(playerPos);
                photonView.RPC("FlagDec", RpcTarget.All);
            }
        }
    }

    [PunRPC] public void FlagInc()
    {
        switch(number)
        {
            case 0:
                FlagBox.enabled = true;
                BoxSize.x = 0.02f;
                FlagBox.size = BoxSize;
                flag1.SetActive(true);
                flag2.SetActive(false);
                flag3.SetActive(false);
                flag4.SetActive(false);
                flag5.SetActive(false);
                number = 1;
                break;
            case 1:
                BoxSize.x = 0.025f;
                FlagBox.size = BoxSize;
                flag1.SetActive(false);
                flag2.SetActive(false);
                flag3.SetActive(false);
                flag4.SetActive(true);
                flag5.SetActive(true);
                number = 2;
                break;
            case 2:
                BoxSize.x = 0.03f;
                FlagBox.size = BoxSize;
                flag1.SetActive(true);
                flag2.SetActive(true);
                flag3.SetActive(true);
                flag4.SetActive(false);
                flag5.SetActive(false);
                number = 3;
                break;
            default:
                break;
        }
    }

    [PunRPC] public void FlagDec()
    {
        switch(number)
        {
            case 1:
                FlagBox.enabled = false;
                flag1.SetActive(false);
                flag2.SetActive(false);
                flag3.SetActive(false);
                flag4.SetActive(false);
                flag5.SetActive(false);
                number = 0;
                break;
            case 2:
                BoxSize.x = 0.02f;
                FlagBox.size = BoxSize;
                flag1.SetActive(true);
                flag2.SetActive(false);
                flag3.SetActive(false);
                flag4.SetActive(false);
                flag5.SetActive(false);
                number = 1;
                break;
            case 3:
                BoxSize.x = 0.025f;
                FlagBox.size = BoxSize;
                flag1.SetActive(false);
                flag2.SetActive(false);
                flag3.SetActive(false);
                flag4.SetActive(true);
                flag5.SetActive(true);
                number = 2;
                break;
            default:
                break;
        }
    }
}

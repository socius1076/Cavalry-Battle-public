using UnityEngine;
using System.Collections;
using Photon.Pun;

[RequireComponent(typeof(Camera))]

public class MainCamera : MonoBehaviour
{
    public GameObject target = null;
    private Vector3 TargetPos = Vector3.zero;
    private Pun pun = null;
    public bool CameraOk = false;
    //private float Vertical = 0;

    private void Start()
    {
        pun = GameObject.Find("Pun").GetComponent<Pun>();
        StartCoroutine(WaitTime());
    }

    private IEnumerator WaitTime()
    {
        while(!PhotonNetwork.InRoom)
        {
            yield return null;
        }
        TargetPos = target.transform.position;
        switch(pun.EntryNumber) //見る位置を決める
        {
            case 1:
                transform.Rotate(new Vector3(30.0f, -90.0f, 0.0f));
                transform.position = TargetPos + new Vector3(5.0f, 4.0f, 0.0f);
                break;
            case 2:
                transform.Rotate(new Vector3(30.0f, 90.0f, 0.0f));
                transform.position = TargetPos + new Vector3(-5.0f, 4.0f, 0.0f);
                break;
            default:
                break;
        }
        if(pun.roomjudge == 1)
        {
            TrainingMenu trainingMenu = GameObject.Find("MenuCanvas").GetComponent<TrainingMenu>();
            trainingMenu.LoadOk();
        }
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["Ready"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        CameraOk = true;
    }

    private void Update()
    {
        if(CameraOk == false || target == null) return;
        transform.position += target.transform.position - TargetPos; //ターゲットが動いていたら合わせる
        TargetPos = target.transform.position; //ターゲットの位置更新
        if(Input.touchCount > 0) //スマートフォンの場合
        {
            Touch touch = Input.GetTouch(0);
            if(Input.touchCount > 1) //最初に左画面をタップした場合
            {
                Touch touch2 = Input.GetTouch(1);
                if(touch2.position.x >= Screen.width / 2)
                {
                    touch = touch2;
                }
            }
            if((touch.phase == TouchPhase.Moved) && (touch.position.x >= Screen.width / 2)) //右画面をタップした場合
            {
                float InputX = touch.deltaPosition.x;
                /*float InputY = touch.deltaPosition.y; //垂直移動
                Vertical -= InputY;
                Vertical = Mathf.Clamp(Vertical, 20.0f, 60.0f);
                if(Time.timeScale == 1.0f)
                {
                    transform.eulerAngles = new Vector3(Vertical, transform.eulerAngles.y, 0.0f);
                }*/
                transform.RotateAround(TargetPos, Vector3.up, InputX * Time.deltaTime * 30.0f); //ターゲットを中心に回転
            }
        }
        else if(Input.GetMouseButton(0)) //PCの場合
        {
            if(Input.mousePosition.x >= Screen.width / 2)
            {
                float InputX = Input.GetAxis("Mouse X");
                transform.RotateAround(TargetPos, Vector3.up, InputX * Time.deltaTime * 400.0f);
            }
        }
    }
}
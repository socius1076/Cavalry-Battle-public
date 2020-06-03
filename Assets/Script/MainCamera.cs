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

    //カメラ準備処理
    private IEnumerator WaitTime()
    {
        //プレイヤーが部屋に入るまで待機
        while(!PhotonNetwork.InRoom) yield return null;
        TargetPos = target.transform.position;
        //プレイヤー同士が向き合うようにカメラが見る位置を決める
        switch(pun.EntryNumber)
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
        //トレーニングモードの場合
        if(pun.roomjudge == 1)
        {
            TrainingMenu trainingMenu = GameObject.Find("MenuCanvas").GetComponent<TrainingMenu>();
            trainingMenu.LoadOk();
        }
        //カスタムプロパティを使用してプレイヤーが"Ready"(準備完了)の合図を出す
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["Ready"] = true;
        //カスタムプロパティの値を同期
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        //Update関数用のフラグ
        CameraOk = true;
    }

    private void Update()
    {
        if(CameraOk == false || target == null) return;
        //ターゲットが動いていたら合わせる
        transform.position += target.transform.position - TargetPos;
        //ターゲットの位置更新
        TargetPos = target.transform.position;
        //スマートフォンの場合
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            //最初に左画面をタップした場合
            if(Input.touchCount > 1)
            {
                Touch touch2 = Input.GetTouch(1);
                if(touch2.position.x >= Screen.width / 2)
                {
                    touch = touch2;
                }
            }
            //右画面をタップした場合
            if((touch.phase == TouchPhase.Moved) && (touch.position.x >= Screen.width / 2))
            {
                float InputX = touch.deltaPosition.x;
                //垂直移動
                /*float InputY = touch.deltaPosition.y;
                Vertical -= InputY;
                Vertical = Mathf.Clamp(Vertical, 20.0f, 60.0f);
                if(Time.timeScale == 1.0f)
                {
                    transform.eulerAngles = new Vector3(Vertical, transform.eulerAngles.y, 0.0f);
                }*/
                //ターゲットを中心に回転
                transform.RotateAround(TargetPos, Vector3.up, InputX * Time.deltaTime * 30.0f);
            }
        }
        //PCの場合
        else if(Input.GetMouseButton(0))
        {
            if(Input.mousePosition.x >= Screen.width / 2)
            {
                float InputX = Input.GetAxis("Mouse X");
                transform.RotateAround(TargetPos, Vector3.up, InputX * Time.deltaTime * 400.0f);
            }
        }
    }
}
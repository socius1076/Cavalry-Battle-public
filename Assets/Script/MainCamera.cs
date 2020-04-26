//カメラ制御

using UnityEngine;
using System.Collections;
using Photon.Pun;

[RequireComponent(typeof(Camera))]

public class MainCamera : MonoBehaviour
{
    public GameObject target = null;
    private Vector3 TargetPos = Vector3.zero;
    //private Vector3 Offset = new Vector3(5.0f, 4.0f, 0.0f);
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
        while(PhotonNetwork.InRoom == false)
        {
            yield return null;
        }
        TargetPos = target.transform.position;
        if(pun.EntryNumber % 2 == 0) //見る位置を決める
        {
            transform.Rotate(new Vector3(30.0f, 90.0f, 0.0f));
            transform.position = TargetPos + new Vector3(-5.0f, 4.0f, 0.0f);
        }
        else
        {
            transform.Rotate(new Vector3(30.0f, -90.0f, 0.0f));
            transform.position = TargetPos + new Vector3(5.0f, 4.0f, 0.0f);
        }
        if(pun.roomjudge == 0)
        {

        }
        else if(pun.roomjudge == 1)
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
        if(CameraOk == false || target == null)
        {
            return;
        }
        transform.position += target.transform.position - TargetPos; //ターゲットが動いていたらそれに合わせる
        TargetPos = target.transform.position; //ターゲットの位置更新
        if(Input.touchCount > 0)
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
                float InputY = touch.deltaPosition.y;
                /*Vertical -= InputY;
                Vertical = Mathf.Clamp(Vertical, 20.0f, 60.0f);
                if(Time.timeScale == 1.0f)
                {
                    transform.eulerAngles = new Vector3(Vertical, transform.eulerAngles.y, 0.0f);
                }*/
                transform.RotateAround(TargetPos, Vector3.up, InputX * Time.deltaTime * 30.0f); //ターゲットを中心に回転
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class Pun : MonoBehaviourPunCallbacks
{
    public bool namecheck = false;
    public int maxmember = 0;
    public int nowmember = 0;
    //部屋判断用変数
    public int roomjudge = 0;
    public bool maxroom = false;
    public PlayerStatus playerStatus = null;
    public int EntryNumber = 0;
    public bool Ready = false;
    public int StartTime = 0;
    public string Judge = "";

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator Start()
    {
        //名前が決定するまで待機
        while(!namecheck) yield return new WaitForSeconds(1.0f);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
    }

    //カスタムプロパティ変更時呼ばれるコールバック
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //最初に部屋を作ったプレイヤーがマスターとなり、カスタムプロパティの更新権限を与える
        //マスターの場合
        if(PhotonNetwork.IsMasterClient)
        {
            //試合が開始している場合
            if(Ready)
            {
                //"Start"(試合開始時間)が更新された場合
                if(changedProps.TryGetValue("Start", out object StartObject))
                {
                    //試合開始時間設定
                    StartTime = (int)StartObject;
                }
                //"Flag"(旗の数)が更新された場合
                if(changedProps.TryGetValue("Flag", out object FlagObject))
                {
                    //連続で呼ばれないようにする
                    if(!string.IsNullOrEmpty(Judge)) return;
                    //メンバー数確認用変数
                    int membercount = 0;
                    //部屋のメンバーの数だけ繰り返す
                    foreach(var Member in PhotonNetwork.PlayerList)
                    {
                        //"Flag"(旗の数)が設定されている場合
                        if(Member.CustomProperties["Flag"] != null)
                        {
                            membercount++;
                        }
                    }
                    //全員の処理を受け取ったら
                    if(membercount == maxmember)
                    {
                        //一番多く旗を集めたプレイヤーの旗の数
                        int max = 0;
                        //一番多く旗を集めたプレイヤーの名前
                        string name = "";
                        //引き分け用フラグ
                        bool draw = false;
                        //部屋のメンバーの数だけ繰り返す 
                        foreach(var Member in PhotonNetwork.PlayerList)
                        {
                            //maxより多い旗を集めている場合
                            if((int)Member.CustomProperties["Flag"] >= max)
                            {
                                max = (int)Member.CustomProperties["Flag"];
                                name = Member.NickName;
                            }
                        }
                        //部屋のメンバーの数だけ繰り返す
                        foreach(var Member in PhotonNetwork.PlayerList)
                        {
                            //一番多く旗を集めたプレイヤーと同じ数だけ旗を持っているプレイヤーがいる場合
                            if((int)Member.CustomProperties["Flag"] == max && !Member.NickName.Equals(name))
                            {
                                //引き分け
                                draw = true;
                            }
                        }
                        //引き分けの場合
                        if(draw)
                        {
                            //"Win"(結果)にDrawを設定
                            var hashtable = new ExitGames.Client.Photon.Hashtable();
                            hashtable["Win"] = "Draw";
                            //カスタムプロパティの値を同期
                            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                        }
                        else
                        {
                            //"Win"(結果)に勝者の名前を設定
                            var hashtable = new ExitGames.Client.Photon.Hashtable();
                            hashtable["Win"] = name;
                            //カスタムプロパティの値を同期
                            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                        }
                    }
                }
                //"Win"(結果)が更新された場合
                if(changedProps.TryGetValue("Win", out object WinObject))
                {
                    //結果を設定
                    Judge = (string)WinObject;
                }
            }
            //試合がまだ始まっていない場合
            else
            {
                //"Go"(全員の準備完了)が更新された場合
                if(changedProps.TryGetValue("Go", out object GoObject))
                {
                    //試合開始フラグ
                    Ready = (bool)GoObject;
                }
                if(Ready) return;
                //メンバー数確認用変数
                int membercount = 0;
                //部屋のメンバーの数だけ繰り返す
                foreach(var Member in PhotonNetwork.PlayerList)
                {
                    //"Ready"(準備完了)が設定されている場合
                    if(Member.CustomProperties["Ready"] != null)
                    {
                        membercount++;
                    }
                }
                //全員の処理を受け取ったら
                if(membercount == maxmember)
                {
                    //"Go"(全員の準備完了)設定
                    var hashtable = new ExitGames.Client.Photon.Hashtable();
                    hashtable["Go"] = true;
                    //カスタムプロパティの値を同期
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                }
            }
        }
        //マスターではない場合
        else
        {
            //"Go"(全員の準備完了)が更新された場合
            if(changedProps.TryGetValue("Go", out object GoObject))
            {
                //試合開始フラグ
                Ready = (bool)GoObject;
            }
            //"Start"(試合開始時間)が更新された場合
            if(changedProps.TryGetValue("Start", out object StartObject))
            {
                //試合開始フラグ
                StartTime = (int)StartObject;
            }
            //"Win"(結果)が更新された場合
            if(changedProps.TryGetValue("Win", out object WinObject))
            {
                //結果を設定
                Judge = (string)WinObject;
            }
        }
    }

    public void SetName(string name)
    {
        PhotonNetwork.LocalPlayer.NickName = name;
    }

    public void RoomEnter()
    {
        roomjudge = 0;
        PhotonNetwork.JoinRoom("Room1");
    }

    public void TrainingEnter()
    {
        roomjudge = 1;
        PhotonNetwork.JoinRoom("Training");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        switch(roomjudge)
        {
            case 0:
                RoomOptions roomOptions = new RoomOptions
                {
                    MaxPlayers = (byte)maxmember
                };
                PhotonNetwork.CreateRoom("Room1", roomOptions);
                break;
            case 1:
                string roomname = PhotonNetwork.LocalPlayer.NickName + "TrainingRoom";
                RoomOptions roomOptions1 = new RoomOptions
                {
                    MaxPlayers = 1
                };
                PhotonNetwork.CreateRoom(roomname, roomOptions1);
                break;
            default:
                break;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(var info in roomList)
        {
            if(info.Name == "Room1")
            {
                nowmember = info.PlayerCount;
                if(nowmember == maxmember)
                {
                    maxroom = true;
                }
                else
                {
                    maxroom = false;
                }
            }
        }
    }

    private void OnGUI()
    {
        GUI.contentColor = Color.black;
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnJoinedRoom()
    {
        EntryNumber = PhotonNetwork.CurrentRoom.PlayerCount;
        GameObject player = null;
        if(roomjudge == 0)
        {
            Vector3 Position = StartPosition();
            Quaternion Direction = StartQuaternion();
            player = PhotonNetwork.Instantiate("Player", Position, Direction);
        }
        else if(roomjudge == 1)
        {
            player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.Euler(0.0f, -90.0f, 0.0f));
        }
        Attack playerattack = player.GetComponent<Attack>();
        OnRideController onRideController = player.GetComponent<OnRideController>();
        playerStatus = player.GetComponent<PlayerStatus>();
        MainCamera mainCamera = Camera.main.GetComponent<MainCamera>();
        mainCamera.target = player;
        Operation operation = GameObject.Find("OperationPanel").GetComponent<Operation>();
        operation.attack = playerattack;
        operation.playerStatus = playerStatus;
        FlagCount flagCount = GameObject.Find("FlagText").GetComponent<FlagCount>();
        flagCount.attack = playerattack;
        flagCount.flagStatus = player.GetComponentInChildren<FlagStatus>();
        MyFloatingJoystick myFloatingJoystick = GameObject.Find("Floating Joystick").GetComponent<MyFloatingJoystick>();
        onRideController.joystick = myFloatingJoystick;
        playerStatus.pun = this;
        if(roomjudge == 1)
        {
            EnemyAppear appear = GameObject.Find("AppearGate").GetComponent<EnemyAppear>();
            appear.playerStatus = playerStatus;
        }
    }

    private Vector3 StartPosition()
    {
        Vector3 Position = Vector3.zero;
        switch(PhotonNetwork.CurrentRoom.PlayerCount)
        {
            case 1:
                Position = new Vector3(12.0f, 0.0f, 0.0f);
                break;
            case 2:
                Position = new Vector3(-12.0f, 0.0f, 0.0f);
                break;
            default:
                break;
        }
        return Position;
    }

    private Quaternion StartQuaternion()
    {
        Quaternion quaternion = Quaternion.identity;
        switch(PhotonNetwork.CurrentRoom.PlayerCount)
        {
            case 1:
                quaternion = Quaternion.Euler(0.0f, -90.0f, 0.0f);
                break;
            case 2:
                quaternion = Quaternion.Euler(0.0f, 90.0f, 0.0f);
                break;
            default:
                break;
        }
        return quaternion;
    }

    public override void OnLeftRoom()
    {
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["Ready"] = null;
        hashtable["Go"] = null;
        hashtable["Start"] = null;
        hashtable["Flag"] = null;
        hashtable["Win"] = null;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        Ready = false;
        Judge = "";
    }
}

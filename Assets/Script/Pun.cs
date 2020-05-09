//ネットワーク管理

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
    public int roomjudge = 0; //部屋判断用変数
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
        while(namecheck == false) //名前が決定するまで待機
        {
            yield return new WaitForSeconds(1.0f);
        }
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 10;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if(PhotonNetwork.IsMasterClient == true)
        {
            if(Ready == true) //試合が開始している場合
            {
                if(changedProps.TryGetValue("Start", out object StartObject) == true)
                {
                    StartTime = (int)StartObject;
                }
                if(changedProps.TryGetValue("Flag", out object FlagObject) == true)
                {
                    if(string.IsNullOrEmpty(Judge) == false) //連続で呼ばれないようにする
                    {
                        return;
                    }
                    int judge = 0;
                    foreach(var Member in PhotonNetwork.PlayerList)
                    {
                        if(Member.CustomProperties["Flag"] != null)
                        {
                            judge++;
                        }
                    }
                    if(judge == maxmember) //全員の処理を受け取ったら
                    {
                        int max = 0;
                        string name = "";
                        bool draw = false; 
                        foreach(var Member in PhotonNetwork.PlayerList)
                        {
                            if((int)Member.CustomProperties["Flag"] >= max)
                            {
                                max = (int)Member.CustomProperties["Flag"];
                                name = Member.NickName;
                            }
                        }
                        foreach(var Member in PhotonNetwork.PlayerList)
                        {
                            if((int)Member.CustomProperties["Flag"] == max && Member.NickName.Equals(name) == false)
                            {
                                draw = true;
                            }
                        }
                        if(draw == true) //引き分けかそうでないか
                        {
                            var hashtable = new ExitGames.Client.Photon.Hashtable();
                            hashtable["Win"] = "Draw";
                            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                        }
                        else
                        {
                            var hashtable = new ExitGames.Client.Photon.Hashtable();
                            hashtable["Win"] = name;
                            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                        }
                    }
                }
                if(changedProps.TryGetValue("Win", out object WinObject) == true)
                {
                    Judge = (string)WinObject;
                }
            }
            else
            {
                if(changedProps.TryGetValue("Go", out object GoObject) == true)
                {
                    Ready = (bool)GoObject;
                }
                if(Ready == true)
                {
                    return;
                }
                int judge = 0;
                foreach(var Member in PhotonNetwork.PlayerList)
                {
                    if(Member.CustomProperties["Ready"] != null)
                    {
                        judge++;
                    }
                }
                if(judge == maxmember)
                {
                    var hashtable = new ExitGames.Client.Photon.Hashtable();
                    hashtable["Go"] = true;
                    PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
                }
            }
        }
        else
        {
            if(changedProps.TryGetValue("Go", out object GoObject) == true)
            {
                Ready = (bool)GoObject;
            }
            if(changedProps.TryGetValue("Start", out object StartObject) == true)
            {
                StartTime = (int)StartObject;
            }
            if(changedProps.TryGetValue("Win", out object WinObject) == true)
            {
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
            player = PhotonNetwork.Instantiate("Doragon", Position, Direction);
        }
        else if(roomjudge == 1)
        {
            player = PhotonNetwork.Instantiate("Doragon", Vector3.zero, Quaternion.Euler(0.0f, -90.0f, 0.0f));
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
        if(roomjudge == 0)
        {
            
        }
        else if(roomjudge == 1)
        {
            Appear appear = GameObject.Find("AppearGate").GetComponent<Appear>();
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
        hashtable["Go"] = null;
        hashtable["Ready"] = null;
        Ready = false;
        Judge = "";
    }
}

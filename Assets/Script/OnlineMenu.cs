using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class OnlineMenu : MonoBehaviour
{
    [SerializeField] private Button MenuButton = null;
    [SerializeField] private GameObject MenuPanel = null;
    [SerializeField] private Button ExitButton = null;
    [SerializeField] private GameObject ExitPanel = null;
    [SerializeField] private Button YesButton = null;
    [SerializeField] private Button NoButton = null;
    [SerializeField] private MainCamera mainCamera = null;
    [SerializeField] private GameObject MatchingPanel = null;
    [SerializeField] private GameObject CountdownPanel = null;
    [SerializeField] private Text CountdownText = null;
    [SerializeField] private Text TimeText = null;
    [SerializeField] private GameObject FinishPanel = null;
    [SerializeField] private GameObject ResultPanel = null;
    [SerializeField] private Text ResultText = null;
    private bool ActivePanel = false;
    private Pun pun = null;
    private bool StartGame = false;
    public int MaxTime = 0;
    private bool ReadyGame = false;
    public int ReadyTime = 0;
    public bool Finish = false;
    public int Margin = 0;

    private void Start()
    {
        pun = GameObject.Find("Pun").GetComponent<Pun>();
        MatchingPanel.SetActive(true);
        MenuPanel.SetActive(false);
        ExitPanel.SetActive(false);
        FinishPanel.SetActive(false);
        ResultPanel.SetActive(false);
        MenuButton.onClick.AddListener(MenuScreen);
        ExitButton.onClick.AddListener(Exit);
        YesButton.onClick.AddListener(Yes);
        NoButton.onClick.AddListener(No);
        StartCoroutine(MatchingWait());
    }

    private IEnumerator MatchingWait()
    {
        while(!pun.Ready) yield return null;
        MenuButton.interactable = false;
        MatchingPanel.SetActive(false);
        CountdownPanel.SetActive(true);
        if(PhotonNetwork.IsMasterClient) //試合開始時間を記録
        {
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable["Start"] = PhotonNetwork.ServerTimestamp;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
        yield return new WaitForSeconds(Margin);
        ReadyGame = true;
    }

    private void Update()
    {
        if(ReadyGame == false || Finish == true) return;
        if(StartGame == false) //試合開始前カウントダウン
        {
            int DiffTime = (PhotonNetwork.ServerTimestamp - pun.StartTime) / 1000; //サーバの時間から試合時間を同期する
            int NowTime = (ReadyTime + Margin) - DiffTime;
            if(NowTime > 3 || NowTime < 0) return;
            CountdownText.text = NowTime.ToString();
            if(NowTime == 0)
            {
                StartCoroutine(CountdownCoroutine());
            }
        }
        else
        {
            int DiffTime = (PhotonNetwork.ServerTimestamp - pun.StartTime) / 1000;
            int NowTime = (MaxTime + ReadyTime + Margin) - DiffTime;
            if(NowTime > 60 || NowTime < 0) return;
            TimeText.text = NowTime.ToString();
            if(NowTime == 0)
            {
                MenuButton.interactable = false;
                ExitPanel.SetActive(false);
                MenuPanel.SetActive(false);
                FinishPanel.SetActive(true);
                pun.playerStatus.LiveState = false;
                Finish = true;
                StartCoroutine(ResultCoroutine());
            }
        }
    }

    private IEnumerator CountdownCoroutine()
    {
        CountdownPanel.SetActive(false);
        CountdownText.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        MenuButton.interactable = true;
        CountdownText.gameObject.SetActive(false);
        StartGame = true;
    }

    private IEnumerator ResultCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["Flag"] = pun.playerStatus.GetComponentInChildren<FlagStatus>().number; //自分の旗の数を送る
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        while(string.IsNullOrEmpty(pun.Judge)) yield return null;
        FinishPanel.SetActive(false);
        ResultPanel.SetActive(true);
        if(pun.Judge.Equals("Draw"))
        {
            ResultText.text = pun.Judge;
        }
        else
        {
            ResultText.text = pun.Judge + " の勝利!!";
        }
        yield return new WaitForSeconds(3.0f);
        MenuButton.interactable = true;
        MenuPanel.SetActive(true);
    }

    private void MenuScreen()
    {
        if(!ActivePanel)
        {
            Audio2d.Instance.Play("Ok");
            MenuPanel.SetActive(true);
            ActivePanel = true;
        }
        else
        {
            Audio2d.Instance.Play("Cancel");
            if(MenuPanel.activeSelf)
            {
                MenuPanel.SetActive(false);
            }
            if(ExitPanel.activeSelf)
            {
                ExitPanel.SetActive(false);
            }
            ActivePanel = false;
        } 
    }

    private void Exit()
    {
        Audio2d.Instance.Play("Ok");
        ExitPanel.SetActive(true);
        MenuPanel.SetActive(false);
    }

    private void Yes()
    {
        Audio2d.Instance.Play("Ok");
        pun.playerStatus.LifeGaugeDelete();
        mainCamera.target = null;
        mainCamera.CameraOk = false;
        ActivePanel = false;
        PhotonNetwork.LeaveRoom();
    }

    private void No()
    {
        Audio2d.Instance.Play("Cancel");
        MenuPanel.SetActive(true);
        ExitPanel.SetActive(false);
    }
}


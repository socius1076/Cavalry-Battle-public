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

    //試合開始処理
    private IEnumerator MatchingWait()
    {
        //マッチング処理が完了するまで待機
        while(!pun.Ready) yield return null;
        MenuButton.interactable = false;
        MatchingPanel.SetActive(false);
        CountdownPanel.SetActive(true);
        //マスターの場合
        if(PhotonNetwork.IsMasterClient)
        {
            //部屋が作られてから経過した時間をサーバから取得
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable["Start"] = PhotonNetwork.ServerTimestamp;
            //カスタムプロパティの値を同期
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
        //画面遷移後数秒待機
        yield return new WaitForSeconds(Margin);
        ReadyGame = true;
    }

    private void Update()
    {
        if(!ReadyGame || Finish) return;
        //試合開始前カウントダウン
        if(!StartGame)
        {
            //部屋が作られてから経過した時間と試合開始時間の差(増えていく)
            int DiffTime = (PhotonNetwork.ServerTimestamp - pun.StartTime) / 1000;
            //ReadyTime(カウントダウン時間)とMargin(画面遷移後待機時間)の和からDiffTimeを引く
            int NowTime = (ReadyTime + Margin) - DiffTime;
            //正しい数値か確認
            if(NowTime > ReadyTime || NowTime < 0) return;
            //数字を画面に表示させる
            CountdownText.text = NowTime.ToString();
            if(NowTime == 0)
            {
                StartCoroutine(CountdownCoroutine());
            }
        }
        //試合時間
        else
        {
            //部屋が作られてから経過した時間と試合開始時間の差(増えていく)
            int DiffTime = (PhotonNetwork.ServerTimestamp - pun.StartTime) / 1000;
            //MaxTime(試合時間)とReadyTime(カウントダウン時間)とMargin(画面遷移後待機時間)の和からDiffTimeを引く
            int NowTime = (MaxTime + ReadyTime + Margin) - DiffTime;
            //正しい数値か確認
            if(NowTime > MaxTime || NowTime < 0) return;
            //数字を画面に表示させる
            TimeText.text = NowTime.ToString();
            //0秒になったらボタンの操作ができないようにする
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

    //"Go!"と画面に表示させてボタンの操作を可能にする
    private IEnumerator CountdownCoroutine()
    {
        CountdownPanel.SetActive(false);
        CountdownText.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        MenuButton.interactable = true;
        CountdownText.gameObject.SetActive(false);
        StartGame = true;
    }

    //試合終了後処理
    private IEnumerator ResultCoroutine()
    {
        yield return new WaitForSeconds(3.0f);
        //自分の旗の数を送る
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable["Flag"] = pun.playerStatus.GetComponentInChildren<FlagStatus>().number;
        //自分の旗の数を送る
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        //結果が設定されるまで待機
        while(string.IsNullOrEmpty(pun.Judge)) yield return null;
        FinishPanel.SetActive(false);
        ResultPanel.SetActive(true);
        //引き分けの場合
        if(pun.Judge.Equals("Draw"))
        {
            ResultText.text = pun.Judge;
        }
        //引き分けでない場合
        else
        {
            ResultText.text = pun.Judge + " の勝利!!";
        }
        yield return new WaitForSeconds(3.0f);
        //ロビーに戻るか確認
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


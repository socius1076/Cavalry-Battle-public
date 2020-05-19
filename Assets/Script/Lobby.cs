using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(RectTransform))]

public class Lobby : MonoBehaviour
{
    [SerializeField] private Button onlineButton = null;
    [SerializeField] private Text text = null;
    [SerializeField] private Button trainingButton = null;
    [SerializeField] private GameObject TitlePanel = null;
    [SerializeField] private Button TitleButton = null;
    [SerializeField] private Button YesButton = null;
    [SerializeField] private Button NoButton = null;
    private Pun pun = null;
    private int maxmember = 0;
    private int nowmember = 0;
    private bool ActivePanel = false;

    private void Start()
    {
        pun = GameObject.Find("Pun").GetComponent<Pun>();
        maxmember = pun.maxmember;
        TitlePanel.SetActive(false);
        onlineButton.onClick.AddListener(RoomEnter);
        trainingButton.onClick.AddListener(TrainingEnter);
        TitleButton.onClick.AddListener(Title);
        YesButton.onClick.AddListener(Yes);
        NoButton.onClick.AddListener(No);
    }

    private void Update()
    {
        if(pun.maxroom)
        {
            onlineButton.interactable = false;
        }
        else
        {
            onlineButton.interactable = true;
        }
        nowmember = pun.nowmember;
        text.text = nowmember + "/" + maxmember;
    }

    private void RoomEnter()
    {
        Audio2d.Instance.Play("Ok");
        SceneManager.LoadScene("OnlineScene");
        pun.RoomEnter();
    }

    private void TrainingEnter()
    {
        Audio2d.Instance.Play("Ok");
        SceneManager.LoadScene("TrainingScene");
        pun.TrainingEnter();
    }

    private void Title()
    {
        if(!ActivePanel)
        {
            Audio2d.Instance.Play("Ok");
            TitlePanel.SetActive(true);
            ActivePanel = true;
        }
        else
        {
            Audio2d.Instance.Play("Cancel");
            if(TitlePanel.activeSelf)
            {
                TitlePanel.SetActive(false);
            }
            ActivePanel = false;
        }
    }

    private void Yes()
    {
        Audio2d.Instance.Play("Ok");
        PhotonNetwork.Disconnect();
        Destroy(pun.gameObject);
        SceneManager.LoadScene("TitleScene");
        ActivePanel = false;
    }

    private void No()
    {
        Audio2d.Instance.Play("Cancel");
        TitlePanel.SetActive(false);
        ActivePanel = false;
    }
}

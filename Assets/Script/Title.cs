using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] private InputField inputField = null;
    [SerializeField] private Text text = null;
    [SerializeField] private Button button = null;
    [SerializeField] private Pun pun = null;
    public string playername = null; //参照用変数

    private void Start()
    {
        inputField.onValueChanged.AddListener(DisplayText);
        inputField.onEndEdit.AddListener(DisplayText);
        button.onClick.AddListener(OnClickButton);
    }

    private void DisplayText(string inputText)
    {
        text.text = inputText;
        playername = text.text;
    }

    private void Update() //名前を一文字以上入力させる
    {
        if(playername.Length > 0)
        {
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
        }
    }

    private void OnClickButton()
    {
        Audio2d.Instance.Play("Ok");
        pun.SetName(playername);
        pun.namecheck = true;
    }
}

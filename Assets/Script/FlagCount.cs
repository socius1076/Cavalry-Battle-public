using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]

public class FlagCount : MonoBehaviour
{
    [SerializeField] private Text flagtext = null;
    public Attack attack = null;
    private Pun pun = null;
    public FlagStatus flagStatus = null;

    private void Start()
    {
        pun = GameObject.Find("Pun").GetComponent<Pun>();
    }

    private void Update()
    {
        if(attack == null || flagStatus == null) return;
        if(pun.roomjudge == 0)
        {
            flagtext.text = "Flag : " + flagStatus.number + "個";
        }
        else if(pun.roomjudge == 1)
        {
            flagtext.text = "獲得したFlag : " + attack.flagcount + "個";
        }
    }
}

using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(RectTransform))]

public class GameOverAnimator : MonoBehaviour
{
    [SerializeField] private Text text = null;
    private Transform _transform;

    private void Start()
    {
        _transform = text.transform;
        Vector3 DefaultPos = _transform.localPosition;
        _transform.localPosition = new Vector3(0.0f, 320.0f, 0.0f);
        _transform.DOLocalMove(DefaultPos, 1.0f).SetEase(Ease.Linear)
            .OnComplete(() => _transform.DOShakePosition(1.5f, 100.0f));
        DOVirtual.DelayedCall(5.0f, LoadSceneMethod);
    }

    private void LoadSceneMethod()
    {
        PhotonNetwork.LeaveRoom();
    }
}

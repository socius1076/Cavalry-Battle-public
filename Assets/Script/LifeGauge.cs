//ライフゲージ

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]

public class LifeGauge : MonoBehaviour
{
    [SerializeField] private Image FillImage = null;
    private RectTransform rectTransform;
    private Camera _camera;
    private ObjectStatus objectStatus;
    [SerializeField] private Text playernamePrefab = null;
    public Text playernameText = null;
    private MainCamera mainCamera = null;

    private void Start()
    {
        playernameText = Instantiate(playernamePrefab, transform);
        if(objectStatus.CompareTag("Enemy") == false)
        {
            OnRideController onRideController = objectStatus.GetComponent<OnRideController>();
            playernameText.text = onRideController.playername;
        }
        mainCamera = _camera.GetComponent<MainCamera>();
    }

    private void Update()
    {
        if(mainCamera.CameraOk == false)
        {
            return;
        }
        if(objectStatus == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            FillImage.fillAmount = objectStatus.NowLife / objectStatus.MaxLife;
            Vector3 screenPoint = _camera.WorldToScreenPoint(objectStatus.transform.position); //オブジェクトのワールド座標からスクリーン座標へ変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out Vector2 localPoint); //スクリーン座標をUIのローカル座標に変換
            transform.localPosition = localPoint + new Vector2(0.0f, 220.0f); //上にずらす
            playernameText.transform.localPosition = localPoint = new Vector2(0.0f, 40.0f);
        }
    }

    public void Initialize(RectTransform recttransform, Camera camera, ObjectStatus Objectstatus)
    {
        rectTransform = recttransform;
        _camera = camera;
        objectStatus = Objectstatus;
    }
}

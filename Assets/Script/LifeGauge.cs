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
        if(!objectStatus.CompareTag("Enemy"))
        {
            OnRideController onRideController = objectStatus.GetComponent<OnRideController>();
            playernameText.text = onRideController.playername;
        }
        mainCamera = _camera.GetComponent<MainCamera>();
    }

    private void Update()
    {
        if(!mainCamera.CameraOk) return;
        if(objectStatus == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            FillImage.fillAmount = objectStatus.NowLife / objectStatus.MaxLife;
            //オブジェクトのワールド座標からスクリーン座標へ変換
            Vector3 screenPoint = _camera.WorldToScreenPoint(objectStatus.transform.position);
            //スクリーン座標をUIのローカル座標に変換
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out Vector2 localPoint);
            //上にずらす
            transform.localPosition = localPoint + new Vector2(0.0f, 220.0f);
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

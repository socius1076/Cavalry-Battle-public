using UnityEngine;
using Photon.Pun;

public class OnRideController : MonoBehaviourPunCallbacks
{
    const float RotationSpeed = 360.0f;
    private float JumpPower = 300.0f;
    public float Velocity = 15.0f;
    private Vector3 MoveVelocity = Vector3.zero;
    private float Xpos,Zpos;
    private Rigidbody _rigidbody;
    private Transform _transform;
    private Animator animator = null;
    [SerializeField] private Animator rideranimator = null;
    private Camera _camera = null;
    private PlayerStatus playerStatus = null;
    public string playername = null;
    public Joystick joystick = null;

    //接地判定
    private bool IsGrounded
    {
        get
        {
            Vector3 Pos = _transform.position + new Vector3(0.0f, 1.0f, 0.0f);
            RaycastHit raycastHit;
            //球状のrayを飛ばす
            if(Physics.SphereCast(Pos, 1.0f, Vector3.down, out raycastHit, 0.01f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerStatus = GetComponent<PlayerStatus>();
        _camera = Camera.main;
        _rigidbody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
        playername = photonView.Owner.NickName;
    }

    private void Update()
    {
        if(playerStatus.LiveState && photonView.IsMine)
        {
            Xpos = joystick.Horizontal;
            Zpos = joystick.Vertical;
        }
        //停止
        else
        {
            animator.SetFloat("MoveSpeed", 0.0f);
            rideranimator.SetFloat("MoveSpeed", 0.0f);
        }
    }

    private void FixedUpdate()
    {
        if(playerStatus.LiveState)
        {
            //カメラの向きからx-z平面の単位ベクトルを得る
            Vector3 cameraForward = Vector3.Scale(_camera.transform.forward, new Vector3(1.0f, 0.0f, 1.0f)).normalized;
            if (IsGrounded) 
            {
                //接地時はyベクトル0
                MoveVelocity.y = 0.0f;
                animator.SetFloat("MoveSpeed", new Vector3(MoveVelocity.x, 0.0f, MoveVelocity.z).magnitude);
                rideranimator.SetFloat("MoveSpeed", new Vector3(MoveVelocity.x, 0.0f, MoveVelocity.z).magnitude);
            }
            //カメラの向きと速度を合わせる
            MoveVelocity = cameraForward * Zpos * Velocity + _camera.transform.right * Xpos * Velocity;
            //向かせたいベクトルの変数
            Vector3 Look = new Vector3(MoveVelocity.x, 0.0f, MoveVelocity.z);
            if(Look != Vector3.zero && Look.magnitude > 0.1f)
            {
                //向かせたい角度の変数
                Quaternion Rotation = Quaternion.LookRotation(Look);
                //滑らかに向く
                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Rotation, RotationSpeed * Time.deltaTime);
            }
            _rigidbody.AddForce(MoveVelocity);
        }
    }

    //DEBUG rayの形状確認
    /*void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0.0f, 1.0f, 0.0f), 1.0f);
    }*/

    [PunRPC] public void Jump()
    {
        if(IsGrounded)
        {
            _rigidbody.AddForce(JumpPower * Vector3.up);
        }
    }
}
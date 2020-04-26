﻿//プレイヤー制御

using UnityEngine;
using Photon.Pun;

public class OnRideController : MonoBehaviourPunCallbacks
{
    const float RotationSpeed = 360.0f; //回転速度
    private float JumpPower = 300.0f; //ジャンプ力
    public float Velocity = 15.0f; //速度
    private Vector3 MoveVelocity = Vector3.zero; //移動速度情報
    private float Xpos,Zpos; //移動速度格納
    private Rigidbody _rigidbody;
    private Transform _transform;
    private Animator animator = null;
    [SerializeField] private Animator rideranimator = null;
    private Camera _camera = null;
    private PlayerStatus playerStatus = null;
    public string playername = null;
    public Joystick joystick = null;

    private bool IsGrounded //接地判定
    {
        get
        {
            Vector3 Pos = _transform.position + new Vector3(0.0f, 1.0f, 0.0f); //位置取得
            RaycastHit raycastHit;
            if(Physics.SphereCast(Pos, 1.0f, Vector3.down, out raycastHit, 0.01f) == true) //球状のrayを飛ばす
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

    private void Update() //入力処理
    {
        if(playerStatus.LiveState == true && photonView.IsMine == true)
        {
            Xpos = joystick.Horizontal;
            Zpos = joystick.Vertical;
        }
        else //停止
        {
            animator.SetFloat("MoveSpeed", 0.0f);
            rideranimator.SetFloat("MoveSpeed", 0.0f);
        }
    }

    private void FixedUpdate() //物理処理
    {
        if(playerStatus.LiveState == true)
        {
            Vector3 cameraForward = Vector3.Scale(_camera.transform.forward, new Vector3(1.0f, 0.0f, 1.0f)).normalized; //カメラの向きからx-z平面の単位ベクトルを得る
            if (IsGrounded == true) 
            {
                MoveVelocity.y = 0.0f; //接地時はyベクトル0
                animator.SetFloat("MoveSpeed", new Vector3(MoveVelocity.x, 0.0f, MoveVelocity.z).magnitude);
                rideranimator.SetFloat("MoveSpeed", new Vector3(MoveVelocity.x, 0.0f, MoveVelocity.z).magnitude);
            }
            MoveVelocity = cameraForward * Zpos * Velocity + _camera.transform.right * Xpos * Velocity; //カメラの向きと速度を合わせる
            Vector3 Look = new Vector3(MoveVelocity.x, 0.0f, MoveVelocity.z);  //向かせたいベクトルの変数
            if(Look != Vector3.zero && Look.magnitude > 0.1f)
            {
                Quaternion Rotation = Quaternion.LookRotation(Look); //向かせたい角度の変数
                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, Rotation, RotationSpeed * Time.deltaTime); //滑らかに向く
            }
            _rigidbody.AddForce(MoveVelocity);
        }
    }

    /*void OnDrawGizmos() //rayの形状確認
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0.0f, 1.0f, 0.0f), 1.0f);
    }*/

    [PunRPC] public void Jump()
    {
        if(IsGrounded == true)
        {
            _rigidbody.AddForce(JumpPower * Vector3.up);
        }
    }
}
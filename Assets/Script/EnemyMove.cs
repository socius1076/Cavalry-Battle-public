//敵を動かす

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyStatus))]

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask = 0;
    private RaycastHit[] HitObject = new RaycastHit[10];
    private EnemyStatus enemyStatus;
    private NavMeshAgent navMeshAgent;
    
    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyStatus = GetComponent<EnemyStatus>();
    }

    public void OnDetectObject(Collider collider) //CollisionDetectorに呼ばれるメソッド
    {
        if(enemyStatus.MoveAble == false)
        {
            navMeshAgent.isStopped = true;
            return;
        }
        if(collider.CompareTag("Player") == true)
        {
            PlayerStatus playerStatus = collider.GetComponent<PlayerStatus>();
            if(playerStatus.NowLife <= 0.0)
            {
                return;
            }
            Vector3 TransformDiff = collider.transform.position - transform.position; //座標の差を格納する変数
            float Distance = TransformDiff.magnitude; //対象との距離を格納する変数
            Vector3 Direction = TransformDiff.normalized; //対象への方向を格納する変数
            int HitCount = Physics.RaycastNonAlloc(transform.position, Direction, HitObject, Distance, layerMask); //ヒットしたオブジェクトの情報を格納する変数

            /*Ray ray = new Ray(transform.position,TransformDiff);
            Debug.DrawRay(transform.position,TransformDiff,Color.red); //rayの可視化
            Physics.Raycast(ray, out RaycastHit hit);
            Debug.Log("HitCount" + HitCount + "   " + hit.collider.gameObject.name); //オブジェクト数確認*/

            if(HitCount == 0) //障害物がない場合
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.destination = collider.transform.position; //対象へ向かう
            }
            else
            {
                navMeshAgent.isStopped = true;
            }
        }
    }
}

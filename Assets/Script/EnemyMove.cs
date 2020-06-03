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

    public void OnDetectObject(Collider collider)
    {
        if(!enemyStatus.MoveAble)
        {
            navMeshAgent.isStopped = true;
            return;
        }
        if(collider.CompareTag("Player"))
        {
            PlayerStatus playerStatus = collider.GetComponent<PlayerStatus>();
            if(playerStatus.NowLife <= 0.0) return;
            //座標の差を格納する変数
            Vector3 TransformDiff = collider.transform.position - transform.position;
            //対象との距離を格納する変数
            float Distance = TransformDiff.magnitude;
            //対象への方向を格納する変数
            Vector3 Direction = TransformDiff.normalized;
            //ヒットしたオブジェクトの情報を格納する変数
            int HitCount = Physics.RaycastNonAlloc(transform.position, Direction, HitObject, Distance, layerMask);
            
            //DEBUG
            /*Ray ray = new Ray(transform.position,TransformDiff);
            //rayの可視化
            Debug.DrawRay(transform.position,TransformDiff,Color.red);
            Physics.Raycast(ray, out RaycastHit hit);
            //オブジェクト数確認
            Debug.Log("HitCount" + HitCount + "   " + hit.collider.gameObject.name);*/

            //障害物がない場合
            if(HitCount == 0)
            {
                navMeshAgent.isStopped = false;
                //対象へ向かう
                navMeshAgent.destination = collider.transform.position;
            }
            else
            {
                navMeshAgent.isStopped = true;
            }
        }
    }
}

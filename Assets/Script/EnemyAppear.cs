using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAppear : MonoBehaviour
{
    public PlayerStatus playerStatus = null;
    [SerializeField] private GameObject EnemyPrefab = null;
    public int EnemyCount = 0;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        //プレイヤー接続まで待機
        if(playerStatus == null) yield return null;
        //プレイヤー接続から数秒待機
        yield return new WaitForSeconds(1.0f);
        while(true)
        {
            Vector3 Distance = new Vector3(3.0f, 0.0f, 0.0f);
            //y軸を中心に回転させたランダムな位置
            Vector3 AnglePosition = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f) * Distance;
            Vector3 Position = playerStatus.transform.position + AnglePosition;
            //プレイヤーの位置から最も近いNavMeshの座標を得る
            if(NavMesh.SamplePosition(Position, out NavMeshHit navMeshHit, 10.0f, NavMesh.AllAreas))
            {
                if(EnemyCount < 10)
                {
                    Instantiate(EnemyPrefab, navMeshHit.position, Quaternion.identity);
                    EnemyCount++;
                }
            }
            yield return new WaitForSeconds(5.0f); //敵の出現間隔
        }
    }
}

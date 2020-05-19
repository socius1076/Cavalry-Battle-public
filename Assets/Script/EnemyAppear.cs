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
        if(playerStatus == null) yield return null; //プレイヤー接続まで待機
        yield return new WaitForSeconds(1.0f); //プレイヤー接続から数秒待機
        while(true)
        {
            Vector3 Distance = new Vector3(3.0f, 0.0f, 0.0f);
            Vector3 AnglePosition = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f) * Distance; //y軸を中心に回転させたランダムな位置
            Vector3 Position = playerStatus.transform.position + AnglePosition;
            if(NavMesh.SamplePosition(Position, out NavMeshHit navMeshHit, 10.0f, NavMesh.AllAreas)) //プレイヤー位置から最も近いNavMeshの座標を得る
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

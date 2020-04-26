﻿//アイテムを出現させる

using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(EnemyStatus))]

public class ItemDrop : MonoBehaviour
{
    [SerializeField][Range(0,1)] private float DropRate = 0.0f; //出現確率
    [SerializeField] private Item ItemPrefab = null;
    [SerializeField] private int Number = 1;
    private EnemyStatus enemyStatus = null;
    private bool DropJudge = false; 

    private void Start()
    {
        enemyStatus = GetComponent<EnemyStatus>();
    }

    private void Update()
    {
        if(enemyStatus.NowLife <= 0.0f)
        {
            Drop();
        }
    }

    private void Drop()
    {
        if(DropJudge == true) //一度だけ実行
        {
            return;
        }
        DropJudge = true;
        if(Random.Range(0.0f,1.0f) >= DropRate)
        {
            return;
        }
        for(int i = 0; i < Number; i++) //Numberの数だけアイテム出現
        {
            Item item = Instantiate(ItemPrefab, transform.position, Quaternion.identity);
            item.Initialize();
        }
    }
}
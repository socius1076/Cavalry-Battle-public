//ライフゲージ管理

using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(RectTransform))]

public class LifeGaugeContainer : MonoBehaviour
{
    private static LifeGaugeContainer instance; //private static 一つしか存在しない

    public static LifeGaugeContainer Instance //インスタンスを返す
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private Camera MainCamera = null;
    [SerializeField] private LifeGauge lifeGaugePrefab = null;
    private RectTransform rectTransform;
    private readonly Dictionary<ObjectStatus, LifeGauge> stateLifeBarMap = new Dictionary<ObjectStatus, LifeGauge>(); //代入不可 連想配列
    
    private void Awake()
    {
        if(null != instance)
        {
            throw new Exception("LifeBarContainer instance already exists.");
        }
        instance = this; //インスタンスを代入
        rectTransform = GetComponent<RectTransform>();
    }

    public void Add(ObjectStatus status)
    {
        LifeGauge lifeGauge = Instantiate(lifeGaugePrefab, transform);
        lifeGauge.Initialize(rectTransform, MainCamera, status);
        stateLifeBarMap.Add(status, lifeGauge);
    }

    public void Remove(ObjectStatus status)
    {
        Destroy(stateLifeBarMap[status].gameObject);
        stateLifeBarMap.Remove(status); //キーを指定して削除
    }
}
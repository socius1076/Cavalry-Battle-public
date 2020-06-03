using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(RectTransform))]

public class LifeGaugeContainer : MonoBehaviour
{
    private static LifeGaugeContainer instance;

    //インスタンスを返す
    public static LifeGaugeContainer Instance
    {
        get { return instance; }
    }

    [SerializeField] private Camera MainCamera = null;
    [SerializeField] private LifeGauge lifeGaugePrefab = null;
    private RectTransform rectTransform;
    private readonly Dictionary<ObjectStatus, LifeGauge> stateLifeBarMap = new Dictionary<ObjectStatus, LifeGauge>();
    
    private void Awake()
    {
        if(instance != null)
        {
            throw new Exception("LifeBarContainer instance already exists.");
        }
        //インスタンスを代入
        instance = this;
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
        //キーを指定して削除
        stateLifeBarMap.Remove(status);
    }
}
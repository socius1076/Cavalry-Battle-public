//衝突検知

using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private TriggerEvent triggerEventEnter = new TriggerEvent(); //インスタンス化
    [SerializeField] private TriggerEvent triggerEventStay = new TriggerEvent();
    
    private void OnTriggerEnter(Collider coll) //他のコライダーに重なったときに呼ばれる
    {
        triggerEventEnter.Invoke(coll);
    }

    private void OnTriggerStay(Collider coll) //他のコライダーに触れ続けている間に呼ばれる
    {
        triggerEventStay.Invoke(coll);
    }

    [Serializable] private class TriggerEvent:UnityEvent<Collider> //コールバック関数処理
    {
        
    }
}

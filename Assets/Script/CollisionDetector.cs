using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private TriggerEvent triggerEventEnter = new TriggerEvent();
    [SerializeField] private TriggerEvent triggerEventStay = new TriggerEvent();
    
    //他のコライダーに重なったときに呼ばれる
    private void OnTriggerEnter(Collider coll)
    {
        triggerEventEnter.Invoke(coll);
    }

    //他のコライダーに触れ続けている間に呼ばれる
    private void OnTriggerStay(Collider coll)
    {
        triggerEventStay.Invoke(coll);
    }

    //コールバック関数処理
    [Serializable] private class TriggerEvent:UnityEvent<Collider>
    {
        
    }
}

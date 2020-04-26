//FloatingJoystick修正

using UnityEngine;
using UnityEngine.EventSystems;

public class MyFloatingJoystick : Joystick
{
    private Vector2 position = Vector2.zero;
    
    protected override void Start()
    {
        base.Start();
        background.gameObject.SetActive(true);
        position = background.transform.position;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        background.transform.position = position;
        base.OnPointerUp(eventData);
    }
}
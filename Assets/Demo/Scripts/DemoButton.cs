using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Demo.Scripts
{
    public class DemoButton : Button
    {
        public UnityEvent onPointerDown = new UnityEvent();
        public UnityEvent onPointerUp = new UnityEvent();
        public bool IsPressing { get; private set; }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            IsPressing = true;
            onPointerDown.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            IsPressing = false;
            onPointerUp.Invoke();
        }
    }
}
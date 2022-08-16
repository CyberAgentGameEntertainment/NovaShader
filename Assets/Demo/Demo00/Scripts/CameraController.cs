using UnityEngine;

namespace Demo.Scripts
{
    [DisallowMultipleComponent]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _speed = 0.01f;
        [SerializeField] private DemoButton _leftButton;
        [SerializeField] private DemoButton _topButton;
        [SerializeField] private DemoButton _rightButton;
        [SerializeField] private DemoButton _downButton;

        private void Update()
        {
            var diffPos = Vector3.zero;
            if (_leftButton.IsPressing)
            {
                diffPos += -transform.right * _speed;
            }

            if (_topButton.IsPressing)
            {
                diffPos += transform.forward * _speed;
            }

            if (_rightButton.IsPressing)
            {
                diffPos += transform.right * _speed;
            }

            if (_downButton.IsPressing)
            {
                diffPos += -transform.forward * _speed;
            }

            transform.position += diffPos;
        }
    }
}
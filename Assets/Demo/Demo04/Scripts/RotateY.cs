using UnityEngine;

namespace Demo.Demo04.Scripts
{
    public class RotateY : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f;

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
    }
}

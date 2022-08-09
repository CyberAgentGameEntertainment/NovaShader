// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Test.TestUberLit_01.Scripts
{
    [RequireComponent(typeof(Text))]
    public class Test : MonoBehaviour
    {
        private const float DisplayTimeSec = 5.0f;
        [SerializeField] private GameObject cameraGameObject;
        [SerializeField] private GameObject[] directionalLightGameObjects;
        [SerializeField] private GameObject dispObjectNameGameObject;
        [SerializeField] private GameObject particleRootObject;
        private readonly Vector3 _lookTarget = new Vector3(0.0f, 0.7f, 0.0f);
        private Text _displayObjectNameText;
        private int _displayObjectNo;
        private float _displayTimerSec;
        private Transform[] _particleTransforms;

        private void Start()
        {
            _particleTransforms = new Transform[particleRootObject.transform.childCount];
            for (var childNo = 0; childNo < particleRootObject.transform.childCount; childNo++)
            {
                _particleTransforms[childNo] = particleRootObject.transform.GetChild(childNo);
                _particleTransforms[childNo].gameObject.SetActive(false);
            }

            _displayObjectNameText = dispObjectNameGameObject.GetComponent<Text>();
            _particleTransforms[0].gameObject.SetActive(true);
            _displayObjectNameText.text = _particleTransforms[0].gameObject.name;
        }

        // Update is called once per frame
        private void Update()
        {
            _displayTimerSec += Time.deltaTime;
            if (_displayTimerSec > DisplayTimeSec)
            {
                _displayTimerSec = 0.0f;
                _particleTransforms[_displayObjectNo].gameObject.SetActive(false);
                _displayObjectNo = ++_displayObjectNo % _particleTransforms.Length;
                _particleTransforms[_displayObjectNo].gameObject.SetActive(true);
                _displayObjectNameText.text = _particleTransforms[_displayObjectNo].gameObject.name;
            }

            RotateLight();
            RotateCamera();
        }

        private void RotateCamera()
        {
            var addRot = Quaternion.AngleAxis(15.0f * Time.deltaTime, Vector3.up);
            var position = cameraGameObject.transform.localPosition;
            var localPosition = position;
            localPosition = addRot * localPosition;
            position = localPosition;
            cameraGameObject.transform.localPosition = position;
            var lookDir = _lookTarget - position;
            cameraGameObject.transform.localRotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
        }

        private void RotateLight()
        {
            var addRot = Quaternion.AngleAxis(120.0f * Time.deltaTime, Vector3.up);
            foreach (var litGo in directionalLightGameObjects)
                litGo.transform.localRotation = addRot * litGo.transform.localRotation;
        }
    }
}
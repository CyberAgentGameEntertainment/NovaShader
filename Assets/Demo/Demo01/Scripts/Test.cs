// --------------------------------------------------------------
// Copyright 2022 CyberAgent, Inc.
// --------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Test.TestUberLit_00.Scripts
{
    public class Test : MonoBehaviour
    {
        private const float RotateLightTime = 10.0f;
        private const float RotateCameraTime = 10.0f;
        [SerializeField] private GameObject[] particleGameObjects0;
        [SerializeField] private GameObject[] particleGameObjects1;
        [SerializeField] private GameObject[] particleGameObjects2;
        [SerializeField] private GameObject[] directionalLightGameObjects;
        [SerializeField] private GameObject cameraGameObject;
        [SerializeField] private GameObject testDescriptionGameObject;
        [SerializeField] private GameObject changeShaderButtonGameObject;
        [SerializeField] private GameObject changeWorkflowButtonGameObject;
        [SerializeField] private TestCase startTestCase;
        private readonly Vector3 _lookTarget = new Vector3(0.0f, 0.5f, 0.0f);
        private readonly ResumedInfo _resumeInfo = new ResumedInfo();
        private Vector3 _cameraInitializedPosition;
        private Quaternion _cameraInitializedRotation;
        private GameObject[] _currentParticleGameObject;
        private Quaternion[] _directionLightInitializedRotation;
        private bool _isFinished;
        private bool _isPause;
        private TestCase _testCase = TestCase.AllMapsSet;
        private Text _testDescriptionText;
        private TestShader _testShader = TestShader.NovaLitMetallic;
        private float _testTimer;

        // Start is called before the first frame update
        private void Start()
        {
            _directionLightInitializedRotation = new Quaternion[directionalLightGameObjects.Length];
            for (var i = 0; i < _directionLightInitializedRotation.Length; i++)
                _directionLightInitializedRotation[i] = directionalLightGameObjects[i].transform.localRotation;

            _cameraInitializedPosition = cameraGameObject.transform.localPosition;
            _cameraInitializedRotation = cameraGameObject.transform.localRotation;

            foreach (var go in particleGameObjects0) go.SetActive(false);

            foreach (var go in particleGameObjects1) go.SetActive(false);

            foreach (var go in particleGameObjects2) go.SetActive(false);

            _testDescriptionText = testDescriptionGameObject.GetComponent<Text>();
            _testCase = startTestCase;
            StartTest();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_isFinished) return;

            UpdateTestDescription();
            UpdateParticleGameObjects();

            if (_isPause) return;
            _testTimer += Time.deltaTime;
            if (_testTimer < RotateLightTime)
            {
                RotateLight();
                RotateCamera();
            }
            else
            {
                // Go to next shader.
                _testTimer = 0.0f;
                _testShader++;
                if (_testShader != TestShader.End) return;
                // Completed test of all shader.
                // Go to next test case.
                _testCase++;
                if (_testCase != TestCase.End)
                {
                    StartTest();
                }
                else
                {
                    _testDescriptionText.text = "Tests has been finished.";
                    _isFinished = true;
                }
            }
        }

        private void SetupCurrentParticleGameObject()
        {
            switch (_testCase)
            {
                case TestCase.AllMapsSet:
                    _currentParticleGameObject = particleGameObjects0;
                    break;
                case TestCase.BaseMapAndNormalMapsSet:
                    _currentParticleGameObject = particleGameObjects1;
                    break;
                case TestCase.BaseMapSetOnly:
                    _currentParticleGameObject = particleGameObjects2;
                    break;
            }
        }

        private void StartTest()
        {
            for (var i = 0; i < _directionLightInitializedRotation.Length; i++)
                directionalLightGameObjects[i].transform.localRotation = _directionLightInitializedRotation[i];

            cameraGameObject.transform.localPosition = _cameraInitializedPosition;
            cameraGameObject.transform.localRotation = _cameraInitializedRotation;

            CalculateCameraDirection();
            DeactivateCurrentParticleGameObjects();
            SetupCurrentParticleGameObject();
            DeactivateCurrentParticleGameObjects();
            _testShader = TestShader.NovaLitMetallic;
        }

        private void DeactivateCurrentParticleGameObjects()
        {
            if (_currentParticleGameObject == null) return;
            foreach (var go in _currentParticleGameObject) go.SetActive(false);
        }

        private void UpdateParticleGameObjects()
        {
            for (var i = 0; i < _currentParticleGameObject.Length; i++)
                if (i != (int)_testShader)
                    _currentParticleGameObject[i].SetActive(false);
            _currentParticleGameObject[(int)_testShader].SetActive(true);
        }

        private void UpdateTestDescription()
        {
            string workflow;
            if (_testShader == TestShader.NovaLitMetallic
                || _testShader == TestShader.UrpLitMetallic)
                workflow = "Workflow : Metallic";
            else
                workflow = "Workflow : Specular";

            string shader;
            if (_testShader == TestShader.NovaLitMetallic
                || _testShader == TestShader.NovaLitSpecular)
                shader = "Shader : Nova/UberLit";
            else
                shader = "Shader : URP/Lit";

            var testCaseDescription = "";
            if (_testCase == TestCase.AllMapsSet)
                testCaseDescription = "Test Case 0 : All maps are set.";
            else if (_testCase == TestCase.BaseMapAndNormalMapsSet)
                testCaseDescription = "Test Case 1 : Base map and Normal map are set.";
            else if (_testCase == TestCase.BaseMapSetOnly) testCaseDescription = "Test Case 2 : Only Base map is set.";

            _testDescriptionText.text = $"{testCaseDescription}\n" +
                                        $"{workflow}\n" +
                                        $"{shader}\n";
        }

        private void CalculateCameraDirection()
        {
            var lookDir = _lookTarget - cameraGameObject.transform.localPosition;
            cameraGameObject.transform.localRotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
        }

        private void RotateCamera()
        {
            var addRot = Quaternion.AngleAxis(15.0f * Time.deltaTime, Vector3.up);
            var localPosition = cameraGameObject.transform.localPosition;
            localPosition = addRot * localPosition;
            cameraGameObject.transform.localPosition = localPosition;
            CalculateCameraDirection();
        }

        private void RotateLight()
        {
            var addRot = Quaternion.AngleAxis(120.0f * Time.deltaTime, Vector3.up);
            foreach (var litGo in directionalLightGameObjects)
                litGo.transform.localRotation = addRot * litGo.transform.localRotation;
        }

        public void OnChangeWorkflowButton()
        {
            _testShader = _testShader switch
            {
                TestShader.NovaLitMetallic => TestShader.NovaLitSpecular,
                TestShader.UrpLitMetallic => TestShader.UrpLitSpecular,
                TestShader.NovaLitSpecular => TestShader.NovaLitMetallic,
                TestShader.UrpLitSpecular => TestShader.UrpLitMetallic,
                _ => _testShader
            };
        }

        public void OnChangeShaderButton()
        {
            _testShader = _testShader switch
            {
                TestShader.NovaLitMetallic => TestShader.UrpLitMetallic,
                TestShader.UrpLitMetallic => TestShader.NovaLitMetallic,
                TestShader.NovaLitSpecular => TestShader.UrpLitSpecular,
                TestShader.UrpLitSpecular => TestShader.NovaLitSpecular,
                _ => _testShader
            };
        }

        public void OnPausePlayButton(GameObject button)
        {
            _isPause = !_isPause;
            changeShaderButtonGameObject.GetComponent<Button>().interactable = _isPause;
            changeWorkflowButtonGameObject.GetComponent<Button>().interactable = _isPause;
            var text = button.GetComponentInChildren<Text>();
            if (_isPause)
            {
                _resumeInfo.TestShader = _testShader;
                text.text = "Play";
            }
            else
            {
                _testShader = _resumeInfo.TestShader;
                text.text = "Pause";
            }
        }

        private enum TestShader
        {
            NovaLitMetallic,
            UrpLitMetallic,
            NovaLitSpecular,
            UrpLitSpecular,
            End
        }

        private enum TestCase
        {
            AllMapsSet,
            BaseMapAndNormalMapsSet,
            BaseMapSetOnly,
            End
        }

        /// <summary>
        ///     This information for resumed to the playback state.
        /// </summary>
        private class ResumedInfo
        {
            public TestShader TestShader { get; set; }
        }
        //   public void OnChangeShader( )
    }
}
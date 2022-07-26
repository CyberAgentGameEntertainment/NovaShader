using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TestUberLit : MonoBehaviour
{
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
        End,
    }

    private enum TestStep
    {
        RotateLight,
        RotateCamera,
        End
    }
    /// <summary>
    /// This information for resumed to the playback state.
    /// </summary>
    class ResumedInfo
    {
        public TestShader TestShader { get; set; }
    }
    [SerializeField] private GameObject[] _particleGameObjects0;
    [SerializeField] private GameObject[] _particleGameObjects1;
    [SerializeField] private GameObject[] _particleGameObjects2;
    [SerializeField] private GameObject[] _directionalLightGameObjects;
    [SerializeField] private GameObject _cameraGameObject;
    [SerializeField] private GameObject _testDescriptionGameObject;
    [SerializeField] private GameObject _changeShaderButtonGameObject;
    [SerializeField] private GameObject _changeWorkflowButtonGameObject;
    [SerializeField] private TestCase _startTestCase;
    
    private const float RotateLightTime = 10.0f;
    private const float RotateCameraTime = 10.0f;
    private GameObject[] _currentParticleGameObject;
    private Text _testDescriptionText;
    private TestCase _testCase = TestCase.AllMapsSet;
    private Quaternion[] _directionLightInitialyzedRotation;
    private Vector3 _lookTarget = new Vector3(0.0f, 0.5f, 0.0f);
    private Vector3 _cameraInitialyzedPosition;
    private Quaternion _cameraInitialyzedRotation;
    private bool _isPause = false;
    private TestShader _testShader = TestShader.NovaLitMetallic;
    private TestStep _testStep = TestStep.RotateLight;
    private float _testTimer = 0.0f;
    private bool _isFinished = false;
    private ResumedInfo _resumeInfo = new ResumedInfo();

    // Start is called before the first frame update
    private void Start()
    {
        _directionLightInitialyzedRotation = new Quaternion[_directionalLightGameObjects.Length];
        for (int i = 0; i < _directionLightInitialyzedRotation.Length; i++)
        {
            _directionLightInitialyzedRotation[i] = _directionalLightGameObjects[i].transform.localRotation;
        }

        _cameraInitialyzedPosition = _cameraGameObject.transform.localPosition;
        _cameraInitialyzedRotation = _cameraGameObject.transform.localRotation;

        foreach (var go in _particleGameObjects0)
        {
            go.SetActive(false);
        }

        foreach (var go in _particleGameObjects1)
        {
            go.SetActive(false);
        }

        foreach (var go in _particleGameObjects2)
        {
            go.SetActive(false);
        }

        _testDescriptionText = _testDescriptionGameObject.GetComponent<Text>();
        _testCase = _startTestCase;
        StartTest();
    }

    void SetupCurrentParticleGameObject()
    {
        switch (_testCase)
        {
            case TestCase.AllMapsSet:
                _currentParticleGameObject = _particleGameObjects0;
                break;
            case TestCase.BaseMapAndNormalMapsSet:
                _currentParticleGameObject = _particleGameObjects1;
                break;
            case TestCase.BaseMapSetOnly:
                _currentParticleGameObject = _particleGameObjects2;
                break;
        }
    }

    private void StartTest()
    {
        for (int i = 0; i < _directionLightInitialyzedRotation.Length; i++)
        {
            _directionalLightGameObjects[i].transform.localRotation = _directionLightInitialyzedRotation[i];
        }

        _cameraGameObject.transform.localPosition = _cameraInitialyzedPosition;
        _cameraGameObject.transform.localRotation = _cameraInitialyzedRotation;

        CalculateCameraDirection();
        SetupCurrentParticleGameObject();
        _testStep = TestStep.RotateLight;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isFinished)
        {
            return;
        }

        UpdateTestDescription();
        UpdateParticleGameObjects();

        if (_isPause)
        {
            return;
        }
        switch (_testStep)
        {
            case TestStep.RotateLight:
                ExecuteTestStep(TestRotateLight);
                break;
            case TestStep.RotateCamera:
                ExecuteTestStep(TestRotateCamera);
                break;
            case TestStep.End:
                // Set next test case.
                DeactiveCurrentParticleGameObjects();
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

                break;
        }
    }

    private void DeactiveCurrentParticleGameObjects()
    {
        foreach (var go in _currentParticleGameObject)
        {
            go.SetActive(false);
        }
    }

    private void UpdateParticleGameObjects()
    {
        DeactiveCurrentParticleGameObjects();
        _currentParticleGameObject[(int)_testShader].SetActive(true);
    }

    private void UpdateTestDescription()
    {
        string workflow;
        if (_testShader == TestShader.NovaLitMetallic
            || _testShader == TestShader.UrpLitMetallic)
        {
            workflow = "Workflow : Metallic";
        }
        else
        {
            workflow = "Workflow : Specular";
        }

        string shader;
        if (_testShader == TestShader.NovaLitMetallic
            || _testShader == TestShader.NovaLitSpecular)
        {
            shader = "Shader : Nova/UberLit";
        }
        else
        {
            shader = "Shader : URP/Lit";
        }

        var testCaseDescription = "";
        if (_testCase == TestCase.AllMapsSet)
        {
            testCaseDescription = "Test Case 0 : All maps are set.";
        }
        else if (_testCase == TestCase.BaseMapAndNormalMapsSet)
        {
            testCaseDescription = "Test Case 1 : Base map and Normal map are set.";
        }
        else if (_testCase == TestCase.BaseMapSetOnly)
        {
            testCaseDescription = "Test Case 2 : Only Base map is set.";
        }

        _testDescriptionText.text = $"{testCaseDescription}\n" +
                                    $"{workflow}\n" +
                                    $"{shader}\n";
    }

    private void CalculateCameraDirection()
    {
        var lookDir = _lookTarget - _cameraGameObject.transform.localPosition;
        _cameraGameObject.transform.localRotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
    }

    private void ExecuteTestStep(Action internalTest)
    {
        _testTimer += Time.deltaTime;
        if (_testTimer > RotateLightTime)
        {
            // Go to next test.
            _testTimer = 0.0f;
            _testShader++;
            if (_testShader == TestShader.End)
            {
                // Completed test of all shader.
                _testShader = TestShader.NovaLitMetallic;
                _testStep++;
            }

            return;
        }

        internalTest();
    }

    private void TestRotateCamera()
    {
        var addRot = Quaternion.AngleAxis(15.0f * Time.deltaTime, Vector3.up);
        var localPosition = _cameraGameObject.transform.localPosition;
        localPosition = addRot * localPosition;
        _cameraGameObject.transform.localPosition = localPosition;
        CalculateCameraDirection();
    }

    private void TestRotateLight()
    {
        var addRot = Quaternion.AngleAxis(60.0f * Time.deltaTime, Vector3.up);
        foreach (var litGo in _directionalLightGameObjects)
        {
            litGo.transform.localRotation = addRot * litGo.transform.localRotation;
        }
    }

    public void OnChangeWorkflowButton()
    {
        if (_testShader == TestShader.NovaLitMetallic)
        {
            _testShader = TestShader.NovaLitSpecular;
        }else if (_testShader == TestShader.UrpLitMetallic)
        {
            _testShader = TestShader.UrpLitSpecular;
        }else if (_testShader == TestShader.NovaLitSpecular)
        {
            _testShader = TestShader.NovaLitMetallic;
        }else if (_testShader == TestShader.UrpLitSpecular)
        {
            _testShader = TestShader.UrpLitMetallic;
        }
    }
    public void OnChangeShaderButton()
    {
        if (_testShader == TestShader.NovaLitMetallic)
        {
            _testShader = TestShader.UrpLitMetallic;
        }else if (_testShader == TestShader.UrpLitMetallic)
        {
            _testShader = TestShader.NovaLitMetallic;
        }else if (_testShader == TestShader.NovaLitSpecular)
        {
            _testShader = TestShader.UrpLitSpecular;
        }else if (_testShader == TestShader.UrpLitSpecular)
        {
            _testShader = TestShader.NovaLitSpecular;
        }
    }
    public void OnPausePlayButton(GameObject button)
    {
        _isPause = !_isPause;
        _changeShaderButtonGameObject.GetComponent<Button>().interactable = _isPause;
        _changeWorkflowButtonGameObject.GetComponent<Button>().interactable = _isPause;
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
    //   public void OnChangeShader( )
}
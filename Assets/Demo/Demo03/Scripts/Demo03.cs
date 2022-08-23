using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Demo03 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject fpsTextGameObject;
    private Text _fpsText;
    private int _frameCount =0;
    private readonly List<float> _fpsCounts = new List<float>();
    void Start()
    {
        Application.targetFrameRate = 120;
        _fpsText = fpsTextGameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        _frameCount++;
        if (_frameCount > 1000)
        {
            float fpsAverage = 0.0f;
            foreach (var fps in _fpsCounts)
            {
                fpsAverage += fps;
            }

            fpsAverage /= _fpsCounts.Count;
            _fpsText.text = $"FPS(Average) = {fpsAverage:0.00}";
        }
        else
        {
            var fps = 1.0f / Time.deltaTime;
            _fpsText.text = $"FPS = {fps:0.00}";
            _fpsCounts.Add(fps);
        }


    }
}

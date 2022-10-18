using UnityEngine;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
    public Button _submitButton;
    public Button _cancelButton;

    void Start()
    {
        _submitButton.onClick.AddListener(Submit);
        _cancelButton.onClick.AddListener(Cancel);
    }
    
    
    private void Submit()
    {
        Debug.Log("Submitしました");
    }
    
    private void Cancel()
    {
        Debug.Log("Cancelしました");
    }
}

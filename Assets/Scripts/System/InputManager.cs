using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action OnClick;
    public Action OnPressSpace;


    private void Update()
    {
        
    }


    private void DetectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
        }
        
    }
}

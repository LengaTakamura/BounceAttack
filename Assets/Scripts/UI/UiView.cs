using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UiView : MonoBehaviour
{
   [SerializeField] private List<UiBase> _uiObjects = new();
   [SerializeField] private Canvas _canvas;
   [SerializeField] private Image _targetImage;

   public Canvas Canvas => _canvas; 
   public Image TargetImage => _targetImage;
   public List<UiBase> UiObjects => _uiObjects;
}

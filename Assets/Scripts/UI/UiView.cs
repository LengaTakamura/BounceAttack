using System.Collections.Generic;
using UI;
using UnityEngine;

public class UiView : MonoBehaviour
{
   [SerializeField]private List<UiBase> _uiObjects = new();

   public List<UiBase> UiObjects => _uiObjects;
}

using UnityEngine;
using UnityEngine.UI;

public class NovelView : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] Text _text;
    [SerializeField] string[] _texts;

    public Text Text => _text;
    public Canvas Canvas => _canvas;

    public string[] Texts => _texts;
}

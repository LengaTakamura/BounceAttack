using Novel;
using UnityEngine;

public class NovelSceneManager : MonoBehaviour
{
    private NovelPresenter _presenter;
    private NovelInputManager _inputManager;
    private NovelUiManager _uiManager;


    [SerializeField] NovelView _view;

    private void OnEnable()
    {
        _inputManager = new NovelInputManager();
        _uiManager = new NovelUiManager(_view);
        _presenter = new NovelPresenter(_inputManager,_uiManager);
        _presenter.Init();
    }

    private void Update()
    {
        _presenter?.OnUpdate();
    }

    private void OnDisable()
    {
        _presenter = null;
        _inputManager = null;
        _uiManager = null;
    }
}

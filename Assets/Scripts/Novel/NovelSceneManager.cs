using System;
using CriWare;
using Novel;
using UnityEngine;

public class NovelSceneManager : MonoBehaviour
{
    private NovelPresenter _presenter;
    private NovelInputManager _inputManager;
    private NovelUiManager _uiManager;
    private NovelSoundManager _novelSoundManager;
    private NovelBeatSystem _novelBeatSystem;
    private CriAtomExPlayback _playBack;


    [SerializeField] private NovelView _view;
    [SerializeField] private CriAtomSource _criAtomSorce;

    private void OnEnable()
    {
        _inputManager = new NovelInputManager();
        _uiManager = new NovelUiManager(_view);
        _presenter = new NovelPresenter(_inputManager, _uiManager);
        _novelSoundManager = new NovelSoundManager(_criAtomSorce);
        _playBack = _novelSoundManager.PlayBGM();
        _novelBeatSystem = new NovelBeatSystem(_playBack);
        BeatSyncDispatcher.Instance.NovelInit(_novelBeatSystem);

        _presenter.Init();
        _uiManager.OnSceneChanged += BeatSyncDispatcher.Instance.Clear;
        _uiManager.OnSceneChanged += SceneChange;
    }

    private void Update()
    {
        _presenter?.OnUpdate();
    }

    private void OnDisable()
    {
        _novelSoundManager?.StopBGM();
        _uiManager.OnSceneChanged -= SceneChange;
        _uiManager.OnSceneChanged -= BeatSyncDispatcher.Instance.Clear;
        _presenter?.Dispose(); // NovelUiManager、NovelInputManagerのDisposeを内包
        _novelSoundManager?.Dispose();
        _presenter = null;
        _inputManager = null;
        _uiManager = null;
        _novelSoundManager = null;
    }
    private async void SceneChange()
    {
        await GameSystem.Instance.SceneChange("Stage1");
    }

}

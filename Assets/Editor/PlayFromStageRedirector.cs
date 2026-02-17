using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class PlayFromStageRedirector
{
    private const string NovelScenePath = "Assets/Scenes/Novel.unity";
    private static readonly string[] StageSceneNames = { "Stage1" };

    private static SceneAsset _originalPlayModeStartScene;
    private static bool _changedByThisTool;

    static PlayFromStageRedirector()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            SetupRedirectIfNeeded();
            return;
        }

        if ((state == PlayModeStateChange.EnteredEditMode || state == PlayModeStateChange.EnteredPlayMode) && _changedByThisTool)
        {
            EditorSceneManager.playModeStartScene = _originalPlayModeStartScene;
            _originalPlayModeStartScene = null;
            _changedByThisTool = false;
        }
    }

    private static void SetupRedirectIfNeeded()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid())
        {
            return;
        }

        bool isStageScene = StageSceneNames.Contains(activeScene.name, StringComparer.OrdinalIgnoreCase);
        if (!isStageScene)
        {
            return;
        }

        var novelSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(NovelScenePath);
        if (novelSceneAsset == null)
        {
            Debug.LogWarning($"PlayFromStageRedirector: Novel scene was not found at path: {NovelScenePath}");
            return;
        }

        _originalPlayModeStartScene = EditorSceneManager.playModeStartScene;
        EditorSceneManager.playModeStartScene = novelSceneAsset;
        _changedByThisTool = true;
    }
}

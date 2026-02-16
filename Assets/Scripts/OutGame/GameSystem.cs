using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance;

    private InGameManager _inGameManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async UniTaskVoid SceneChange(string sceneName)
    {
        await SceneManager.LoadSceneAsync(sceneName);
    }
}

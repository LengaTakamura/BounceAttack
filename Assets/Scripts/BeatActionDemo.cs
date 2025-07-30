using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using CriWare;

public class BeatActionDemo : MonoBehaviour
{
    [SerializeField]private string _cueSheetName;
    [SerializeField]private string _acbFilePath;
    [SerializeField] private string _bgmCueName;
    private CriAtomExAcb _criAtomExAcb;
    private CriAtomExPlayer _atomExPlayer;
    private CancellationTokenSource _cts;

    [SerializeField] private GameObject _prefab;
    private async void Start()
    {
        _cts = new CancellationTokenSource();

        _atomExPlayer = new CriAtomExPlayer();

        // キューシートの追加
        var cueSheet = CriAtom.AddCueSheet(_cueSheetName, _acbFilePath, "");

        // 非同期でロード完了を待つ
        await UniTask.WaitUntil(() => !cueSheet.IsLoading, cancellationToken: _cts.Token);

        _criAtomExAcb = cueSheet.acb;

        PlayBgm(_bgmCueName);
    }

    private void OnDestroy()
    {
        CriAtomExBeatSync.OnCallback -= CriAtomExBeatSync_OnCallback;
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private void PlayBgm(string bgmCueName)
    {
        if (!_criAtomExAcb.Exists(bgmCueName)) return;

        _atomExPlayer.SetCue(_criAtomExAcb, bgmCueName);
        _atomExPlayer.Start();

        CriAtomExBeatSync.OnCallback += CriAtomExBeatSync_OnCallback;
    }

    private void CriAtomExBeatSync_OnCallback(ref CriAtomExBeatSync.Info info)
    {
         Instantiate(_prefab, transform);
    }
}
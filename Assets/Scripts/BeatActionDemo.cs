using System;
using UnityEngine;
using System.Threading;
using CriWare;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BeatActionDemo : MonoBehaviour,IBeatSyncListener
{
    [SerializeField] private string _bgmCueName;
    private CriAtomExAcb _criAtomExAcb;
    private CriAtomExPlayer _atomExPlayer;
    private CancellationTokenSource _cts;

    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }

    [SerializeField] private GameObject _prefab;
    [SerializeField]private CriAtomSource _source;
    private CriAtomExPlayback _playback;
    [SerializeField] private Image _image;
    private float _second;
    private float _nextBeatTime;
    private CancellationTokenSource _cts2;
    private void Start()
    {
        Init();
        PlayBgm(_bgmCueName);
        BeatSyncDispatcher.Instance.Register(this);

    }
    private void Init()
    {
        _cts = new CancellationTokenSource();
        _cts2 = new CancellationTokenSource();
    }
    
    private void PlayBgm(string bgmCueName)
    {
       _playback = _source.Play(bgmCueName);
    }
    
    private void OnDestroy()
    {
        _cts?.Dispose();
        _cts2?.Dispose();
    }
    
    private void OnDisable()
    {
        BeatSyncDispatcher.Instance.Unregister(this);
    }

    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
        Debug.Log("OnBeat");
        //Instantiate(_prefab, transform);
        _second = 60f / info.bpm;
        
        var nowTime = _playback.GetTimeSyncedWithAudio() / 1000f;
        _nextBeatTime = nowTime + _second * (1f - info.beatProgress);
    }


    
    private void Update()
    {
        ActionB();
    }


    private void ActionB()
    {
        if (!Input.GetKeyDown(KeyCode.B) || !_playback.GetBeatSyncInfo(out CriAtomExBeatSync.Info info)) return;
        float progress = info.beatProgress; 
        float bpm = info.bpm;
        float secondsPerBeat = 60f / bpm;
        if (progress < 0.1f || progress > 0.9f)
        {
            Debug.Log("Great");
            var obj = Instantiate(_prefab, transform.position, Quaternion.identity);
            var random = Random.Range(-1f, 1f);
            var random2 = Random.Range(-1f, 1f);
            obj.GetComponent<Rigidbody>().AddForce(new Vector3(random,random2,1) * 10, ForceMode.Impulse);
        }
        else if (progress < 0.3f || progress > 0.7f)
        {
            Debug.Log("Good");
        }
        else
        {
            Debug.Log("Bad");
        }
    }
}

public interface IBeatSyncListener
{
    void OnBeat(ref CriAtomExBeatSync.Info info);
    
    int CurrentBpm { get; set;}
    
    int DiffBpm { get; set;}
    
    bool IsBeating { get; set;}
    
    
}
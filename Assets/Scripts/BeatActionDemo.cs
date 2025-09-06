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
    private int _count;
    private float _diff;
    private CriAtomExBeatSync.Info _info;
    private float _prevBeatTime;
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
       if (_playback.GetBeatSyncInfo(out CriAtomExBeatSync.Info info))
       {
           _info = info;
       }
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
        _count++;
        if (_count % 2 == 0)
        {
            Debug.Log("OnBeat");
        }
        //Instantiate(_prefab, transform);
        
        var nowTime = _playback.GetTime() / 1000f;
        float secondsPerBeat = 60f / info.bpm / 2;
        _prevBeatTime = nowTime;
        _nextBeatTime = nowTime + secondsPerBeat;
    }


    
    private void Update()
    {
        ActionB();
    }
    private void ActionB()
    {
        if (!Input.GetKeyDown(KeyCode.B)) return;
        if (_playback.GetBeatSyncInfo(out CriAtomExBeatSync.Info info))
        {
            var nowTime = _playback.GetTime() / 1000f;
            float secondsPerBeat = 60f / info.bpm / 2;
            float diffPrev = Mathf.Abs(nowTime - _prevBeatTime);
            float diffNext = Mathf.Abs(nowTime - _nextBeatTime);
            var diff = Mathf.Min(diffPrev,diffNext);
            var greatDiff = secondsPerBeat * 0.2f;
            var goodDiff = secondsPerBeat * 0.4f;
            if (diff < greatDiff)
            {
                Debug.Log("Great" + diff);
                var obj = Instantiate(_prefab, transform.position, Quaternion.identity);
                var random = Random.Range(-1f, 1f);
                var random2 = Random.Range(-1f, 1f);
                obj.GetComponent<Rigidbody>().AddForce(new Vector3(random,random2,1) * 10, ForceMode.Impulse);
            }
            else if (diff < goodDiff)
            {
                Debug.Log("Good" + diff);
            }
            else
            {
                Debug.Log("Bad" + diff);
            }
        }
       
    }
}

public enum BeatActionType
{
    Bad,Good,Great
}

public interface IBeatSyncListener
{
    void OnBeat(ref CriAtomExBeatSync.Info info);
    
    int CurrentBpm { get; set;}
    
    int DiffBpm { get; set;}
    
    bool IsBeating { get; set;}
    
    
}
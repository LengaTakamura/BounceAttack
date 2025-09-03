using CriWare;
using UnityEngine;
using DG.Tweening;
public class BeatScaleDemo : MonoBehaviour,IBeatSyncListener
{
    
    public int CurrentBpm { get; set; }
    public int DiffBpm { get; set; }
    public bool IsBeating { get; set; }
    void Start()
    {
        BeatSyncDispatcher.Instance.Register(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        BeatSyncDispatcher.Instance.Unregister(this);
    }

    public void OnBeat(ref CriAtomExBeatSync.Info info)
    {
        gameObject.transform.DOScale(new Vector3(3f, 3f, 3f), 0.1f).OnComplete(OnComp);
    }

    void OnComp()
    {
        gameObject.transform.DOScale(new Vector3(1f, 1f, 1f), 0.1f);
    }


}

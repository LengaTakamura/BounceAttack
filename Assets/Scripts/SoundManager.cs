using CriWare;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]private CriAtomSource _source;
    [SerializeField] private string _bgmCueName;
    public CriAtomExPlayback PlayBgm()
    {
        var playback = _source.Play(_bgmCueName);
        return playback;
    }
}

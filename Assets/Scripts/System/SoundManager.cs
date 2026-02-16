using System;
using CriWare;

public class SoundManager : IDisposable
{
    private CriAtomSource _source;
    private string _bgmCueName;
    public void InGameInit(CriAtomSource source, SoundData soundData)
    {
        _source = source;
        _bgmCueName = soundData.CueName;
        _source.cueSheet = soundData.CueSheet;
    }
    public CriAtomExPlayback PlayBgm()
    {
        var playback = _source.Play(_bgmCueName);
        return playback;
    }

    public void Dispose()
    {
        _source?.Stop();
        _source = null;
        _bgmCueName = null;
    }
}

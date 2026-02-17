using System;
using CriWare;

public class NovelSoundManager:IDisposable
{
    private CriAtomSource _criAtomSource;

    public NovelSoundManager(CriAtomSource criAtomSource)
    {
        _criAtomSource = criAtomSource;
    }

    public CriAtomExPlayback PlayBGM()
    {
        var playback = _criAtomSource.Play();
        return playback;
    }

    public void StopBGM()
    {
        _criAtomSource.Stop();
    }

    public void Dispose()
    {
        _criAtomSource = null;
    }
}

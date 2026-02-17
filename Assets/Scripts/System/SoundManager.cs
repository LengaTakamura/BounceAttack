using System;
using CriWare;

public class SoundManager : IDisposable
{
    private CriAtomSource _source;
    private string _cueName;

    public void InGameInit(CriAtomSource source, SoundData soundData)
    {
        _source = source;
        _cueName = soundData != null ? soundData.CueName : null;
    }

    public CriAtomExPlayback PlayBgm()
    {
        if (_source == null)
        {
            return default;
        }

        var playback = string.IsNullOrWhiteSpace(_cueName)
            ? _source.Play()
            : _source.Play(_cueName);

        return playback;
    }

    public void Dispose()
    {
        _source?.Stop();
        _source = null;
        _cueName = null;
    }
}

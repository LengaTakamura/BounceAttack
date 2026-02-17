

using System;
using CriWare;

public class NovelBeatSystem : IBeatSyncListener,IDisposable
{
    private int _count;
    public TempoState CurrentTempo;
    private CriAtomExPlayback _playback;
    public NovelBeatSystem(CriAtomExPlayback playback)
    {
        _playback = playback;
    }
    public void OnBeat(BeatInfo info)
    {
        
    }

    public BeatInfo UpdateInfo(CriAtomExBeatSync.Info info)
        {
            _count++;
            var time = (double)_playback.GetTime() / 1000f;
            var secondsPerBeat = 60f / info.bpm;

            var copy = new BeatInfo
            {
                Bpm = info.bpm,
                SecondsPerBeat = secondsPerBeat,
                BeatCount = info.beatCount,
                CurrentBeat = _count,
                NowTime = time,
                PrevBeatTime = time,
                NextBeatTime = time + secondsPerBeat,
                Playback = _playback,
            };

            return copy;
        }

    public void Dispose()
    {
        
    }
}

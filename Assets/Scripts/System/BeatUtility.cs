using System;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BeatUtility
{
    
    
    /// <summary>
    /// Beatに現在のタイミングがどれだけ近いかをTypeで返す
    /// divideBeatでBPMを減らす場合、時間の更新も併せて減らして下さい
    /// </summary>
    public static BeatActionType JudgeBeatAction(BeatInfo info)                                        
    {
        var nowTime = (double)info.Playback.GetTime() / 1000f;
        var secondsPerBeat = info.SecondsPerBeat;
        var diffPrev = nowTime - info.PrevBeatTime;
        var diffNext = info.NextBeatTime - nowTime;
        var diff =  diffPrev > diffNext ? diffNext : diffPrev;
        var greatDiff = secondsPerBeat * 0.2f;
        var goodDiff = secondsPerBeat * 0.4f;
        if (diff < greatDiff)
        {
            return BeatActionType.Great;
        }

        if (diff < goodDiff)
        {
            return BeatActionType.Good;
        }

        return BeatActionType.Bad;
    }

    public static double TimeUntilBeat(BeatInfo info, float preparationTime, int beatOffset)
    {
        var nowTime = info.NowTime;
        var targetTime = info.SecondsPerBeat * (info.CurrentBeat + beatOffset);
        var startTime = targetTime - preparationTime;
        var waitTime = (double)startTime - nowTime;
        return waitTime > 0 ? waitTime : 0;
    }
}

public enum BeatActionType
{
    Bad,
    Good,
    Great,
    None
}
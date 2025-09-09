using System;
using UnityEngine;

public static class BeatUtility
{
    public static BeatActionType JudgeBeatAction(BeatInfo info, float prevBeatTime, float nextBeatTime,
        int beatDivision)
    {
        var nowTime = info.NowTime;
        float secondsPerBeat = 60f / info.Bpm / beatDivision;
        float diffPrev = Mathf.Abs(nowTime - prevBeatTime);
        float diffNext = Mathf.Abs(nowTime - nextBeatTime);
        var diff = Mathf.Min(diffPrev, diffNext);
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
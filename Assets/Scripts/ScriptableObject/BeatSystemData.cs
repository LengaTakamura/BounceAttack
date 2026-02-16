using UnityEngine;

[CreateAssetMenu(fileName = "BeatSystemData", menuName = "Beat/BeatSystemData")]
public class BeatSystemData : ScriptableObject
{
    /// <summary>
    /// 最初の猶予拍数
    /// </summary>
    public int prepareBeat;

    /// <summary>
    /// BPM変更時の、次のBPMStateまでの猶予拍数
    /// </summary>
    public int BetweenBeats;

    /// <summary>
    /// 曲開始までの待機時間
    /// </summary>
    public int WaitingTime;

    /// <summary>
    /// BPM変更する拍数
    /// </summary>
    public int ChangeTempoBeat;
}
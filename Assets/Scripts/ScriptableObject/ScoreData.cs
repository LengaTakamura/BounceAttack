using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Data/ScoreData")]
public class ScoreData : ScriptableObject
{
    public int Score;

    /// <summary>
    /// ゲーム開始時などにリセットするためのメソッド
    /// </summary>
    public void ResetScore()
    {
        Score = 0;
    }
}
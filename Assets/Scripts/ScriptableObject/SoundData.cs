using CriWare;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Beat/SoundData")]
public class SoundData : ScriptableObject
{
    /// <summary>
    /// CueSheet名
    /// </summary>
    public CriAtomCueSheet CueSheet;

    /// <summary>
    /// Cue名
    /// </summary>
    public string CueName;
}
using UnityEngine;
using System;

namespace System
{
    [CreateAssetMenu(fileName = "InputManagerData", menuName = "Beat/InputManagerData")]
    public class InputManagerData : ScriptableObject
    {
        [Header("Base Scores")]
        [Tooltip("入力タイプごとの基本スコア")]
        public SerializableDictionary<InputType, int> BaseScores = new();

        [Header("Score Multipliers")]
        [Tooltip("Bad判定時のスコア倍率")]
        public float BadMultiplier = 0.5f;

        [Tooltip("Good判定時のスコア倍率")]
        public float GoodMultiplier = 1.0f;

        [Tooltip("Great判定時のスコア倍率")]
        public float GreatMultiplier = 1.5f;
    }
}
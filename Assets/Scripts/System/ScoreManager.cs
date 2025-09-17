using System;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static int _score;
    private SerializedDictionary<InputType,int> _baseScores = new();

    private void Start()
    {
        
    }
}
[System.Serializable]
public class SerializedDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [System.Serializable]
    internal class KeyValue
    {
        public K Key;
        public V Value;

        public KeyValue(K key, V value)
        {
            Key = key;
            Value = value;
        }
    }
    [SerializeField] private List<KeyValue> _list;

    public virtual K DefaultKey => default;
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Clear();
        foreach (var item in _list)
        {
            this[ContainsKey(item.Key) ? DefaultKey : item.Key] = item.Value;
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        _list = new List<KeyValue>(Count);
        foreach (var kvp in this)
        {
            _list.Add(new KeyValue(kvp.Key, kvp.Value));
        }
    }
}
[System.Serializable]
public class SerializedDictionary<V> : SerializedDictionary<string, V>
{
    public override string DefaultKey => string.Empty;
}
[System.Serializable]
public class SerializedDictionaryC<K, V> : SerializedDictionary<K, V> where K : new()
{
    public override K DefaultKey => new();
}
using NaughtyAttributes;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField]
    private SerializableDictionary<string, string> dict = new();

    [Button]
    private void Test()
    {
        Debug.Log(dict.Dictionary.Count);
    }

    // [Button]
    // private void Clear()
    // {
    //     dict.Clear();
    // }
}

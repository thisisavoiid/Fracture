using System;
using UnityEngine;

[Serializable]
public struct TimeMS
{
    public TimeMS(float totalSeconds)
    {
        Minutes = Mathf.Floor(totalSeconds / 60.0f);
        Seconds = totalSeconds % 60.0f;
    }
    public float Minutes;
    public float Seconds;

    public float TotalSeconds => (Minutes * 60) + Seconds;

    public override string ToString() => $"(Minutes: {Minutes}, Seconds: {Seconds})";
}
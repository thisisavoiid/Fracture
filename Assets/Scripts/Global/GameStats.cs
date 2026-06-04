using System;

[Serializable]
public class GameStats
{
    public int Score => _score;
    private int _score = 0;
    
    public void AddScore(int value) => _score += value;
    public void Reset()
    {
        _score = 0;
    }
}
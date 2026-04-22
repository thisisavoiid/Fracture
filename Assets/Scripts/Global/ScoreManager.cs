using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    private static ScoreManager _instance;
    public static ScoreManager Instance => _instance;
    public UnityEvent<int> OnScoreUpdate;
    private int _score = 0;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void AddScore(int score)
    {
        _score += score;
        OnScoreUpdate?.Invoke(_score);
        Debug.Log($"[SCORE MANAGER] Score updated: {_score} (Added: {score}) -");
    }
}
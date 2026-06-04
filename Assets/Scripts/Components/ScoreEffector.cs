using UnityEngine;

public class ScoreEffector : MonoBehaviour
{
    [SerializeField] private int _scoreEffect = 0;
    
    public void ApplyScore()
    {
        if (GameStatsManager.Instance == null)
            return;
        
        GameStatsManager.Instance.AddScore(_scoreEffect);
    }
}
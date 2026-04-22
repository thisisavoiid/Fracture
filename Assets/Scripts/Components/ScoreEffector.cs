using UnityEngine;

public class ScoreEffector : MonoBehaviour
{
    [SerializeField] private int _scoreEffect = 0;
    
    public void ApplyScore()
    {
        if (ScoreManager.Instance == null)
            return;
        
        ScoreManager.Instance.AddScore(_scoreEffect);
    }
}
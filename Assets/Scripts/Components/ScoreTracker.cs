using TMPro;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;

    private void Start()
    {
        if (ScoreManager.Instance == null)
            return;
        
        ScoreManager.Instance.OnScoreUpdate.AddListener((score) => RefreshTextMesh(score.ToString()));
    }

    private void RefreshTextMesh(string text)
    {
        _textMesh.text = text;
    }
}

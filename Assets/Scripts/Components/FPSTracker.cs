using TMPro;
using UnityEngine;

public class FPSTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMesh;
    [Range(1,4)] [SerializeField] private float _updatesPerSecond = 1;
    private float _timePassed = 0.0f;
    private int _deltaFrames = 0;
    private int _currentFPS = 0;

    private void Update()
    {
        if (_timePassed >= 1/_updatesPerSecond)
        {
            _currentFPS = _deltaFrames;
            _deltaFrames = 0;
            _timePassed = 0.0f;
        }

        _timePassed += Time.deltaTime;
        _deltaFrames += 1;

        if (_textMesh != null)
            _textMesh.text = GetFPS().ToString();
    }

    public float GetFPS() => _currentFPS * _updatesPerSecond;
}

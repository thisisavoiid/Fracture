using UnityEngine;

[RequireComponent(typeof(VerticeRetriever))]
public class DestructibleObject : MonoBehaviour, IShootable
{
    private VerticeRetriever _vertRetriever;

    private void Awake()
    {
        _vertRetriever = GetComponent<VerticeRetriever>();
    }

    public void Hit(float dmg)
    {
        _vertRetriever.TriggerFractureExplosions();
    }
}
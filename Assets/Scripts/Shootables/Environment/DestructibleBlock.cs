using UnityEngine;

[RequireComponent(typeof(VerticeRetriever))]
public class DestructibleObject : MonoBehaviour, IShootable
{
    private VerticeRetriever _vertRetriever;

    private void Awake()
    {
        _vertRetriever = GetComponent<VerticeRetriever>();
    }

    public void Damage(float dmg)
    {
        _vertRetriever.TriggerFractureExplosions();
    }
}
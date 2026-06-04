using UnityEngine;

public abstract class Bee : MonoBehaviour, ICollectionMember
{
    public abstract void Tick();
    public abstract void SetPosition(Vector3 pos);
    public abstract void SetState(BeeState state);

    public void OnEnable()
    {
        Subscribe();
    }

    public void OnDisable()
    {
        Unsubscribe();
    }

    public void Subscribe()
    {
        EnemyCollectionManager.Instance?.Subscribe(this);
    }

    public void Unsubscribe()
    {
        EnemyCollectionManager.Instance?.Unsubscribe(this);
    }
}
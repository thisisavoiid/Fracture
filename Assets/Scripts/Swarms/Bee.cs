using UnityEngine;
using UnityEngine.UIElements;

public abstract class Bee : MonoBehaviour, ICollectionMember
{
    public abstract void Tick();
    public abstract void SetPosition(Vector3 pos);
    public abstract void SetState(BeeState state);

    public void Subscribe()
    {
        EnemyCollectionManager manager = EnemyCollectionManager.Instance;

        if (manager == null)
            return;
        
        manager.Subscribe(this);
    }

    public void Unsubscribe()
    {
        EnemyCollectionManager manager = EnemyCollectionManager.Instance;

        if (manager == null)
            return;
        
        manager.Unsubscribe(this);
    }
}
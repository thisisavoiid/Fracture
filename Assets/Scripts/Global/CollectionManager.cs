using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class CollectionManager : MonoBehaviour
{
    [SerializeField] private UnityEvent _onLastMemberRemoved;
    [SerializeField] private UnityEvent<ICollectionMember> _onMemberRemoved;
    [SerializeField] private UnityEvent<ICollectionMember> _onMemberAdded;

    protected List<ICollectionMember> _members = new();

    public void ResetCollection()
    {
        _members.Clear();
    }

    public void Subscribe(ICollectionMember member)
    {
        if (_members.Contains(member))
            return;

        _members.Add(member);

        _onMemberAdded?.Invoke(member);
    }

    public void Unsubscribe(ICollectionMember member)
    {
        if (!_members.Contains(member))
            return;

        _members.Remove(member);

        _onMemberRemoved?.Invoke(member);

        if (_members.Count == 0)
            _onLastMemberRemoved?.Invoke();
    }

    [Button]
    public void GetMemberCount()
    {
        if (_members == null)
            return;

        Debug.Log(_members.Count);
    }
}
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(RigidbodyMovement))]
public class BeeController : Swarm, IShootable
{
    [SerializeField] private float _moveSpeed;
    private SwarmContext _ctx;
    private Health _health;
    private RigidbodyMovement _rbMovement;
    private bool _isAttacking = false;

    public void Hit(float dmg, Vector3 point)
    {
        _health.TakeDamage(dmg);
        
        if (!_isAttacking)
            StartAttack();
    }

    public override void Init(SwarmContext ctx)
    {
        _health = GetComponent<Health>();
        _rbMovement = GetComponent<RigidbodyMovement>();

        _ctx = ctx;
        transform.position = _ctx.StartPosition;
    }

    public override void InvokeDeath()
    {
        if (_ctx.OnSwarmDeathAction == null)
            return;
        
        _ctx.OnSwarmDeathAction.Invoke(this);
    }

    public override void StartAttack()
    {
        _isAttacking = true;
    }

    private void FixedUpdate()
    {
        if (!_isAttacking)
            return;

        transform.LookAt(_ctx.TargetTransform.Value.position);
        _rbMovement.Move(transform.forward, _moveSpeed);
    }
}
using System.Numerics;

public interface IExplosive
{
    public void Explode(Vector3 origin, float radius, float damage);
}
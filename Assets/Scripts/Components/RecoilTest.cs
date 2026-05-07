using UnityEngine;

public class RecoilTest : MonoBehaviour
{
    [SerializeField] private Transform _gunRecoilTransform;
    public void Test(GunConfig config) => Debug.Log(config.Stats.ToString());
}
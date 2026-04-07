using System.Runtime.InteropServices;
using UnityEngine;

public class GunBehaviourLoader : MonoBehaviour
{
    public void LoadBehaviour(GunType type, out GunBehaviour behaviour)
    {
        Debug.Log($"[GUN BEHAVIOUR LOADER] Loading Behaviour: {type} -");
        behaviour = null;
        switch (type)
        {
            case GunType.Semi_Automatic:
                behaviour = gameObject.AddComponent<SemiAutomaticBehaviour>();
                return;

            // case GunType.Burst:
            //     behaviour = gameObject.AddComponent<BurstBehaviour>();
            //     return;

            // case GunType.Automatic:
            //     behaviour = gameObject.AddComponent<AutomaticBehaviour>();
            //     return;
        }
    }
}
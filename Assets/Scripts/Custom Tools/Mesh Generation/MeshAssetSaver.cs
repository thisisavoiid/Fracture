using UnityEngine;
using System.IO;




#if UNITY_EDITOR
using UnityEditor;
#endif
public class MeshAssetSaver : MonoBehaviour
{
    public void SaveMesh(Mesh mesh, string path = null, string name = null)
    {
        if (mesh == null)
        {
            Debug.LogError("[MESH ASSET SAVER] The asset provided was null! -");
            return;
        }

#if UNITY_EDITOR
        Mesh assetInstance = Instantiate(mesh);

        if (name != null)
            assetInstance.name = name;

        string fullPath;

        if (path == null)
        {
            fullPath = $"Assets/{assetInstance.name}.asset";
        }
        else
        {
            fullPath = $"Assets/{path}/{assetInstance.name}.asset";
            
            if (!AssetDatabase.AssetPathExists($"Assets/{path}/"))
                Directory.CreateDirectory($"Assets/{path}/");
        }
    
        try
        {
            AssetDatabase.CreateAsset(
                assetInstance,
                fullPath
            );

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch
        {
            Debug.LogError("[MESH ASSET SAVER] Something went wrong while saving the mesh as an asset! -");
            return;
        }

        Debug.Log($"[MESH ASSET SAVER] Successfully saved asset at: {path} -");

#else
        Debug.LogWarning("[MESH ASSET SAVER] Asset saving can only be done in the inspector but not in build or play mode! -");

#endif
    }
}
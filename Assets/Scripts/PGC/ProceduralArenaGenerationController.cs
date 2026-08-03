using System.Collections.Generic;
using NaughtyAttributes;
using NaughtyAttributes.Test;
using UnityEngine;

public class ProceduralArenaGenerationController : MonoBehaviour
{
    [BoxGroup("Core References")]
    [SerializeField]
    [Expandable]
    private ArenaGenerationRulesAsset _rules;

    [BoxGroup("Core References")]
    [SerializeField]
    [Expandable]
    private ArenaGenerationConstraintsAsset _generationConstraints;

    [BoxGroup("Core References")]
    [SerializeField]
    [OnValueChanged("CheckForValidInstantiatorInstance")]
    private MonoBehaviour _instantiator;

    [BoxGroup("Grid Settings")]
    [SerializeField]
    private Vector2Int _arenaSize;

    [BoxGroup("Grid Settings")]
    [SerializeField]
    [Range(1f, 20f)]
    private float _cellSize;

    [BoxGroup("Grid Settings")]
    [SerializeField]
    private Vector3 _globalOffset = Vector3.zero;

    [BoxGroup("Noise Settings")]
    [SerializeField]
    [MinValue(0.001f)]
    private float _noiseScale;

    [BoxGroup("Noise Settings")]
    [SerializeField]
    private Vector2 _noiseOffset;

    [BoxGroup("Noise Settings")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _threshold;

    [BoxGroup("Seed Settings")]
    [SerializeField]
    private bool _useCustomSeed;

    [BoxGroup("Seed Settings")]
    [SerializeField]
    [ShowIf("_useCustomSeed")]
    private int _customSeed;

    [BoxGroup("Seed Settings")]
    [SerializeField]
    [ReadOnly]
    private int _currentSeed;

    [Button("Delete all generated assets")]
    public void Clear()
    {
        IArenaContentInstantiator instantiator = _instantiator as IArenaContentInstantiator;

        if (instantiator == null)
            return;

        instantiator.Clear();
    }

    [Button("Generate Arena")]
    public void Generate()
    {
        _currentSeed = _useCustomSeed ? _customSeed : SeedGenerator.GenerateSeed();

        IArenaLayoutGenerator generator = new WFCArenaLayoutGenerator(
            _rules,
            _noiseScale,
            _threshold,
            _noiseOffset
        );

        Debug.Log($"[PROCEDURAL ARENA GENERATION CONTROLLER] Requesting arena generation with seed: {_currentSeed}");

        ArenaCellData[,] layout = generator.Generate(_arenaSize, _currentSeed);

        IArenaContentInstantiator instantiator = _instantiator as IArenaContentInstantiator;

        if (instantiator == null)
            return;

        if (_generationConstraints == null)
        {
            Debug.LogWarning($"[PROCEDURAL ARENA GENERATION CONTROLLER] No generation constraint asset has been selected! -");
        }
        else
        {
            GenerationConstraintsApplicator constraintsApplicator = new GenerationConstraintsApplicator();
            constraintsApplicator.ApplyConstraints(layout, _generationConstraints.Constraints);
        }

        ArenaGenerationData data = new ArenaGenerationData(
            layout,
            _cellSize,
            _globalOffset
        );

        instantiator.Clear();
        instantiator.Build(data);
    }

    private void CheckForValidInstantiatorInstance()
    {
        if (!_instantiator.TryGetComponent(out IArenaContentInstantiator _))
        {
            Debug.LogWarning("[PROCEDURAL ARENA GENERATION CONTROLLER] Note that only classes that implement the 'IArenaContentInstantiator' interface can be selected as instantiator! -");
            _instantiator = null;
        }
    }

    private void OnDrawGizmos()
    {
        float halfWidth = (_arenaSize.x - 1) * _cellSize / 2f;
        float halfHeight = (_arenaSize.y - 1) * _cellSize / 2f;

        Vector3 offset = new Vector3(
            -halfWidth,
            0,
            -halfHeight
        );

        for (int x = 0; x < _arenaSize.x; x++)
        {
            for (int z = 0; z < _arenaSize.y; z++)
            {
                Vector3 pointDrawPosition = new Vector3(
                    transform.position.x + x * _cellSize + offset.x + _globalOffset.x,
                    transform.position.y,
                    transform.position.z + z * _cellSize + offset.z + _globalOffset.z
                );

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(pointDrawPosition, 0.25f * Vector3.one);

                Vector3 cubeDrawPosition = new Vector3(
                    transform.position.x + x * _cellSize + offset.x + _globalOffset.x,
                    transform.position.y + _globalOffset.y,
                    transform.position.z + z * _cellSize + offset.z + _globalOffset.z
                );

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(cubeDrawPosition, Vector3.one * _cellSize);

            }
        }
    }
}
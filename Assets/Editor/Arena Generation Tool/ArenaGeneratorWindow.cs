using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using ToolkitByJonathan;

public class ArenaGeneratorWindow : EditorWindow
{
    #region Constant Values
    private const string _windowTitle = "Arena Generator";
    private const float _minCellSize = 0.5f;
    private const float _maxCellSize = 20.0f;
    private const float _minThreshold = 0.0f;
    private const float _maxThreshold = 1.0f;
    private static readonly string[] _tabs = { "Settings", "Rules", "Prefabs", "Constraints", "Preview & Generation" };
    #endregion

    #region References
    private ArenaGenerationRulesAsset _generationRulesAsset;
    private TilePrefabAsset _tilePrefabAsset;
    private ArenaGenerationConstraintsAsset _generationConstraintAsset;
    private Object _instantiator = null;
    #endregion

    #region Grid Settings
    private Vector2Int _arenaSize = Vector2Int.zero;
    private float _cellSize = _minCellSize;
    private Vector3 _globalOffset = Vector3.zero;
    #endregion

    #region Noise Settings
    private float _noiseScale = 1.0f;
    private Vector2 _noiseOffset;
    private float _threshold = _minThreshold;
    #endregion

    #region Seed Settings
    private bool _useCustomSeed = false;
    private int _currentSeed = 0;
    private int _customSeed = 0;
    #endregion

    private Vector2 _globalScrollPos;
    private int _currentTab = 0;

    [MenuItem("Window/Arena Generation/Arena Generator")]
    private static void OnWindowOpened()
    {
        ArenaGeneratorWindow window = GetWindow<ArenaGeneratorWindow>(_windowTitle);
        window.Show();
    }

    private void OnGUI()
    {
        _globalScrollPos = EditorGUILayout.BeginScrollView(_globalScrollPos);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(24);
        EditorGUILayout.BeginVertical();

        GUILayout.Space(12);
        DrawTabSection();
        GUILayout.Space(10);
        DrawHelper.DrawSeperator();
        GUILayout.Space(12);

        switch (_currentTab)
        {
            case 0:
                DrawSettingsTab();
                break;

            case 1:
                DrawRulesTab();
                break;

            case 2:
                DrawPrefabsTab();
                break;

            case 3:
                DrawConstraintsTab();
                break;

            case 4:
                DrawPreviewTab();
                break;
        }

        GUILayout.Space(12);
        EditorGUILayout.EndVertical();

        GUILayout.Space(24);
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(12);

        EditorGUILayout.EndScrollView();
    }

    private void FetchAndSetAssets()
    {
        if (_tilePrefabAsset == null)
            _tilePrefabAsset = ScriptableObjectFetcher.FindFirstObjectOfType<TilePrefabAsset>();

        if (_generationRulesAsset == null)
            _generationRulesAsset = ScriptableObjectFetcher.FindFirstObjectOfType<ArenaGenerationRulesAsset>();

        if (_instantiator == null)
            _instantiator = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IArenaContentInstantiator>().FirstOrDefault() as Object;

        if (_generationConstraintAsset == null)
            _generationConstraintAsset = ScriptableObjectFetcher.FindFirstObjectOfType<ArenaGenerationConstraintsAsset>();
    }

    private void DrawPrefabsTab()
    {
        DrawPrefabsSection();
    }

    private void DrawConstraintsTab()
    {
        if (_generationConstraintAsset == null)
        {
            EditorGUILayout.HelpBox(
                "No generation constraints asset has been selected. Go to the Settings tab and select a constraints asset.",
                MessageType.Warning
            );

            return;
        }

        List<ArenaItemType> alreadyVisitedTypes = new();

        DrawHelper.DrawHeader($"Constraints ({_generationConstraintAsset.Constraints.Count} Constraint(s) assigned)");
        GUILayout.Space(10);

        for (int i = 0; i < _generationConstraintAsset.Constraints.Count; i++)
        {
            var constraintItem = _generationConstraintAsset.Constraints[i];

            GenerationConstraint constraint = _generationConstraintAsset.Constraints[i];

            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(6);

            string warningIcon = alreadyVisitedTypes.Contains(constraint.Type) ? "⚠️ " : "";
            string entryName = $"Constraint {i + 1}: {constraint.Type}";

            DrawHelper.DrawHeader(warningIcon + entryName);
            GUILayout.Space(16);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(2);

            ArenaItemType newType = (ArenaItemType)EditorGUILayout.EnumPopup(
                "Specified for Type",
                constraintItem.Type
            );

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();

            int newMinQuota = Mathf.Max(
                1,
                EditorGUILayout.IntField(
                "Mininum Quota Required",
                _generationConstraintAsset.Constraints[i].MinQuota
            )
            );

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(8);

            if (GUILayout.Button("- Delete Constraint", GUILayout.Width(200)))
            {
                Undo.RecordObject(_generationConstraintAsset, "Deleted Constraint Entry");
                _generationConstraintAsset.Constraints.RemoveAt(i);
                EditorUtility.SetDirty(_generationConstraintAsset);

                EditorGUILayout.EndVertical();
                break;
            }

            if (!alreadyVisitedTypes.Contains(constraint.Type))
            {
                alreadyVisitedTypes.Add(constraint.Type);
            }
            else
            {  
                GUILayout.Space(6);

                EditorGUILayout.HelpBox(
                    $"There is more than one constraint entry assigned for {constraint.Type.ToString()}!",
                    MessageType.Warning
                );
            }

            EditorGUILayout.EndVertical();

            if (newType != constraintItem.Type || newMinQuota != constraintItem.MinQuota)
            {
                Undo.RecordObject(_generationConstraintAsset, "Modified Constraint Asset");
                _generationConstraintAsset.Constraints[i] = new GenerationConstraint
                {
                    MinQuota = newMinQuota,
                    Type = newType
                };
                EditorUtility.SetDirty(_generationConstraintAsset);
            }
        }

        GUILayout.Space(8);

        if (GUILayout.Button("+ Create New Constraint"))
        {
            Undo.RecordObject(_generationConstraintAsset, "Created New Constraint Entry");
            GenerationConstraint newConstraint = new GenerationConstraint()
            {
                MinQuota = 0,
                Type = ArenaItemType.None
            };

            _generationConstraintAsset.Constraints.Add(newConstraint);
            EditorUtility.SetDirty(_generationConstraintAsset);
        }

    }

    private void DrawPrefabsSection()
    {
        if (_tilePrefabAsset == null)
        {
            EditorGUILayout.HelpBox(
                "No tile prefab asset has been selected. Go to the Settings tab and select a tile prefab asset.",
                MessageType.Warning
            );

            return;
        }

        List<BetterKeyValuePair<ArenaItemType, GameObject>> tilePrefabMap = _tilePrefabAsset.TilePrefabs;
        List<ArenaItemType> alreadyVisitedTypes = new();

        DrawHelper.DrawHeader($"Prefabs ({tilePrefabMap.Count} Prefab(s) assigned)");
        GUILayout.Space(10);

        for (int i = 0; i < tilePrefabMap.Count; i++)
        {
            var item = tilePrefabMap[i];

            Texture2D assetPreview = AssetPreview.GetAssetPreview(item.Value);

            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(6);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(2);

            EditorGUILayout.BeginVertical(GUILayout.Width(85));

            string warningIcon = alreadyVisitedTypes.Contains(item.Key) ? "⚠️ " : "";
            string entryName = item.Value != null ? item.Value.name : "New Entry";

            DrawHelper.DrawHeader(warningIcon + entryName);

            if (item.Value != null)
            {
                GUILayout.Space(6);
                GUILayout.Label(assetPreview, GUILayout.Width(75), GUILayout.Height(75));
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(16);

            EditorGUILayout.BeginVertical();

            GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Asset",
                item.Value,
                typeof(GameObject),
                false
            );

            GUILayout.Space(4);

            ArenaItemType newType = (ArenaItemType)EditorGUILayout.EnumPopup(
                "Specified For Type",
                item.Key
            );

            if (!alreadyVisitedTypes.Contains(newType))
            {
                alreadyVisitedTypes.Add(newType);
            }
            else
            {
                GUILayout.Space(6);
                EditorGUILayout.HelpBox(
                    $"There is more than one prefab assigned for {newType.ToString()}!",
                    MessageType.Warning
                );
            }

            if (newPrefab != item.Value || newType != item.Key)
            {
                Undo.RecordObject(_tilePrefabAsset, "Changed Tile Prefab");
                tilePrefabMap[i] = new BetterKeyValuePair<ArenaItemType, GameObject>(newType, newPrefab);
                EditorUtility.SetDirty(_tilePrefabAsset);
            }

            GUILayout.Space(8);

            if (GUILayout.Button("- Remove Entry", GUILayout.Width(120)))
            {
                Undo.RecordObject(_tilePrefabAsset, "Removed Tile Prefab");
                tilePrefabMap.RemoveAt(i);
                EditorUtility.SetDirty(_tilePrefabAsset);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(6);
                EditorGUILayout.EndVertical();
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(6);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
        }

        GUILayout.Space(6);

        if (GUILayout.Button("+ Create New"))
        {
            Undo.RecordObject(_tilePrefabAsset, "Added A New Tile Prefab Entry");
            tilePrefabMap.Add(new BetterKeyValuePair<ArenaItemType, GameObject>(ArenaItemType.None, null));
            EditorUtility.SetDirty(_tilePrefabAsset);
        }
    }

    private void DrawRulesTab()
    {
        DrawRuleSection();
    }

    private void DrawRuleSection()
    {
        if (_generationRulesAsset == null)
        {
            EditorGUILayout.HelpBox(
                "No generation rules asset has been selected. Go to the Settings tab and select a rule asset.",
                MessageType.Warning
            );

            return;
        }

        List<TypeRule> typeRules = _generationRulesAsset.Rules;
        List<ArenaItemType> visitedItemTypes = new();

        DrawHelper.DrawHeader($"Generation Rules ({typeRules.Count} Rule(s) assigned)");
        GUILayout.Space(10);

        for (int i = 0; i < typeRules.Count; i++)
        {
            TypeRule rule = typeRules[i];

            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(6);

            string warningIcon = visitedItemTypes.Contains(rule.Type) ? "⚠️ " : "";
            string entryName = $"Rule {i + 1}: {rule.Type}";

            DrawHelper.DrawHeader(warningIcon + entryName);

            GUILayout.Space(8);

            DrawRuleElement(ref rule);
            typeRules[i] = rule;

            if (!visitedItemTypes.Contains(rule.Type))
            {
                visitedItemTypes.Add(rule.Type);
            }
            else
            {
                GUILayout.Space(6);
                EditorGUILayout.HelpBox(
                    $"There is more than one rule set up for type {rule.Type.ToString()}!",
                    MessageType.Warning
                );
            }

            GUILayout.Space(10);

            if (GUILayout.Button("- Delete Rule", GUILayout.Width(120)))
            {
                Undo.RecordObject(_generationRulesAsset, "Removed A Rule Asset Entry");
                _generationRulesAsset.RemoveRule(typeRules[i]);
                EditorUtility.SetDirty(_generationRulesAsset);
                GUILayout.Space(6);
                EditorGUILayout.EndVertical();
                break;
            }

            GUILayout.Space(6);
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);
        }

        GUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("+ Create New Rule"))
        {
            Undo.RecordObject(_generationRulesAsset, "Added A New Rule Asset");
            _generationRulesAsset.Rules.Add(new TypeRule()
            {
                Weight = 0.5f,
                Type = ArenaItemType.None,
                ExcludesFromNeighbors = new()
            });
        }

        GUILayout.Space(8);

        if (GUILayout.Button("Reset Ruleset"))
        {
            Undo.RecordObject(_generationRulesAsset, "Cleared Rule Asset");
            _generationRulesAsset.Rules.Clear();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);
        DrawHelper.DrawSeperator();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_generationRulesAsset);
        }
    }

    private void DrawRuleElement(ref TypeRule typeRule)
    {
        typeRule.Type = (ArenaItemType)EditorGUILayout.EnumPopup(
            "Type",
            typeRule.Type
        );

        typeRule.Weight = EditorGUILayout.Slider(
            "Weight",
            typeRule.Weight,
            0f,
            1f
        );

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Excludes From Neighbors");
        GUILayout.Space(2);

        EditorGUI.indentLevel++;

        for (int i = 0; i < typeRule.ExcludesFromNeighbors.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            typeRule.ExcludesFromNeighbors[i] = (ArenaItemType)EditorGUILayout.EnumPopup(typeRule.ExcludesFromNeighbors[i]);

            GUILayout.Space(4);

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                Undo.RecordObject(_generationRulesAsset, "Modified A Rule Asset Entry");
                typeRule.ExcludesFromNeighbors.RemoveAt(i);
                EditorGUILayout.EndHorizontal();
                break;
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);
        }

        GUILayout.Space(4);

        if (GUILayout.Button("+ Add Exclude", GUILayout.Width(120)))
        {
            Undo.RecordObject(_generationRulesAsset, "Modified A Rule Asset Entry");
            typeRule.ExcludesFromNeighbors.Add(ArenaItemType.None);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawPreviewTab()
    {
        GUILayout.Space(12);
        DrawGenerationButtonSection();
    }

    private void DrawSettingsTab()
    {
        DrawReferenceSection();
        GUILayout.Space(14);
        DrawHelper.DrawSeperator();
        GUILayout.Space(14);

        DrawGridSettingsSection();
        GUILayout.Space(14);
        DrawHelper.DrawSeperator();
        GUILayout.Space(14);

        DrawNoiseSettingsSection();
        GUILayout.Space(14);
        DrawHelper.DrawSeperator();
        GUILayout.Space(14);

        DrawSeedSettingsSection();
        GUILayout.Space(14);
        DrawHelper.DrawSeperator();
    }

    private void DrawTabSection()
    {
        _currentTab = GUILayout.Toolbar(
            _currentTab,
            _tabs,
            GUILayout.Height(24)
        );
    }

    private void DrawGenerationButtonSection()
    {
        EditorGUILayout.BeginHorizontal();

        bool isInvalid = _instantiator == null || _tilePrefabAsset == null || _generationRulesAsset == null;

        using (new EditorGUI.DisabledScope(isInvalid))
        {
            if (GUILayout.Button("Delete all generated assets", GUILayout.Height(28)))
            {
                (_instantiator as IArenaContentInstantiator).Clear();
            }

            GUILayout.Space(12);

            if (GUILayout.Button("Generate Arena", GUILayout.Height(28)))
            {
                _currentSeed = _useCustomSeed ? _customSeed : SeedGenerator.GenerateSeed();

                (_instantiator as IArenaContentInstantiator).Clear();

                WFCArenaLayoutGenerator layoutGenerator = new WFCArenaLayoutGenerator(
                    _generationRulesAsset,
                    _noiseScale,
                    _threshold,
                    _noiseOffset
                );

                ArenaCellData[,] layout = layoutGenerator.Generate(_arenaSize, _currentSeed);

                ArenaGenerationData generationData = new ArenaGenerationData(
                    layout,
                    _cellSize,
                    _globalOffset
                );

                (_instantiator as IArenaContentInstantiator).Build(generationData);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawSeedSettingsSection()
    {
        DrawHelper.DrawHeader("Seed Settings");
        GUILayout.Space(8);

        _useCustomSeed = EditorGUILayout.Toggle(
            "Use Custom Seed",
            _useCustomSeed
        );

        GUILayout.Space(4);

        EditorGUILayout.LabelField(
            "Current Seed",
            _currentSeed.ToString()
        );

        if (_useCustomSeed)
        {
            GUILayout.Space(4);

            EditorGUI.indentLevel++;
            _customSeed = EditorGUILayout.IntField(
                "Custom Seed",
                _customSeed
            );
            EditorGUI.indentLevel--;
        }
    }

    private void DrawNoiseSettingsSection()
    {
        DrawHelper.DrawHeader("Noise Settings");
        GUILayout.Space(8);

        _noiseScale = EditorGUILayout.FloatField(
            "Noise Scale",
            _noiseScale
        );

        GUILayout.Space(4);

        _noiseOffset = EditorGUILayout.Vector2Field(
            "Noise Offset",
            _noiseOffset
        );

        GUILayout.Space(4);

        _threshold = EditorGUILayout.Slider(
            "Threshold",
            _threshold,
            _minThreshold,
            _maxThreshold
        );
    }

    private void DrawGridSettingsSection()
    {
        DrawHelper.DrawHeader("Grid Settings");
        GUILayout.Space(8);

        _arenaSize = EditorGUILayout.Vector2IntField(
            "Arena Size",
            _arenaSize
        );

        GUILayout.Space(4);

        _cellSize = EditorGUILayout.Slider(
            "Cell Size",
            _cellSize,
            _minCellSize,
            _maxCellSize
        );

        GUILayout.Space(4);

        _globalOffset = EditorGUILayout.Vector3Field(
            "Global Offset",
            _globalOffset
        );
    }

    private void DrawReferenceSection()
    {
        DrawHelper.DrawHeader("References");
        GUILayout.Space(8);

        _generationRulesAsset = (ArenaGenerationRulesAsset)EditorGUILayout.ObjectField(
            "Rules Asset",
            _generationRulesAsset,
            typeof(ArenaGenerationRulesAsset),
            false
        );

        if (_generationRulesAsset == null)
        {
            GUILayout.Space(3);
            EditorGUILayout.HelpBox("Please select a generation rules asset or click the button below to fetch the required assets automatically.", MessageType.Warning);
        }

        GUILayout.Space(6);

        _tilePrefabAsset = (TilePrefabAsset)EditorGUILayout.ObjectField(
            "Tile Prefab Asset",
            _tilePrefabAsset,
            typeof(TilePrefabAsset),
            false
        );

        if (_tilePrefabAsset == null)
        {
            GUILayout.Space(3);
            EditorGUILayout.HelpBox("Please select a tile prefab asset or click the button below to fetch the required assets automatically.", MessageType.Warning);
        }

        GUILayout.Space(6);

        _generationConstraintAsset = (ArenaGenerationConstraintsAsset)EditorGUILayout.ObjectField(
            "Generation Constraints Asset",
            _generationConstraintAsset,
            typeof(TilePrefabAsset),
            false
        );

        GUILayout.Space(6);

        _instantiator = EditorGUILayout.ObjectField(
            "Instantiator",
            _instantiator,
            typeof(IArenaContentInstantiator),
            true
        );

        if (_instantiator == null)
        {
            GUILayout.Space(3);
            EditorGUILayout.HelpBox("Please select a content instantiator game object or click the button below to fetch the required assets automatically.", MessageType.Warning);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Fetch assets"))
        {
            FetchAndSetAssets();
        }
    }
}
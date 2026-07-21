using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ArenaGeneratorWindow : EditorWindow
{
    #region Constant Values
    private const string _windowTitle = "Arena Generator";
    private const float _minCellSize = 0.5f;
    private const float _maxCellSize = 20.0f;
    private const float _minThreshold = 0.0f;
    private const float _maxThreshold = 1.0f;
    private static readonly string[] _tabs = { "Settings", "Rules", "Prefabs", "Preview" };
    #endregion

    #region References
    private ArenaGenerationRulesAsset _generationRulesAsset;
    private TilePrefabAsset _tilePrefabAsset;
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
        GUILayout.Space(20);
        EditorGUILayout.BeginVertical(GUILayout.MaxWidth(600));

        GUILayout.Space(8);
        DrawTabSection();
        GUILayout.Space(4);
        DrawHelper.DrawSeperator();
        GUILayout.Space(8);

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
                DrawPreviewTab();
                break;
        }

        GUILayout.Space(12);
        DrawGenerationButtonSection();

        GUILayout.Space(8);
        DrawHelper.DrawSeperator();
        GUILayout.Space(8);

        EditorGUILayout.EndVertical();

        GUILayout.Space(20);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndScrollView();
    }

    private void DrawPrefabsTab()
    {
        DrawPrefabsSection();
    }

    private void DrawPrefabsSection()
    {
        List<BetterKeyValuePair<ArenaItemType, GameObject>> tilePrefabMap = _tilePrefabAsset.TilePrefabs;
        List<ArenaItemType> alreadyVisitedTypes = new();

        DrawHelper.DrawHeader($"Prefabs ({tilePrefabMap.Count} Prefab(s) assigned)");
        GUILayout.Space(6);

        for (int i = 0; i < tilePrefabMap.Count; i++)
        {
            var item = tilePrefabMap[i];

            Texture2D assetPreview = AssetPreview.GetAssetPreview(item.Value);

            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(4);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(85));

            DrawHelper.DrawHeader(item.Value != null ? item.Value.name : "New Entry");

            if (item.Value != null)
            {
                GUILayout.Space(4);
                GUILayout.Label(assetPreview, GUILayout.Width(75), GUILayout.Height(75));
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(12);

            EditorGUILayout.BeginVertical();

            GameObject newPrefab = (GameObject)EditorGUILayout.ObjectField(
                "Asset",
                item.Value,
                typeof(GameObject),
                false
            );

            GUILayout.Space(2);

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
                GUILayout.Space(4);
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

            GUILayout.Space(6);

            if (GUILayout.Button("- Remove Entry", GUILayout.Width(120)))
            {
                Undo.RecordObject(_tilePrefabAsset, "Removed Tile Prefab");
                tilePrefabMap.RemoveAt(i);
                EditorUtility.SetDirty(_tilePrefabAsset);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(4);
                EditorGUILayout.EndVertical();
                break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(4);
            EditorGUILayout.EndVertical();

            GUILayout.Space(6);
        }

        GUILayout.Space(4);

        if (GUILayout.Button("+ Create New"))
        {
            Undo.RecordObject(_tilePrefabAsset, "Added A New Tile Prefab Entry");
            tilePrefabMap.Add(new BetterKeyValuePair<ArenaItemType, GameObject>(ArenaItemType.None, null));
            EditorUtility.SetDirty(_tilePrefabAsset);
        }
    }

    private void DrawRulesTab()
    {
        DrawReferenceSection();
        GUILayout.Space(8);
        DrawHelper.DrawSeperator();
        GUILayout.Space(8);

        DrawRuleSection();
    }

    private void DrawRuleSection()
    {
        List<TypeRule> typeRules = _generationRulesAsset.Rules;
        List<ArenaItemType> visitedItemTypes = new();

        DrawHelper.DrawHeader($"Generation Rules ({typeRules.Count} Rule(s) assigned)");
        GUILayout.Space(6);

        for (int i = 0; i < typeRules.Count; i++)
        {
            TypeRule rule = typeRules[i];

            EditorGUILayout.BeginVertical("box");
            GUILayout.Space(4);

            DrawHelper.DrawHeader($"Rule {i + 1}: {rule.Type}");
            GUILayout.Space(4);

            DrawRuleElement(ref rule);
            typeRules[i] = rule;

            if (!visitedItemTypes.Contains(rule.Type))
            {
                visitedItemTypes.Add(rule.Type);
            }
            else
            {
                GUILayout.Space(4);
                EditorGUILayout.HelpBox(
                    $"There is more than one rule set up for type {rule.Type.ToString()}!",
                    MessageType.Warning
                );
            }

            GUILayout.Space(6);

            if (GUILayout.Button("- Delete Rule", GUILayout.Width(120)))
            {
                Undo.RecordObject(_generationRulesAsset, "Removed A Rule Asset Entry");
                _generationRulesAsset.RemoveRule(typeRules[i]);
                EditorUtility.SetDirty(_generationRulesAsset);
                GUILayout.Space(4);
                EditorGUILayout.EndVertical();
                break;
            }

            GUILayout.Space(4);
            EditorGUILayout.EndVertical();

            GUILayout.Space(6);
        }

        GUILayout.Space(4);

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

        GUILayout.Space(6);

        if (GUILayout.Button("Reset Ruleset"))
        {
            Undo.RecordObject(_generationRulesAsset, "Cleared Rule Asset");
            _generationRulesAsset.Rules.Clear();
        }

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(6);
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

        GUILayout.Space(6);
        EditorGUILayout.LabelField("Excludes From Neighbors");

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
            GUILayout.Space(2);
        }

        GUILayout.Space(2);

        if (GUILayout.Button("+ Add Exclude", GUILayout.Width(120)))
        {
            Undo.RecordObject(_generationRulesAsset, "Modified A Rule Asset Entry");
            typeRule.ExcludesFromNeighbors.Add(ArenaItemType.None);
        }

        EditorGUI.indentLevel--;
    }

    private void DrawPreviewTab() { }

    private void DrawSettingsTab()
    {
        DrawGridSettingsSection();
        GUILayout.Space(10);
        DrawHelper.DrawSeperator();
        GUILayout.Space(10);

        DrawNoiseSettingsSection();
        GUILayout.Space(10);
        DrawHelper.DrawSeperator();
        GUILayout.Space(10);

        DrawSeedSettingsSection();
        GUILayout.Space(10);
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

        if (GUILayout.Button("Delete all generated assets", GUILayout.Height(28)))
        {

        }

        GUILayout.Space(8);

        if (GUILayout.Button("Generate Arena", GUILayout.Height(28)))
        {

        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawSeedSettingsSection()
    {
        DrawHelper.DrawHeader("Seed Settings");
        GUILayout.Space(4);

        _useCustomSeed = EditorGUILayout.Toggle(
            "Use Custom Seed",
            _useCustomSeed
        );

        GUILayout.Space(2);

        EditorGUILayout.LabelField(
            "Current Seed",
            _currentSeed.ToString()
        );

        if (_useCustomSeed)
        {
            GUILayout.Space(2);

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
        GUILayout.Space(4);

        _noiseScale = EditorGUILayout.FloatField(
            "Noise Scale",
            _noiseScale
        );

        GUILayout.Space(2);

        _noiseOffset = EditorGUILayout.Vector2Field(
            "Noise Offset",
            _noiseOffset
        );

        GUILayout.Space(2);

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
        GUILayout.Space(4);

        _arenaSize = EditorGUILayout.Vector2IntField(
            "Arena Size",
            _arenaSize
        );

        GUILayout.Space(2);

        _cellSize = EditorGUILayout.Slider(
            "Cell Size",
            _cellSize,
            _minCellSize,
            _maxCellSize
        );

        GUILayout.Space(2);

        _globalOffset = EditorGUILayout.Vector3Field(
            "Global Offset",
            _globalOffset
        );
    }

    private void DrawReferenceSection()
    {
        DrawHelper.DrawHeader("References");
        GUILayout.Space(4);

        _generationRulesAsset = (ArenaGenerationRulesAsset)EditorGUILayout.ObjectField(
            "Rules Asset",
            _generationRulesAsset,
            typeof(ArenaGenerationRulesAsset),
            false
        );

        GUILayout.Space(2);

        _tilePrefabAsset = (TilePrefabAsset)EditorGUILayout.ObjectField(
            "Tile Prefab Asset",
            _tilePrefabAsset,
            typeof(TilePrefabAsset),
            false
        );

        GUILayout.Space(2);

        _instantiator = EditorGUILayout.ObjectField(
            "Instantiator",
            _instantiator,
            typeof(IArenaContentInstantiator),
            true
        );
    }
}

using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using System.Reflection;

public class ModularBuilding : EditorWindow
{
    private static bool _snapAlwaysOn;
    private static bool _followSelectionX;
    private static bool _followSelectionY;
    private static bool _unselectedGridHiding;
    private static float _gridSize = 1f;
    private static bool _showCustomGrid = true;
    private static bool _showYGrid = false;
    private static float _ySnapHeight = 1f;
    private static bool _settingsLoaded;
    private static Color _hGridColor;
    private static Color _vGridColor;
    private static int _duplicateNum;

    private const string SnapPrefKey = "ModularBuilding_SnapAlwaysOn";
    private const string GridFollowX = "ModularBuilding_GridFollowX";
    private const string GridFollowY = "ModularBuilding_GridFollowY";
    private const string AutoGridHiding = "ModularBuilding_AutoGridHiding";
    private const string GridSizePrefKey = "ModularBuilding_GridSize";
    private const string ShowGridPrefKey = "ModularBuilding_ShowGrid";
    private const string ShowYGridPrefKey = "ModularBuilding_ShowYGrid";
    private const string YSnapHeightPrefKey = "ModularBuilding_YSnapHeight";
    private const string XGridColorPrefKey = "XCustomGridColor";
    private const string YGridColorPrefKey = "YCustomGridColor";
    public const string ShortcutId = "Tools/Grid Snap";

    private static Vector3 _selectedObject;

    [MenuItem("Tools/Grid Snap Tool")]
    private static void OpenWindow() => GetWindow<ModularBuilding>("Grid Snap");

    [Shortcut(ShortcutId, KeyCode.Space)]
    public static void ShortcutSnap() => _snapAlwaysOn = !_snapAlwaysOn;

    private void OnEnable()
    {
        LoadSettings();
        SceneView.duringSceneGui += OnSceneGUI;
        HideUnityGrid(true);
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        HideUnityGrid(false);
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Snap Tool", EditorStyles.boldLabel);
        _gridSize = EditorGUILayout.FloatField("Grid Size (XZ)", _gridSize);
        _gridSize = Mathf.Max(0.0001f, _gridSize);

        _snapAlwaysOn = EditorGUILayout.Toggle("Snap Always On", _snapAlwaysOn);
        _duplicateNum = EditorGUILayout.IntField("Duplicate Number", Mathf.Max(1, _duplicateNum));
        _unselectedGridHiding = EditorGUILayout.Toggle("Unselected Grid Hiding", _unselectedGridHiding);
        if (GUILayout.Button("Duplicate"))
            DuplicateItems();

        bool prevShowGrid = _showCustomGrid;
        _showCustomGrid = EditorGUILayout.Toggle("Show XZ Grid", _showCustomGrid);

        _showYGrid = EditorGUILayout.Toggle("Show Y Grid", _showYGrid);
        _followSelectionX = EditorGUILayout.Toggle("Follow Selection X", _followSelectionX);
        _followSelectionY = EditorGUILayout.Toggle("Follow Selection Y", _followSelectionY);
        _ySnapHeight = EditorGUILayout.FloatField("Y Snap Height", _ySnapHeight);
        _ySnapHeight = Mathf.Max(0.0001f, _ySnapHeight);

        _hGridColor = EditorGUILayout.ColorField("H Grid Color", _hGridColor);
        _vGridColor = EditorGUILayout.ColorField("V Grid Color", _vGridColor);

        GUILayout.Space(8);

        if (GUILayout.Button("Reset to Default"))
        {
            _gridSize = 1f;
            _ySnapHeight = 1f;
            _snapAlwaysOn = false;
            _unselectedGridHiding = true;
            _showCustomGrid = true;
            _showYGrid = false;
            _followSelectionX = false;
            _followSelectionY = false;
            HideUnityGrid(_showCustomGrid);
        }

        SaveSettings();
        SceneView.RepaintAll();
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (Selection.activeGameObject == null)
        {
            _selectedObject = Vector3.zero;
            if (_unselectedGridHiding)
                return;
        }
        else
        {
            Vector3 selPos = Selection.activeTransform.localPosition;

            if (_followSelectionX && !_followSelectionY)
                _selectedObject = new Vector3(_selectedObject.x, selPos.y, _selectedObject.z);
            else if (_followSelectionY && !_followSelectionX)
                _selectedObject = new Vector3(selPos.x, _selectedObject.y, selPos.z);
            else if (_followSelectionX && _followSelectionY)
                _selectedObject = selPos;
            else
                _selectedObject = Vector3.zero;
        }

        if (_showCustomGrid)
            DrawXZGrid(sceneView);

        if (_showYGrid && Selection.activeGameObject != null)
            DrawYGrid(sceneView);

        if (!_snapAlwaysOn || Selection.transforms.Length == 0)
            return;

        foreach (Transform t in Selection.transforms)
        {
            if (t.hasChanged)
            {
                Undo.RecordObject(t, "Snap To Grid (Auto)");
                Vector3 pos = t.position;
                pos.x = Mathf.Round(pos.x / _gridSize) * _gridSize;
                pos.z = Mathf.Round(pos.z / _gridSize) * _gridSize;
                pos.y = Mathf.Round(pos.y / _ySnapHeight) * _ySnapHeight;
                t.position = pos;
                t.hasChanged = false;
            }
        }
    }
    
    private static void DuplicateItems()
    {
        if (Selection.transforms.Length == 0)
        {
            return;
        }

        foreach (Transform original in Selection.transforms)
        {
            Vector3 basePos = original.position;

            for (int i = 1; i <= _duplicateNum; i++)
            {
                GameObject newObj = Object.Instantiate(original.gameObject, original.parent);
                Undo.RegisterCreatedObjectUndo(newObj, "Duplicate Along X Grid");

                Vector3 newPos = basePos + Vector3.right * (i * _gridSize);
                newPos.x = Mathf.Round(newPos.x / _gridSize) * _gridSize;
                newPos.y = Mathf.Round(newPos.y / _ySnapHeight) * _ySnapHeight;
                newPos.z = Mathf.Round(newPos.z / _gridSize) * _gridSize;

                newObj.transform.position = newPos;
            }
        }
        SceneView.RepaintAll();
    }

    private static void DrawXZGrid(SceneView sceneView)
    {
        Handles.color = _hGridColor;
        int lines = 50;
        float extent = lines * _gridSize;

        for (int i = -lines; i <= lines; i++)
        {
            float p = i * _gridSize;
            Handles.DrawLine(new Vector3(p, _selectedObject.y, -extent), new Vector3(p, _selectedObject.y, extent));
            Handles.DrawLine(new Vector3(-extent, _selectedObject.y, p), new Vector3(extent, _selectedObject.y, p));
        }
    }

    private static void DrawYGrid(SceneView sceneView)
    {
        Handles.color = _vGridColor;
        int lines = 50;
        float horizontalExtent = lines * _gridSize;
        float verticalExtent = lines * _ySnapHeight;

        for (int i = -lines; i <= lines; i++)
        {
            float x = i * _gridSize;
            Handles.DrawLine(
                new Vector3(x, -verticalExtent, _selectedObject.z),
                new Vector3(x, verticalExtent, _selectedObject.z)
            );
        }

        for (int j = -lines; j <= lines; j++)
        {
            float y = j * _ySnapHeight;
            Handles.DrawLine(
                new Vector3(-horizontalExtent, y, _selectedObject.z),
                new Vector3(horizontalExtent, y, _selectedObject.z)
            );
        }
    }

    private static void LoadSettings()
    {
        if (_settingsLoaded) return;

        _snapAlwaysOn = EditorPrefs.GetBool(SnapPrefKey, false);
        _followSelectionX = EditorPrefs.GetBool(GridFollowX, false);
        _followSelectionY = EditorPrefs.GetBool(GridFollowY, false);
        _unselectedGridHiding = EditorPrefs.GetBool(AutoGridHiding, true);
        _gridSize = EditorPrefs.GetFloat(GridSizePrefKey, 1f);
        _ySnapHeight = EditorPrefs.GetFloat(YSnapHeightPrefKey, 1f);
        _showCustomGrid = EditorPrefs.GetBool(ShowGridPrefKey, true);
        _showYGrid = EditorPrefs.GetBool(ShowYGridPrefKey, false);
        _settingsLoaded = true;

        if (ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(XGridColorPrefKey, "4D4D4DFF"),
                out var hSavedColor))
            _hGridColor = hSavedColor;
        else
            _hGridColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        if (ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(YGridColorPrefKey, "4D4D4DFF"),
                out var vSavedColor))
            _vGridColor = vSavedColor;
        else
            _vGridColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    }

    private static void SaveSettings()
    {
        EditorPrefs.SetBool(SnapPrefKey, _snapAlwaysOn);
        EditorPrefs.SetBool(GridFollowX, _followSelectionX);
        EditorPrefs.SetBool(GridFollowY, _followSelectionY);
        EditorPrefs.SetBool(AutoGridHiding, _unselectedGridHiding);
        EditorPrefs.SetFloat(GridSizePrefKey, _gridSize);
        EditorPrefs.SetFloat(YSnapHeightPrefKey, _ySnapHeight);
        EditorPrefs.SetBool(ShowGridPrefKey, _showCustomGrid);
        EditorPrefs.SetBool(ShowYGridPrefKey, _showYGrid);
        EditorPrefs.SetString(XGridColorPrefKey, ColorUtility.ToHtmlStringRGBA(_hGridColor));
        EditorPrefs.SetString(YGridColorPrefKey, ColorUtility.ToHtmlStringRGBA(_vGridColor));
    }

    private static void HideUnityGrid(bool hide)
    {
        System.Type sceneViewType = typeof(SceneView);
        PropertyInfo showGridProp =
            sceneViewType.GetProperty("showGrid", BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (SceneView view in SceneView.sceneViews)
        {
            if (showGridProp != null)
            {
                showGridProp.SetValue(view, !hide);
            }
            else
            {
                System.Type gridType = typeof(SceneView).Assembly.GetType("UnityEditor.SceneViewGrid");
                FieldInfo gridField = sceneViewType.GetField("m_Grid", BindingFlags.Instance | BindingFlags.NonPublic);
                object gridInstance = gridField != null ? gridField.GetValue(view) : null;
                PropertyInfo gridProp = gridType?.GetProperty("showGrid",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (gridProp != null && gridInstance != null)
                    gridProp.SetValue(gridInstance, !hide);
            }

            view.Repaint();
        }
    }
}
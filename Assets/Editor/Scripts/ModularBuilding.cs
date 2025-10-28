using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using System.Reflection;

public class ModularBuilding : EditorWindow
{
    #region Variables

    private static bool _snapAlwaysOn;
    private static bool _followSelectionX;
    private static bool _followSelectionY;
    private static bool _unselectedGridHiding;
    private static float _gridSize;
    private static bool _showCustomGrid;
    private static bool _showYGrid;
    private static float _ySnapHeight;
    private static bool _settingsLoaded;
    private static Color _hGridColor;
    private static Color _vGridColor;
    private static Vector3Int _duplicateNum;
    private static Vector3 _selectedObject;
    private static AxisDirection _xDirection;
    private static AxisDirection _yDirection;
    private static AxisDirection _zDirection;
    private static bool _showDuplicateDirections;
    private static bool _showGridParam;
    private static bool _buildingParam;
    private static int _linesNumber;

    #endregion

    #region Duplication Axis And Mode

    public enum BuildMode
    {
        Normal,
        Stair
    }

    private static BuildMode _buildMode = BuildMode.Normal;

    [System.Flags]
    private enum AxisDirection
    {
        Positive = 1 << 0,
        Negative = 1 << 1,
        Both = Positive | Negative
    }

    #endregion

    #region SaveKey

    private const string ModeKey = "GridDuplicate_Mode";
    private const string SnapPrefKey = "ModularBuilding_SnapAlwaysOn";
    private const string GridFollowX = "ModularBuilding_GridFollowX";
    private const string GridFollowY = "ModularBuilding_GridFollowY";
    private const string DuplicationNum = "ModularBuilding_DuplicationNum";
    private const string AutoGridHiding = "ModularBuilding_AutoGridHiding";
    private const string GridSizePrefKey = "ModularBuilding_GridSize";
    private const string ShowGridPrefKey = "ModularBuilding_ShowGrid";
    private const string ShowYGridPrefKey = "ModularBuilding_ShowYGrid";
    private const string YSnapHeightPrefKey = "ModularBuilding_YSnapHeight";
    private const string XGridColorPrefKey = "XCustomGridColor";
    private const string YGridColorPrefKey = "YCustomGridColor";
    public const string ShortcutId = "Tools/Grid Snap";
    private const string XDirectionKey = "ModularBuilding_XDirection";
    private const string YDirectionKey = "ModularBuilding_YDirection";
    private const string ZDirectionKey = "ModularBuilding_ZDirection";
    private const string LinesNumber = "ModularBuilding_LinesNumber";

    #endregion

    #region Open Window

    [MenuItem("Tools/Grid Snap Tool")]
    private static void OpenWindow() => GetWindow<ModularBuilding>("Grid Snap");

    #endregion

    #region ShortCuts

    [Shortcut(ShortcutId, KeyCode.Space)]
    public static void ShortcutSnap() => _snapAlwaysOn = !_snapAlwaysOn;

    #endregion

    #region Grid On/Off

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

    #endregion

    private void OnGUI()
    {
        _showGridParam = EditorGUILayout.Foldout(_showGridParam, "Grid Parameters", true, EditorStyles.foldoutHeader);
        if (_showGridParam)
        {
            EditorGUI.indentLevel++;
            _linesNumber = EditorGUILayout.IntField("Number of Grid Lines", _linesNumber, GUILayout.Width(250));

            GUILayout.Space(10);

            _gridSize = EditorGUILayout.FloatField("Grid Size (XZ)", _gridSize, GUILayout.Width(250));
            _gridSize = Mathf.Max(0.0001f, _gridSize);
            _ySnapHeight = EditorGUILayout.FloatField("Grid Size (Y)", _ySnapHeight, GUILayout.Width(250));
            _ySnapHeight = Mathf.Max(0.0001f, _ySnapHeight);

            GUILayout.Space(10);

            _showCustomGrid = EditorGUILayout.Toggle("Show XZ Grid", _showCustomGrid, GUILayout.Width(250));
            _showYGrid = EditorGUILayout.Toggle("Show Y Grid", _showYGrid, GUILayout.Width(250));

            GUILayout.Space(10);

            _hGridColor = EditorGUILayout.ColorField("H Grid Color", _hGridColor, GUILayout.Width(250));
            _vGridColor = EditorGUILayout.ColorField("V Grid Color", _vGridColor, GUILayout.Width(250));

            GUILayout.Space(10);

            _followSelectionX = EditorGUILayout.Toggle("Follow Selection X", _followSelectionX);
            _followSelectionY = EditorGUILayout.Toggle("Follow Selection Y", _followSelectionY);

            GUILayout.Space(10);

            _unselectedGridHiding = EditorGUILayout.Toggle("Unselected Grid Hiding", _unselectedGridHiding);
            _snapAlwaysOn = EditorGUILayout.Toggle("Snap Always On", _snapAlwaysOn);
            EditorGUI.indentLevel--;
        }

        _buildingParam = EditorGUILayout.Foldout(_buildingParam, "Building Options", true, EditorStyles.foldoutHeader);
        if (_buildingParam)
        {
            EditorGUI.indentLevel++;

            _buildMode = (BuildMode)EditorGUILayout.EnumPopup("Build Mode", _buildMode);

            GUILayout.Space(10);

            _duplicateNum = EditorGUILayout.Vector3IntField("Duplicate Number", Vector3Int.Max(_duplicateNum, Vector3Int.zero));

            GUILayout.Space(10);

            _showDuplicateDirections =
                EditorGUILayout.Foldout(_showDuplicateDirections, "Duplication Directions", true);
            if (_showDuplicateDirections)
            {
                EditorGUI.indentLevel++;
                _xDirection = (AxisDirection)EditorGUILayout.EnumPopup("X Axis Direction", _xDirection);
                _yDirection = (AxisDirection)EditorGUILayout.EnumPopup("Y Axis Direction", _yDirection);
                _zDirection = (AxisDirection)EditorGUILayout.EnumPopup("Z Axis Direction", _zDirection);
                EditorGUI.indentLevel--;
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Duplicate"))
                DuplicateItems();

            EditorGUI.indentLevel--;
        }

        GUILayout.Space(15);

        if (GUILayout.Button("Reset to Default"))
        {
            _linesNumber = 25;
            _gridSize = 1f;
            _ySnapHeight = 1f;
            _snapAlwaysOn = true;
            _unselectedGridHiding = true;
            _showCustomGrid = true;
            _showYGrid = false;
            _followSelectionX = false;
            _followSelectionY = false;
            _buildMode = BuildMode.Normal;
            _duplicateNum = Vector3Int.zero;
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
                SnapToGrid(pos, t);
                t.hasChanged = false;
            }
        }
    }

    private static void DuplicateObject(Transform original, Vector3 position)
    {
        GameObject clone = Instantiate(original.gameObject, position, original.rotation, original.parent);
        Undo.RegisterCreatedObjectUndo(clone, "Grid Duplicate");
        SnapToGrid(clone.transform.position, clone.transform);
    }

    private static void BuildCube(Vector3 start, Vector3Int count, Transform original)
    {
        for (int x = 0; x <= count.x; x++)
        {
            for (int y = 0; y <= count.y; y++)
            {
                for (int z = 0; z <= count.z; z++)
                {
                    Vector3 offset = new(x, y, z);
                    if (offset != Vector3.zero)
                        DuplicateObject(original, start + offset);
                }
            }
        }
    }

    private static void BuildStairs(Vector3 start, Vector3Int count, Transform original)
    {
        float stepX = count.x != 0 ? _gridSize : 0f;
        float stepY = count.y != 0 ? _ySnapHeight : 0f;
        float stepZ = count.z != 0 ? _gridSize : 0f;

        for (int x = 0; x <= count.x; x++)
        {
            Vector3 offset = new(stepX * x, stepY * x, stepZ * x);
            if (offset != Vector3.zero)
                DuplicateObject(original, start + offset);
        }
    }

    private static void DuplicateItems()
    {
        if (Selection.transforms.Length == 0)
        {
            Debug.LogWarning("No Items Selected");
            return;
        }

        if (_duplicateNum == Vector3Int.zero)
            return;

        foreach (Transform original in Selection.transforms)
        {
            Vector3 objectPos = original.position;

            objectPos = new Vector3
            {
                x = _xDirection.HasFlag(AxisDirection.Negative) ? objectPos.x - _duplicateNum.x * _gridSize : objectPos.x,
                y = _yDirection.HasFlag(AxisDirection.Negative) ? objectPos.y - _duplicateNum.y * _ySnapHeight : objectPos.y,
                z = _zDirection.HasFlag(AxisDirection.Negative) ? objectPos.z - _duplicateNum.z * _gridSize : objectPos.z
            };

            Vector3Int count = _duplicateNum;
            if (_xDirection == AxisDirection.Both)
                count.x *= 2;

            if (_yDirection == AxisDirection.Both)
                count.y *= 2;

            if (_zDirection == AxisDirection.Both)
                count.z *= 2;

            switch (_buildMode)
            {
                case BuildMode.Normal:
                    BuildCube(objectPos, count, original);
                    break;
                case BuildMode.Stair:
                    BuildStairs(objectPos, count, original);
                    break;
                default:
                    Debug.LogAssertion($"Unhandled BuildMode {_buildMode}");
                    break;
            }
        }

        SceneView.RepaintAll();
    }


    private static void SnapToGrid(Vector3 snapPosition, Transform snapTarget)
    {
        snapPosition.x = Mathf.Round(snapPosition.x / _gridSize) * _gridSize;
        snapPosition.y = Mathf.Round(snapPosition.y / _ySnapHeight) * _ySnapHeight;
        snapPosition.z = Mathf.Round(snapPosition.z / _gridSize) * _gridSize;
        snapTarget.position = snapPosition;
    }

    #region Draw Grid

    private static void DrawXZGrid(SceneView sceneView)
    {
        Handles.color = _hGridColor;
        float extent = _linesNumber * _gridSize;

        for (int i = -_linesNumber; i <= _linesNumber; i++)
        {
            float p = i * _gridSize;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.DrawLine(new Vector3(p, _selectedObject.y, -extent), new Vector3(p, _selectedObject.y, extent));
            Handles.DrawLine(new Vector3(-extent, _selectedObject.y, p), new Vector3(extent, _selectedObject.y, p));
        }
    }

    private static void DrawYGrid(SceneView sceneView)
    {
        Handles.color = _vGridColor;
        float horizontalExtent = _linesNumber * _gridSize;
        float verticalExtent = _linesNumber * _ySnapHeight;

        Vector3 camForward = sceneView.camera.transform.forward;
        Vector3 absForward = new(Mathf.Abs(camForward.x), Mathf.Abs(camForward.y), Mathf.Abs(camForward.z));

        bool alignWithX = absForward.x > absForward.z;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

        if (alignWithX)
        {
            for (int i = -_linesNumber; i <= _linesNumber; i++)
            {
                float z = i * _gridSize;
                Handles.DrawLine(
                    new Vector3(_selectedObject.x, -verticalExtent, z),
                    new Vector3(_selectedObject.x, verticalExtent, z)
                );
            }

            for (int j = -_linesNumber; j <= _linesNumber; j++)
            {
                float y = j * _ySnapHeight;
                Handles.DrawLine(
                    new Vector3(_selectedObject.x, y, -horizontalExtent),
                    new Vector3(_selectedObject.x, y, horizontalExtent)
                );
            }
        }
        else
        {
            for (int i = -_linesNumber; i <= _linesNumber; i++)
            {
                float x = i * _gridSize;
                Handles.DrawLine(
                    new Vector3(x, -verticalExtent, _selectedObject.z),
                    new Vector3(x, verticalExtent, _selectedObject.z)
                );
            }

            for (int j = -_linesNumber; j <= _linesNumber; j++)
            {
                float y = j * _ySnapHeight;
                Handles.DrawLine(
                    new Vector3(-horizontalExtent, y, _selectedObject.z),
                    new Vector3(horizontalExtent, y, _selectedObject.z)
                );
            }
        }
    }

    #endregion

    #region Save/Load Settings

    private static void LoadSettings()
    {
        if (_settingsLoaded)
            return;

        _snapAlwaysOn = EditorPrefs.GetBool(SnapPrefKey, false);
        _followSelectionX = EditorPrefs.GetBool(GridFollowX, false);
        _followSelectionY = EditorPrefs.GetBool(GridFollowY, false);
        _unselectedGridHiding = EditorPrefs.GetBool(AutoGridHiding, true);
        _gridSize = EditorPrefs.GetFloat(GridSizePrefKey, 1f);
        _ySnapHeight = EditorPrefs.GetFloat(YSnapHeightPrefKey, 1f);
        _showCustomGrid = EditorPrefs.GetBool(ShowGridPrefKey, true);
        _showYGrid = EditorPrefs.GetBool(ShowYGridPrefKey, false);
        string duplicateNumJson = EditorPrefs.GetString(DuplicationNum, "{\"x\":0,\"y\":0,\"z\":0}");
        _duplicateNum = JsonUtility.FromJson<Vector3Int>(duplicateNumJson);
        _linesNumber = EditorPrefs.GetInt(LinesNumber, 25);
        _settingsLoaded = true;
        _buildMode = (BuildMode)EditorPrefs.GetInt(ModeKey, (int)_buildMode);
        _xDirection = (AxisDirection)EditorPrefs.GetInt(XDirectionKey, (int)AxisDirection.Positive);
        _yDirection = (AxisDirection)EditorPrefs.GetInt(YDirectionKey, (int)AxisDirection.Positive);
        _zDirection = (AxisDirection)EditorPrefs.GetInt(ZDirectionKey, (int)AxisDirection.Positive);

        if (ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(XGridColorPrefKey, "4D4D4DFF"),
                out Color hSavedColor))
            _hGridColor = hSavedColor;
        else
            _hGridColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        if (ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(YGridColorPrefKey, "4D4D4DFF"),
                out Color vSavedColor))
            _vGridColor = vSavedColor;
        else
            _vGridColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    }

    private static void SaveSettings()
    {
        EditorPrefs.SetBool(SnapPrefKey, _snapAlwaysOn);
        EditorPrefs.SetBool(GridFollowX, _followSelectionX);
        EditorPrefs.SetBool(GridFollowY, _followSelectionY);
        EditorPrefs.SetString(DuplicationNum, JsonUtility.ToJson(_duplicateNum));
        EditorPrefs.SetBool(AutoGridHiding, _unselectedGridHiding);
        EditorPrefs.SetFloat(GridSizePrefKey, _gridSize);
        EditorPrefs.SetFloat(YSnapHeightPrefKey, _ySnapHeight);
        EditorPrefs.SetBool(ShowGridPrefKey, _showCustomGrid);
        EditorPrefs.SetBool(ShowYGridPrefKey, _showYGrid);
        EditorPrefs.SetString(XGridColorPrefKey, ColorUtility.ToHtmlStringRGBA(_hGridColor));
        EditorPrefs.SetString(YGridColorPrefKey, ColorUtility.ToHtmlStringRGBA(_vGridColor));
        EditorPrefs.SetInt(ModeKey, (int)_buildMode);
        EditorPrefs.SetInt(XDirectionKey, (int)_xDirection);
        EditorPrefs.SetInt(YDirectionKey, (int)_yDirection);
        EditorPrefs.SetInt(ZDirectionKey, (int)_zDirection);
        EditorPrefs.SetInt(LinesNumber, _linesNumber);
    }

    #endregion

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
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using System.Reflection;

public class ModularBuilding : EditorWindow
{
    private static bool snapAlwaysOn;
    private static float gridSize = 1f;
    private static bool showCustomGrid = true;
    private static bool showYGrid = false;
    private static float ySnapHeight = 1f;
    private static bool settingsLoaded;
    private static Color gridColor;

    private const string SnapPrefKey = "ModularBuilding_SnapAlwaysOn";
    private const string GridSizePrefKey = "ModularBuilding_GridSize";
    private const string ShowGridPrefKey = "ModularBuilding_ShowGrid";
    private const string ShowYGridPrefKey = "ModularBuilding_ShowYGrid";
    private const string YSnapHeightPrefKey = "ModularBuilding_YSnapHeight";
    private const string GridColorPrefKey = "CustomGridColor";
    public const string ShortcutId = "Tools/Grid Snap";

    [MenuItem("Tools/Grid Snap Tool")]
    private static void OpenWindow() => GetWindow<ModularBuilding>("Grid Snap");

    [Shortcut(ShortcutId, KeyCode.Space)]
    public static void ShortcutSnap() => snapAlwaysOn = !snapAlwaysOn;

    private void OnEnable()
    {
        LoadSettings();
        SceneView.duringSceneGui += OnSceneGUI;
        HideUnityGrid(showCustomGrid);
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        HideUnityGrid(false);
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Snap Tool", EditorStyles.boldLabel);
        gridSize = EditorGUILayout.FloatField("Grid Size (XZ)", gridSize);
        gridSize = Mathf.Max(0.0001f, gridSize);

        snapAlwaysOn = EditorGUILayout.Toggle("Snap Always On", snapAlwaysOn);

        bool prevShowGrid = showCustomGrid;
        showCustomGrid = EditorGUILayout.Toggle("Show XZ Grid", showCustomGrid);

        showYGrid = EditorGUILayout.Toggle("Show Y Grid", showYGrid);
        ySnapHeight = EditorGUILayout.FloatField("Y Snap Height", ySnapHeight);
        ySnapHeight = Mathf.Max(0.0001f, ySnapHeight);

        if (prevShowGrid != showCustomGrid)
            HideUnityGrid(showCustomGrid);

        gridColor = EditorGUILayout.ColorField("Grid Color", gridColor);

        GUILayout.Space(8);

        if (GUILayout.Button("Reset to Default"))
        {
            gridSize = 1f;
            ySnapHeight = 1f;
            snapAlwaysOn = false;
            showCustomGrid = true;
            showYGrid = false;
            HideUnityGrid(showCustomGrid);
        }

        SaveSettings();
        SceneView.RepaintAll();
    }

    private static void OnSceneGUI(SceneView sceneView)
    {
        if (showCustomGrid)
            DrawXZGrid(sceneView);

        if (showYGrid)
            DrawYGrid(sceneView);

        if (!snapAlwaysOn || Selection.transforms.Length == 0)
            return;

        foreach (Transform t in Selection.transforms)
        {
            if (t.hasChanged)
            {
                Undo.RecordObject(t, "Snap To Grid (Auto)");
                Vector3 pos = t.position;
                pos.x = Mathf.Round(pos.x / gridSize) * gridSize;
                pos.z = Mathf.Round(pos.z / gridSize) * gridSize;
                pos.y = Mathf.Round(pos.y / ySnapHeight) * ySnapHeight;
                t.position = pos;
                t.hasChanged = false;
            }
        }
    }

    private static void DrawXZGrid(SceneView sceneView)
    {
        Handles.color = gridColor;
        int lines = 50;
        float extent = lines * gridSize;

        for (int i = -lines; i <= lines; i++)
        {
            float p = i * gridSize;
            Handles.DrawLine(new Vector3(p, 0, -extent), new Vector3(p, 0, extent)); // X lines
            Handles.DrawLine(new Vector3(-extent, 0, p), new Vector3(extent, 0, p)); // Z lines
        }
    }

    private static void DrawYGrid(SceneView sceneView)
    {
        Handles.color = gridColor;
        int lines = 50;
        float horizontalExtent = lines * gridSize;
        float verticalExtent = lines * ySnapHeight;

        // Vertical lines (parallel to Y axis)
        for (int i = -lines; i <= lines; i++)
        {
            float x = i * gridSize;
            Handles.DrawLine(
                new Vector3(x, -verticalExtent, 0),
                new Vector3(x, verticalExtent, 0)
            );
        }

        // Horizontal lines (parallel to X axis)
        for (int j = -lines; j <= lines; j++)
        {
            float y = j * ySnapHeight;
            Handles.DrawLine(
                new Vector3(-horizontalExtent, y, 0),
                new Vector3(horizontalExtent, y, 0)
            );
        }
    }

    private static void LoadSettings()
    {
        if (settingsLoaded) return;

        snapAlwaysOn = EditorPrefs.GetBool(SnapPrefKey, false);
        gridSize = EditorPrefs.GetFloat(GridSizePrefKey, 1f);
        ySnapHeight = EditorPrefs.GetFloat(YSnapHeightPrefKey, 1f);
        showCustomGrid = EditorPrefs.GetBool(ShowGridPrefKey, true);
        showYGrid = EditorPrefs.GetBool(ShowYGridPrefKey, false);
        settingsLoaded = true;

        float r = EditorPrefs.GetFloat(GridColorPrefKey + "_R", 0.3f);
        float g = EditorPrefs.GetFloat(GridColorPrefKey + "_G", 0.3f);
        float b = EditorPrefs.GetFloat(GridColorPrefKey + "_B", 0.3f);
        float a = EditorPrefs.GetFloat(GridColorPrefKey + "_A", 1f);
        gridColor = new Color(r, g, b, a);
    }

    private static void SaveSettings()
    {
        EditorPrefs.SetBool(SnapPrefKey, snapAlwaysOn);
        EditorPrefs.SetFloat(GridSizePrefKey, gridSize);
        EditorPrefs.SetFloat(YSnapHeightPrefKey, ySnapHeight);
        EditorPrefs.SetBool(ShowGridPrefKey, showCustomGrid);
        EditorPrefs.SetBool(ShowYGridPrefKey, showYGrid);
        EditorPrefs.SetFloat(GridColorPrefKey + "_R", gridColor.r);
        EditorPrefs.SetFloat(GridColorPrefKey + "_G", gridColor.g);
        EditorPrefs.SetFloat(GridColorPrefKey + "_B", gridColor.b);
        EditorPrefs.SetFloat(GridColorPrefKey + "_A", gridColor.a);
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
                PropertyInfo gridProp = gridType != null
                    ? gridType.GetProperty("showGrid",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    : null;

                if (gridProp != null && gridInstance != null)
                    gridProp.SetValue(gridInstance, !hide);
            }

            view.Repaint();
        }
    }
}

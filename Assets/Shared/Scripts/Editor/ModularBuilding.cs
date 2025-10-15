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
    private static Color hGridColor;
    private static Color vGridColor;

    private const string SnapPrefKey = "ModularBuilding_SnapAlwaysOn";
    private const string GridSizePrefKey = "ModularBuilding_GridSize";
    private const string ShowGridPrefKey = "ModularBuilding_ShowGrid";
    private const string ShowYGridPrefKey = "ModularBuilding_ShowYGrid";
    private const string YSnapHeightPrefKey = "ModularBuilding_YSnapHeight";
    private const string XGridColorPrefKey = "XCustomGridColor";
    private const string YGridColorPrefKey = "YCustomGridColor";
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

        hGridColor = EditorGUILayout.ColorField("H Grid Color", hGridColor);
        vGridColor = EditorGUILayout.ColorField("V Grid Color", vGridColor);

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

        if (showYGrid && Selection.activeGameObject != null)
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
        Handles.color = hGridColor;
        int lines = 50;
        float extent = lines * gridSize;

        for (int i = -lines; i <= lines; i++)
        {
            float p = i * gridSize;
            Handles.DrawLine(new Vector3(p, 0, -extent), new Vector3(p, 0, extent));
            Handles.DrawLine(new Vector3(-extent, 0, p), new Vector3(extent, 0, p));
        }
    }

    private static void DrawYGrid(SceneView sceneView)
    {
        Handles.color = vGridColor;
        int lines = 50;
        float horizontalExtent = lines * gridSize;
        float verticalExtent = lines * ySnapHeight;

        for (int i = -lines; i <= lines; i++)
        {
            float x = i * gridSize;
            Handles.DrawLine(
                new Vector3(x, -verticalExtent, Selection.activeTransform.localPosition.z),
                new Vector3(x, verticalExtent, Selection.activeTransform.localPosition.z)
            );
        }

        for (int j = -lines; j <= lines; j++)
        {
            float y = j * ySnapHeight;
            Handles.DrawLine(
                new Vector3(-horizontalExtent, y, Selection.activeTransform.localPosition.z),
                new Vector3(horizontalExtent, y, Selection.activeTransform.localPosition.z)
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
        
        if (ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(XGridColorPrefKey, "4D4D4DFF"), out var hSavedColor))
            hGridColor = hSavedColor;
        else
            hGridColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        if (ColorUtility.TryParseHtmlString("#" + EditorPrefs.GetString(YGridColorPrefKey, "4D4D4DFF"), out var vSavedColor))
            vGridColor = vSavedColor;
        else
            vGridColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    }

    private static void SaveSettings()
    {
        EditorPrefs.SetBool(SnapPrefKey, snapAlwaysOn);
        EditorPrefs.SetFloat(GridSizePrefKey, gridSize);
        EditorPrefs.SetFloat(YSnapHeightPrefKey, ySnapHeight);
        EditorPrefs.SetBool(ShowGridPrefKey, showCustomGrid);
        EditorPrefs.SetBool(ShowYGridPrefKey, showYGrid);
        EditorPrefs.SetString(XGridColorPrefKey, ColorUtility.ToHtmlStringRGBA(hGridColor));
        EditorPrefs.SetString(YGridColorPrefKey, ColorUtility.ToHtmlStringRGBA(vGridColor));
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
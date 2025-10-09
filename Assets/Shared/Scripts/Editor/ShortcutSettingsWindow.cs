using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class ShortcutSettingsWindow : EditorWindow
{
    private IShortcutManager _shortcutManager;
    private ShortcutBinding _snapBinding;

    private bool _isWaitingForKey = false;

    [MenuItem("Tools/Shortcut Settings")]
    private static void Open()
    {
        GetWindow<ShortcutSettingsWindow>("Shortcut Settings");
    }

    private void OnEnable()
    {
        _shortcutManager = ShortcutManager.instance;
        LoadBindings();
    }

    private void LoadBindings()
    {
        _snapBinding = _shortcutManager.GetShortcutBinding(ModularBuilding.ShortcutId);
    }

    private void OnGUI()
    {
        GUILayout.Label("Shortcut Settings", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("Snap To Grid:", EditorStyles.label);
        GUILayout.Label($"Current Key: {_snapBinding}");

        GUILayout.Space(5);
        if (!_isWaitingForKey)
        {
            if (GUILayout.Button("Change Shortcut"))
                _isWaitingForKey = true;
        }
        else
        {
            GUILayout.Label("Press a new key...", EditorStyles.helpBox);
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode != KeyCode.None)
            {
                var newBinding = new ShortcutBinding(new KeyCombination(Event.current.keyCode));
                _shortcutManager.RebindShortcut(ModularBuilding.ShortcutId, newBinding);
                LoadBindings();
                Repaint();

                Debug.Log($"Shortcut rebound to {Event.current.keyCode}");
                _isWaitingForKey = false;
                Event.current.Use();
            }
        }
    }
}
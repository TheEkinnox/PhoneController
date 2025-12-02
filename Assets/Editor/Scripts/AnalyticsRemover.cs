using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Experimental;
using UnityEngine;

public class AnalyticsRemover : IPreprocessBuildWithReport
{
    public int callbackOrder { get; }

    public void OnPreprocessBuild(BuildReport report)
    {
        Object connectSettingsRes = EditorResources.Load<Object>("ProjectSettings/UnityConnectSettings.asset");
        SerializedObject connectSettingsObj = new(connectSettingsRes);
        connectSettingsObj.FindProperty("m_Enabled").boolValue = false;
        connectSettingsObj.FindProperty("UnityAnalyticsSettings" ).FindPropertyRelative( "m_Enabled" ).boolValue = false;
        connectSettingsObj.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();

        Debug.Log("Analytics disabled");
    }
}
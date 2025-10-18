using UnityEditor;
using UnityEditor.Build;

[InitializeOnLoad]
public class AppIdManager
{
    private static string _previousCompany;
    private static string _previousProduct;

    static AppIdManager()
    {
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (PlayerSettings.companyName == _previousCompany && PlayerSettings.productName == _previousProduct)
            return;

        UnifyAppIdentifier();
        _previousCompany = PlayerSettings.companyName;
        _previousProduct = PlayerSettings.productName;
    }

    private static void UnifyAppIdentifier()
    {
        string targetIdentifier = $"com.{PlayerSettings.companyName}.{PlayerSettings.productName}".ToLowerInvariant().Replace(" ", "");

        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, targetIdentifier);
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, targetIdentifier);
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, targetIdentifier);
    }
}
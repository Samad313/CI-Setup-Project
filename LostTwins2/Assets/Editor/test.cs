using UnityEngine;
using UnityEditor;

public class MenuItems
{
    [MenuItem("Versioning Maintainer/Set Verisons")]
    private static void NewMenuOption()
    {
        VersioningMaintainer.VersionSetter();
    }
}
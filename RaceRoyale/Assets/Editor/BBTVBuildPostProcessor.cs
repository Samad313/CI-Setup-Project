#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Collections;
using UnityEngine;

public class BBTVBuildPostProcessor
{
    private static PBXProject _project;

    private static string _path = default;
    private static string _projectPath = default;
    private static string _target = default;
    private static string _infoPlistPath = default;
    private static string _targetName = default;
    private static string _ProjectName = default;

    private static string _UIRequiredDeviceCompatobilities = default;
    private static string _NSCalanderUsageDescriptionKey = default;
    private static string _NSCalanderUsageDescriptionValue = default;
    private static string _NSUserTrackingUsageDescriptionKey = default;
    private static string _NSUserTrackingUsageDescriptionValue = default;
    private static string _AdmobApplicationIdentifierKey = default;
    private static string _AdmobApplicationIdentifierValue = default;
    private static string _EntitlementText = default;
    private static string[] _AssociatedDomains = default;
    private static string _AssociatedDomainsEntitlementProperty = default;
    private static string _SkAdNetworkPlsitItemsArrayID = default;
    private static string _SkAdNetworkPlsitItemKey = default;
    private static string[] _SKAdNetworkIDsArray = default;
    private static string[] _FrameworksNames = default;
    private static string _EnableBitcodeKey = default;
    private static string _EnableBitcodeValue = default;

    private static void OpenProject()
    {
        _projectPath = _path + BuildPostProcessConstants.ProjectPath;
        _infoPlistPath = _path + BuildPostProcessConstants.InfoPlistPath;
        _targetName = BuildPostProcessConstants.TargetName;
        _ProjectName = BuildPostProcessConstants.ProjectName;
        _UIRequiredDeviceCompatobilities = BuildPostProcessConstants.UIRequiredDeviceCompatobilities;
        _NSCalanderUsageDescriptionKey = BuildPostProcessConstants.NSCalanderUsageDescriptionKey;
        _NSCalanderUsageDescriptionValue = BuildPostProcessConstants.NSCalanderUsageDescriptionValue;
        _NSUserTrackingUsageDescriptionKey = BuildPostProcessConstants.NSUserTrackingUsageDescriptionKey;
        _NSUserTrackingUsageDescriptionValue = BuildPostProcessConstants.NSUserTrackingUsageDescriptionValue;
        _AdmobApplicationIdentifierKey = BuildPostProcessConstants.AdmobApplicationIdentifierKey;
        _AdmobApplicationIdentifierValue = BuildPostProcessConstants.AdmobApplicationIdentifierValue;
        _EntitlementText = BuildPostProcessConstants.EntitlementText;
        _AssociatedDomains = BuildPostProcessConstants.AssociatedDomains;
        _AssociatedDomainsEntitlementProperty = BuildPostProcessConstants.AssociatedDomainsEntitlementProperty;
        _SkAdNetworkPlsitItemsArrayID = BuildPostProcessConstants.SkAdNetworkPlsitItemsArrayID;
        _SkAdNetworkPlsitItemKey = BuildPostProcessConstants.SkAdNetworkPlsitItemKey;
        _SKAdNetworkIDsArray = BuildPostProcessConstants.SKAdNetworkIDsArray;
        _FrameworksNames = BuildPostProcessConstants.FrameWorkNames;
        _EnableBitcodeKey = BuildPostProcessConstants.EnableBitcodeKey;
        _EnableBitcodeValue = BuildPostProcessConstants.EnableBitcodeValue;

        _project = new PBXProject();
        _project.ReadFromFile(_projectPath);
        #if UNITY_2018_4
        _target = _project.TargetGuidByName(_targetName);
        #elif UNITY_2018_4_OR_NEWER
        _target = _project.GetUnityMainTargetGuid();
        #endif

    }

    private static void CloseProject()
    {
        File.WriteAllText(_projectPath, _project.WriteToString());
    }

    private static void AddFramework(string framework)
    {
        if (_project.ContainsFramework(_target,framework)) return;
        _project.AddFrameworkToProject(_target, framework, false);
    }

    private static void RemoveUIRequiredDeviceCapabilities()
    {   
        var plistParser = new PlistDocument();
        plistParser.ReadFromFile(_infoPlistPath);
        var deviceCapabilityRoot = plistParser.root; 
        var deviceCapabilityRootDictionary = deviceCapabilityRoot.values;
        deviceCapabilityRootDictionary.Remove(_UIRequiredDeviceCompatobilities);
        plistParser.WriteToFile(_infoPlistPath);
    }

    private static void AddCalandarUsage()
    {
        var plistParser = new PlistDocument();
        plistParser.ReadFromFile(_infoPlistPath);
        plistParser.root.SetString(_NSCalanderUsageDescriptionKey,_NSCalanderUsageDescriptionValue);
        plistParser.WriteToFile(_infoPlistPath);
    }

    private static void AddTrackingUserDescription()
    {
        var plistParser = new PlistDocument();
        plistParser.ReadFromFile(_infoPlistPath);
        plistParser.root.SetString(_NSUserTrackingUsageDescriptionKey, _NSUserTrackingUsageDescriptionValue);
        plistParser.WriteToFile(_infoPlistPath);
    }

    private static void AddAdMobKeyiOS()
    {
        var plistParser = new PlistDocument();
        plistParser.ReadFromFile(_infoPlistPath);
        plistParser.root.SetString(_AdmobApplicationIdentifierKey, _AdmobApplicationIdentifierValue);
        plistParser.WriteToFile(_infoPlistPath);
    }

    private static void AddAssociatedDomains()
    {
        var separator = Path.DirectorySeparatorChar;
        var entitlementPath = _projectPath + separator + _targetName + separator + _ProjectName + _EntitlementText;
        var entitlementFileName = Path.GetFileName(entitlementPath);
        var relativeDestination = _targetName + separator + entitlementFileName;
        var capabilityManager = new ProjectCapabilityManager(_projectPath, relativeDestination, _targetName);
        capabilityManager.AddAssociatedDomains(_AssociatedDomains);
        _project.AddBuildProperty(_target, _AssociatedDomainsEntitlementProperty, relativeDestination);
        capabilityManager.WriteToFile();
    }

    private static void AddSKAdNetworksIDs()
    {
        var plistParser = new PlistDocument();
        plistParser.ReadFromFile(_infoPlistPath);
        PlistElementDict rootDict = plistParser.root;

        PlistElementArray array = rootDict.CreateArray(_SkAdNetworkPlsitItemsArrayID);
        for (int i= 0; i<_SKAdNetworkIDsArray.Length;i++)
        {
            array.AddDict().SetString(_SkAdNetworkPlsitItemKey, _SKAdNetworkIDsArray[i]);
        }
        plistParser.WriteToFile(_infoPlistPath);
    }

    private static void AddFrameworks()
    {
        for (int i = 0; i <_FrameworksNames.Length; i++)
            AddFramework(_FrameworksNames[i]);
    }

    private static void SetFlagsInXcode()
    {
        _project.AddBuildProperty(_target, _EnableBitcodeKey, _EnableBitcodeValue);
    }

    [PostProcessBuild]
    public static void OnPostprocessBuild(UnityEditor.BuildTarget buildTarget, string path)
    {
        _path = path;
        OpenProject();

        if(BuildPostProcessConstants.PostProcessConfiguration.removeRequiredCapabilities)
            RemoveUIRequiredDeviceCapabilities();
        if (BuildPostProcessConstants.PostProcessConfiguration.addAssociatedDomain)
            AddAssociatedDomains();
        if (BuildPostProcessConstants.PostProcessConfiguration.enableCalendarUsageDescription)
            AddCalandarUsage();
        if (BuildPostProcessConstants.PostProcessConfiguration.addFrameworks)
            AddFrameworks();
        if (BuildPostProcessConstants.PostProcessConfiguration.addAdmobKey)
            AddAdMobKeyiOS();
        if (BuildPostProcessConstants.PostProcessConfiguration.enableAppTrackingTransparency)
        {
            AddTrackingUserDescription();
            AddSKAdNetworksIDs();
        }
        if (BuildPostProcessConstants.PostProcessConfiguration.disableBitcode)
            SetFlagsInXcode();

        CloseProject();
    }


}
#endif
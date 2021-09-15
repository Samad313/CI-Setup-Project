using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class BuildPostProcessConstants
{
    //Configure values based on project requirements. 
    public static class PostProcessConfiguration
    {
        public static bool removeRequiredCapabilities = true;
        public static bool enableCalendarUsageDescription = true;
        public static bool enableAppTrackingTransparency = true;
        public static bool addAdmobKey = false;
        public static bool addAssociatedDomain = false;
        public static bool addFrameworks = false;
        public static bool addSKAdnetworks = false;
        public static bool disableBitcode = true;
    }
    private static class SKNetworkIDsWithName
    {
        public const string Vungle = "GTA9LK7P23.skadnetwork";
        public const string Adikteev = "ydx93a7ass.skadnetwork";
        public const string Aarki = "4FZDC2EVR5.skadnetwork";
        public const string AdColony = "4PFYVQ9L8R.skadnetwork";
        public const string Appier = "v72qych5uu.skadnetwork";
        public const string Appreciate = "mlmmfzh3r3.skadnetwork";
        public const string Beeswax = "c6k4g5qg8m.skadnetwork";
        public const string Jampp = "YCLNXRL5PM.skadnetwork";
        public const string LoopMe = "5lm9lj6jb7.skadnetwork";
        public const string MyTarget = "n9x2a789qt.skadnetwork";
        public const string Pubnative = "TL55SBB4FM.skadnetwork";
        public const string Remerge = "2U9PT9HC89.skadnetwork";
        public const string RTB_House = "8s468mfl3y.skadnetwork";
        public const string Sabio = "GLQZH8VGBY.skadnetwork";
        public const string YouAppi = "3RD42EKR43.skadnetwork";
        public const string ironSource = "su67r6k2v3.skadnetwork";
        public const string AdMob = "cstr6suwn9.skadnetwork";
        public const string AppLovin = "ludvb6z3bs.skadnetwork";
        public const string Chartboost = "f38h382jlk.skadnetwork";
        public const string Facebook_1 = "v9wttpbfk9.skadnetwork";
        public const string Facebook_2 = "n38lu8286q.skadnetwork";
        public const string Fyber = "9g2aggbj52.skadnetwork";
        public const string HyprMx = "nu4557a4je.skadnetwork";
        public const string InMobi = "wzmmz9fp6w.skadnetwork";
        public const string Maio = "v4nxqhlyqp.skadnetwork";
        public const string Pangle_Non_CN = "22mmun2rn5.skadnetwork";
        public const string Pangle_CN = "238da6jt44.skadnetwork";
        public const string Snap = "424m5254lk.skadnetwork";
        public const string TapJoy = "ecpz2srf59.skadnetwork";
        public const string UnityAds = "4dzt52r2t5.skadnetwork";
        public const string Criteo = "hs6bdukanm.skadnetwork";
        public const string CrossInstall = "prcb7njmu6.skadnetwork";
        public const string Dataseat = "m8dbw4sv7c.skadnetwork";
        public const string Kayzen = "4468km3ulz.skadnetwork";
        public const string LifeStreet = "t38b2kh725.skadnetwork";
        public const string manage = "9rd848q2bz.skadnetwork";
        public const string MediaSmart = "7rz58n8ntl.skadnetwork";
        public const string MMG = "ejvt5qm6ak.skadnetwork";
        public const string Moloco = "9t245vhmpl.skadnetwork";
        public const string Personaly = "44jx6755aq.skadnetwork";
        public const string Sift = "klf5c3l5u5.skadnetwork";
        public const string Smadex = "ppxm28t8ap.skadnetwork";
        public const string Valassis = "mtkv5xtk9e.skadnetwork";
        public const string Liftoff = "7ug5zh24hu.skadnetwork";
        public const string Unicorn = "578prtvx9j.skadnetwork";
        public const string Unknown_1 = "5a6flpkh64.skadnetwork";
        public const string Unknown_2 = "wg4vff78zm.skadnetwork";
        public const string Unknown_3 = "3qy4746246.skadnetwork";
        public const string Unknown_4 = "252b5q8x7y.skadnetwork";
        public const string Unknown_5 = "24t9a8vw3c.skadnetwork";
        public const string Unknown_6 = "kbd757ywx3.skadnetwork";
        public const string ScaleMonk_Inc = "av6w8kgt66.skadnetwork";
        public const string Unknown_7 = "hdw39hrw9y.skadnetwork";
        public const string Unknown_8 = "y45688jllp.skadnetwork";
        public const string Unknown_9 = "dzg6xy7pwj.skadnetwork";
        public const string Unknown_10 = "3sh42y64q3.skadnetwork";
        public const string Unknown_11 = "f73kdq92p3.skadnetwork";
        public const string Unknown_12 = "5l3tpt7t6e.skadnetwork";
        public const string Unknown_13 = "uw77j35x4d.skadnetwork";
        public const string Unknown_14 = "cg4yq2srnc.skadnetwork";
        public const string Unknown_15 = "ggvn48r87g.skadnetwork";
        public const string Unknown_16 = "w9q455wk68.skadnetwork";
        public const string Unknown_17 = "p78axxw29g.skadnetwork";
        public const string SpykeMediaGmbH = "44n7hlldy6.skadnetwork";
        public const string MaidenMarketingPvtLtd = "zmvfpc5aq8.skadnetwork";
        public const string UnityTechnologies = "bvpn9ufa9b.skadnetwork";
        public const string BideaseInc = "s39g8k73mm.skadnetwork";
        public const string ADTIMING_TECHNOLOGY_PTE_LTD = "488r3q3dtq.skadnetwork";
        public const string Kidoz_Ltd = "v79kvwwj4g.skadnetwork";
        public const string Apptimus_LTD = "lr83yxwka7.skadnetwork";

    }
    private static class Frameworks
    {
        public const string Security = "Security.framework";
        public const string UserNotifications = "UserNotifications.framework";
    }

    //plist and project path
    private const string PROJECT_PATH = "/Unity-iPhone.xcodeproj/project.pbxproj";
    private const string INFO_PLIST_PATH = "/Info.plist";
    private const string TARGET_NAME = "Unity-iPhone";
    private const string PROJECT_NAME = "dobre";
    private const string ENTITLEMENTS_TEXT = ".entitlements";

    //Required capabilities like game git [using it to remove keys]
    private const string UI_REQUIRED_DEVICE_CAPABILITIES = "UIRequiredDeviceCapabilities";

    //Privacy - Calender api usage reason
    private const string NS_CALANDAR_USAGE_DESCRIPTION_KEY = "NSCalendarsUsageDescription";
    private const string NS_CALANDAR_USAGE_DESCRIPTION_VALUE = "Ad Network is using this key for Calendar API acess";

    //Privacy - Apple App tracking transparency
    private const string NS_USER_TRACKING_USAGE_DESCRIPTION_KEY= "NSUserTrackingUsageDescription";
    private const string NS_USER_TRACKING_USAGE_DESCRIPTION_VALUE = "This identifier will be used to deliver ads to you.";

    //Google Admob keys
    private const string ADMOB_APPLICATIONIDENTIFIER_KEY = "GADApplicationIdentifier";
    private const string ADMOB_APPLICATIONIDENTIFIER_VALUE = "ca-app-pub-5976593420349780~7362499407";

    //Associated Domains
    private static readonly string[] ASSOCIATED_DOMAINS_LINKS = new string[] { "applinks:bbtvrideordie.page.link" };

    //Xcode Properties
    private const string CODE_SIGN_ENTITLEMENTS = "CODE_SIGN_ENTITLEMENTS";
    private const string ENABLE_BITCODE_KEY = "ENABLE_BITCODE";
    private const string ENABLE_BITCODE_VALUE = "false"; 

    //Xcode Frameworks
    private static readonly string[] FRAMEWORKS_NAMES = new string[] { Frameworks.Security , Frameworks.UserNotifications };

    //SKADNetwork IDs
    private const string SKADNETWORK_PLIST_ITEMS_ARRAY_ID = "SKAdNetworkItems";
    private const string SKADNETWORK_PLIST_ITEMS_KEY = "SKAdNetworkIdentifier";
    private static readonly string[] SKADNETWORK_PLIST_ITEMS_VALUES = new string[] {
        SKNetworkIDsWithName.Aarki,
        SKNetworkIDsWithName.AdColony,
        SKNetworkIDsWithName.Adikteev,
        SKNetworkIDsWithName.AdMob,
        SKNetworkIDsWithName.ADTIMING_TECHNOLOGY_PTE_LTD,
        SKNetworkIDsWithName.Appier,
        SKNetworkIDsWithName.AppLovin,
        SKNetworkIDsWithName.Appreciate,
        SKNetworkIDsWithName.Apptimus_LTD,
        SKNetworkIDsWithName.Beeswax,
        SKNetworkIDsWithName.BideaseInc,
        SKNetworkIDsWithName.Chartboost,
        SKNetworkIDsWithName.Criteo,
        SKNetworkIDsWithName.CrossInstall,
        SKNetworkIDsWithName.Dataseat,
        SKNetworkIDsWithName.Facebook_1,
        SKNetworkIDsWithName.Facebook_2,
        SKNetworkIDsWithName.Fyber,
        SKNetworkIDsWithName.HyprMx,
        SKNetworkIDsWithName.InMobi,
        SKNetworkIDsWithName.ironSource,
        SKNetworkIDsWithName.Jampp,
        SKNetworkIDsWithName.Kayzen,
        SKNetworkIDsWithName.Kidoz_Ltd,
        SKNetworkIDsWithName.LifeStreet,
        SKNetworkIDsWithName.Liftoff,
        SKNetworkIDsWithName.LoopMe,
        SKNetworkIDsWithName.MaidenMarketingPvtLtd,
        SKNetworkIDsWithName.Maio,
        SKNetworkIDsWithName.manage,
        SKNetworkIDsWithName.MediaSmart,
        SKNetworkIDsWithName.MMG,
        SKNetworkIDsWithName.Moloco,
        SKNetworkIDsWithName.MyTarget,
        SKNetworkIDsWithName.Pangle_CN,
        SKNetworkIDsWithName.Pangle_Non_CN,
        SKNetworkIDsWithName.Personaly,
        SKNetworkIDsWithName.Pubnative,
        SKNetworkIDsWithName.Remerge,
        SKNetworkIDsWithName.RTB_House,
        SKNetworkIDsWithName.Sabio,
        SKNetworkIDsWithName.ScaleMonk_Inc,
        SKNetworkIDsWithName.Sift,
        SKNetworkIDsWithName.Smadex,
        SKNetworkIDsWithName.Snap,
        SKNetworkIDsWithName.SpykeMediaGmbH,
        SKNetworkIDsWithName.TapJoy,
        SKNetworkIDsWithName.Unicorn,
        SKNetworkIDsWithName.UnityAds,
        SKNetworkIDsWithName.UnityTechnologies,
        SKNetworkIDsWithName.Unknown_1,
        SKNetworkIDsWithName.Unknown_2,
        SKNetworkIDsWithName.Unknown_3,
        SKNetworkIDsWithName.Unknown_4,
        SKNetworkIDsWithName.Unknown_5,
        SKNetworkIDsWithName.Unknown_6,
        SKNetworkIDsWithName.Unknown_7,
        SKNetworkIDsWithName.Unknown_8,
        SKNetworkIDsWithName.Unknown_9,
        SKNetworkIDsWithName.Unknown_10,
        SKNetworkIDsWithName.Unknown_11,
        SKNetworkIDsWithName.Unknown_12,
        SKNetworkIDsWithName.Unknown_13,
        SKNetworkIDsWithName.Unknown_14,
        SKNetworkIDsWithName.Unknown_15,
        SKNetworkIDsWithName.Unknown_16,
        SKNetworkIDsWithName.Unknown_17,
        SKNetworkIDsWithName.Valassis,
        SKNetworkIDsWithName.Vungle,
        SKNetworkIDsWithName.YouAppi,
    };

    //
    public static string ProjectPath   
    {
        get { return PROJECT_PATH; }
    }
    public static string InfoPlistPath
    {
        get { return INFO_PLIST_PATH; }
    }
    public static string TargetName
    {
        get { return TARGET_NAME; }
    }
    public static string ProjectName
    {
        get { return PROJECT_NAME; }
    }
    public static string EntitlementText
    {
        get { return ENTITLEMENTS_TEXT; }
    }

    //
    public static string UIRequiredDeviceCompatobilities
    {
        get { return UI_REQUIRED_DEVICE_CAPABILITIES; }
    }

    //
    public static string NSCalanderUsageDescriptionKey
    {
        get { return NS_CALANDAR_USAGE_DESCRIPTION_KEY; }
    }
    public static string NSCalanderUsageDescriptionValue
    {
        get { return NS_CALANDAR_USAGE_DESCRIPTION_VALUE; }
    }

    //
    public static string NSUserTrackingUsageDescriptionKey
    {
        get { return NS_USER_TRACKING_USAGE_DESCRIPTION_KEY; }
    }
    public static string NSUserTrackingUsageDescriptionValue
    {
        get { return NS_USER_TRACKING_USAGE_DESCRIPTION_VALUE; }
    }

    //
    public static string AdmobApplicationIdentifierKey
    {
        get { return ADMOB_APPLICATIONIDENTIFIER_KEY; }
    }
    public static string AdmobApplicationIdentifierValue
    {
        get { return ADMOB_APPLICATIONIDENTIFIER_VALUE; }
    }

    //
    public static string[] AssociatedDomains
    {
        get { return ASSOCIATED_DOMAINS_LINKS; }
    }
    public static string AssociatedDomainsEntitlementProperty
    {
        get { return CODE_SIGN_ENTITLEMENTS; }
    }

    //
    public static string EnableBitcodeKey
    {
        get { return ENABLE_BITCODE_KEY; }
    }
    public static string EnableBitcodeValue
    {
        get { return ENABLE_BITCODE_VALUE; }
    }

    //
    public static string[] FrameWorkNames
    {
        get { return FRAMEWORKS_NAMES; }
    }

    //
    public static string SkAdNetworkPlsitItemsArrayID
    {
        get { return SKADNETWORK_PLIST_ITEMS_ARRAY_ID; }
    }
    public static string SkAdNetworkPlsitItemKey
    {
        get { return SKADNETWORK_PLIST_ITEMS_KEY; }
    }
    public static string[] SKAdNetworkIDsArray
    {
        get { return SKADNETWORK_PLIST_ITEMS_VALUES; }
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public static class MyAnalytics
{
    public static void CustomEvent(string eventName, Dictionary<string, object> eventData)
    {
#if !UNITY_EDITOR
        //AnalyticsEvent.Custom(eventName, eventData);
#endif
    }
}

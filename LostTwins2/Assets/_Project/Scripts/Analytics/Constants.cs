using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants : MonoBehaviour
{
    public static class Prefs
    {
        public static string FirstSession = "first_session";
        public const string GameProgressionStates = "game_progression_states";
    }

    public static class Scenes
    {
        public static string MainScene = "Main";
        public static string FTUE_Scene = "Home _ trailer";
    }

    public static class Analytics
    {
        public const string FirstSession = "first_session";//
        public const string TutorialStart = "tutorial_start";//
        public const string TutorialCompletion = "tutorial_completion";//
        public const string LevelStart = "level_start";//
        public const string LevelComplete = "level_complete";//
        public const string LevelRestart = "level_restart";
        public const string LevelSkip = "level_skip";
        public const string CrystalCollected = "crystal_collected";//

        public const string LevelNumber = "level_number";//
        public const string TimeToCompleteLevel = "time_to_complete_level";//
        public const string CrystalsCollectedInThisLevel = "crystals_collected_in_this_level";//
        public const string TimeToCollectCrystal = "time_to_collect_crystal";//
        public const string CrystalNumber = "crystal_number";//
    }

    public static class Audio
    {
        public const string run = "run";
    }
}

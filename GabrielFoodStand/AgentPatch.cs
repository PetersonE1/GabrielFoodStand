using System;
using HarmonyLib;
using TMPro;
using GabrielFoodStand.Agent;
using UnityEngine;
using UnityEngine.UI;

namespace GabrielFoodStand.Patches
{
    [HarmonyPatch(typeof(LevelSelectPanel))]
    public static class LevelSelectPanelPatch
    {
        [HarmonyPatch("OnEnable"), HarmonyPostfix]
        static void OnEnablePostfix(LevelSelectPanel __instance)
        {
            Text text = __instance.transform.Find("Name").GetComponent<Text>();
            string levelName = AgentRegistry.LevelDatabase[__instance.levelNumber];
            if (!FoodData.Data.standsIncomplete.Contains(levelName))
                text.color = Color.green;
        }
    }

    [HarmonyPatch(typeof(FinalPit), "SendInfo")]
    public static class FinalPitEventPatch
    {
        public static void Postfix()
        {
            BestUtilityEverCreated.OnLevelComplete?.Invoke();
        }
    }

    [HarmonyPatch(typeof(PlayerActivatorRelay), "Activate")]
    public static class PlayerActivatorEventPatch
    {
        public static void Postfix()
        {
            BestUtilityEverCreated.OnPlayerActivated?.Invoke();
        }
    }
}

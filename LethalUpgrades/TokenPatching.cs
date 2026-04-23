using GameNetcodeStuff;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace LethalUpgrades.Patches;
internal class TokenPatching
{
    [HarmonyPatch(typeof(Terminal), "Update")]
    [HarmonyPostfix]
    static void DisplayTokens(Terminal __instance)
    {
        __instance.topRightText.text = $"${__instance.groupCredits} | Ŧ{LethalUpgradesBase.tokens}";
    }

    [HarmonyPatch(typeof(HUDManager), "FillEndGameStats")]
    [HarmonyPostfix]
    static void UpdateTokenMeter(HUDManager __instance)
    {
        var letter_grade = __instance.statsUIElements.gradeLetter.text;
        LethalUpgradesBase.mls.LogInfo($"Grade: {letter_grade}");
        switch (letter_grade)
        {
            case "S":
                LethalUpgradesBase.token_meter += 50;
                break;
            case "A":
                LethalUpgradesBase.token_meter += 30;
                break;
            case "B":
                LethalUpgradesBase.token_meter += 15;
                break;
            case "C":
                LethalUpgradesBase.token_meter += 10;
                break;
            case "F":
                LethalUpgradesBase.token_meter -= 10;
                if(LethalUpgradesBase.token_meter<0){LethalUpgradesBase.token_meter=0;}
                break;
        }

        if(LethalUpgradesBase.token_meter >= 100)
        {
            LethalUpgradesBase.tokens += 1;
            LethalUpgradesBase.token_meter = LethalUpgradesBase.token_meter-100;
            __instance.StartCoroutine(DelayedTipDisplay(__instance));
            LethalUpgradesBase.mls.LogInfo($"Token Meter: {LethalUpgradesBase.token_meter}/100");
            return;
        }
        LethalUpgradesBase.mls.LogInfo($"Token Meter: {LethalUpgradesBase.token_meter}/100");
    }

    private static System.Collections.IEnumerator DelayedTipDisplay(HUDManager __instance)
    {
        yield return new WaitForSeconds(20f);
        __instance.DisplayTip("Lethal Upgrades", "Your performance thus far was considered acceptable. An upgrade token has been transferred!.");
    }
}
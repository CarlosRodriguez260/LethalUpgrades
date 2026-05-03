using GameNetcodeStuff;
using HarmonyLib;
using LethalNetworkAPI.Utils;
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
        // Token Meter is host only
        if(!LNetworkUtils.IsHostOrServer) return;
        
        var letter_grade = __instance.statsUIElements.gradeLetter.text;
        LethalUpgradesBase.mls.LogInfo($"Grade: {letter_grade}");
        switch (letter_grade)
        {
            case "S":
                LethalUpgradesBase.token_meter += 50;
                break;
            case "A":
                LethalUpgradesBase.token_meter += 35; 
                break;
            case "B":
                LethalUpgradesBase.token_meter += 10; 
                break;
            case "C":
                LethalUpgradesBase.token_meter += 5;
                break;
            case "F":
                LethalUpgradesBase.token_meter -= 10;
                if(LethalUpgradesBase.token_meter<0){LethalUpgradesBase.token_meter=0;}
                break;
        }

        if(LethalUpgradesBase.token_meter >= 100)
        {
            LethalUpgradesBase.mls.LogInfo("Filled Token Meter!");
            LethalUpgradesBase.token_meter = LethalUpgradesBase.token_meter-100;
            __instance.StartCoroutine(DelayedTipDisplay(__instance));
            LethalUpgradesBase.mls.LogInfo($"Token Meter: {LethalUpgradesBase.token_meter}/100");
            return;
        }
        LethalUpgradesBase.mls.LogInfo($"Token Meter: {LethalUpgradesBase.token_meter}/100");
    }

    private static System.Collections.IEnumerator DelayedTipDisplay(HUDManager __instance)
    {
        yield return new WaitForSeconds(22f);
        LethalUpgradesNetwork.tokens.Value += 1;
        __instance.DisplayTip("Lethal Upgrades", "Your performance thus far was considered acceptable. An upgrade token has been transferred!");
    }

    [HarmonyPatch(typeof(StartOfRound), "Start")]
    [HarmonyPostfix]
    static void PreScumCreditFix()
    {
        LethalUpgradesBase.mls.LogInfo("StartOfRound initiated!");
        LethalUpgradesBase.mls.LogInfo($"Pre-scum credits = {LethalUpgradesBase.pre_scum_scredits}");
        if(LethalUpgradesBase.pre_scum_scredits > 0)
        {
            LethalUpgradesBase.mls.LogInfo("Setting pre-scum credits. Check one.");
            var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
            if(terminal == null)
            {
                LethalUpgradesBase.mls.LogInfo("Tried setting pre-scum credits, but did not find terminal");
                return;
            }
            CreditFix(terminal);
        }
    }

    static async Task CreditFix(Terminal terminal)
    {
        await Task.Delay(1000);
        LethalUpgradesBase.mls.LogInfo("Setting pre-scum credits. Check two.");
        terminal.SyncGroupCreditsServerRpc(LethalUpgradesBase.pre_scum_scredits, terminal.numberOfItemsInDropship);
        LethalUpgradesBase.mls.LogInfo("Setting pre-scum credits. Succesful.");
    }
}
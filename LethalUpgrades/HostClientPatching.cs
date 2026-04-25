using LethalNetworkAPI.Utils;
using LethalNetworkAPI;
using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;
using OdinSerializer;

namespace LethalUpgrades.Patches;
public class HostClientPatching
{
    [HarmonyPatch(typeof(StartOfRound), "OnClientConnect")]
    [HarmonyPostfix]
    static void SyncVarWithHost()
    {
        if (LNetworkUtils.IsHostOrServer) return;
        LethalUpgradesNetwork.syncing = true;
        LethalUpgradesNetwork.syncer.InvokeServer();
        LethalUpgradesNetwork.syncing = false;
    }
}
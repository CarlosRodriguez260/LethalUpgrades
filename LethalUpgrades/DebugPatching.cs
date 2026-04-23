using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalUpgrades.Patches;
internal class DebugPatching
{
    private static float lastLogTime = 0f;
    private static float logInterval = 1f;
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void LogUpdater()
    {
        if (Time.time - lastLogTime >= logInterval)
        {
            lastLogTime = Time.time;
            var player = GameNetworkManager.Instance?.localPlayerController;
            var terminalNode = UnityEngine.Object.FindFirstObjectByType<Terminal>();
            if (player != null)
            {
                LethalUpgradesBase.mls.LogInfo($"Movement Speed: {player.movementSpeed:R}");
                LethalUpgradesBase.mls.LogInfo($"Crouching?: {player.isCrouching}");
                LethalUpgradesBase.mls.LogInfo($"Jump Strength: {player.jumpForce}");
                LethalUpgradesBase.mls.LogInfo($"Terminal In Use?: {terminalNode.terminalInUse}");
            }
        }
    }
}

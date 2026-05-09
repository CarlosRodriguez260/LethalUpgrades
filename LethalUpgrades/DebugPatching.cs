using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace LethalUpgrades.Patches;
internal class DebugPatching
{
    private static float lastLogTime = 0f;
    private static float logInterval = 3f;
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void LogUpdater()
    {
        var player = GameNetworkManager.Instance?.localPlayerController;
        if (Time.time - lastLogTime >= logInterval)
        {
            lastLogTime = Time.time;
            // var terminalNode = UnityEngine.Object.FindFirstObjectByType<Terminal>();
            // if(player != null)
            // {
            //     LethalUpgradesBase.mls.LogInfo($"Sprint Time: {player.sprintTime:R}");
            //     LethalUpgradesBase.mls.LogInfo($"Movement Speed: {player.movementSpeed:R}");
            //     LethalUpgradesBase.mls.LogInfo($"Crouching?: {player.isCrouching}");
            //     LethalUpgradesBase.mls.LogInfo($"Jump Strength: {player.jumpForce}");
            //     // LethalUpgradesBase.mls.LogInfo($"Terminal In Use?: {terminalNode.terminalInUse}");
            //     LethalUpgradesBase.mls.LogInfo($"Carry Weight: {player.carryWeight}");
            // }

            var rm = RoundManager.Instance;
            var level = rm.currentLevel;
            // if(rm != null)
            // {
            //     var moon = rm.currentLevel;
            //     LethalUpgradesBase.mls.LogInfo($"Looking at {moon.PlanetName} enemy list");
            //     foreach(SpawnableEnemyWithRarity enemy in moon.Enemies)
            //     {
            //         LethalUpgradesBase.mls.LogInfo($"Enemy: {enemy.enemyType.enemyName} | Rarity: {enemy.rarity}");
            //     }
            // }

            var scrap_list = rm.currentLevel.spawnableScrap;
            LethalUpgradesBase.mls.LogInfo($"Looking at {rm.currentLevel.PlanetName} scrap table.");
            foreach(SpawnableItemWithRarity scrap in scrap_list)
            {
                LethalUpgradesBase.mls.LogInfo($"Scrap {scrap.spawnableItem.itemName} | Rarity {scrap.rarity}");
            }
        }
    }
}

namespace LethalUpgrades.Patches;
using LethalModDataLib.Features;
using LethalModDataLib.Enums;
using LethalNetworkAPI.Utils;
using HarmonyLib;

internal class SaveLoadUpgradesPatching
{
    public static void SaveAllUpgradeData()
    {
        if(!LNetworkUtils.IsHostOrServer) return;

        LethalUpgradesBase.mls.LogInfo("Trying to save upgrade data...");

        // Tokens and currency
        SaveLoadHandler.SaveData(LethalUpgradesBase.tokens, "chuito_tokens", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.token_meter, "chuito_token_meter", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.reroll, "chuito_reroll", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.pre_scum_scredits, "chuito_pre_scum_scredits", SaveLocation.CurrentSave, true);
            
        // Health upgrades
        SaveLoadHandler.SaveData(LethalUpgradesBase.health_t1, "chuito_health_t1", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.health_t2, "chuito_health_t2", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.health_t3, "chuito_health_t3", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.health_leg, "chuito_health_leg", SaveLocation.CurrentSave, true);
            
        // Stamina upgrades
        SaveLoadHandler.SaveData(LethalUpgradesBase.stamina_t1, "chuito_stamina_t1", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.stamina_t2, "chuito_stamina_t2", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.stamina_t3, "chuito_stamina_t3", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.stamina_leg, "chuito_stamina_leg", SaveLocation.CurrentSave, true);
            
        // Movement upgrades
        SaveLoadHandler.SaveData(LethalUpgradesBase.movement_t1, "chuito_movement_t1", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.movement_t2, "chuito_movement_t2", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.movement_t3, "chuito_movement_t3", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.movement_leg, "chuito_movement_leg", SaveLocation.CurrentSave, true);
            
        // Utility upgrades
        SaveLoadHandler.SaveData(LethalUpgradesBase.utility_t1, "chuito_utility_t1", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.utility_t2, "chuito_utility_t2", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.utility_t3, "chuito_utility_t3", SaveLocation.CurrentSave, true);
        SaveLoadHandler.SaveData(LethalUpgradesBase.utility_leg, "chuito_utility_leg", SaveLocation.CurrentSave, true);

        LethalUpgradesBase.mls.LogInfo("All upgrade data saved successfully");
    }

    [HarmonyPatch(typeof(StartOfRound), "Awake")]
    [HarmonyPostfix]
    public static void LoadAllUpgradeData()
    {
        if(!LNetworkUtils.IsHostOrServer) return;

        LethalUpgradesBase.mls.LogInfo("Trying to load upgrade data...");

        // Tokens and currency
        LethalUpgradesBase.tokens = SaveLoadHandler.LoadData<int>("chuito_tokens", SaveLocation.CurrentSave, defaultValue: 0, autoAddGuid: true);
        LethalUpgradesBase.token_meter = SaveLoadHandler.LoadData<int>("chuito_token_meter", SaveLocation.CurrentSave, defaultValue: 0, autoAddGuid: true);
        LethalUpgradesBase.reroll = SaveLoadHandler.LoadData<bool>("chuito_reroll", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.pre_scum_scredits = SaveLoadHandler.LoadData<int>("chuito_pre_scum_scredits", SaveLocation.CurrentSave, defaultValue: 0, autoAddGuid: true);
            
        // Health upgrades
        LethalUpgradesBase.health_t1 = SaveLoadHandler.LoadData<bool>("chuito_health_t1", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.health_t2 = SaveLoadHandler.LoadData<bool>("chuito_health_t2", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.health_t3 = SaveLoadHandler.LoadData<bool>("chuito_health_t3", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.health_leg = SaveLoadHandler.LoadData<bool>("chuito_health_leg", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
            
        // Stamina upgrades
        LethalUpgradesBase.stamina_t1 = SaveLoadHandler.LoadData<bool>("chuito_stamina_t1", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.stamina_t2 = SaveLoadHandler.LoadData<bool>("chuito_stamina_t2", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.stamina_t3 = SaveLoadHandler.LoadData<bool>("chuito_stamina_t3", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.stamina_leg = SaveLoadHandler.LoadData<bool>("chuito_stamina_leg", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
            
        // Movement upgrades
        LethalUpgradesBase.movement_t1 = SaveLoadHandler.LoadData<bool>("chuito_movement_t1", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.movement_t2 = SaveLoadHandler.LoadData<bool>("chuito_movement_t2", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.movement_t3 = SaveLoadHandler.LoadData<bool>("chuito_movement_t3", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.movement_leg = SaveLoadHandler.LoadData<bool>("chuito_movement_leg", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
            
        // Utility upgrades
        LethalUpgradesBase.utility_t1 = SaveLoadHandler.LoadData<bool>("chuito_utility_t1", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.utility_t2 = SaveLoadHandler.LoadData<bool>("chuito_utility_t2", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.utility_t3 = SaveLoadHandler.LoadData<bool>("chuito_utility_t3", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);
        LethalUpgradesBase.utility_leg = SaveLoadHandler.LoadData<bool>("chuito_utility_leg", SaveLocation.CurrentSave, defaultValue: false, autoAddGuid: true);

        LethalUpgradesBase.mls.LogInfo("All upgrade data loaded successfully");
    }
}
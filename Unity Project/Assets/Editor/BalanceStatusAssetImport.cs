using System;
using System.Linq;
using Scripts.Enchantment;
using Scripts.Status;
using Scripts.TierManipulation;
using UnityEditor;
using UnityEngine;

public static class BalanceStatusAssetImport
{
    [MenuItem("Tools/Balance/Create Status Enchantments")]
    public static void Run()
    {
        const string folder = "Assets/Scriptable Object/Status Enchantment";
        if (!AssetDatabase.IsValidFolder(folder)) throw new Exception("Missing destination: " + folder);
        var types = new[] { StatusType.HEALTH, StatusType.PHYSIC_FORCE, StatusType.MAGIC_FORCE,
            StatusType.CRITICAL_CHANCE, StatusType.CRITICAL_DAMAGE, StatusType.STAMINA,
            StatusType.ATTACK_SPEED, StatusType.LIFE_STEAL, StatusType.VAMP };
        var names = new[] { "Vital Spark", "Iron Resolve", "Arcane Ember", "Keen Instinct",
            "Ruthless Edge", "Second Wind", "Swift Rhythm", "Blood Pact", "Soul Siphon" };
        var ranks = new[] { "Lesser", "Greater", "Exalted", "Mythic" };
        var values = new float[,] {
            {50,5,5,2,10,10,2.5f,0.2f,0.2f},
            {100,10,10,4,20,20,4,0.4f,0.4f},
            {160,15,15,6,30,30,6,0.7f,0.7f},
            {250,25,25,8,45,45,8,1,1} };
        var percentNames = new[] { "Heart of Eternity", "Titan Ascension", "Arcane Awakening", "Merciless Fate", "Boundless Vigor" };
        var percentIndices = new[] { 0, 1, 2, 4, 5 };
        // Validate every destination before creating anything; never overwrite user assets.
        var paths = (from tier in Enumerable.Range(0,4)
                     from i in Enumerable.Range(0,9)
                     select folder + "/" + ranks[tier] + " " + names[i] + "_StatusUpgradeSO.asset")
            .Concat(percentNames.Select(n => folder + "/" + n + "_StatusUpgradeSO.asset")).ToArray();
        foreach (string path in paths)
            if (AssetDatabase.LoadMainAssetAtPath(path) != null) throw new Exception("Already exists: " + path);
        int count = 0;
        for (int tier = 0; tier < 4; tier++)
            for (int i = 0; i < types.Length; i++)
                Create(paths[count++], ranks[tier] + " " + names[i], (Tier)tier, types[i], values[tier,i], false);
        for (int i = 0; i < percentIndices.Length; i++)
            Create(paths[count++], percentNames[i], Tier.TIER_4, types[percentIndices[i]], 40, true);
        Debug.Log("BALANCE_IMPORT_OK: " + count + " assets created and verified. No decks changed.");
    }

    private static void Create(string path, string title, Tier tier, StatusType type, float value, bool percent)
    {
        var asset = ScriptableObject.CreateInstance<StatusUpgradeSO>();
        asset.ID = Guid.NewGuid().ToString();
        asset.DisplayName = title;
        asset.Tier = tier;
        asset.EnchantmentType = EnchantmentType.STATUS;
        asset.Description = "<statusmodifier index=0>";
        var serialized = new SerializedObject(asset);
        serialized.FindProperty("_canRepeat").boolValue = !percent;
        var modifiers = serialized.FindProperty("_list_StatusModifiers");
        modifiers.arraySize = 1;
        var modifier = modifiers.GetArrayElementAtIndex(0);
        modifier.FindPropertyRelative("StatusType").intValue = (int)type;
        modifier.FindPropertyRelative("ValueType").intValue = (int)(percent ? EnchantmentValueType.PERCENT_OF_CURRENT : EnchantmentValueType.FLAT);
        modifier.FindPropertyRelative("Value").floatValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssetIfDirty(asset);
        var readback = AssetDatabase.LoadAssetAtPath<StatusUpgradeSO>(path);
        if (readback == null || readback.Tier != tier || readback.CanRepeat != !percent ||
            readback.List_StatusModifiers.Count != 1 || readback.List_StatusModifiers[0].StatusType != type ||
            readback.List_StatusModifiers[0].Value != value)
            throw new Exception("Verification failed: " + path);
    }
}

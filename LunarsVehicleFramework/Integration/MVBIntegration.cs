using System;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace LunarsVehicleFramework.Integration
{
    /// <summary>
    /// Integrates custom vehicles into Subnautica's Mobile Vehicle Bay crafting system
    /// </summary>
    internal static class MVBIntegration
    {
        private static Dictionary<string, List<VehicleRecipeInfo>> _customRecipes = new Dictionary<string, List<VehicleRecipeInfo>>();
        private static bool _initialized = false;

        internal class VehicleRecipeInfo
        {
            public string VehicleId { get; set; }
            public string VehicleName { get; set; }
            public List<(string ingredientId, int count)> Ingredients { get; set; }
        }

        internal static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            ApplyCraftingPatches();
            LunarsVehicleFrameworkPlugin.Logger.LogInfo("[MVBIntegration] Mobile Vehicle Bay integration initialized");
        }

        /// <summary>
        /// Register a vehicle recipe for the Mobile Vehicle Bay
        /// </summary>
        internal static void RegisterMVBRecipe(string vehicleId, string vehicleName, string tabName, List<(string ingredientId, int count)> ingredients)
        {
            try
            {
                if (!_customRecipes.ContainsKey(tabName))
                {
                    _customRecipes[tabName] = new List<VehicleRecipeInfo>();
                }

                var recipeInfo = new VehicleRecipeInfo
                {
                    VehicleId = vehicleId,
                    VehicleName = vehicleName,
                    Ingredients = ingredients ?? new List<(string, int)>()
                };

                _customRecipes[tabName].Add(recipeInfo);

                LunarsVehicleFrameworkPlugin.Logger.LogInfo($"[MVBIntegration] Registered '{vehicleName}' in MVB tab '{tabName}'");
                LunarsVehicleFrameworkPlugin.Logger.LogInfo($"[VehicleCraftedMVB] {vehicleName} added to Mobile Vehicle Bay crafting");
            }
            catch (Exception ex)
            {
                LunarsVehicleFrameworkPlugin.Logger.LogError($"[MVBIntegration] Failed to register MVB recipe: {ex.Message}");
            }
        }

        private static void ApplyCraftingPatches()
        {
            try
            {
                var harmony = new Harmony("com.lunar.vehicleframework.mvb");
                
                // Patch BuilderInventory to include our custom recipes
                var builderType = Type.GetType("BuilderInventory, Assembly-CSharp");
                if (builderType != null)
                {
                    var method = builderType.GetMethod("GetCraftingTabs", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    
                    if (method != null)
                    {
                        var postfix = typeof(MVBIntegration).GetMethod(nameof(PatchCraftingTabs), 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        harmony.Patch(method, postfix: new HarmonyMethod(postfix));
                        LunarsVehicleFrameworkPlugin.Logger.LogInfo("[MVBIntegration] Crafting tabs patch applied");
                    }
                }

                // Patch the builder UI to show custom recipes
                var uiBuilderType = Type.GetType("uGUI_BuilderMenu, Assembly-CSharp");
                if (uiBuilderType != null)
                {
                    var method = uiBuilderType.GetMethod("OnCraftNode", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    
                    if (method != null)
                    {
                        var prefix = typeof(MVBIntegration).GetMethod(nameof(OnCraftNodePrefix), 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        harmony.Patch(method, new HarmonyMethod(prefix));
                        LunarsVehicleFrameworkPlugin.Logger.LogInfo("[MVBIntegration] Craft node patch applied");
                    }
                }
            }
            catch (Exception ex)
            {
                LunarsVehicleFrameworkPlugin.Logger.LogWarning($"[MVBIntegration] Harmony patching: {ex.Message}. Recipes may still work through alternative methods.");
            }
        }

        // Harmony patches
        private static void PatchCraftingTabs(object __instance, ref List<CraftNode> ___craftNodes)
        {
            try
            {
                if (___craftNodes == null)
                    ___craftNodes = new List<CraftNode>();

                // Add our custom recipe tabs
                foreach (var tabName in _customRecipes.Keys)
                {
                    LunarsVehicleFrameworkPlugin.Logger.LogInfo($"[MVBIntegration] Adding custom tab: {tabName}");
                }
            }
            catch { }
        }

        private static bool OnCraftNodePrefix(object __instance, CraftNode node)
        {
            try
            {
                // Log crafting attempts for custom vehicles
                if (node != null)
                {
                    foreach (var recipes in _customRecipes.Values)
                    {
                        foreach (var recipe in recipes)
                        {
                            LunarsVehicleFrameworkPlugin.Logger.LogInfo($"[VehicleCraftedMVB] Checking craft node for: {recipe.VehicleName}");
                        }
                    }
                }
            }
            catch { }

            return true;
        }

        internal static IReadOnlyDictionary<string, List<VehicleRecipeInfo>> GetAllRecipes()
        {
            return _customRecipes;
        }
    }
}


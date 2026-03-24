using System;
using System.Collections.Generic;
using UnityEngine;

namespace LunarsVehicleFramework.Models
{
    /// <summary>
    /// Handles AssetBundle management and conversion
    /// </summary>
    public static class BundleConverter
    {
        private static Dictionary<string, AssetBundle> _loadedBundles = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// Load an AssetBundle from file
        /// </summary>
        public static AssetBundle LoadBundle(string bundlePath)
        {
            if (string.IsNullOrEmpty(bundlePath))
                throw new ArgumentNullException(nameof(bundlePath));

            // Check if already loaded
            if (_loadedBundles.TryGetValue(bundlePath, out var cachedBundle) && cachedBundle != null)
                return cachedBundle;

            try
            {
                var bundle = AssetBundle.LoadFromFile(bundlePath);
                if (bundle == null)
                    throw new InvalidOperationException($"Failed to load AssetBundle: {bundlePath}");

                _loadedBundles[bundlePath] = bundle;
                LunarsVehicleFrameworkPlugin.Logger.LogInfo($"AssetBundle loaded: {bundlePath}");

                return bundle;
            }
            catch (Exception ex)
            {
                LunarsVehicleFrameworkPlugin.Logger.LogError($"Error loading bundle '{bundlePath}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Load an asset from a bundle
        /// </summary>
        public static T LoadAsset<T>(AssetBundle bundle, string assetName) where T : UnityEngine.Object
        {
            if (bundle == null)
                throw new ArgumentNullException(nameof(bundle));

            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException(nameof(assetName));

            try
            {
                var asset = bundle.LoadAsset<T>(assetName);
                if (asset == null)
                    throw new InvalidOperationException($"Asset '{assetName}' not found in bundle");

                return asset;
            }
            catch (Exception ex)
            {
                LunarsVehicleFrameworkPlugin.Logger.LogError($"Error loading asset '{assetName}': {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Unload a bundle and free its resources
        /// </summary>
        public static void UnloadBundle(string bundlePath, bool unloadAllLoadedObjects = false)
        {
            if (_loadedBundles.TryGetValue(bundlePath, out var bundle) && bundle != null)
            {
                bundle.Unload(unloadAllLoadedObjects);
                _loadedBundles.Remove(bundlePath);
                LunarsVehicleFrameworkPlugin.Logger.LogInfo($"AssetBundle unloaded: {bundlePath}");
            }
        }

        /// <summary>
        /// Unload all loaded bundles
        /// </summary>
        public static void UnloadAllBundles(bool unloadAllLoadedObjects = false)
        {
            foreach (var bundle in _loadedBundles.Values)
            {
                if (bundle != null)
                    bundle.Unload(unloadAllLoadedObjects);
            }

            _loadedBundles.Clear();
            LunarsVehicleFrameworkPlugin.Logger.LogInfo("All AssetBundles unloaded");
        }

        /// <summary>
        /// Get all asset names from a bundle
        /// </summary>
        public static string[] GetAllAssetNames(AssetBundle bundle)
        {
            if (bundle == null)
                throw new ArgumentNullException(nameof(bundle));

            return bundle.GetAllAssetNames();
        }

        /// <summary>
        /// Create a bundle manifest for tracking bundle contents
        /// </summary>
        public static BundleManifest CreateManifest(string bundleName, params string[] assetNames)
        {
            return new BundleManifest
            {
                BundleName = bundleName,
                CreatedAt = DateTime.Now,
                AssetNames = new List<string>(assetNames)
            };
        }
    }

    /// <summary>
    /// Manifest data for tracking bundle contents
    /// </summary>
    public class BundleManifest
    {
        public string BundleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> AssetNames { get; set; } = new List<string>();
    }
}

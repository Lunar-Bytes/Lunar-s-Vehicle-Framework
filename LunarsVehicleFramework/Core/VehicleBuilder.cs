using System;
using System.Collections.Generic;
using UnityEngine;

namespace LunarsVehicleFramework.Core
{
    /// <summary>
    /// Builder class for easily creating new vehicle types in Subnautica
    /// </summary>
    public class VehicleBuilder
    {
        private string _vehicleId;
        private string _vehicleName;
        private string _modelPath;
        private Vector3 _scale = Vector3.one;
        private Dictionary<string, object> _properties = new Dictionary<string, object>();
        private List<VehicleRecipeData> _recipes = new List<VehicleRecipeData>();
        private string _mvbTabName = "Vehicles"; // Default tab
        private bool _enableLogging = true;
        private bool _enableSpawnCommand = true;

        public VehicleBuilder(string vehicleId, string vehicleName)
        {
            _vehicleId = vehicleId ?? throw new ArgumentNullException(nameof(vehicleId));
            _vehicleName = vehicleName ?? throw new ArgumentNullException(nameof(vehicleName));
        }

        /// <summary>
        /// Set the 3D model file path (.fbx, .glTF, etc.)
        /// </summary>
        public VehicleBuilder WithModel(string modelPath)
        {
            _modelPath = modelPath ?? throw new ArgumentNullException(nameof(modelPath));
            return this;
        }

        /// <summary>
        /// Set the scale of the vehicle model
        /// </summary>
        public VehicleBuilder WithScale(float x, float y, float z)
        {
            _scale = new Vector3(x, y, z);
            return this;
        }

        /// <summary>
        /// Add a custom property to the vehicle
        /// </summary>
        public VehicleBuilder WithProperty(string key, object value)
        {
            _properties[key] = value;
            return this;
        }

        /// <summary>
        /// Add a recipe for crafting this vehicle (e.g., in the Mobile Vehicle Bay)
        /// </summary>
        public VehicleBuilder AddRecipe(string fabricatorId, int outputCount, params (string ingredientId, int count)[] ingredients)
        {
            var recipe = new VehicleRecipeData
            {
                FabricatorId = fabricatorId,
                OutputCount = outputCount,
                Ingredients = new List<(string, int)>(ingredients)
            };

            _recipes.Add(recipe);
            return this;
        }

        /// <summary>
        /// Set the Mobile Vehicle Bay tab where this vehicle will appear
        /// </summary>
        public VehicleBuilder WithMVBTab(string tabName)
        {
            _mvbTabName = tabName ?? throw new ArgumentNullException(nameof(tabName));
            return this;
        }

        /// <summary>
        /// Enable or disable automatic logging (default: enabled)
        /// </summary>
        public VehicleBuilder WithLogging(bool enabled)
        {
            _enableLogging = enabled;
            return this;
        }

        /// <summary>
        /// Enable or disable automatic spawn command (default: enabled)
        /// </summary>
        public VehicleBuilder WithSpawnCommand(bool enabled)
        {
            _enableSpawnCommand = enabled;
            return this;
        }

        /// <summary>
        /// Build and register the vehicle
        /// </summary>
        public void Build()
        {
            if (string.IsNullOrEmpty(_modelPath))
                throw new InvalidOperationException("Model path must be set using WithModel()");

            var vehicle = new VehicleData
            {
                Id = _vehicleId,
                Name = _vehicleName,
                ModelPath = _modelPath,
                Scale = _scale,
                Properties = _properties,
                Recipes = _recipes,
                MVBTabName = _mvbTabName,
                EnableLogging = _enableLogging,
                EnableSpawnCommand = _enableSpawnCommand
            };

            VehicleRegistry.Register(vehicle);
            
            if (_enableLogging)
            {
                LunarsVehicleFrameworkPlugin.Logger.LogInfo($"[VehicleLoad] Vehicle '{_vehicleName}' (ID: {_vehicleId}) loaded successfully");
            }

            if (_enableSpawnCommand)
            {
                VehicleCommandHandler.RegisterSpawnCommand(_vehicleId, _vehicleName);
            }
        }
    }

    internal class VehicleData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ModelPath { get; set; }
        public Vector3 Scale { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public List<VehicleRecipeData> Recipes { get; set; } = new List<VehicleRecipeData>();
        public string MVBTabName { get; set; } = "Vehicles";
        public bool EnableLogging { get; set; } = true;
        public bool EnableSpawnCommand { get; set; } = true;
    }

    internal class VehicleRecipeData
    {
        public string FabricatorId { get; set; }
        public int OutputCount { get; set; }
        public List<(string Id, int Count)> Ingredients { get; set; }
    }

    internal static class VehicleRegistry
    {
        private static Dictionary<string, VehicleData> _vehicles = new Dictionary<string, VehicleData>();

        internal static void Register(VehicleData vehicle)
        {
            if (_vehicles.ContainsKey(vehicle.Id))
                throw new InvalidOperationException($"Vehicle with ID '{vehicle.Id}' is already registered");

            _vehicles[vehicle.Id] = vehicle;
        }

        internal static VehicleData Get(string vehicleId)
        {
            if (_vehicles.TryGetValue(vehicleId, out var vehicle))
                return vehicle;

            throw new KeyNotFoundException($"Vehicle with ID '{vehicleId}' not found");
        }

        internal static IReadOnlyDictionary<string, VehicleData> GetAll()
        {
            return _vehicles;
        }
    }

    internal static class VehicleCommandHandler
    {
        private static Dictionary<string, (string id, string name)> _spawnCommands = new Dictionary<string, (string, string)>();

        internal static void RegisterSpawnCommand(string vehicleId, string vehicleName)
        {
            _spawnCommands[vehicleId.ToLower()] = (vehicleId, vehicleName);
            LunarsVehicleFrameworkPlugin.Logger.LogInfo($"[SpawnCommand] Registered command: spawn {vehicleId}");
        }

        /// <summary>
        /// Execute spawn command (called by mod or console)
        /// </summary>
        public static void ExecuteSpawnCommand(string vehicleId)
        {
            if (_spawnCommands.TryGetValue(vehicleId.ToLower(), out var vehicle))
            {
                LunarsVehicleFrameworkPlugin.Logger.LogInfo($"[VehicleSpawned] Spawning vehicle: {vehicle.name} (ID: {vehicle.id})");
                // Actual spawn logic would be implemented by the game integration
            }
            else
            {
                LunarsVehicleFrameworkPlugin.Logger.LogWarning($"[SpawnCommand] Vehicle not found: {vehicleId}");
            }
        }

        internal static IReadOnlyDictionary<string, (string id, string name)> GetAllSpawnCommands()
        {
            return _spawnCommands;
        }
    }
}

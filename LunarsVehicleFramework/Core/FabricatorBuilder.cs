using System;
using System.Collections.Generic;

namespace LunarsVehicleFramework.Core
{
    /// <summary>
    /// Builder class for easily creating new fabricators in Subnautica
    /// </summary>
    public class FabricatorBuilder
    {
        private string _fabricatorId;
        private string _fabricatorName;
        private List<RecipeData> _recipes = new List<RecipeData>();
        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public FabricatorBuilder(string fabricatorId, string fabricatorName)
        {
            _fabricatorId = fabricatorId ?? throw new ArgumentNullException(nameof(fabricatorId));
            _fabricatorName = fabricatorName ?? throw new ArgumentNullException(nameof(fabricatorName));
        }

        /// <summary>
        /// Add a recipe to the fabricator
        /// </summary>
        public FabricatorBuilder AddRecipe(string outputId, int outputCount, params (string ingredientId, int count)[] ingredients)
        {
            var recipe = new RecipeData
            {
                OutputId = outputId,
                OutputCount = outputCount,
                Ingredients = new List<(string, int)>(ingredients)
            };

            _recipes.Add(recipe);
            return this;
        }

        /// <summary>
        /// Add a custom property to the fabricator
        /// </summary>
        public FabricatorBuilder WithProperty(string key, object value)
        {
            _properties[key] = value;
            return this;
        }

        /// <summary>
        /// Build and register the fabricator
        /// </summary>
        public void Build()
        {
            var fabricator = new FabricatorData
            {
                Id = _fabricatorId,
                Name = _fabricatorName,
                Recipes = _recipes,
                Properties = _properties
            };

            FabricatorRegistry.Register(fabricator);
            LunarsVehicleFrameworkPlugin.Logger.LogInfo($"Fabricator '{_fabricatorName}' registered with {_recipes.Count} recipes");
        }
    }

    internal class RecipeData
    {
        public string OutputId { get; set; }
        public int OutputCount { get; set; }
        public List<(string Id, int Count)> Ingredients { get; set; }
    }

    internal class FabricatorData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<RecipeData> Recipes { get; set; }
        public Dictionary<string, object> Properties { get; set; }
    }

    internal static class FabricatorRegistry
    {
        private static Dictionary<string, FabricatorData> _fabricators = new Dictionary<string, FabricatorData>();

        internal static void Register(FabricatorData fabricator)
        {
            if (_fabricators.ContainsKey(fabricator.Id))
                throw new InvalidOperationException($"Fabricator with ID '{fabricator.Id}' is already registered");

            _fabricators[fabricator.Id] = fabricator;
        }

        internal static FabricatorData Get(string fabricatorId)
        {
            if (_fabricators.TryGetValue(fabricatorId, out var fabricator))
                return fabricator;

            throw new KeyNotFoundException($"Fabricator with ID '{fabricatorId}' not found");
        }

        internal static IReadOnlyDictionary<string, FabricatorData> GetAll()
        {
            return _fabricators;
        }
    }
}

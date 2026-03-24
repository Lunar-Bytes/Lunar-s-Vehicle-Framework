using System;
using BepInEx;
using BepInEx.Logging;

namespace LunarsVehicleFramework
{
        [BepInPlugin("com.lunar.vehicleframework", "Lunar's Vehicle Framework", "1.2.0")]
    [BepInDependency("com.bepinex.bepinexpack")]
    public class LunarsVehicleFrameworkPlugin : BaseUnityPlugin
    {
        public static new ManualLogSource Logger { get; private set; }

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo("Lunar's Vehicle Framework loaded successfully!");
        }

        private void OnDestroy()
        {
            Logger?.LogInfo("Lunar's Vehicle Framework unloaded");
        }
    }
}

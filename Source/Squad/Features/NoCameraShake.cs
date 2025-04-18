using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class NoCameraShake : Manager
    {
        public const string NAME = "NoCameraShake";
        
        public bool _isNoCameraShakeEnabled = false;

        public NoCameraShake(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable no camera shake - local player is not valid");
                return;
            }
            
            _isNoCameraShakeEnabled = enable;
            Logger.Debug($"[{NAME}] No camera shake {(enable ? "enabled" : "disabled")}");
            Apply();
        }

        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply no camera shake - local player is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] === {(_isNoCameraShakeEnabled ? "ENABLING" : "DISABLING")} NO CAMERA SHAKE ===");

                // Get camera manager
                ulong cameraManagerPtr = Memory.ReadPtr(_playerController + PlayerController.PlayerCameraManager);
                if (cameraManagerPtr == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no camera shake - camera manager is not valid");
                    return;
                }

                // Get camera shake modifier
                ulong cameraShakeModPtr = Memory.ReadPtr(cameraManagerPtr + PlayerCameraManager.CachedCameraShakeMod);
                if (cameraShakeModPtr == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no camera shake - camera shake modifier is not valid");
                    return;
                }

                // Get active shakes data
                ulong activeShakesDataPtr = Memory.ReadPtr(cameraShakeModPtr + UCameraModifier_CameraShake.ActiveShakes);

                // Get number of active shakes
                int activeShakesCount = Memory.ReadValue<int>(cameraShakeModPtr + UCameraModifier_CameraShake.ActiveShakes + 0x8);
                if (activeShakesCount <= 0)
                {
                    Logger.Debug($"[{NAME}] No active camera shakes to modify");
                    return;
                }

                // Create scatter write entries for each active shake
                var scatterEntries = new List<IScatterWriteDataEntry<float>>();
                const int shakeInfoSize = 0x18;

                for (int i = 0; i < activeShakesCount; i++)
                {
                    ulong shakeBasePtr = Memory.ReadPtr(activeShakesDataPtr + (uint)(i * shakeInfoSize));
                    if (shakeBasePtr != 0 && _isNoCameraShakeEnabled)
                    {
                        scatterEntries.Add(new ScatterWriteDataEntry<float>(shakeBasePtr + UCameraShakeBase.ShakeScale, 0f));
                    }
                }

                if (scatterEntries.Count > 0)
                {
                    Memory.WriteScatter(scatterEntries);
                    Logger.Debug($"[{NAME}] Successfully modified {scatterEntries.Count} camera shakes");
                }

                Logger.Debug($"[{NAME}] =============================");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting no camera shake: {ex.Message}");
            }
        }
    }
} 
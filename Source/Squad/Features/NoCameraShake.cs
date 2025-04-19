using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class NoCameraShake : Manager
    {
        public const string NAME = "NoCameraShake";
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isEnabled;
        private float _originalShakeScale;
        private bool _hasOriginalValue;
        
        public NoCameraShake(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _isEnabled = false;
            _hasOriginalValue = false;
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable no camera shake - local player is not valid");
                return;
            }
            
            _isEnabled = enable;
            Logger.Debug($"[{NAME}] No camera shake {(enable ? "enabled" : "disabled")}");
            
            if (enable)
            {
                StartTimer();
            }
            else
            {
                StopTimer();
                RestoreOriginalValues();
            }
        }

        private void StartTimer()
        {
            Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested && _isEnabled)
                {
                    try
                    {
                        Apply();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"[{NAME}] Error in timer task: {ex.Message}");
                    }
                    await Task.Delay(1, _cancellationTokenSource.Token);
                }
            }, _cancellationTokenSource.Token);
        }

        private void StopTimer()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void RestoreOriginalValues()
        {
            if (!_hasOriginalValue) return;

            try
            {
                ulong cameraManagerPtr = Memory.ReadPtr(_playerController + PlayerController.PlayerCameraManager);
                if (cameraManagerPtr == 0) return;

                ulong cameraShakeModPtr = Memory.ReadPtr(cameraManagerPtr + PlayerCameraManager.CachedCameraShakeMod);
                if (cameraShakeModPtr == 0) return;

                // Restore original shake scale
                Memory.WriteValue(cameraShakeModPtr + UCameraModifier_CameraShake.SplitScreenShakeScale, _originalShakeScale);
                Logger.Debug($"[{NAME}] Restored original shake scale: {_originalShakeScale}");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error restoring original values: {ex.Message}");
            }
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

                // Store original shake scale if we haven't already
                if (!_hasOriginalValue)
                {
                    _originalShakeScale = Memory.ReadValue<float>(cameraShakeModPtr + UCameraModifier_CameraShake.SplitScreenShakeScale);
                    _hasOriginalValue = true;
                    Logger.Debug($"[{NAME}] Stored original shake scale: {_originalShakeScale}");
                }

                // Prevent new shakes by setting scale to 0
                Memory.WriteValue(cameraShakeModPtr + UCameraModifier_CameraShake.SplitScreenShakeScale, 0f);

                // Get active shakes data
                ulong activeShakesDataPtr = Memory.ReadPtr(cameraShakeModPtr + UCameraModifier_CameraShake.ActiveShakes);
                if (activeShakesDataPtr == 0) return;

                // Get number of active shakes
                int activeShakesCount = Memory.ReadValue<int>(cameraShakeModPtr + UCameraModifier_CameraShake.ActiveShakes + 0x8);
                if (activeShakesCount <= 0) return;

                // Create scatter write entries for each active shake
                var scatterEntries = new List<IScatterWriteDataEntry<float>>();
                const int shakeInfoSize = 0x18;

                for (int i = 0; i < activeShakesCount; i++)
                {
                    ulong shakeBasePtr = Memory.ReadPtr(activeShakesDataPtr + (uint)(i * shakeInfoSize));
                    if (shakeBasePtr != 0)
                    {
                        // Set shake scale to 0 to ensure shake is removed
                        scatterEntries.Add(new ScatterWriteDataEntry<float>(shakeBasePtr + UCameraShakeBase.ShakeScale, 0f));
                    }
                }

                if (scatterEntries.Count > 0)
                {
                    Memory.WriteScatter(scatterEntries);
                    Logger.Debug($"[{NAME}] Successfully modified {scatterEntries.Count} camera shakes");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting no camera shake: {ex.Message}");
            }
        }

        public void Dispose()
        {
            StopTimer();
            RestoreOriginalValues();
            _cancellationTokenSource.Dispose();
        }
    }
} 
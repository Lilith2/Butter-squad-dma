using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class NoCameraShake : Manager
    {
        public const string NAME = "NoCameraShake";
        private bool _isEnabled = false;
        
        public bool IsEnabled => _isEnabled;
        
        private CancellationTokenSource _cancellationTokenSource;
        private float _originalShakeScale;
        
        public NoCameraShake(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
            _cancellationTokenSource = new CancellationTokenSource();
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

        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    //Logger.Error($"[{NAME}] Cannot apply no camera shake - local player is not valid");
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
                    //Logger.Debug($"[{NAME}] Successfully modified {scatterEntries.Count} camera shakes");
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
            _cancellationTokenSource.Dispose();
        }
    }
} 
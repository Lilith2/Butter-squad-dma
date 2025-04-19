using System;
using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class Suppression : Manager
    {
        public const string NAME = "Suppression";
        
        private bool _isEnabled = false;
        
        public bool IsEnabled => _isEnabled;
        
        private float _originalSuppressionPercentage = 0.0f;
        private float _originalMaxSuppression = -1.0f;
        private float _originalSuppressionMultiplier = 1.0f;
        private bool _originalCameraRecoil = true;
        
        public Suppression(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }
                
        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable suppression - local player is not valid");
                return;
            }
            
            _isEnabled = enable;
            Logger.Debug($"[{NAME}] Suppression {(enable ? "enabled" : "disabled")}");
            Apply();
        }
        
        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply suppression - local player is not valid");
                    return;
                }
                
                UpdateCachedPointers();
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply suppression - soldier actor is not valid");
                    return;
                }

                if (Program.Config.DisableSuppression)
                {
                    // Store original values if we don't have them cached
                    if (_originalSuppressionPercentage == 0.0f && _originalMaxSuppression == -1.0f && 
                        _originalSuppressionMultiplier == 1.0f && _originalCameraRecoil)
                    {
                        _originalSuppressionPercentage = Memory.ReadValue<float>(soldierActor + ASQSoldier.UnderSuppressionPercentage);
                        _originalMaxSuppression = Memory.ReadValue<float>(soldierActor + ASQSoldier.MaxSuppressionPercentage);
                        _originalSuppressionMultiplier = Memory.ReadValue<float>(soldierActor + ASQSoldier.SuppressionMultiplier);
                        _originalCameraRecoil = Memory.ReadValue<bool>(soldierActor + ASQSoldier.bIsCameraRecoilActive);

                        Logger.Debug($"[{NAME}] Loaded original suppression values: Percentage={_originalSuppressionPercentage}, Max={_originalMaxSuppression}, Multiplier={_originalSuppressionMultiplier}, CameraRecoil={_originalCameraRecoil}");

                        // Save original values to config
                        if (Config.TryLoadConfig(out var config))
                        {
                            config.OriginalSuppressionPercentage = _originalSuppressionPercentage;
                            config.OriginalMaxSuppression = _originalMaxSuppression;
                            config.OriginalSuppressionMultiplier = _originalSuppressionMultiplier;
                            config.OriginalCameraRecoil = _originalCameraRecoil;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original suppression values to config");
                        }
                        else
                        {
                            Logger.Error($"[{NAME}] Failed to save original suppression values to config");
                        }
                    }

                    Memory.WriteValue<float>(soldierActor + ASQSoldier.UnderSuppressionPercentage, 0.0f);
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.MaxSuppressionPercentage, 0.0f);
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.SuppressionMultiplier, 0.0f);
                    Memory.WriteValue(soldierActor + ASQSoldier.bIsCameraRecoilActive, false);
                    Logger.Debug($"[{NAME}] Set all suppression values to 0 and disabled camera recoil");
                }
                else
                {
                    if (_originalSuppressionPercentage != 0.0f && _originalMaxSuppression != -1.0f && 
                        _originalSuppressionMultiplier != 1.0f)
                    {
                        Memory.WriteValue<float>(soldierActor + ASQSoldier.UnderSuppressionPercentage, _originalSuppressionPercentage);
                        Memory.WriteValue<float>(soldierActor + ASQSoldier.MaxSuppressionPercentage, _originalMaxSuppression);
                        Memory.WriteValue<float>(soldierActor + ASQSoldier.SuppressionMultiplier, _originalSuppressionMultiplier);
                        Memory.WriteValue(soldierActor + ASQSoldier.bIsCameraRecoilActive, _originalCameraRecoil);
                        Logger.Debug($"[{NAME}] Restored original suppression values: Percentage={_originalSuppressionPercentage}, Max={_originalMaxSuppression}, Multiplier={_originalSuppressionMultiplier}, CameraRecoil={_originalCameraRecoil}");
                    }
                    else
                    {
                        Logger.Error($"[{NAME}] Cannot restore suppression values - original values not loaded");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error applying suppression: {ex.Message}");
            }
        }
    }
} 
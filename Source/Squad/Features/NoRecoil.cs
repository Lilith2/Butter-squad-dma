using Offsets;
using squad_dma;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class NoRecoil : Manager
    {
        public const string NAME = "NoRecoil";
        
        public bool _isNoRecoilEnabled = false;
        
        // Original values for animation instance
        private Dictionary<ulong, float> _originalAnimValues = new Dictionary<ulong, float>();
        
        // Original values for weapon static info
        private Dictionary<ulong, float> _originalWeaponValues = new Dictionary<ulong, float>();
        
        // Original camera recoil state
        private bool _originalCameraRecoil = true;

        // Config storage for animation values
        private Dictionary<string, float> _configAnimValues = new Dictionary<string, float>();

        // Config storage for weapon values
        private Dictionary<string, float> _configWeaponValues = new Dictionary<string, float>();
        
        private readonly List<IScatterWriteDataEntry<float>> _noRecoilAnimEntries = new List<IScatterWriteDataEntry<float>>
        {
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeapRecoilRelLoc, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeapRecoilRelLoc + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeapRecoilRelLoc + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MoveRecoilFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.RecoilCanRelease, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilMean, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilSigma, 0f),    // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilMean, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunch, 0f),          // Pitch
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunch + 4, 0f),      // Yaw
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunch + 8, 0f),      // Roll
        };

        private readonly List<IScatterWriteDataEntry<float>> _noRecoilWeaponEntries = new List<IScatterWriteDataEntry<float>>
        {
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.RecoilCameraOffsetFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.RecoilWeaponRelLocFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.AddMoveRecoil, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MaxMoveRecoilFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilMean, 0f),   // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilSigma, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilMean, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilSigma, 0f),    // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilMean, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilMean, 0f),   // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilSigma, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilMean, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilSigma + 8, 0f), // Z
        };

        public NoRecoil(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
            // Load original values from config if they exist
            if (Config.TryLoadConfig(out var config))
            {
                _originalCameraRecoil = config.OriginalCameraRecoil;
                _configAnimValues = config.OriginalNoRecoilAnimValues ?? new Dictionary<string, float>();
                _configWeaponValues = config.OriginalNoRecoilWeaponValues ?? new Dictionary<string, float>();
                Logger.Debug($"[{NAME}] Loaded original values from config");
            }
            else
            {
                Logger.Debug($"[{NAME}] No config found, will load original values when no recoil is enabled");
            }
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable no recoil - local player is not valid");
                return;
            }
            
            _isNoRecoilEnabled = enable;
            Logger.Debug($"[{NAME}] No recoil {(enable ? "enabled" : "disabled")}");
            Apply();
        }

        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - local player is not valid");
                    return;
                }

                // Get soldier actor
                ulong playerState = Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (playerState == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - player state is not valid");
                    return;
                }

                ulong soldierActor = Memory.ReadPtr(playerState + ASQPlayerState.Soldier);
                if (soldierActor == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - soldier actor is not valid");
                    return;
                }

                // Get anim instance
                ulong animInstance = Memory.ReadPtr(soldierActor + ASQSoldier.CachedAnimInstance1p);
                if (animInstance == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - animation instance is not valid");
                    return;
                }

                // Get current weapon
                ulong inventoryComponent = Memory.ReadPtr(soldierActor + ASQSoldier.InventoryComponent);
                if (inventoryComponent == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - inventory component is not valid");
                    return;
                }

                ulong currentWeapon = Memory.ReadPtr(inventoryComponent + USQPawnInventoryComponent.CurrentWeapon);
                if (currentWeapon == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - current weapon is not valid");
                    return;
                }

                ulong weaponStaticInfo = Memory.ReadPtr(currentWeapon + ASQEquipableItem.ItemStaticInfo);
                if (weaponStaticInfo == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - weapon static info is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] Applying no recoil to anim instance at 0x{animInstance:X}");
                // Apply no recoil to anim instance
                if (_isNoRecoilEnabled)
                {
                    // Store original values when first enabled
                    if (_originalAnimValues.Count == 0)
                    {
                        foreach (var entry in _noRecoilAnimEntries)
                        {
                            float originalValue = Memory.ReadValue<float>(animInstance + entry.Address);
                            _originalAnimValues[entry.Address] = originalValue;
                            
                            // Store in config with descriptive key
                            string configKey = $"Anim_{entry.Address:X}";
                            _configAnimValues[configKey] = originalValue;
                            
                            Logger.Debug($"[{NAME}] Stored original anim value at 0x{entry.Address:X}: {originalValue}");
                        }

                        // Save to config
                        if (Config.TryLoadConfig(out var config))
                        {
                            config.OriginalNoRecoilAnimValues = _configAnimValues;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original anim values to config");
                        }
                    }

                    var animEntries = _noRecoilAnimEntries.Select(entry => 
                        new ScatterWriteDataEntry<float>(animInstance + entry.Address, entry.Data)).ToList();
                    Memory.WriteScatter(animEntries);
                }
                else
                {
                    // Restore original values
                    if (_originalAnimValues.Count > 0)
                    {
                        var restoreEntries = _originalAnimValues.Select(kvp => 
                            new ScatterWriteDataEntry<float>(animInstance + kvp.Key, kvp.Value)).ToList();
                        Memory.WriteScatter(restoreEntries);
                        Logger.Debug($"[{NAME}] Restored original anim values");
                    }
                }

                Logger.Debug($"[{NAME}] Applying no recoil to weapon static info at 0x{weaponStaticInfo:X}");
                // Apply no recoil to weapon static info
                if (_isNoRecoilEnabled)
                {
                    // Store original values when first enabled
                    if (_originalWeaponValues.Count == 0)
                    {
                        foreach (var entry in _noRecoilWeaponEntries)
                        {
                            float originalValue = Memory.ReadValue<float>(weaponStaticInfo + entry.Address);
                            _originalWeaponValues[entry.Address] = originalValue;
                            
                            // Store in config with descriptive key
                            string configKey = $"Weapon_{entry.Address:X}";
                            _configWeaponValues[configKey] = originalValue;
                            
                            Logger.Debug($"[{NAME}] Stored original weapon value at 0x{entry.Address:X}: {originalValue}");
                        }

                        // Save to config
                        if (Config.TryLoadConfig(out var config))
                        {
                            config.OriginalNoRecoilWeaponValues = _configWeaponValues;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original weapon values to config");
                        }
                    }

                    var weaponEntries = _noRecoilWeaponEntries.Select(entry => 
                        new ScatterWriteDataEntry<float>(weaponStaticInfo + entry.Address, entry.Data)).ToList();
                    Memory.WriteScatter(weaponEntries);
                }
                else
                {
                    // Restore original values
                    if (_originalWeaponValues.Count > 0)
                    {
                        var restoreEntries = _originalWeaponValues.Select(kvp => 
                            new ScatterWriteDataEntry<float>(weaponStaticInfo + kvp.Key, kvp.Value)).ToList();
                        Memory.WriteScatter(restoreEntries);
                        Logger.Debug($"[{NAME}] Restored original weapon values");
                    }
                }

                // Handle camera recoil
                if (_isNoRecoilEnabled)
                {
                    // Store original value when first enabled
                    if (_originalCameraRecoil)
                    {
                        _originalCameraRecoil = Memory.ReadValue<bool>(soldierActor + ASQSoldier.bIsCameraRecoilActive);
                        
                        // Save to config
                        if (Config.TryLoadConfig(out var config))
                        {
                            config.OriginalCameraRecoil = _originalCameraRecoil;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original camera recoil state to config: {_originalCameraRecoil}");
                        }
                    }
                    
                    Memory.WriteValue(soldierActor + ASQSoldier.bIsCameraRecoilActive, false);
                    Logger.Debug($"[{NAME}] Disabled camera recoil");
                }
                else
                {
                    Memory.WriteValue(soldierActor + ASQSoldier.bIsCameraRecoilActive, _originalCameraRecoil);
                    Logger.Debug($"[{NAME}] Restored original camera recoil state: {_originalCameraRecoil}");
                }

                Logger.Debug($"[{NAME}] Successfully {(_isNoRecoilEnabled ? "enabled" : "disabled")} no recoil");
                Logger.Debug("=============================");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting no recoil: {ex.Message}");
            }
        }
    }
}
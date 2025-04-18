using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class NoSpread : Manager
    {
        public const string NAME = "NoSpread";
        
        public bool _isNoSpreadEnabled = false;
        
        // Original values for animation instance
        private Dictionary<ulong, float> _originalAnimValues = new Dictionary<ulong, float>();
        
        // Original values for weapon static info
        private Dictionary<ulong, float> _originalWeaponValues = new Dictionary<ulong, float>();

        // Config storage for animation values
        private Dictionary<string, float> _configAnimValues = new Dictionary<string, float>();

        // Config storage for weapon values
        private Dictionary<string, float> _configWeaponValues = new Dictionary<string, float>();
        
        private readonly List<IScatterWriteDataEntry<float>> _noSpreadAnimEntries = new List<IScatterWriteDataEntry<float>>
        {
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MoveDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ShotDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalDeviation + 4, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalDeviation + 8, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalDeviation + 12, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.AddMoveDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MoveDeviationFactorRelease, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MaxMoveDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinMoveDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FullStaminaDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.LowStaminaDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.AddShotDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.AddShotDeviationFactorAds, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ShotDeviationFactorRelease, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinShotDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MaxShotDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinProneAdsDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinProneDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinCrouchAdsDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinCrouchDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinStandAdsDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinStandDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MinProneTransitionDeviation, 0f),
        };

        private readonly List<IScatterWriteDataEntry<float>> _noSpreadWeaponEntries = new List<IScatterWriteDataEntry<float>>
        {
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinShotDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MaxShotDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.AddShotDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.AddShotDeviationFactorAds, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ShotDeviationFactorRelease, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.LowStaminaDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.FullStaminaDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MoveDeviationFactorRelease, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.AddMoveDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MaxMoveDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinMoveDeviationFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinProneAdsDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinProneDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinCrouchAdsDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinCrouchDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinStandAdsDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinStandDeviation, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MinProneTransitionDeviation, 0f),
        };

        public NoSpread(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
            // Load original values from config if they exist
            if (Config.TryLoadConfig(out var config))
            {
                _configAnimValues = config.OriginalNoSpreadAnimValues ?? new Dictionary<string, float>();
                _configWeaponValues = config.OriginalNoSpreadWeaponValues ?? new Dictionary<string, float>();
                Logger.Debug($"[{NAME}] Loaded original values from config");
            }
            else
            {
                Logger.Debug($"[{NAME}] No config found, will load original values when no spread is enabled");
            }
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable no spread - local player is not valid");
                return;
            }
            
            _isNoSpreadEnabled = enable;
            Logger.Debug($"[{NAME}] No spread {(enable ? "enabled" : "disabled")}");
            Apply();
        }

        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply no spread - local player is not valid");
                    return;
                }

                // Get soldier actor
                ulong playerState = Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (playerState == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no spread - player state is not valid");
                    return;
                }

                ulong soldierActor = Memory.ReadPtr(playerState + ASQPlayerState.Soldier);
                if (soldierActor == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no spread - soldier actor is not valid");
                    return;
                }

                // Get anim instance
                ulong animInstance = Memory.ReadPtr(soldierActor + ASQSoldier.CachedAnimInstance1p);
                if (animInstance == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no spread - animation instance is not valid");
                    return;
                }

                // Get current weapon
                ulong inventoryComponent = Memory.ReadPtr(soldierActor + ASQSoldier.InventoryComponent);
                if (inventoryComponent == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no spread - inventory component is not valid");
                    return;
                }

                ulong currentWeapon = Memory.ReadPtr(inventoryComponent + USQPawnInventoryComponent.CurrentWeapon);
                if (currentWeapon == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no spread - current weapon is not valid");
                    return;
                }

                ulong weaponStaticInfo = Memory.ReadPtr(currentWeapon + ASQEquipableItem.ItemStaticInfo);
                if (weaponStaticInfo == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no spread - weapon static info is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] Applying no spread to anim instance at 0x{animInstance:X}");
                // Apply no spread to anim instance
                if (_isNoSpreadEnabled)
                {
                    // Store original values when first enabled
                    if (_originalAnimValues.Count == 0)
                    {
                        foreach (var entry in _noSpreadAnimEntries)
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
                            config.OriginalNoSpreadAnimValues = _configAnimValues;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original anim values to config");
                        }
                    }

                    var animEntries = _noSpreadAnimEntries.Select(entry => 
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

                Logger.Debug($"[{NAME}] Applying no spread to weapon static info at 0x{weaponStaticInfo:X}");
                // Apply no spread to weapon static info
                if (_isNoSpreadEnabled)
                {
                    // Store original values when first enabled
                    if (_originalWeaponValues.Count == 0)
                    {
                        foreach (var entry in _noSpreadWeaponEntries)
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
                            config.OriginalNoSpreadWeaponValues = _configWeaponValues;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original weapon values to config");
                        }
                    }

                    var weaponEntries = _noSpreadWeaponEntries.Select(entry => 
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

                Logger.Debug($"[{NAME}] Successfully {(_isNoSpreadEnabled ? "enabled" : "disabled")} no spread");
                Logger.Debug("=============================");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting no spread: {ex.Message}");
            }
        }
    }
} 
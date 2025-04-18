using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class NoSway : Manager
    {
        public const string NAME = "NoSway";
        
        public bool _isNoSwayEnabled = false;
        
        // Original values for animation instance
        private Dictionary<ulong, float> _originalAnimValues = new Dictionary<ulong, float>();
        
        // Original values for weapon static info
        private Dictionary<ulong, float> _originalWeaponValues = new Dictionary<ulong, float>();

        // Config storage for animation values
        private Dictionary<string, float> _configAnimValues = new Dictionary<string, float>();

        // Config storage for weapon values
        private Dictionary<string, float> _configWeaponValues = new Dictionary<string, float>();
        
        private readonly List<IScatterWriteDataEntry<float>> _noSwayAnimEntries = new List<IScatterWriteDataEntry<float>>
        {
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MoveSwayFactorMultiplier, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SuppressSwayFactorMultiplier, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunchSwayCombinedRotator, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunchSwayCombinedRotator + 4, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunchSwayCombinedRotator + 8, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.UnclampedTotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayData + FSQSwayData.UnclampedTotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayData + FSQSwayData.TotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayData + FSQSwayData.Sway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayData + FSQSwayData.Sway + 4, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayData + FSQSwayData.Sway + 8, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayAlignmentData + FSQSwayData.UnclampedTotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayAlignmentData + FSQSwayData.TotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayAlignmentData + FSQSwayData.Sway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayAlignmentData + FSQSwayData.Sway + 4, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.SwayAlignmentData + FSQSwayData.Sway + 8, 0f),
        };

        private readonly List<IScatterWriteDataEntry<float>> _noSwayWeaponEntries = new List<IScatterWriteDataEntry<float>>
        {
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.AddMoveSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MaxMoveSwayFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayData + FSQSwayData.UnclampedTotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayData + FSQSwayData.TotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayData + FSQSwayData.Sway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayData + FSQSwayData.Sway + 4, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayData + FSQSwayData.Sway + 8, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayAlignmentData + FSQSwayData.UnclampedTotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayAlignmentData + FSQSwayData.TotalSway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayAlignmentData + FSQSwayData.Sway, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayAlignmentData + FSQSwayData.Sway + 4, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.SwayAlignmentData + FSQSwayData.Sway + 8, 0f),
        };

        public NoSway(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
            // Load original values from config if they exist
            if (Config.TryLoadConfig(out var config))
            {
                _configAnimValues = config.OriginalNoSwayAnimValues ?? new Dictionary<string, float>();
                _configWeaponValues = config.OriginalNoSwayWeaponValues ?? new Dictionary<string, float>();
                Logger.Debug($"[{NAME}] Loaded original values from config");
            }
            else
            {
                Logger.Debug($"[{NAME}] No config found, will load original values when no sway is enabled");
            }
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable no sway - local player is not valid");
                return;
            }
            
            _isNoSwayEnabled = enable;
            Logger.Debug($"[{NAME}] No sway {(enable ? "enabled" : "disabled")}");
            Apply();
        }

        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply no sway - local player is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] === {(_isNoSwayEnabled ? "ENABLING" : "DISABLING")} NO SWAY ===");

                // Get soldier actor
                ulong playerState = Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (playerState == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no sway - player state is not valid");
                    return;
                }

                ulong soldierActor = Memory.ReadPtr(playerState + ASQPlayerState.Soldier);
                if (soldierActor == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no sway - soldier actor is not valid");
                    return;
                }

                // Get anim instance
                ulong animInstance = Memory.ReadPtr(soldierActor + ASQSoldier.CachedAnimInstance1p);
                if (animInstance == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no sway - animation instance is not valid");
                    return;
                }

                // Get current weapon
                ulong inventoryComponent = Memory.ReadPtr(soldierActor + ASQSoldier.InventoryComponent);
                if (inventoryComponent == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no sway - inventory component is not valid");
                    return;
                }

                ulong currentWeapon = Memory.ReadPtr(inventoryComponent + USQPawnInventoryComponent.CurrentWeapon);
                if (currentWeapon == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no sway - current weapon is not valid");
                    return;
                }

                ulong weaponStaticInfo = Memory.ReadPtr(currentWeapon + ASQEquipableItem.ItemStaticInfo);
                if (weaponStaticInfo == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no sway - weapon static info is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] Applying no sway to anim instance at 0x{animInstance:X}");
                // Apply no sway to anim instance
                if (_isNoSwayEnabled)
                {
                    // Store original values when first enabled
                    if (_originalAnimValues.Count == 0)
                    {
                        foreach (var entry in _noSwayAnimEntries)
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
                            config.OriginalNoSwayAnimValues = _configAnimValues;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original anim values to config");
                        }
                    }

                    var animEntries = _noSwayAnimEntries.Select(entry => 
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

                Logger.Debug($"[{NAME}] Applying no sway to weapon static info at 0x{weaponStaticInfo:X}");
                // Apply no sway to weapon static info
                if (_isNoSwayEnabled)
                {
                    // Store original values when first enabled
                    if (_originalWeaponValues.Count == 0)
                    {
                        foreach (var entry in _noSwayWeaponEntries)
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
                            config.OriginalNoSwayWeaponValues = _configWeaponValues;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original weapon values to config");
                        }
                    }

                    var weaponEntries = _noSwayWeaponEntries.Select(entry => 
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

                Logger.Debug($"[{NAME}] Successfully {(_isNoSwayEnabled ? "enabled" : "disabled")} no sway");
                Logger.Debug($"[{NAME}] =============================");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting no sway: {ex.Message}");
            }
        }
    }
} 
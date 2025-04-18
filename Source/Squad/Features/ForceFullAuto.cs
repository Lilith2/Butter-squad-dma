using System;
using System.Collections.Generic;
using Offsets;
using squad_dma;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class ForceFullAuto : Manager
    {
        public const string NAME = "ForceFullAuto";
        
        public bool _isForceFullAutoEnabled = false;
        
        private int[] _originalFireModes = null;
        private bool _originalManualBolt = false;
        private bool _originalRequireAdsToShoot = false;
        
        public ForceFullAuto(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
            // Load original values from config if they exist
            if (Config.TryLoadConfig(out var config))
            {
                if (config.OriginalFireModes != null && config.OriginalFireModes.Length > 0)
                {
                    _originalFireModes = config.OriginalFireModes;
                    Logger.Debug($"[{NAME}] Loaded original fire modes from config: {string.Join(", ", _originalFireModes)}");
                }
            }
        }
        
        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isForceFullAutoEnabled = enable;
            Logger.Debug($"[{NAME}] Force Full Auto {(enable ? "enabled" : "disabled")}");
            Apply();
        }

        // FireModes :
        // 1 = Single Fire
        // 3 = Burst
        // -1 = Full Auto
        
        public override void Apply()
        {
            try
            {
                if (!_isForceFullAutoEnabled && _originalFireModes == null)
                    return;
                
                if (!IsLocalPlayerValid()) return;
                
                // First try to apply to soldier weapon
                UpdateCachedPointers();
                ulong currentWeapon = _cachedCurrentWeapon;
                if (currentWeapon != 0)
                {
                    Logger.Debug($"[{NAME}] Applying to soldier weapon");
                    Apply(currentWeapon);
                }

                // Then check if player is in a vehicle and apply to vehicle weapon
                ulong playerState = Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (playerState != 0)
                {
                    ulong currentSeat = Memory.ReadPtr(playerState + ASQPlayerState.CurrentSeat);
                    if (currentSeat != 0)
                    {
                        ulong seatPawn = Memory.ReadPtr(currentSeat + USQVehicleSeatComponent.SeatPawn);
                        if (seatPawn != 0)
                        {
                            ulong vehicleInventory = Memory.ReadPtr(seatPawn + ASQVehicleSeat.VehicleInventory);
                            if (vehicleInventory != 0)
                            {
                                ulong vehicleWeapon = Memory.ReadPtr(vehicleInventory + USQPawnInventoryComponent.CurrentWeapon);
                                if (vehicleWeapon != 0)
                                {
                                    Logger.Debug($"[{NAME}] Applying to vehicle weapon");
                                    Apply(vehicleWeapon);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting force full auto: {ex.Message}");
            }
        }

        private void Apply(ulong weapon)
        {
            try
            {
                ulong weaponConfig = weapon + ASQWeapon.WeaponConfig;
                ulong fireModesArray = weaponConfig + FSQWeaponData.FireModes;
                ulong fireModesData = Memory.ReadPtr(fireModesArray);
                if (fireModesData == 0)
                {
                    Logger.Error($"[{NAME}] Failed to get fire modes data");
                    return;
                }
                
                int fireModeCount = Memory.ReadValue<int>(fireModesArray + 0x8);
                if (fireModeCount <= 0)
                {
                    Logger.Error($"[{NAME}] Invalid fire mode count");
                    return;
                }
                
                ulong itemStaticInfo = Memory.ReadPtr(weapon + ASQEquipableItem.ItemStaticInfo);
                if (itemStaticInfo == 0)
                {
                    Logger.Error($"[{NAME}] Failed to get weapon static info");
                    return;
                }

                if (_isForceFullAutoEnabled)
                {
                    if (_originalFireModes == null)
                    {
                        _originalFireModes = new int[fireModeCount];
                        for (int i = 0; i < fireModeCount; i++)
                        {
                            ulong fireModeAddress = fireModesData + ((ulong)i * 4);
                            _originalFireModes[i] = Memory.ReadValue<int>(fireModeAddress);
                        }
                        _originalManualBolt = Memory.ReadValue<bool>(itemStaticInfo + USQWeaponStaticInfo.bRequiresManualBolt);
                        _originalRequireAdsToShoot = Memory.ReadValue<bool>(itemStaticInfo + USQWeaponStaticInfo.bRequireAdsToShoot);

                        Logger.Debug($"[{NAME}] Stored original fire modes: {string.Join(", ", _originalFireModes)}, ManualBolt={_originalManualBolt}, RequireAdsToShoot={_originalRequireAdsToShoot}");

                        // Save original values to config
                        if (Config.TryLoadConfig(out var config))
                        {
                            config.OriginalFireModes = _originalFireModes;
                            config.OriginalManualBolt = _originalManualBolt;
                            config.OriginalRequireAdsToShoot = _originalRequireAdsToShoot;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original values to config");
                        }
                    }

                    for (int i = 0; i < fireModeCount; i++)
                    {
                        ulong fireModeAddress = fireModesData + ((ulong)i * 4);
                        Memory.WriteValue(fireModeAddress, -1);
                    }
                    
                    Memory.WriteValue(weapon + ASQWeapon.CurrentFireMode, 0);
                    Memory.WriteValue(itemStaticInfo + USQWeaponStaticInfo.bRequiresManualBolt, false);
                    Memory.WriteValue(itemStaticInfo + USQWeaponStaticInfo.bRequireAdsToShoot, false);
                    
                    Logger.Debug($"[{NAME}] Set all fire modes to Full Auto (-1) and disabled manual bolt and ADS requirement");
                }
                else
                {
                    // Restore original values or use defaults if not found
                    if (_originalFireModes != null)
                    {
                        for (int i = 0; i < fireModeCount && i < _originalFireModes.Length; i++)
                        {
                            ulong fireModeAddress = fireModesData + ((ulong)i * 4);
                            Memory.WriteValue(fireModeAddress, _originalFireModes[i]);
                        }
                        
                        Memory.WriteValue(itemStaticInfo + USQWeaponStaticInfo.bRequiresManualBolt, _originalManualBolt);
                        Memory.WriteValue(itemStaticInfo + USQWeaponStaticInfo.bRequireAdsToShoot, _originalRequireAdsToShoot);
                        Logger.Debug($"[{NAME}] Restored original fire modes: {string.Join(", ", _originalFireModes)}, ManualBolt={_originalManualBolt}, RequireAdsToShoot={_originalRequireAdsToShoot}");
                    }
                    else
                    {
                        for (int i = 0; i < fireModeCount; i++)
                        {
                            ulong fireModeAddress = fireModesData + ((ulong)i * 4);
                            Memory.WriteValue(fireModeAddress, 1);
                        }
                        Memory.WriteValue(itemStaticInfo + USQWeaponStaticInfo.bRequiresManualBolt, true);
                        Memory.WriteValue(itemStaticInfo + USQWeaponStaticInfo.bRequireAdsToShoot, true);
                        Logger.Debug($"[{NAME}] Restored default fire modes (Single Fire) and enabled manual bolt and ADS requirement");
                    }
                    
                    _originalFireModes = null;
                    _originalManualBolt = false;
                    _originalRequireAdsToShoot = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error applying force full auto to weapon: {ex.Message}");
            }
        }
    }
} 
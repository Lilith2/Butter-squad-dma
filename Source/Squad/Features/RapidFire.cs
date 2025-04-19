using System;
using Offsets;
using squad_dma;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class RapidFire : Manager
    {
        public const string NAME = "RapidFire";
                
        // Original values for soldier weapon
        private float _soldierOriginalTimeBetweenShots = 0.0f;
        private float _soldierOriginalTimeBetweenSingleShots = 0.0f;
        
        // Original values for vehicle weapon
        private float _vehicleOriginalTimeBetweenShots = 0.0f;
        private float _vehicleOriginalTimeBetweenSingleShots = 0.0f;
        
        public RapidFire(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
            // Load original values from config if they exist
            if (Config.TryLoadConfig(out var config))
            {
                _soldierOriginalTimeBetweenShots = config.OriginalTimeBetweenShots;
                _soldierOriginalTimeBetweenSingleShots = config.OriginalTimeBetweenSingleShots;
                _vehicleOriginalTimeBetweenShots = config.OriginalVehicleTimeBetweenShots;
                _vehicleOriginalTimeBetweenSingleShots = config.OriginalVehicleTimeBetweenSingleShots;
                Logger.Debug($"[{NAME}] Loaded original rapid fire values from config: Soldier={_soldierOriginalTimeBetweenShots}/{_soldierOriginalTimeBetweenSingleShots}, Vehicle={_vehicleOriginalTimeBetweenShots}/{_vehicleOriginalTimeBetweenSingleShots}");
            }
            else
            {
                Logger.Debug($"[{NAME}] No config found, will load original values when rapid fire is enabled");
            }
        }
        
        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable rapid fire - local player is not valid");
                return;
            }
            
            Logger.Debug($"[{NAME}] Rapid fire {(enable ? "enabled" : "disabled")}");
            Apply();
        }
        
        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply rapid fire - local player is not valid");
                    return;
                }
                
                // First try to apply to soldier weapon
                UpdateCachedPointers();
                ulong currentWeapon = _cachedCurrentWeapon;
                if (currentWeapon != 0)
                {
                    Apply(currentWeapon, ref _soldierOriginalTimeBetweenShots, ref _soldierOriginalTimeBetweenSingleShots, false);
                }
                else
                {
                    Logger.Debug($"[{NAME}] No soldier weapon found");
                }

                // Then check if player is in a vehicle and apply to vehicle weapon
                ulong playerState = Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (playerState != 0)
                {                    
                    ulong currentSeat = Memory.ReadPtr(playerState + ASQPlayerState.CurrentSeat);
                    if (currentSeat != 0)
                    {
                        Logger.Debug($"[{NAME}] Found current seat at 0x{currentSeat:X}");
                        
                        ulong seatPawn = Memory.ReadPtr(currentSeat + USQVehicleSeatComponent.SeatPawn);
                        if (seatPawn != 0)
                        {
                            Logger.Debug($"[{NAME}] Found seat pawn at 0x{seatPawn:X}");
                            
                            ulong vehicleInventory = Memory.ReadPtr(seatPawn + ASQVehicleSeat.VehicleInventory);
                            if (vehicleInventory != 0)
                            {
                                Logger.Debug($"[{NAME}] Found vehicle inventory at 0x{vehicleInventory:X}");
                                
                                ulong vehicleWeapon = Memory.ReadPtr(vehicleInventory + USQPawnInventoryComponent.CurrentWeapon);
                                if (vehicleWeapon != 0)
                                {
                                    Logger.Debug($"[{NAME}] Found vehicle weapon at 0x{vehicleWeapon:X}");
                                    Apply(vehicleWeapon, ref _vehicleOriginalTimeBetweenShots, ref _vehicleOriginalTimeBetweenSingleShots, true);
                                }
                                else
                                {
                                    Logger.Debug($"[{NAME}] No vehicle weapon found");
                                }
                            }
                            else
                            {
                                Logger.Debug($"[{NAME}] No vehicle inventory found");
                            }
                        }
                        else
                        {
                            Logger.Debug($"[{NAME}] No seat pawn found");
                        }
                    }
                    else
                    {
                        Logger.Debug($"[{NAME}] No current seat found");
                    }
                }
                else
                {
                    Logger.Debug($"[{NAME}] No player state found");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting rapid fire: {ex.Message}");
            }
        }

        private void Apply(ulong weapon, ref float originalTimeBetweenShots, ref float originalTimeBetweenSingleShots, bool isVehicle)
        {
            try
            {
                ulong weaponConfigOffset = weapon + ASQWeapon.WeaponConfig;
                Logger.Debug($"[{NAME}] Found weapon config at 0x{weaponConfigOffset:X} for {(isVehicle ? "vehicle" : "soldier")} weapon");

                if (Program.Config.RapidFire)
                {
                    // Store original values when first enabled
                    if (originalTimeBetweenShots == 0.0f)
                    {
                        originalTimeBetweenShots = Memory.ReadValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenShots);
                        originalTimeBetweenSingleShots = Memory.ReadValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenSingleShots);

                        Logger.Debug($"[{NAME}] Loaded original {(isVehicle ? "vehicle" : "soldier")} weapon values: TimeBetweenShots={originalTimeBetweenShots}, TimeBetweenSingleShots={originalTimeBetweenSingleShots}");

                        // Save original values to config
                        if (Config.TryLoadConfig(out var config))
                        {
                            if (isVehicle)
                            {
                                config.OriginalVehicleTimeBetweenShots = originalTimeBetweenShots;
                                config.OriginalVehicleTimeBetweenSingleShots = originalTimeBetweenSingleShots;
                            }
                            else
                            {
                                config.OriginalTimeBetweenShots = originalTimeBetweenShots;
                                config.OriginalTimeBetweenSingleShots = originalTimeBetweenSingleShots;
                            }
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original {(isVehicle ? "vehicle" : "soldier")} weapon values to config");
                        }
                        else
                        {
                            Logger.Error($"[{NAME}] Failed to save original {(isVehicle ? "vehicle" : "soldier")} weapon values to config");
                        }
                    }

                    const float RAPID_FIRE_VALUE = 0.01f;
                    Memory.WriteValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenShots, RAPID_FIRE_VALUE);
                    Memory.WriteValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenSingleShots, RAPID_FIRE_VALUE);
                    Logger.Debug($"[{NAME}] Set {(isVehicle ? "vehicle" : "soldier")} weapon rapid fire values to {RAPID_FIRE_VALUE}");
                }
                else
                {
                    if (originalTimeBetweenShots != 0.0f)
                    {
                        Memory.WriteValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenShots, originalTimeBetweenShots);
                        Memory.WriteValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenSingleShots, originalTimeBetweenSingleShots);
                        Logger.Debug($"[{NAME}] Restored original {(isVehicle ? "vehicle" : "soldier")} weapon values: TimeBetweenShots={originalTimeBetweenShots}, TimeBetweenSingleShots={originalTimeBetweenSingleShots}");
                    }
                    else
                    {
                        Logger.Error($"[{NAME}] Cannot restore {(isVehicle ? "vehicle" : "soldier")} weapon values - original values not loaded");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error applying rapid fire to {(isVehicle ? "vehicle" : "soldier")} weapon: {ex.Message}");
            }
        }
    }
} 
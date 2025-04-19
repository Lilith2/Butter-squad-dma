using System;
using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class InfiniteAmmo : Manager
    {
        public const string NAME = "InfiniteAmmo";
        
        public InfiniteAmmo(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }
        
        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable infinite ammo - local player is not valid");
                return;
            }
            
            Logger.Debug($"[{NAME}] Infinite ammo {(enable ? "enabled" : "disabled")}");
            Apply();
        }
        
        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply infinite ammo - local player is not valid");
                    return;
                }
                
                // First try to apply to soldier weapon
                UpdateCachedPointers();
                ulong currentWeapon = _cachedCurrentWeapon;
                if (currentWeapon != 0)
                {
                    Logger.Debug($"[{NAME}] Applying infinite ammo to soldier weapon at 0x{currentWeapon:X}");
                    Apply(currentWeapon);
                }
                else
                {
                    Logger.Debug($"[{NAME}] No soldier weapon found to apply infinite ammo");
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
                                    Logger.Debug($"[{NAME}] Applying infinite ammo to vehicle weapon at 0x{vehicleWeapon:X}");
                                    Apply(vehicleWeapon);
                                }
                                else
                                {
                                    Logger.Debug($"[{NAME}] No vehicle weapon found to apply infinite ammo");
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
                Logger.Error($"[{NAME}] Error applying infinite ammo: {ex.Message}");
            }
        }

        private void Apply(ulong weapon)
        {
            try
            {
                ulong weaponConfigOffset = weapon + ASQWeapon.WeaponConfig;

                if (Program.Config.InfiniteAmmo)
                {
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteAmmo, 1);
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteMags, 1);
                    Logger.Debug($"[{NAME}] Enabled infinite ammo");
                }
                else
                {
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteAmmo, 0);
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteMags, 0);
                    Logger.Debug($"[{NAME}] Disabled infinite ammo");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error applying infinite ammo to weapon at 0x{weapon:X}: {ex.Message}");
            }
        }
    }
} 
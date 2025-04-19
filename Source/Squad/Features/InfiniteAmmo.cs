using System;
using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class InfiniteAmmo : Manager, Weapon
    {
        public const string NAME = "InfiniteAmmo";
        private ulong _lastWeapon = 0;
        private bool _isEnabled = false;
        
        public bool IsEnabled => _isEnabled;
        
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
            
            _isEnabled = enable;
            Logger.Debug($"[{NAME}] Infinite ammo {(enable ? "enabled" : "disabled")}");
            
            // If disabling, restore the last weapon's state
            if (!enable && _lastWeapon != 0)
            {
                RestoreWeapon(_lastWeapon);
            }
            
            // Apply to current weapon if enabled
            if (enable)
            {
                UpdateCachedPointers();
                if (_cachedCurrentWeapon != 0)
                {
                    Apply(_cachedCurrentWeapon);
                }

                // Check for vehicle weapon
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
                            }
                        }
                    }
                }
            }
        }
        
        public void OnWeaponChanged(ulong newWeapon, ulong oldWeapon)
        {
            if (!IsLocalPlayerValid()) return;
            
            try
            {
                // Restore the old weapon's state
                if (oldWeapon != 0)
                {
                    RestoreWeapon(oldWeapon);
                }
                
                // Apply to new weapon if enabled
                if (Program.Config.InfiniteAmmo && newWeapon != 0)
                {
                    Apply(newWeapon);
                }
                
                _lastWeapon = newWeapon;
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error handling weapon change: {ex.Message}");
            }
        }
        
        private void RestoreWeapon(ulong weapon)
        {
            try
            {
                ulong weaponConfigOffset = weapon + ASQWeapon.WeaponConfig;
                Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteAmmo, 0);
                Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteMags, 0);
                Logger.Debug($"[{NAME}] Restored weapon at 0x{weapon:X}");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error restoring weapon at 0x{weapon:X}: {ex.Message}");
            }
        }
        
        private void Apply(ulong weapon)
        {
            try
            {
                ulong weaponConfigOffset = weapon + ASQWeapon.WeaponConfig;
                Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteAmmo, 1);
                Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteMags, 1);
                Logger.Debug($"[{NAME}] Applied infinite ammo to weapon at 0x{weapon:X}");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error applying infinite ammo to weapon at 0x{weapon:X}: {ex.Message}");
            }
        }
        
        // Override Apply to do nothing since we handle weapon changes in OnWeaponChanged
        public override void Apply() { }
    }
} 
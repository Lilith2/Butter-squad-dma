using System;
using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class QuickSwap : Manager
    {
        public const string NAME = "QuickSwap";
        
        public bool _isQuickSwapEnabled = false;
        
        public QuickSwap(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }
        
        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable quick swap - local player is not valid");
                return;
            }
            
            _isQuickSwapEnabled = enable;
            Logger.Debug($"[{NAME}] Quick swap {(enable ? "enabled" : "disabled")}");
            Apply();
        }
        
        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply quick swap - local player is not valid");
                    return;
                }
                
                UpdateCachedPointers();
                ulong currentWeapon = _cachedCurrentWeapon;
                if (currentWeapon == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply quick swap - current weapon is not valid");
                    return;
                }
                
                if (_isQuickSwapEnabled)
                {
                    const float FAST_SWAP_VALUE = 0.01f;
                    Logger.Debug($"[{NAME}] Setting quick swap values to {FAST_SWAP_VALUE}");
                    
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.EquipDuration, FAST_SWAP_VALUE);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.UnequipDuration, FAST_SWAP_VALUE);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.CachedEquipDuration, FAST_SWAP_VALUE);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.CachedUnequipDuration, FAST_SWAP_VALUE);
                }
                else
                {
                    Logger.Debug($"[{NAME}] Restoring default swap values");
                    
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.EquipDuration, 1.2f);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.UnequipDuration, 1.067f);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.CachedEquipDuration, 1.2f);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.CachedUnequipDuration, 1.067f);
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error applying quick swap: {ex.Message}");
            }
        }
    }
} 
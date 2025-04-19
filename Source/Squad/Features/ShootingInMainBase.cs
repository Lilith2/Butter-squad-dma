using System;
using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class ShootingInMainBase : Manager
    {
        public const string NAME = "ShootingInMainBase";
        
        public ShootingInMainBase(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }
        
        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable shooting in main base - local player is not valid");
                return;
            }
            
            Logger.Debug($"[{NAME}] Shooting in main base {(enable ? "enabled" : "disabled")}");
            Apply();
        }
        
        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply shooting in main base - local player is not valid");
                    return;
                }
                
                UpdateCachedPointers();
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply shooting in main base - soldier actor is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] Found soldier actor at 0x{soldierActor:X}");

                ulong inventoryComponent = _cachedInventoryComponent;
                if (inventoryComponent == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply shooting in main base - inventory component is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] Found inventory component at 0x{inventoryComponent:X}");

                ulong currentItemStaticInfo = Memory.ReadPtr(inventoryComponent + ASQSoldier.CurrentItemStaticInfo);
                if (currentItemStaticInfo == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply shooting in main base - current item static info is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] Found current item static info at 0x{currentItemStaticInfo:X}");

                if (Program.Config.AllowShootingInMainBase)
                {
                    Memory.WriteValue<bool>(currentItemStaticInfo + ASQSoldier.bUsableInMainBase, true);
                    Logger.Debug($"[{NAME}] Enabled shooting in main base");
                }
                else
                {
                    Memory.WriteValue<bool>(currentItemStaticInfo + ASQSoldier.bUsableInMainBase, false);
                    Logger.Debug($"[{NAME}] Disabled shooting in main base");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting shooting in main base: {ex.Message}");
            }
        }
    }
} 
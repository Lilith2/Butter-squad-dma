using System;
using Offsets;
using System.Security.AccessControl;
using squad_dma;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class Collision : Manager
    {
        public const string NAME = "Collision";
        
        public bool IsCollisionDisabled { get; private set; } = false;
        
        public Collision(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            IsCollisionDisabled = enable;
            Logger.Debug($"[{NAME}] Collision {(enable ? "disabled" : "enabled")}");
            Apply();
        }

        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                UpdateCachedPointers();
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;

                ulong rootComponent = Memory.ReadPtr(soldierActor + Actor.RootComponent);
                if (rootComponent == 0) return;

                ulong bodyInstanceAddr = rootComponent + UPrimitiveComponent.BodyInstance;
                
                if (IsCollisionDisabled)
                {
                    Memory.WriteValue<byte>(bodyInstanceAddr + FBodyInstance.CollisionEnabled, 0); // NoCollision
                    Logger.Debug($"[{NAME}] Set collision to NoCollision (0)");
                }
                else
                {
                    Memory.WriteValue<byte>(bodyInstanceAddr + FBodyInstance.CollisionEnabled, 1); // QueryOnly
                    Logger.Debug($"[{NAME}] Set collision to QueryOnly (1)");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting collision: {ex.Message}");
            }
        }
    }
} 
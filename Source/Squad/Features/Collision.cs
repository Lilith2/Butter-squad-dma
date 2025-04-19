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
        private bool _isEnabled = false;
        
        public bool IsEnabled => _isEnabled;
                
        public Collision(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isEnabled = enable;
            Logger.Debug($"[{NAME}] Collision {(enable ? "enabled" : "disabled")}");
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
                
                if (Program.Config.DisableCollision)
                {
                    Memory.WriteValue<byte>(bodyInstanceAddr + FBodyInstance.CollisionEnabled, 0); // NoCollision
                    Logger.Debug($"[{NAME}] Set collision to NoCollision (0)");
                }
                else
                {
                    Memory.WriteValue<byte>(bodyInstanceAddr + FBodyInstance.CollisionEnabled, 1); // QueryOnly (normal collision)
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
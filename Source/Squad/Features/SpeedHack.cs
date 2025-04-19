using System;
using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class SpeedHack : Manager
    {
        public const string NAME = "SpeedHack";
        private bool _isEnabled = false;
        
        public bool IsEnabled => _isEnabled;
        
        private float _originalMaxFlySpeed;
        private float _originalMaxCustomMovementSpeed;
        private float _originalMaxAcceleration;
        
        public SpeedHack(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }
        
        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable speed hack - local player is not valid");
                return;
            }
            
            _isEnabled = enable;
            Logger.Debug($"[{NAME}] Speed hack {(enable ? "enabled" : "disabled")}");
            Apply();
        }
        
        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error($"[{NAME}] Cannot apply speed hack - local player is not valid");
                    return;
                }
                
                UpdateCachedPointers();
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply speed hack - soldier actor is not valid");
                    return;
                }

                Logger.Debug($"[{NAME}] Found soldier actor at 0x{soldierActor:X}");

                if (Program.Config.SetSpeedHack)
                {
                    const float SPEED_MULTIPLIER = 4.0f;
                    Memory.WriteValue<float>(soldierActor + Actor.CustomTimeDilation, SPEED_MULTIPLIER);
                    Logger.Debug($"[{NAME}] Set time dilation to {SPEED_MULTIPLIER}x");
                }
                else
                {
                    const float NORMAL_SPEED = 1.0f;
                    Memory.WriteValue<float>(soldierActor + Actor.CustomTimeDilation, NORMAL_SPEED);
                    Logger.Debug($"[{NAME}] Restored normal time dilation");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting time dilation: {ex.Message}");
            }
        }
    }
} 
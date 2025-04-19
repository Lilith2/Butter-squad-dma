using System;
using Offsets;
using squad_dma;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class AirStuck : Manager
    {
        public const string NAME = "AirStuck";
        
        private bool _isEnabled = false;
        
        public bool IsEnabled => _isEnabled;
        
        private Collision _collision;
        
        // Original values
        private byte _originalMovementMode = 1; // MOVE_Walking
        private byte _originalReplicatedMovementMode = 1; // MOVE_Walking
        private byte _originalReplicateMovement = 16;
        private float _originalMaxFlySpeed = 200.0f;
        private float _originalMaxCustomMovementSpeed = 600.0f;
        private float _originalMaxAcceleration = 500.0f;
        
        public AirStuck(ulong playerController, bool inGame, Collision collisionMod)
            : base(playerController, inGame)
        {
            _collision = collisionMod;
        }

        private enum EMovementMode : byte
        {
            MOVE_None = 0,
            MOVE_Walking = 1,
            MOVE_NavWalking = 2,
            MOVE_Falling = 3,
            MOVE_Swimming = 4,
            MOVE_Flying = 5,
            MOVE_Custom = 6,
            MOVE_MAX = 7
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable air stuck - local player is not valid");
                return;
            }
            
            _isEnabled = enable;
            Logger.Debug($"[{NAME}] Air stuck {(enable ? "enabled" : "disabled")}");
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

                ulong characterMovement = _cachedCharacterMovement;
                if (characterMovement == 0) return;

                if (Program.Config.SetAirStuck)
                {
                    // Store original values if we don't have them cached
                    if (_originalMovementMode == 1 && _originalReplicatedMovementMode == 1 && 
                        _originalReplicateMovement == 16 && _originalMaxFlySpeed == 200.0f &&
                        _originalMaxCustomMovementSpeed == 600.0f && _originalMaxAcceleration == 500.0f)
                    {
                        _originalMovementMode = Memory.ReadValue<byte>(characterMovement + CharacterMovementComponent.MovementMode);
                        _originalReplicatedMovementMode = Memory.ReadValue<byte>(characterMovement + Character.ReplicatedMovementMode);
                        _originalReplicateMovement = Memory.ReadValue<byte>(soldierActor + Actor.bReplicateMovement);
                        _originalMaxFlySpeed = Memory.ReadValue<float>(characterMovement + CharacterMovementComponent.MaxFlySpeed);
                        _originalMaxCustomMovementSpeed = Memory.ReadValue<float>(characterMovement + CharacterMovementComponent.MaxCustomMovementSpeed);
                        _originalMaxAcceleration = Memory.ReadValue<float>(characterMovement + CharacterMovementComponent.MaxAcceleration);

                        Logger.Debug($"[{NAME}] Stored original values: MovementMode={_originalMovementMode}, ReplicatedMode={_originalReplicatedMovementMode}, ReplicateMovement={_originalReplicateMovement}");

                        // Save original values to config
                        if (Config.TryLoadConfig(out var config))
                        {
                            config.OriginalMovementMode = _originalMovementMode;
                            config.OriginalReplicatedMovementMode = _originalReplicatedMovementMode;
                            config.OriginalReplicateMovement = _originalReplicateMovement;
                            config.OriginalMaxFlySpeed = _originalMaxFlySpeed;
                            config.OriginalMaxCustomMovementSpeed = _originalMaxCustomMovementSpeed;
                            config.OriginalMaxAcceleration = _originalMaxAcceleration;
                            Config.SaveConfig(config);
                            Logger.Debug($"[{NAME}] Saved original values to config");
                        }
                    }

                    Memory.WriteValue<byte>(characterMovement + CharacterMovementComponent.MovementMode, (byte)EMovementMode.MOVE_Flying);
                    Memory.WriteValue<byte>(characterMovement + Character.ReplicatedMovementMode, (byte)EMovementMode.MOVE_Flying);
                    Memory.WriteValue<byte>(soldierActor + Actor.bReplicateMovement, 0);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxFlySpeed, 2000.0f);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxCustomMovementSpeed, 2000.0f);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxAcceleration, 2000.0f);
                    
                    Logger.Debug($"[{NAME}] Applied Air Stuck values: MovementMode=Flying, MaxSpeed=2000, MaxAcceleration=2000");
                }
                else
                {
                    Memory.WriteValue<byte>(characterMovement + CharacterMovementComponent.MovementMode, _originalMovementMode);
                    Memory.WriteValue<byte>(characterMovement + Character.ReplicatedMovementMode, _originalReplicatedMovementMode);
                    Memory.WriteValue<byte>(soldierActor + Actor.bReplicateMovement, _originalReplicateMovement);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxFlySpeed, _originalMaxFlySpeed);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxCustomMovementSpeed, _originalMaxCustomMovementSpeed);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxAcceleration, _originalMaxAcceleration);
                    
                    Logger.Debug($"[{NAME}] Restored original values: MovementMode={_originalMovementMode}, MaxSpeed={_originalMaxFlySpeed}, MaxAcceleration={_originalMaxAcceleration}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error setting air stuck: {ex.Message}");
            }
        }
    }
} 
using Offsets;

namespace squad_dma.Source.Squad.Features
{
    public class LocalSoldier
    {
        private readonly ulong _playerController;
        private readonly bool _inGame;
        private CancellationTokenSource _cancellationTokenSource;

        private ulong _cachedPlayerState = 0;
        private ulong _cachedSoldierActor = 0;
        private ulong _cachedInventoryComponent = 0;
        private ulong _cachedCurrentWeapon = 0;
        private ulong _cachedCharacterMovement = 0;
        private DateTime _lastPointerUpdate = DateTime.MinValue;

        private bool _isSuppressionEnabled = false;
        private bool _isInteractionDistancesEnabled = false;
        private bool _isShootingInMainBaseEnabled = false;
        private bool _isSpeedHackEnabled = false;
        private bool _isAirStuckEnabled = false;
        private bool _isHideActorEnabled = false;
        private bool _isQuickZoomEnabled = false;
        private bool _isRapidFireEnabled = false;
        private bool _isInfiniteAmmoEnabled = false;
        private bool _isQuickSwapEnabled = false;
        private bool _isCollisionDisabled = false;

        // Movement modes
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

        // Original Values to restore
        private float _originalTimeBetweenShots = 0.0f;
        private float _originalTimeBetweenSingleShots = 0.0f;
        private float _originalFov;

        public LocalSoldier(ulong playerController, bool inGame, RegistredActors actors)
        {
            _playerController = playerController;
            _inGame = inGame;
            _cancellationTokenSource = new CancellationTokenSource();
            UpdateCachedPointers(); // Initial update of pointers
            StartFeatureTimer();
        }

        // Start a timer to apply features every second
        // Simple fix for when the Localplayer respawns
        // Need to change it to a better solution
        private void StartFeatureTimer()
        {
            Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        UpdateCachedPointers();
                        
                        if (_isSuppressionEnabled)
                            ApplySuppression();
                        if (_isInteractionDistancesEnabled)
                            ApplyInteractionDistances();
                        if (_isShootingInMainBaseEnabled)
                            ApplyShootingInMainBase();
                        if (_isSpeedHackEnabled)
                            ApplySpeedHack();
                        if (_isAirStuckEnabled)
                            ApplyAirStuck();
                        
                        // Handle DisableCollision, ensuring it's disabled if AirStuck is disabled
                        if (!_isAirStuckEnabled && _isCollisionDisabled)
                        {
                            _isCollisionDisabled = false;
                        }
                        
                        if (_isCollisionDisabled)
                            DisableCollision(_isCollisionDisabled);   
                        if (_isHideActorEnabled)
                            HideActor();
                        if (_isRapidFireEnabled || _originalTimeBetweenShots != 0.0f)
                            ApplyRapidFire();
                        if (_isInfiniteAmmoEnabled)
                            ApplyInfiniteAmmo();
                        if (_isQuickSwapEnabled)
                            ApplyQuickSwap();
                    }
                    catch { /* Silently fail */ }
                    await Task.Delay(1000, _cancellationTokenSource.Token);
                }
            }, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Updates cached pointers to avoid redundant memory reads
        /// </summary>
        private void UpdateCachedPointers()
        {
            try
            {
                if ((DateTime.Now - _lastPointerUpdate).TotalMilliseconds < 500 && 
                    _cachedPlayerState != 0 && _cachedSoldierActor != 0)
                    return;
                    
                if (!_inGame || _playerController == 0) return;
                
                _cachedPlayerState = Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (_cachedPlayerState == 0) return;
                
                _cachedSoldierActor = Memory.ReadPtr(_cachedPlayerState + ASQPlayerState.Soldier);
                if (_cachedSoldierActor == 0) return;
                
                _cachedInventoryComponent = Memory.ReadPtr(_cachedSoldierActor + ASQSoldier.InventoryComponent);
                if (_cachedInventoryComponent != 0)
                {
                    _cachedCurrentWeapon = Memory.ReadPtr(_cachedInventoryComponent + USQPawnInventoryComponent.CurrentWeapon);
                }
                
                _cachedCharacterMovement = Memory.ReadPtr(_cachedSoldierActor + Character.CharacterMovement);
                
                _lastPointerUpdate = DateTime.Now;
            }
            catch
            {
                // Reset pointers on error
                _cachedPlayerState = 0;
                _cachedSoldierActor = 0;
                _cachedInventoryComponent = 0;
                _cachedCurrentWeapon = 0;
                _cachedCharacterMovement = 0;
            }
        }

        /// <summary>
        /// Checks if the local player is valid (has a valid player state and soldier actor)
        /// </summary>
        /// <returns>True if local player is valid, false otherwise</returns>
        private bool IsLocalPlayerValid()
        {
            try
            {
                if (!_inGame || _playerController == 0) return false;
                
                ulong playerState = _cachedPlayerState != 0 ? _cachedPlayerState : Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (playerState == 0) return false;
                
                ulong soldierActor = _cachedSoldierActor != 0 ? _cachedSoldierActor : Memory.ReadPtr(playerState + ASQPlayerState.Soldier);
                if (soldierActor == 0) return false;
                
                return true;
            }
            catch
            { return false; }
        }

        public void SetSuppression(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isSuppressionEnabled = enable;
            ApplySuppression();
        }

        private void ApplySuppression()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;

                if (_isSuppressionEnabled)
                {
                    // Apply suppression settings
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.UnderSuppressionPercentage, 0.0f);
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.MaxSuppressionPercentage, 0.0f);
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.SuppressionMultiplier, 0.0f);
                }
                else
                {
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.UnderSuppressionPercentage, 0);
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.MaxSuppressionPercentage, -1);
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.SuppressionMultiplier, 1);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting suppression: {ex.Message}");
            }
        }

        public void SetInteractionDistances(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isInteractionDistancesEnabled = enable;
            ApplyInteractionDistances();
        }

        private void ApplyInteractionDistances()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;

                if (_isInteractionDistancesEnabled)
                {
                    // Apply interaction distances settings
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.UseInteractDistance, 5000.0f);
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.InteractableRadiusMultiplier, 70.0f);
                }
                else
                {
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.UseInteractDistance, 220);
                    Memory.WriteValue<float>(soldierActor + ASQSoldier.InteractableRadiusMultiplier, 1.2f);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting interaction distances: {ex.Message}");
            }
        }

        public void SetShootingInMainBase(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isShootingInMainBaseEnabled = enable;
            ApplyShootingInMainBase();
        }

        private void ApplyShootingInMainBase()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;

                ulong inventoryComponent = _cachedInventoryComponent;
                if (inventoryComponent == 0) return;

                ulong currentItemStaticInfo = Memory.ReadPtr(inventoryComponent + ASQSoldier.CurrentItemStaticInfo);
                if (currentItemStaticInfo == 0) return;

                if (_isShootingInMainBaseEnabled)
                {
                    // Apply setting
                    Memory.WriteValue<bool>(currentItemStaticInfo + ASQSoldier.bUsableInMainBase, true);
                }
                else
                {
                    Memory.WriteValue<bool>(currentItemStaticInfo + ASQSoldier.bUsableInMainBase, false);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting shooting in main base: {ex.Message}");
            }
        }

        public void SetSpeedHack(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isSpeedHackEnabled = enable;
            ApplySpeedHack();
        }

        private void ApplySpeedHack()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;

                if (_isSpeedHackEnabled)
                {
                    // Apply speed hack setting
                    Memory.WriteValue<float>(soldierActor + Actor.CustomTimeDilation, 4.0f);
                }
                else
                {
                    // Restore original value
                    Memory.WriteValue<float>(soldierActor + Actor.CustomTimeDilation, 1);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting time dilation: {ex.Message}");
            }
        }

        public void SetAirStuck(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isAirStuckEnabled = enable;
            ApplyAirStuck();
        }

        private void ApplyAirStuck()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;

                ulong characterMovement = _cachedCharacterMovement;
                if (characterMovement == 0) return;

                if (_isAirStuckEnabled)
                {
                    // Apply air stuck settings
                    Memory.WriteValue<byte>(characterMovement + CharacterMovementComponent.MovementMode, (byte)EMovementMode.MOVE_Flying);
                    Memory.WriteValue<byte>(characterMovement + Character.ReplicatedMovementMode, (byte)EMovementMode.MOVE_Flying);
                    Memory.WriteValue<byte>(soldierActor + Actor.bReplicateMovement, 0);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxFlySpeed, 2000.0f);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxCustomMovementSpeed, 2000.0f);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxAcceleration, 2000.0f);
                }
                else
                {
                    Memory.WriteValue<byte>(characterMovement + CharacterMovementComponent.MovementMode, (byte)EMovementMode.MOVE_Walking);
                    Memory.WriteValue<byte>(characterMovement + Character.ReplicatedMovementMode, (byte)EMovementMode.MOVE_Walking);
                    Memory.WriteValue<byte>(soldierActor + Actor.bReplicateMovement, 16);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxFlySpeed, 200);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxCustomMovementSpeed, 600);
                    Memory.WriteValue<float>(characterMovement + CharacterMovementComponent.MaxAcceleration, 500);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting air stuck: {ex.Message}");
            }
        }

        public void SetQuickZoom(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isQuickZoomEnabled = enable;
            ApplyQuickZoom();
        }

        public void ApplyQuickZoom()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong cameraManager = Memory.ReadPtr(_playerController + PlayerController.PlayerCameraManager);
                if (cameraManager == 0) return;
                
                if (_isQuickZoomEnabled)
                {
                    _originalFov = Memory.ReadValue<float>(cameraManager + PlayerCameraManager.DefaultFOV);
                    Memory.WriteValue<float>(cameraManager + PlayerCameraManager.DefaultFOV, 20.0f);
                }
                else if (_originalFov != 0.0f)
                {
                    Memory.WriteValue<float>(cameraManager + PlayerCameraManager.DefaultFOV, _originalFov);
                    _originalFov = 0.0f;
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting Quick Zoom: {ex.Message}");
            }
        }
        public void SetHideActor(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isHideActorEnabled = enable;
            HideActor();
        }

        private void HideActor()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;

                if (_isHideActorEnabled)
                {
                    // Apply hide actor settings
                    Memory.WriteValue<byte>(soldierActor + Actor.bReplicateMovement, 0);
                    Memory.WriteValue<byte>(soldierActor + Actor.bHidden, 1);
                }
                else
                {
                    Memory.WriteValue<byte>(soldierActor + Actor.bReplicateMovement, 16);
                    Memory.WriteValue<byte>(soldierActor + Actor.bHidden, 16);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting hide actor: {ex.Message}");
            }
        }

        /// <summary>
        /// Enables or disables collision for the soldier actor
        /// </summary>
        /// <param name="disable">True to disable collision, false to restore normal collision</param>
        public void DisableCollision(bool disable)
        {
            if (!IsLocalPlayerValid()) return;
            
            // Only allow enabling if AirStuck is enabled
            if (disable && !_isAirStuckEnabled)
            {
                // If AirStuck is disabled, ensure DisableCollision is also disabled
                _isCollisionDisabled = false;
                return;
            }
            
            _isCollisionDisabled = disable;
            
            try
            {
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;

                ulong rootComponent = Memory.ReadPtr(soldierActor + Actor.RootComponent);
                if (rootComponent == 0) return;

                // Access the FBodyInstance struct which is at offset 0x2c8 in UPrimitiveComponent
                // CollisionEnabled is at offset 0x20 in FBodyInstance
                ulong bodyInstanceAddr = rootComponent + 0x2c8;
                
                if (disable)
                {                  
                    // Set to NoCollision (0)
                    Memory.WriteValue<byte>(bodyInstanceAddr + 0x20, 0);
                }
                else
                {
                    // Restore original value
                    Memory.WriteValue<byte>(bodyInstanceAddr + 0x20, 1);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error {(disable ? "disabling" : "enabling")} collision: {ex.Message}");
            }
        }

        public void SetRapidFire(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isRapidFireEnabled = enable;
            ApplyRapidFire();
        }

        // Dynamic Restore Logic has the chance of returning 
        // bad values if the radar decides the game isnt running.
        private void ApplyRapidFire()
        {
            try
            {
                // Don't attempt any memory operations if the feature is disabled
                // and we don't have original values to restore
                if (!_isRapidFireEnabled && _originalTimeBetweenShots == 0.0f)
                    return;
                
                if (!IsLocalPlayerValid()) return;
                
                ulong currentWeapon = _cachedCurrentWeapon;
                if (currentWeapon == 0) return;
                
                ulong weaponConfigOffset = currentWeapon + ASQWeapon.WeaponConfig;

                if (_isRapidFireEnabled)
                {
                    // Store original values when first enabled
                    if (_originalTimeBetweenShots == 0.0f)
                    {
                        _originalTimeBetweenShots = Memory.ReadValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenShots);
                        _originalTimeBetweenSingleShots = Memory.ReadValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenSingleShots);
                    }

                    // Apply rapid fire settings
                    Memory.WriteValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenShots, 0.01f);
                    Memory.WriteValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenSingleShots, 0.01f);
                }
                else
                {
                    // Only restore if we have saved original values
                    if (_originalTimeBetweenShots != 0.0f)
                    {
                        // Restore original values
                        Memory.WriteValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenShots, _originalTimeBetweenShots);
                        Memory.WriteValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenSingleShots, _originalTimeBetweenSingleShots);
                        
                        // Reset stored values
                        _originalTimeBetweenShots = 0.0f;
                        _originalTimeBetweenSingleShots = 0.0f;
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting rapid fire: {ex.Message}");
            }
        }

        public void SetInfiniteAmmo(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isInfiniteAmmoEnabled = enable;
            ApplyInfiniteAmmo();
        }

        private void ApplyInfiniteAmmo()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong currentWeapon = _cachedCurrentWeapon;
                if (currentWeapon == 0) return;
                
                ulong weaponConfigOffset = currentWeapon + ASQWeapon.WeaponConfig;

                if (_isInfiniteAmmoEnabled)
                {
                    // Apply infinite ammo settings
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteAmmo, 1);
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteMags, 1);
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bCreateProjectileOnServer, 1);
                }
                else
                {
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteAmmo, 0);
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteMags, 0);
                    Memory.WriteValue<byte>(weaponConfigOffset + FSQWeaponData.bCreateProjectileOnServer, 0);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting infinite ammo: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets whether quick weapon swapping is enabled
        /// </summary>
        /// <param name="enable">Whether to enable quick weapon swapping</param>
        public void SetQuickSwap(bool enable)
        {
            if (!IsLocalPlayerValid()) return;
            _isQuickSwapEnabled = enable;
            ApplyQuickSwap();
        }

        /// <summary>
        /// Applies or removes quick weapon swapping
        /// </summary>
        private void ApplyQuickSwap()
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                ulong currentWeapon = _cachedCurrentWeapon;
                if (currentWeapon == 0) return;
                
                if (_isQuickSwapEnabled)
                {
                    const float FAST_SWAP_VALUE = 0.01f;
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.EquipDuration, FAST_SWAP_VALUE);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.UnequipDuration, FAST_SWAP_VALUE);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.CachedEquipDuration, FAST_SWAP_VALUE);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.CachedUnequipDuration, FAST_SWAP_VALUE);
                }
                else
                {
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.EquipDuration, 1.2f);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.UnequipDuration, 1.067f);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.CachedEquipDuration, 1.2f);
                    Memory.WriteValue<float>(currentWeapon + ASQEquipableItem.CachedUnequipDuration, 1.067f);
                }
            }
            catch (Exception ex)
            {
                Program.Log($"Error setting quick swap: {ex.Message}");
            }
        }
        public void LogCurrentValues()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Program.Log("LocalPlayer is not valid - cannot log values");
                    return;
                }

                UpdateCachedPointers();
                
                ulong soldierActor = _cachedSoldierActor;
                ulong inventoryComponent = _cachedInventoryComponent;
                ulong currentWeapon = _cachedCurrentWeapon;
                ulong characterMovement = _cachedCharacterMovement;
                
                if (soldierActor == 0)
                {
                    Program.Log("SoldierActor is null - cannot log values");
                    return;
                }

                Program.Log("=== LOCAL SOLDIER DEBUG VALUES ===");
                
                // Suppression values
                float underSuppressionPercentage = Memory.ReadValue<float>(soldierActor + ASQSoldier.UnderSuppressionPercentage);
                float maxSuppressionPercentage = Memory.ReadValue<float>(soldierActor + ASQSoldier.MaxSuppressionPercentage);
                float suppressionMultiplier = Memory.ReadValue<float>(soldierActor + ASQSoldier.SuppressionMultiplier);
                
                Program.Log($"Suppression [Enabled: {_isSuppressionEnabled}]:");
                Program.Log($"  - UnderSuppressionPercentage: {underSuppressionPercentage}");
                Program.Log($"  - MaxSuppressionPercentage: {maxSuppressionPercentage}");
                Program.Log($"  - SuppressionMultiplier: {suppressionMultiplier}");
                
                // Interaction distances
                float useInteractDistance = Memory.ReadValue<float>(soldierActor + ASQSoldier.UseInteractDistance);
                float interactableRadiusMultiplier = Memory.ReadValue<float>(soldierActor + ASQSoldier.InteractableRadiusMultiplier);
                
                Program.Log($"Interaction Distances [Enabled: {_isInteractionDistancesEnabled}]:");
                Program.Log($"  - UseInteractDistance: {useInteractDistance}");
                Program.Log($"  - InteractableRadiusMultiplier: {interactableRadiusMultiplier}");
                
                // Speed hack
                float timeDilation = Memory.ReadValue<float>(soldierActor + Actor.CustomTimeDilation);
                
                Program.Log($"Speed Hack [Enabled: {_isSpeedHackEnabled}]:");
                Program.Log($"  - CustomTimeDilation: {timeDilation}");
                
                // AirStuck
                if (characterMovement != 0)
                {
                    byte movementMode = Memory.ReadValue<byte>(characterMovement + CharacterMovementComponent.MovementMode);
                    byte replicatedMovementMode = Memory.ReadValue<byte>(characterMovement + Character.ReplicatedMovementMode);
                    byte replicateMovement = Memory.ReadValue<byte>(soldierActor + Actor.bReplicateMovement);
                    float maxFlySpeed = Memory.ReadValue<float>(characterMovement + CharacterMovementComponent.MaxFlySpeed);
                    float maxCustomMovementSpeed = Memory.ReadValue<float>(characterMovement + CharacterMovementComponent.MaxCustomMovementSpeed);
                    float maxAcceleration = Memory.ReadValue<float>(characterMovement + CharacterMovementComponent.MaxAcceleration);
                    
                    Program.Log($"AirStuck [Enabled: {_isAirStuckEnabled}]:");
                    Program.Log($"  - MovementMode: {movementMode} ({(EMovementMode)movementMode})");
                    Program.Log($"  - ReplicatedMovementMode: {replicatedMovementMode} ({(EMovementMode)replicatedMovementMode})");
                    Program.Log($"  - bReplicateMovement: {replicateMovement}");
                    Program.Log($"  - MaxFlySpeed: {maxFlySpeed}");
                    Program.Log($"  - MaxCustomMovementSpeed: {maxCustomMovementSpeed}");
                    Program.Log($"  - MaxAcceleration: {maxAcceleration}");
                }
                
                // HideActor
                byte hideActorReplicateMovement = Memory.ReadValue<byte>(soldierActor + Actor.bReplicateMovement);
                byte hidden = Memory.ReadValue<byte>(soldierActor + Actor.bHidden);
                
                Program.Log($"HideActor [Enabled: {_isHideActorEnabled}]:");
                Program.Log($"  - bReplicateMovement: {hideActorReplicateMovement}");
                Program.Log($"  - bHidden: {hidden}");
                
                // Collision
                ulong rootComponent = Memory.ReadPtr(soldierActor + Actor.RootComponent);
                if (rootComponent != 0)
                {
                    ulong bodyInstanceAddr = rootComponent + 0x2c8;
                    byte collisionEnabled = Memory.ReadValue<byte>(bodyInstanceAddr + 0x20);
                    
                    Program.Log($"Collision [Disabled: {_isCollisionDisabled}]:");
                    Program.Log($"  - CollisionEnabled: {collisionEnabled}");
                }
                
                // Quick Zoom
                ulong cameraManager = Memory.ReadPtr(_playerController + PlayerController.PlayerCameraManager);
                if (cameraManager != 0)
                {
                    float defaultFOV = Memory.ReadValue<float>(cameraManager + PlayerCameraManager.DefaultFOV);
                    
                    Program.Log($"Quick Zoom [Enabled: {_isQuickZoomEnabled}]:");
                    Program.Log($"  - DefaultFOV: {defaultFOV}");
                }
                
                // Current Weapon Info
                if (currentWeapon != 0)
                {
                    Program.Log($"Current Weapon:");
                    
                    // Shooting in Main Base
                    ulong currentItemStaticInfo = Memory.ReadPtr(inventoryComponent + ASQSoldier.CurrentItemStaticInfo);
                    if (currentItemStaticInfo != 0)
                    {
                        bool usableInMainBase = Memory.ReadValue<bool>(currentItemStaticInfo + ASQSoldier.bUsableInMainBase);
                        
                        Program.Log($"Shooting in Main Base [Enabled: {_isShootingInMainBaseEnabled}]:");
                        Program.Log($"  - bUsableInMainBase: {usableInMainBase}");
                    }
                    
                    // Rapid Fire
                    ulong weaponConfigOffset = currentWeapon + ASQWeapon.WeaponConfig;
                    float timeBetweenShots = Memory.ReadValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenShots);
                    float timeBetweenSingleShots = Memory.ReadValue<float>(weaponConfigOffset + FSQWeaponData.TimeBetweenSingleShots);
                    
                    Program.Log($"Rapid Fire [Enabled: {_isRapidFireEnabled}]:");
                    Program.Log($"  - TimeBetweenShots: {timeBetweenShots}");
                    Program.Log($"  - TimeBetweenSingleShots: {timeBetweenSingleShots}");
                    
                    // Infinite Ammo
                    byte infiniteAmmo = Memory.ReadValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteAmmo);
                    byte infiniteMags = Memory.ReadValue<byte>(weaponConfigOffset + FSQWeaponData.bInfiniteMags);
                    byte createProjectileOnServer = Memory.ReadValue<byte>(weaponConfigOffset + FSQWeaponData.bCreateProjectileOnServer);
                    
                    Program.Log($"Infinite Ammo [Enabled: {_isInfiniteAmmoEnabled}]:");
                    Program.Log($"  - bInfiniteAmmo: {infiniteAmmo}");
                    Program.Log($"  - bInfiniteMags: {infiniteMags}");
                    Program.Log($"  - bCreateProjectileOnServer: {createProjectileOnServer}");
                    
                    // Quick Swap
                    float equipDuration = Memory.ReadValue<float>(currentWeapon + ASQEquipableItem.EquipDuration);
                    float unequipDuration = Memory.ReadValue<float>(currentWeapon + ASQEquipableItem.UnequipDuration);
                    float cachedEquipDuration = Memory.ReadValue<float>(currentWeapon + ASQEquipableItem.CachedEquipDuration);
                    float cachedUnequipDuration = Memory.ReadValue<float>(currentWeapon + ASQEquipableItem.CachedUnequipDuration);
                    
                    Program.Log($"Quick Swap [Enabled: {_isQuickSwapEnabled}]:");
                    Program.Log($"  - EquipDuration: {equipDuration}");
                    Program.Log($"  - UnequipDuration: {unequipDuration}");
                    Program.Log($"  - CachedEquipDuration: {cachedEquipDuration}");
                    Program.Log($"  - CachedUnequipDuration: {cachedUnequipDuration}");
                }
                
                Program.Log("=============================");
            }
            catch (Exception ex)
            {
                Program.Log($"Error in LogCurrentValues: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
        
        /// <summary>
        /// Reads and logs just the essential weapon name information
        /// </summary>
        /// <param name="soldierActor">The soldier actor address</param>
        /// <param name="label">Label for logging</param>
        private void ReadWeaponInfo(ulong soldierActor, string label)
        {
            try
            {
                // Get inventory component
                ulong inventoryComponent = Memory.ReadPtr(soldierActor + ASQSoldier.InventoryComponent);
                if (inventoryComponent == 0) return;
                
                ulong currentWeapon = Memory.ReadPtr(inventoryComponent + USQPawnInventoryComponent.CurrentWeapon);
                if (currentWeapon == 0) return;
                
                Program.Log($"{label} Weapon:");
                
                // Get weapon object name 
                try
                {
                    int nameIndex = Memory.ReadValue<int>(currentWeapon + 0x18);
                    Dictionary<uint, string> names = Memory.GetNamesById(new List<uint> { (uint)nameIndex });
                    
                    if (names.ContainsKey((uint)nameIndex))
                    {
                        string weaponName = names[(uint)nameIndex];
                        Program.Log($"  - Object: {weaponName}");
                    }
                }
                catch { /* Silently fail */ }
                
                // Get static info name
                try
                {
                    ulong itemStaticInfo = Memory.ReadPtr(currentWeapon + ASQEquipableItem.ItemStaticInfo);
                    
                    if (itemStaticInfo != 0)
                    {
                        int staticInfoNameIndex = Memory.ReadValue<int>(itemStaticInfo + 0x18);
                        Dictionary<uint, string> names = Memory.GetNamesById(new List<uint> { (uint)staticInfoNameIndex });
                        
                        if (names.ContainsKey((uint)staticInfoNameIndex))
                        {
                            string infoName = names[(uint)staticInfoNameIndex];
                            Program.Log($"  - Static: {infoName}");
                        }
                    }
                }
                catch { /* Silently fail */ }
            }
            catch { /* Silently fail */ }
        }
        
        /// <summary>
        /// Reads and logs the current weapon of the local player and optionally other players
        /// </summary>
        /// <param name="includeOtherPlayers">Whether to include other players' weapons in the log</param>
        public void ReadCurrentWeapons(bool includeOtherPlayers = false)
        {
            try
            {
                if (!IsLocalPlayerValid()) return;
                
                Program.Log("=== READING CURRENT WEAPONS ===");
                
                UpdateCachedPointers();
                
                // Get local player's weapon
                ulong soldierActor = _cachedSoldierActor;
                if (soldierActor == 0) return;
                
                ReadWeaponInfo(soldierActor, "Local Player");
                
                // If requested, try to find a few other players by team
                if (includeOtherPlayers)
                {
                    // This is a simplified approach that doesn't rely on traversing the player array
                    int localTeamId = Memory.ReadValue<int>(_cachedPlayerState + ASQPlayerState.TeamID);
                    Program.Log($"Local player is on team: {localTeamId}");
                }
                
                Program.Log("=============================");
            }
            catch { /* Silently fail */ }
        }
    }
} 
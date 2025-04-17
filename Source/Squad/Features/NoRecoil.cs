/*using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    public class NoRecoil : Manager
    {
        public bool _isNoRecoilEnabled = false;
        
        private readonly List<IScatterWriteEntry> _noRecoilAnimEntries = new List<IScatterWriteEntry>
        {
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.WeapRecoilRelLoc, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.WeapRecoilRelLoc + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.WeapRecoilRelLoc + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.MoveRecoilFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.RecoilCanRelease, 0f),
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.FinalRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.FinalRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.FinalRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.FinalRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.FinalRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.FinalRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.StandRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.StandRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.StandRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.StandRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.StandRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.StandRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.CrouchRecoilMean, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.CrouchRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.CrouchRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.CrouchRecoilSigma, 0f),    // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.CrouchRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.CrouchRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneTransitionRecoilMean, 0f), // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneTransitionRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneTransitionRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.WeaponPunch, 0f),          // Pitch
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.WeaponPunch + 4, 0f),      // Yaw
            new ScatterWriteDataEntry<float>(0 + Offsets.USQAnimInstanceSoldier1P.WeaponPunch + 8, 0f),      // Roll
        };

        private readonly List<IScatterWriteEntry> _noRecoilWeaponEntries = new List<IScatterWriteEntry>
        {
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.RecoilCameraOffsetFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.RecoilWeaponRelLocFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.AddMoveRecoil, 0f),
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.MaxMoveRecoilFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandAdsRecoilMean, 0f),   // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandAdsRecoilSigma, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.StandAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchRecoilMean, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchRecoilSigma, 0f),    // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchAdsRecoilMean, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchAdsRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.CrouchAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneAdsRecoilMean, 0f),   // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneAdsRecoilSigma, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneTransitionRecoilMean, 0f), // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneTransitionRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneTransitionRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneTransitionRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneTransitionRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + Offsets.USQWeaponStaticInfo.ProneTransitionRecoilSigma + 8, 0f), // Z
        };

        public NoRecoil(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
            Logger.Debug($"NoRecoil initialized with playerController: 0x{playerController:X}, inGame: {inGame}");
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error("Cannot enable/disable no recoil - local player is not valid");
                return;
            }
            
            _isNoRecoilEnabled = enable;
            Logger.Debug($"No recoil {(enable ? "enabled" : "disabled")}");
            Apply();
        }

        public override void Apply()
        {
            try
            {
                if (!IsLocalPlayerValid())
                {
                    Logger.Error("Cannot apply no recoil - local player is not valid");
                    return;
                }

                Logger.Debug($"=== {'ENABLING' if _isNoRecoilEnabled else 'DISABLING'} NO RECOIL ===");

                // Get soldier actor
                ulong playerState = Memory.ReadPtr(_playerController + Controller.PlayerState);
                if (playerState == 0)
                {
                    Logger.Error("Cannot apply no recoil - player state is not valid");
                    return;
                }

                ulong soldierActor = Memory.ReadPtr(playerState + ASQPlayerState.Soldier);
                if (soldierActor == 0)
                {
                    Logger.Error("Cannot apply no recoil - soldier actor is not valid");
                    return;
                }

                // Get anim instance
                ulong animInstance = Memory.ReadPtr(soldierActor + ASQSoldier.CachedAnimInstance1p);
                if (animInstance == 0)
                {
                    Logger.Error("Cannot apply no recoil - animation instance is not valid");
                    return;
                }

                // Get current weapon
                ulong inventoryComponent = Memory.ReadPtr(soldierActor + ASQSoldier.InventoryComponent);
                if (inventoryComponent == 0)
                {
                    Logger.Error("Cannot apply no recoil - inventory component is not valid");
                    return;
                }

                ulong currentWeapon = Memory.ReadPtr(inventoryComponent + USQPawnInventoryComponent.CurrentWeapon);
                if (currentWeapon == 0)
                {
                    Logger.Error("Cannot apply no recoil - current weapon is not valid");
                    return;
                }

                ulong weaponStaticInfo = Memory.ReadPtr(currentWeapon + ASQEquipableItem.ItemStaticInfo);
                if (weaponStaticInfo == 0)
                {
                    Logger.Error("Cannot apply no recoil - weapon static info is not valid");
                    return;
                }

                Logger.Debug($"Applying no recoil to anim instance at 0x{animInstance:X}");
                // Apply no recoil to anim instance
                foreach (var entry in _noRecoilAnimEntries)
                {
                    entry.Address = animInstance;
                    if (_isNoRecoilEnabled)
                    {
                        entry.Write();
                    }
                }

                Logger.Debug($"Applying no recoil to weapon static info at 0x{weaponStaticInfo:X}");
                // Apply no recoil to weapon static info
                foreach (var entry in _noRecoilWeaponEntries)
                {
                    entry.Address = weaponStaticInfo;
                    if (_isNoRecoilEnabled)
                    {
                        entry.Write();
                    }
                }

                // Disable camera recoil
                Memory.WriteValue(soldierActor + ASQSoldier.bIsCameraRecoilActive, !_isNoRecoilEnabled);
                Logger.Debug($"Camera recoil {( _isNoRecoilEnabled ? "disabled" : "enabled")}");

                Logger.Debug($"Successfully {'enabled' if _isNoRecoilEnabled else 'disabled'} no recoil");
                Logger.Debug("=============================");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error setting no recoil: {ex.Message}");
            }
        }
    }
}*/
using Offsets;
using squad_dma;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{


    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3_
    {
        public float X;
        public float Y;
        public float Z;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector3_18x
    {
        public Vector3_ V0;
        public Vector3_ V1;
        public Vector3_ V2;
        public Vector3_ V3;
        public Vector3_ V4;
        public Vector3_ V5;
        public Vector3_ V6;
        public Vector3_ V7;
        public Vector3_ V8;
        public Vector3_ V9;
        public Vector3_ V10;
        public Vector3_ V11;
        public Vector3_ V12;
        public Vector3_ V13;
        public Vector3_ V14;
        public Vector3_ V15;
        public Vector3_ V16;
        public Vector3_ V17;
    }




    public class NoRecoil : Manager, Weapon
    {
        public Vector3_18x vec18 = new Vector3_18x();
        public const string NAME = "NoRecoil";
        private bool _isEnabled = false;
        
        public bool IsEnabled => _isEnabled;
                
        // Original values for animation instance
        private Dictionary<ulong, float> _originalAnimValues = new Dictionary<ulong, float>();
        
        // Original values for weapon static info
        private Dictionary<ulong, float> _originalWeaponValues = new Dictionary<ulong, float>();
        
        // Original camera recoil state
        private bool _originalCameraRecoil = true;

        // Config storage for animation values
        private Dictionary<string, float> _configAnimValues = new Dictionary<string, float>();

        // Config storage for weapon values
        private Dictionary<string, float> _configWeaponValues = new Dictionary<string, float>();
        
        private ulong _lastWeapon = 0;
        
        private readonly List<IScatterWriteDataEntry<float>> _noRecoilAnimEntries = new List<IScatterWriteDataEntry<float>>
        {
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeapRecoilRelLoc, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeapRecoilRelLoc + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeapRecoilRelLoc + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.MoveRecoilFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.RecoilCanRelease, 0f),
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.FinalRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.StandRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilMean, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilSigma, 0f),    // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.CrouchRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilMean, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.ProneTransitionRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunch, 0f),          // Pitch
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunch + 4, 0f),      // Yaw
            new ScatterWriteDataEntry<float>(0 + USQAnimInstanceSoldier1P.WeaponPunch + 8, 0f),      // Roll
        };

        private readonly List<IScatterWriteDataEntry<float>> _noRecoilWeaponEntries = new List<IScatterWriteDataEntry<float>>
        {
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.RecoilCameraOffsetFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.RecoilWeaponRelLocFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.AddMoveRecoil, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.MaxMoveRecoilFactor, 0f),
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilMean, 0f),   // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilSigma, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.StandAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilMean, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilSigma, 0f),    // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilMean, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.CrouchAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilMean, 0f),      // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilMean + 4, 0f),  // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilMean + 8, 0f),  // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilSigma, 0f),     // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneRecoilSigma + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilMean, 0f),   // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilMean + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilMean + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilSigma, 0f),  // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilSigma + 4, 0f),// Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneAdsRecoilSigma + 8, 0f),// Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilMean, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilMean + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilMean + 8, 0f), // Z
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilSigma, 0f), // X
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilSigma + 4, 0f), // Y
            new ScatterWriteDataEntry<float>(0 + USQWeaponStaticInfo.ProneTransitionRecoilSigma + 8, 0f), // Z
        };

        public NoRecoil(ulong playerController, bool inGame)
            : base(playerController, inGame)
        {
        }

        public void SetEnabled(bool enable)
        {
            if (!IsLocalPlayerValid())
            {
                Logger.Error($"[{NAME}] Cannot enable/disable no recoil - local player is not valid");
                return;
            }
            
            _isEnabled = enable;
            Logger.Debug($"[{NAME}] No recoil {(enable ? "enabled" : "disabled")}");
            
            //// If disabling, restore the last weapon's state
            //if (!enable && _lastWeapon != 0)
            //{
            //    RestoreWeapon(_lastWeapon);
            //}
            
            // Apply to current weapon if enabled
            if (enable)
            {
                UpdateCachedPointers();
                if (_cachedCurrentWeapon != 0)
                {
                    Apply(_cachedCurrentWeapon);
                }
            }
        }
        
        public void OnWeaponChanged(ulong newWeapon, ulong oldWeapon)
        {
            if (!IsLocalPlayerValid()) return;
            
            try
            {
                //// Restore the old weapon's state
                //if (oldWeapon != 0)
                //{
                //    RestoreWeapon(oldWeapon);
                //}
                
                // Apply to new weapon if enabled
                if (Program.Config.NoRecoil && newWeapon != 0)
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
                if (_cachedSoldierActor == 0)
                {
                    Logger.Error($"[{NAME}] Cannot restore weapon - soldier actor is not valid");
                    return;
                }

                ulong animInstance = Memory.ReadPtr(_cachedSoldierActor + ASQSoldier.CachedAnimInstance1p);
                if (animInstance == 0)
                {
                    Logger.Error($"[{NAME}] Cannot restore weapon - animation instance is not valid");
                    return;
                }

                ulong weaponStaticInfo = GetCachedWeaponStaticInfo(weapon);
                if (weaponStaticInfo == 0)
                {
                    Logger.Error($"[{NAME}] Cannot restore weapon - weapon static info is not valid");
                    return;
                }

                // Restore original values
                if (_originalAnimValues.Count > 0)
                {
                    var restoreEntries = _originalAnimValues.Select(kvp => 
                        new ScatterWriteDataEntry<float>(animInstance + kvp.Key, kvp.Value)).ToList();
                    Memory.WriteScatter(restoreEntries);
                    Logger.Debug($"[{NAME}] Restored original anim values");
                }

                if (_originalWeaponValues.Count > 0)
                {
                    var restoreEntries = _originalWeaponValues.Select(kvp => 
                        new ScatterWriteDataEntry<float>(weaponStaticInfo + kvp.Key, kvp.Value)).ToList();
                    Memory.WriteScatter(restoreEntries);
                    Logger.Debug($"[{NAME}] Restored original weapon values");
                }

                // Restore camera recoil
                Memory.WriteValue(_cachedSoldierActor + ASQSoldier.bIsCameraRecoilActive, _originalCameraRecoil);
                Logger.Debug($"[{NAME}] Restored original camera recoil state: {_originalCameraRecoil}");
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
                // Use cached pointers
                if (_cachedSoldierActor == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - soldier actor is not valid");
                    return;
                }

                // Get anim instance from cached soldier actor
                ulong animInstance = Memory.ReadPtr(_cachedSoldierActor + ASQSoldier.CachedAnimInstance1p);
                if (animInstance == 0)
                {
                    Logger.Error($"[{NAME}] Cannot apply no recoil - animation instance is not valid");
                    return;
                }
                Logger.Debug($"[{NAME}] Applying no recoil to anim instance at 0x{animInstance:X}");
                // Apply no recoil to anim instance
                if (Program.Config.NoRecoil)
                {
                    Memory.WriteValue(animInstance + 0xA08, vec18); //ProneAdsRecoilMean + 17 more FVector variables
                    float value = 0f;
                    Memory.WriteValue(animInstance + 0xAE8, value);
                    Memory.WriteValue(animInstance + 0xB7C, value);
                    /*
                    
                    class USQAnimInstanceSoldier1P //go to this class (ASQWeapon Address + 0x438), find the 2 struct FSQSwayData parts of the class.

                    struct FSQSwayAspect                          Aspect;

                    then go to it & add  0x8

                    write that float to 0 (float HasSway; variable within struct FSQSwayAspect)
                     
                     */
                }

                Logger.Debug($"[{NAME}] Successfully {(Program.Config.NoRecoil ? "enabled" : "disabled")} no recoil");
                Logger.Debug("=============================");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Error applying no recoil to weapon at 0x{weapon:X}: {ex.Message}");
            }
        }
        
        // Override Apply to do nothing since we handle weapon changes in OnWeaponChanged
        public override void Apply() { }
    }
}
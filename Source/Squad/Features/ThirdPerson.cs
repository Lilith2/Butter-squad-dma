using System;
using System.Numerics;
using Offsets;
using squad_dma.Source.Misc;

namespace squad_dma.Source.Squad.Features
{
    // Works but buggy
    // Game is constantly trying to reset the camera position
    // Dont know of a fix
    public class ThirdPerson : Manager
    {
        public const string NAME = "ThirdPerson";
        
        private Vector3 _smoothedCameraOffset;
        private float _cameraDistance = 400f;
        private float _cameraHeight = 100f;
        private bool _isThirdPersonEnabled = false;

        public ThirdPerson(ulong playerController, bool inGame) : base(playerController, inGame)
        {
        }

        public void SetEnabled(bool enable, float distance = 400f, float height = 100f)
        {
            _isThirdPersonEnabled = enable;
            _cameraDistance = distance;
            _cameraHeight = height;

            if (!_inGame || _playerController == 0)
            {
                Logger.Error($"[{NAME}] Cannot enable/disable third person - game not in progress or player controller invalid");
                return;
            }

            try
            {
                ulong pawnPtr = Memory.ReadPtr(_playerController + Controller.Pawn);
                if (pawnPtr == 0)
                {
                    Logger.Error($"[{NAME}] Cannot enable/disable third person - pawn pointer is invalid");
                    return;
                }

                Logger.Debug($"[{NAME}] Found pawn at 0x{pawnPtr:X}");

                bool currentMode = Memory.ReadValue<bool>(pawnPtr + (ulong)0x1654);
                if (currentMode != enable)
                {
                    Memory.WriteValue<bool>(pawnPtr + (ulong)0x1654, enable);
                    Logger.Debug($"[{NAME}] Set third person mode to: {enable}");
                }

                if (enable)
                {
                    Vector3 controlRot = Memory.ReadValue<Vector3>(pawnPtr + (ulong)0x2168);
                    float yawRad = controlRot.Y * (MathF.PI / 180f);

                    Vector3 idealOffset = new Vector3(
                        -distance * MathF.Cos(yawRad),
                        -distance * MathF.Sin(yawRad),
                        height
                    );

                    Vector3 currentOffset = Memory.ReadValue<Vector3>(pawnPtr + (ulong)0x21D0);
                    Vector3 smoothedOffset = Vector3.Lerp(currentOffset, idealOffset, 0.3f);

                    Memory.WriteValue<Vector3>(pawnPtr + (ulong)0x21D0, smoothedOffset);
                    Memory.WriteValue<float>(pawnPtr + (ulong)0x21DC, distance);

                    Memory.WriteValue<int>(pawnPtr + (ulong)0x21CD, 4); // DebugCameraFollowCharacter
                    Memory.WriteValue<bool>(pawnPtr + (ulong)0x2200, false);

                    Logger.Debug($"[{NAME}] Set camera offset to: {smoothedOffset}, distance: {distance}, height: {height}");
                }
                else
                {
                    // Reset to first-person
                    Memory.WriteValue<float>(pawnPtr + (ulong)0x21DC, 0);
                    Memory.WriteValue<Vector3>(pawnPtr + (ulong)0x21D0, Vector3.Zero);
                    Logger.Debug($"[{NAME}] Reset camera to first-person view");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Camera error: {ex.Message}");
            }
        }

        public override void Apply()
        {
            if (!_isThirdPersonEnabled || !_inGame || _playerController == 0)
            {
                Logger.Debug($"[{NAME}] Skipping camera update - third person disabled or game not in progress");
                return;
            }

            try
            {
                ulong pawnPtr = Memory.ReadPtr(_playerController + Controller.Pawn);
                if (pawnPtr == 0)
                {
                    Logger.Error($"[{NAME}] Cannot update camera - pawn pointer is invalid");
                    return;
                }

                Logger.Debug($"[{NAME}] Updating camera for pawn at 0x{pawnPtr:X}");

                Memory.WriteValue<int>(pawnPtr + (ulong)0x21CD, 4); // DebugCameraFollowCharacter
                Memory.WriteValue<bool>(pawnPtr + (ulong)0x1654, true);

                Vector3 controlRot = Memory.ReadValue<Vector3>(pawnPtr + (ulong)0x2168); // ControlRotation
                Logger.Debug($"[{NAME}] Current control rotation: {controlRot}");

                float yawRad = controlRot.Y * (MathF.PI / 180f);
                Vector3 idealOffset = new Vector3(
                    -_cameraDistance * MathF.Cos(yawRad),
                    -_cameraDistance * MathF.Sin(yawRad),
                    _cameraHeight
                );

                _smoothedCameraOffset = Vector3.Lerp(_smoothedCameraOffset, idealOffset, 0.1f);

                Memory.WriteValue<Vector3>(pawnPtr + (ulong)0x21D0, _smoothedCameraOffset);
                Memory.WriteValue<float>(pawnPtr + (ulong)0x21DC, _cameraDistance);
                Memory.WriteValue<Vector3>(pawnPtr + (ulong)0x21E0, controlRot);

                Logger.Debug($"[{NAME}] Updated camera offset to: {_smoothedCameraOffset}, distance: {_cameraDistance}");
            }
            catch (Exception ex)
            {
                Logger.Error($"[{NAME}] Camera update error: {ex.Message}");
            }
        }
    }
} 
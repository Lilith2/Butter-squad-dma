using System;

namespace squad_dma.Source.Squad.Features
{
    /// <summary>
    /// Interface for features that modify weapon behavior.
    /// Implement this interface to receive notifications when the player's weapon changes.
    /// </summary>
    public interface Weapon
    {
        /// <summary>
        /// Gets whether the feature is currently enabled
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Called when the player switches weapons or enters/exits a vehicle.
        /// </summary>
        /// <param name="newWeapon">Address of the new weapon. Can be a soldier weapon or vehicle weapon.</param>
        /// <param name="oldWeapon">Address of the previous weapon. Can be 0 if no weapon was equipped.</param>
        void OnWeaponChanged(ulong newWeapon, ulong oldWeapon);
    }
} 
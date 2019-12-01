//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet.UnityEngine.Hitbox
{
    /// <summary>
    ///     A QNetObject hitbox interface utilized by <see cref="QNetObjectHitboxBody"/>
    /// </summary>
    public interface IQNetObjectHitbox
    {
        /// <summary>
        ///     Name of the hitbox.
        /// </summary>
        string HitboxName { get; set; }

        /// <summary>
        ///     Id of hitbox.
        /// </summary>
        int HitboxId { get; set; }

        /// <summary>
        ///     Defines is this a proximity hitbox.
        /// </summary>
        bool IsProximityHitbox { get; set; }

        /// <summary>
        ///     Checks if this hitbox is valid.
        /// </summary>
        bool IsValid();

        /// <summary>
        ///     Checks if this hitbox is active and can be included in body test.
        /// </summary>
        bool IsActive();

        /// <summary>
        ///     Returns a sample of this hitbox.
        /// </summary>
        IQNetObjectHitboxSample GetSample();

        /// <summary>
        ///     Apply given sample to this hitbox.
        /// </summary>
        void ApplyState(IQNetObjectHitboxSample sample);

        /// <summary>
        ///     Restore default state of the hitbox.
        /// </summary>
        void RestoreState();

        /// <summary>
        ///     Called when <see cref="QNetObjectHitboxBody"/> need to update the collider state of this hitbox.
        ///     Usually called before and after the Client or Server body test.
        /// </summary>
        void SetColliderActiveState(bool activeState);
    }
}

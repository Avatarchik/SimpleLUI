//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JetBrains.Annotations;
using System;
using UnityEngine;

namespace JEM.UnityEngine.Extension
{
    /// <summary>
    ///     Set of utility extensions to Transform class.
    /// </summary>
    public static class JEMExtensionTransform
    {
        /// <summary>
        ///     Look at smoothly (Using <see cref="Quaternion.Lerp"/>).
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LookAtSmooth([NotNull] this Transform transform, [NotNull] Transform target, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            if (target == null) throw new ArgumentNullException(nameof(target));
            LookAtSmooth(transform, target.position, time);
        }

        /// <summary>
        ///     Look at smoothly  (Using <see cref="Quaternion.Lerp"/>)..
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LookAtSmooth([NotNull] this Transform transform, Vector3 point, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            LerpRotation(transform, Quaternion.LookRotation(point - transform.position), time);
        }

        #region LERP_CLAMPED

        #region POSITION

        /// <summary>
        ///     Interpolate transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpPosition([NotNull] this Transform transform, float x, float time) =>
            LerpPosition(transform, new Vector3(x, transform.position.y, transform.position.z), time);

        /// <summary>
        ///     Interpolate transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpPosition([NotNull] this Transform transform, float x, float y, float time) =>
            LerpPosition(transform, new Vector3(x, y, transform.position.z), time);
        
        /// <summary>
        ///     Interpolate transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpPosition([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpPosition(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Interpolate transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpPosition([NotNull] this Transform transform, Vector3 point, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.position = Vector3.Lerp(transform.position, point, time);
        }

        #endregion

        #region LOCAL_POSITION

        /// <summary>
        ///     Interpolate transform local position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalPosition([NotNull] this Transform transform, float x, float time) =>
            LerpLocalPosition(transform, new Vector3(x, transform.localPosition.y, transform.localPosition.z), time);

        /// <summary>
        ///     Interpolate transform local position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalPosition([NotNull] this Transform transform, float x, float y, float time) =>
            LerpLocalPosition(transform, new Vector3(x, y, transform.localPosition.z), time);

        /// <summary>
        ///     Interpolate transform local position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalPosition([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpLocalPosition(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Interpolate transform local position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalPosition([NotNull] this Transform transform, Vector3 point, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localPosition = Vector3.Lerp(transform.localPosition, point, time);
        }

        #endregion

        #region LOCAL_SCALE

        /// <summary>
        ///     Interpolate transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalScale([NotNull] this Transform transform, float x, float time) =>
            LerpLocalScale(transform, new Vector3(x, transform.localScale.y, transform.localScale.z), time);

        /// <summary>
        ///     Interpolate transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalScale([NotNull] this Transform transform, float x, float y, float time) =>
            LerpLocalScale(transform, new Vector3(x, y, transform.localScale.z), time);

        /// <summary>
        ///     Interpolate transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalScale([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpLocalScale(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Interpolate transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalScale([NotNull] this Transform transform, Vector3 point, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localScale = Vector3.Lerp(transform.localScale, point, time);
        }

        #endregion

        #region ROTATION

        /// <summary>
        ///     Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpRotation([NotNull] this Transform transform, float x, float time) =>
            LerpRotation(transform, Quaternion.Euler(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpRotation([NotNull] this Transform transform, float x, float y, float time) =>
            LerpRotation(transform, Quaternion.Euler(x, y, transform.rotation.eulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpRotation([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpRotation(transform, Quaternion.Euler(x, y, z), time);

        /// <summary>
        ///     Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpRotation([NotNull] this Transform transform, Vector3 euler, float time) =>
            LerpRotation(transform, Quaternion.Euler(euler), time);

        /// <summary>
        ///     Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpRotation([NotNull] this Transform transform, Quaternion rotation, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, time);
        }

        #endregion

        #region LOCAL_ROTATION
     
        /// <summary>
        ///     Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalRotation([NotNull] this Transform transform, float x, float time) =>
            LerpLocalRotation(transform, Quaternion.Euler(x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalRotation([NotNull] this Transform transform, float x, float y, float time) =>
            LerpLocalRotation(transform, Quaternion.Euler(x, y, transform.localRotation.eulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalRotation([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpLocalRotation(transform, Quaternion.Euler(x, y, z), time);

        /// <summary>
        ///     Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalRotation([NotNull] this Transform transform, Vector3 euler, float time) =>
            LerpLocalRotation(transform, Quaternion.Euler(euler), time);

        /// <summary>
        ///     Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalRotation([NotNull] this Transform transform, Quaternion rotation, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, time);
        }

        #endregion

        #region EULER

        /// <summary>
        ///     Interpolate transform eulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpEulerAngles([NotNull] this Transform transform, float x, float time) =>
            LerpEulerAngles(transform, new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform eulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpEulerAngles([NotNull] this Transform transform, float x, float y, float time) =>
            LerpEulerAngles(transform, new Vector3(x, y, transform.eulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform eulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpEulerAngles([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpEulerAngles(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Interpolate transform eulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpEulerAngles([NotNull] this Transform transform, Vector3 euler, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, euler, time);
        }

        #endregion

        #region LOCAL_EULER

        /// <summary>
        ///     Interpolate transform localEulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalEulerAngles([NotNull] this Transform transform, float x, float time) =>
            LerpLocalEulerAngles(transform, new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform localEulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalEulerAngles([NotNull] this Transform transform, float x, float y, float time) =>
            LerpLocalEulerAngles(transform, new Vector3(x, y, transform.localEulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform localEulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalEulerAngles([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpLocalEulerAngles(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Interpolate transform localEulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpLocalEulerAngles([NotNull] this Transform transform, Vector3 euler, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, euler, time);
        }

        #endregion

        #endregion

        #region LERP_UNCLAMPED

        #region UNCLAMPED_POSITION

        /// <summary>
        ///     Unclamped Interpolate transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedPosition([NotNull] this Transform transform, float x, float time) =>
            LerpUnclampedPosition(transform, new Vector3(x, transform.position.y, transform.position.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedPosition([NotNull] this Transform transform, float x, float y, float time) =>
            LerpUnclampedPosition(transform, new Vector3(x, y, transform.position.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedPosition([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpUnclampedPosition(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Unclamped Interpolate transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedPosition([NotNull] this Transform transform, Vector3 point, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.position = Vector3.LerpUnclamped(transform.position, point, time);
        }

        #endregion

        #region UNCLAMPED_LOCAL_POSITION

        /// <summary>
        ///     Unclamped Interpolate transform local position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalPosition([NotNull] this Transform transform, float x, float time) =>
            LerpUnclampedLocalPosition(transform, new Vector3(x, transform.localPosition.y, transform.localPosition.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform local position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalPosition([NotNull] this Transform transform, float x, float y, float time) =>
            LerpUnclampedLocalPosition(transform, new Vector3(x, y, transform.localPosition.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform local position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalPosition([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpUnclampedLocalPosition(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Unclamped Interpolate transform local position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalPosition([NotNull] this Transform transform, Vector3 point, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localPosition = Vector3.LerpUnclamped(transform.localPosition, point, time);
        }

        #endregion

        #region UNCLAMPED_LOCAL_SCALE

        /// <summary>
        ///     Unclamped Interpolate transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalScale([NotNull] this Transform transform, float x, float time) =>
            LerpUnclampedLocalScale(transform, new Vector3(x, transform.localScale.y, transform.localScale.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalScale([NotNull] this Transform transform, float x, float y, float time) =>
            LerpUnclampedLocalScale(transform, new Vector3(x, y, transform.localScale.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalScale([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpUnclampedLocalScale(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Unclamped Interpolate transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalScale([NotNull] this Transform transform, Vector3 point, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localScale = Vector3.LerpUnclamped(transform.localScale, point, time);
        }

        #endregion

        #region UNCLAMPED_ROTATION
    
        /// <summary>
        ///     Unclamped Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedRotation([NotNull] this Transform transform, float x, float time) =>
            LerpUnclampedRotation(transform, Quaternion.Euler(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedRotation([NotNull] this Transform transform, float x, float y, float time) =>
            LerpUnclampedRotation(transform, Quaternion.Euler(x, y, transform.rotation.eulerAngles.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedRotation([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpUnclampedRotation(transform, Quaternion.Euler(x, y, z), time);

        /// <summary>
        ///     Unclamped Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedRotation([NotNull] this Transform transform, Vector3 euler, float time) =>
            LerpUnclampedRotation(transform, Quaternion.Euler(euler), time);

        /// <summary>
        ///     Unclamped Interpolate transform rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedRotation([NotNull] this Transform transform, Quaternion rotation, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.rotation = Quaternion.LerpUnclamped(transform.rotation, rotation, time);
        }

        #endregion

        #region UNCLAMPED_LOCAL_ROTATION

        /// <summary>
        ///     Unclamped Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalRotation([NotNull] this Transform transform, float x, float time) =>
            LerpUnclampedLocalRotation(transform, Quaternion.Euler(x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalRotation([NotNull] this Transform transform, float x, float y, float time) =>
            LerpUnclampedLocalRotation(transform, Quaternion.Euler(x, y, transform.localRotation.eulerAngles.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalRotation([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpUnclampedLocalRotation(transform, Quaternion.Euler(x, y, z), time);

        /// <summary>
        ///     Unclamped Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalRotation([NotNull] this Transform transform, Vector3 euler, float time) =>
            LerpUnclampedLocalRotation(transform, Quaternion.Euler(euler), time);

        /// <summary>
        ///     Unclamped Interpolate transform local rotation.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalRotation([NotNull] this Transform transform, Quaternion rotation, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localRotation = Quaternion.LerpUnclamped(transform.localRotation, rotation, time);
        }

        #endregion

        #region INCLAMPED_EULER

        /// <summary>
        ///     Unclamped Interpolate transform eulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedEulerAngles([NotNull] this Transform transform, float x, float time) =>
            LerpUnclampedEulerAngles(transform, new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform eulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedEulerAngles([NotNull] this Transform transform, float x, float y, float time) =>
            LerpUnclampedEulerAngles(transform, new Vector3(x, y, transform.eulerAngles.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform eulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedEulerAngles([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpUnclampedEulerAngles(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Unclamped Interpolate transform eulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedEulerAngles([NotNull] this Transform transform, Vector3 euler, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.eulerAngles = Vector3.LerpUnclamped(transform.eulerAngles, euler, time);
        }

        #endregion

        #region UNCLAMPED_LOCAL_EULER

        /// <summary>
        ///     Unclamped Interpolate transform localEulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalEulerAngles([NotNull] this Transform transform, float x, float time) =>
            LerpUnclampedLocalEulerAngles(transform, new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z), time);

        /// <summary>
        ///     Interpolate transform localEulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalEulerAngles([NotNull] this Transform transform, float x, float y, float time) =>
            LerpUnclampedLocalEulerAngles(transform, new Vector3(x, y, transform.localEulerAngles.z), time);

        /// <summary>
        ///     Unclamped Interpolate transform localEulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalEulerAngles([NotNull] this Transform transform, float x, float y, float z, float time) =>
            LerpUnclampedLocalEulerAngles(transform, new Vector3(x, y, z), time);

        /// <summary>
        ///     Unclamped Interpolate transform localEulerAngles.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void LerpUnclampedLocalEulerAngles([NotNull] this Transform transform, Vector3 euler, float time)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localEulerAngles = Vector3.LerpUnclamped(transform.localEulerAngles, euler, time);
        }

        #endregion

        #endregion

        /// <summary>
        ///     Sets the angle (z of euler) of transform.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void SetAngle([NotNull] this Transform transform, float angle)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, angle);
        }

        /// <summary>
        ///     Sets the local angle (z of local euler) of transform.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static void SetLocalAngle([NotNull] this Transform transform, float angle)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, angle);
        }

        /// <summary>
        ///     Gets the angle (z of euler) of transform.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float GetAngle([NotNull] this Transform transform)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            return transform.eulerAngles.z;
        }

        /// <summary>
        ///     Gets the local angle (z of local euler) of transform.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float GetLocalAngle([NotNull] this Transform transform)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            return transform.localEulerAngles.z;
        }

        /// <summary>
        ///     Returns distance between this and target transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float Distance([NotNull] this Transform transform, [NotNull] Transform target) => Distance(transform, target.position);

        /// <summary>
        ///     Returns distance between this transform and target position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float Distance([NotNull] this Transform transform, Vector3 target)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            return Vector3.Distance(transform.position, target);
        }

        /// <summary>
        ///     Returns distance between this and target transform position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float Distance2D([NotNull] this Transform transform, [NotNull] Transform target) =>
            Distance2D(transform, target.position);

        /// <summary>
        ///     Returns distance between this and target position.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float Distance2D([NotNull] this Transform transform, Vector2 target)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            return Vector2.Distance(transform.position, target);
        }

        /// <summary>
        ///     Returns distance between this and target transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float ScaleDistance([NotNull] this Transform transform, [NotNull] Transform target) =>
            ScaleDistance(transform, target.localScale);

        /// <summary>
        ///     Returns distance between this and target localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float ScaleDistance([NotNull] this Transform transform, Vector3 target)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            return Vector3.Distance(transform.localScale, target);
        }

        /// <summary>
        ///     Returns distance between this and target transform localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float ScaleDistance2D([NotNull] this Transform transform, [NotNull] Transform target) =>
            ScaleDistance2D(transform, target.localScale);

        /// <summary>
        ///     Returns distance between this and target localScale.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public static float ScaleDistance2D([NotNull] this Transform transform, Vector2 target)
        {
            if (transform == null) throw new ArgumentNullException(nameof(transform));
            if (target == null) throw new ArgumentNullException(nameof(target));
            return Vector2.Distance(transform.localScale, target);
        }
    }
}
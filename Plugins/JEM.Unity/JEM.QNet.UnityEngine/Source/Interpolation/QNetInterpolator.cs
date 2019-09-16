//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.QNet.UnityEngine.Objects;
using System;
using UnityEngine;

namespace JEM.QNet.UnityEngine.Interpolation
{
    /// <summary>
    ///     Defines a interpolation method of <see cref="QNetInterpolator"/>.
    /// </summary>
    public enum QNetInterpolationMethod
    {
        /// <summary>
        ///     The interpolator will interpolate Transform (Position,Rotation,Scale).
        /// </summary>
        Transform
    }

    /// <inheritdoc />
    /// <summary>
    ///     QNet interpolator component.
    ///     Allows to implement objects interpolation with minimal or zero custom code.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed partial class QNetInterpolator : QNetBehaviour
    {
        [Header("Base Configuration")]
        [SerializeField] private QNetInterpolationMethod _method;

        /// <summary>
        ///     Used interpolation method.
        /// </summary>
        public QNetInterpolationMethod Method
        {
            get => _method;
            set
            {
                if (_method == value)
                {
                    return;
                }

                _method = value;
                ApplyInterpolator();
            }
        }

        private void OnNetworkSpawned() => ApplyInterpolator();
        
        private void UnsafeSimulate()
        {
            switch (_method)
            {
                case QNetInterpolationMethod.Transform:
                    ApplyTransform();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        internal override void InterpolateFrame()
        {
            switch (_method)
            {
                case QNetInterpolationMethod.Transform:
                    InterpolateTransform();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnSendSnapshot()
        {
            switch (_method)
            {
                case QNetInterpolationMethod.Transform:
                    SendTransform();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ApplyInterpolator()
        {
            switch (_method)
            {
                case QNetInterpolationMethod.Transform:
                    LoadTransform();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

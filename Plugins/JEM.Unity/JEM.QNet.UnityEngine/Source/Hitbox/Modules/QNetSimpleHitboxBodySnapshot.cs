//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

namespace JEM.QNet.UnityEngine.Hitbox.Modules
{
    /// <inheritdoc />
    /// <summary>
    ///     Body snapshot used by <see cref="QNetSimpleHitboxBody" />
    /// </summary>
    internal class QNetSimpleHitboxBodySnapshot : QNetObjectHitboxBodySnapshot
    {
        public QNetSimpleHitboxBodySnapshot(int numHitboxes)
        {
            HitboxesSamples = new IQNetObjectHitboxSample[numHitboxes];
            for (int index = 0; index < numHitboxes; index++)
            {
                HitboxesSamples[index] = new QNetSimpleHitboxSample();
            }

            ProximitySample = new QNetSimpleHitboxSample();
        }
    }
}

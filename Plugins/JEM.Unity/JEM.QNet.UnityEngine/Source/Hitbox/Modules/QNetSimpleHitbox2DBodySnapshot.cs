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
    internal class QNetSimpleHitbox2DBodySnapshot : QNetObjectHitboxBodySnapshot
    {
        public QNetSimpleHitbox2DBodySnapshot(int numHitboxes)
        {
            HitboxesSamples = new IQNetObjectHitboxSample[numHitboxes];
            for (int index = 0; index < numHitboxes; index++)
            {
                HitboxesSamples[index] = new QNetSimpleHitbox2DSample();
            }

            ProximitySample = new QNetSimpleHitbox2DSample();
        }
    }
}

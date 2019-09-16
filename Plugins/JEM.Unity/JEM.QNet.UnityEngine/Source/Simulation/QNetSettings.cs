//
// QNet For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using JEM.Core.CVar;

namespace JEM.QNet.UnityEngine.Simulation
{
    /// <summary>
    ///     Contains a cVar controlled properties for the QNet system.
    /// </summary>
    public static class QNetSettings
    {
        /// <summary>
        ///     The client-side prediction active state.
        ///     While prediction is disabled, client will be interpolated by server instead.
        /// </summary>
        [JEMCVar("qnet.client_side_prediction", IsNetworkVar = true)]
        public static bool ClientSidePrediction { get; set; } = true;

        /// <summary>
        ///     The entity interpolation time in milliseconds.
        /// </summary>
        [JEMCVar("qnet.interpolation", "The entity interpolation time in milliseconds.", IsNetworkVar = true)]
        public static int Interpolation { get; set; } = 100;

        /// <summary>
        ///     The entity extrapolation time in milliseconds.
        /// </summary>
        [JEMCVar("qnet.extrapolation", "The entity extrapolation time in milliseconds.", IsNetworkVar = true)]
        public static int Extrapolation { get; set; } = 0;

        /// <summary>
        ///     The object result position maximal mismatch between client and server in units.
        /// </summary>
        [JEMCVar("qnet.max_position_mismatch",
            "The player result position maximal mismatch between client and server in units.", IsNetworkVar = true)]
        public static float PositionMaxMismatch { get; set; } = 0.005f;

        /// <summary>
        ///     The grid snapping precision used to snap object's simulation results.
        /// </summary>
        [JEMCVar("qnet.position_grid_snap", "The grid snapping precision " +
                                        "used to snap player's movement simulation results.", IsNetworkVar = true)]
        public static float PositionGridSnap { get; set; } = 0.0001f;
    }
}

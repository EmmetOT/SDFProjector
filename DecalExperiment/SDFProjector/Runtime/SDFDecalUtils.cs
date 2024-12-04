using System.Runtime.InteropServices;
using UnityEngine;

namespace Seed.DecalProjector {
    public static class SDFDecalUtils {
        /// Converts a unity color to a Vector3, discarding the alpha channel.
        public static Vector3 ToVector3(this Color colour) {
            return new Vector3(colour.r, colour.g, colour.b);
        }
    }
}
using System;
using UnityEngine;

namespace Seed.DecalProjector {
    [Serializable]
    public struct SDFColourProfile {
        [ColorUsage(false, false)]
        public Color MainColour;
        [ColorUsage(false, false)]
        public Color SecondaryColour;
        [ColorUsage(false, false)]
        public Color OutlineColour;
        [Min(0f)]
        public float OutlineWidth;
        [Min(0f)]
        public float AlternateColourFrequency;
        public float AlternateColourSpeed;
        public float AlternateColourAngle;
        
        public SDFColourProfile(Color mainColour, Color secondaryColour, Color outlineColour, float outlineWidth, float alternateColourFrequency, float alternateColourSpeed, float alternateColourAngle) {
            MainColour = mainColour;
            SecondaryColour = secondaryColour;
            OutlineColour = outlineColour;
            OutlineWidth = outlineWidth;
            AlternateColourFrequency = alternateColourFrequency;
            AlternateColourSpeed = alternateColourSpeed;
            AlternateColourAngle = alternateColourAngle;
        }
    }
}
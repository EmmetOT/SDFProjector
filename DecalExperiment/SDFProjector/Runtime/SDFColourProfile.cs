using System;
using UnityEngine;

namespace Seed.DecalProjector {
    [Serializable]
    public struct SDFColourProfile {
        public Color MainColour;
        public Color SecondaryColour;
        public float AlternateColourFrequency;
        public float AlternateColourSpeed;
        public float AlternateColourAngle;
        
        public SDFColourProfile(Color mainColour, Color secondaryColour, float alternateColourFrequency, float alternateColourSpeed, float alternateColourAngle) {
            MainColour = mainColour;
            SecondaryColour = secondaryColour;
            AlternateColourFrequency = alternateColourFrequency;
            AlternateColourSpeed = alternateColourSpeed;
            AlternateColourAngle = alternateColourAngle;
        }
    }
}
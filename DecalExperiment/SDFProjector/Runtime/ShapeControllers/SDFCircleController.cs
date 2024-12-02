using UnityEngine;

namespace Seed.DecalProjector {
    public class SDFCircleController : SDFShapeController {
        [SerializeField]
        [Min(0f)]
        private float radius = 1f;
        
        private SDFCircle sdfCircle;

        private void OnDisable() {
            if (Canvas != null && sdfCircle != null) {
                Canvas.RemoveCircle(sdfCircle);
            }
        }
        
        protected override void OnSetup() {
            if (sdfCircle == null) {
                sdfCircle = Canvas.CreateCircle(Position2D, radius, SDFColourProfile);
            } else {
                sdfCircle.Position = Position2D;
                sdfCircle.Radius = radius;
                sdfCircle.SDFColourProfile = SDFColourProfile;
            }
        }

        protected override void OnMoved() {
            if (sdfCircle == null) {
                sdfCircle = Canvas.CreateCircle(Position2D, radius, SDFColourProfile);
            } else {
                sdfCircle.Position = Position2D;
            }
        }
    }
}
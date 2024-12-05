using UnityEngine;

namespace Seed.DecalProjector {
    public class SDFCircleController : SDFShapeController {
        [SerializeField]
        [Min(0f)]
        private float radius = 1f;
        
        private SDFCircle sdfCircle;

        public void SetRadius(float newRadius) {
            radius = newRadius;
            Setup();
        }
        
        private void OnDisable() {
            if (Canvas != null && sdfCircle != null) {
                Canvas.RemoveCircle(sdfCircle);
                sdfCircle = null;
            }
        }
        
        protected override void OnSetup() {
            if (Canvas == null) {
                return;
            }
            
            if (sdfCircle == null || !Canvas.HasCircle(sdfCircle)) {
                sdfCircle = Canvas.CreateCircle(Position2D, radius, SDFColourProfile);
            } else {
                sdfCircle.Position = Position2D;
                sdfCircle.Radius = radius;
                sdfCircle.SDFColourProfile = SDFColourProfile;
                sdfCircle.Skip = skip;
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
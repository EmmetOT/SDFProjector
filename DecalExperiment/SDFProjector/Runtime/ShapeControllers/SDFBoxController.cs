using UnityEngine;

namespace Seed.DecalProjector {
    public class SDFBoxController : SDFShapeController {
        [SerializeField]
        private Vector2 size = Vector2.one;
        
        [SerializeField]
        [Min(0f)]
        private float rotation = 0f;

        [SerializeField]
        private Vector4 roundedness = Vector4.zero;

        private SDFBox sdfBox;
        
        private void OnDisable() {
            if (Canvas != null && sdfBox != null) {
                Canvas.RemoveBox(sdfBox);
                sdfBox = null;
            }
        }

        protected override void OnSetup() {
            if (Canvas == null) {
                return;
            }
            
            if (sdfBox == null || !Canvas.HasBox(sdfBox)) {
                sdfBox = Canvas.CreateBox(Position2D, rotation, size, roundedness, SDFColourProfile);
            } else {
                sdfBox.Position = Position2D;
                sdfBox.Size = size;
                sdfBox.Rotation = rotation;
                sdfBox.Roundedness = roundedness;
                sdfBox.SDFColourProfile = SDFColourProfile;
            }
        }

        protected override void OnMoved() {
            if (sdfBox == null) {
                sdfBox = Canvas.CreateBox(Position2D, rotation, size, roundedness, SDFColourProfile);
            } else {
                sdfBox.Position = Position2D;
            }
        }

        public void SetSize(Vector2 size) {
            this.size = size;
            Setup();
        }
    }
}
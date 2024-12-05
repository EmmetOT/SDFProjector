using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Seed.DecalProjector {
    [ExecuteInEditMode]
    public abstract class SDFShapeController : MonoBehaviour {
        [SerializeField]
        protected bool skip = false;
        
        [SerializeField]
        private SDFColourProfile sdfColourProfile = new() { MainColour = Color.white };
        protected SDFColourProfile SDFColourProfile => sdfColourProfile;
        
        public Vector2 Position2D => new(transform.position.x, transform.position.z);

        protected SDFCanvas Canvas => SDFCanvasController.Instance?.Canvas;
        
        private void Reset() => Setup();

        private void OnValidate() => Setup();
        private void OnEnable() => Setup();

        public void Setup() {
            OnSetup();
        }

        protected abstract void OnSetup();
        protected virtual void OnMoved() {}

        protected virtual void Update() {
            if (transform.hasChanged) {
                transform.hasChanged = false;
                OnMoved();
            }
        }
        
        
        public void SetColour(Color colour) {
            sdfColourProfile.MainColour = colour;
            Setup();
        }

    }
}
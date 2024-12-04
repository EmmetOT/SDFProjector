using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Seed.DecalProjector {
    [Serializable]
    public class SDFCircle : IDisposable {
        public event Action<SDFCircle> OnChange;
        public event Action<SDFCircle> OnRelease;
        
        private Vector2 position;
        public Vector2 Position {
            get => position;
            set {
                position = value;
                OnChange?.Invoke(this);
            }
        }
        
        private float radius;
        public float Radius {
            get => radius;
            set {
                radius = value;
                OnChange?.Invoke(this);
            }
        }
        
        private SDFColourProfile sdfColourProfile;
        public SDFColourProfile SDFColourProfile {
            get => sdfColourProfile;
            set {
                sdfColourProfile = value;
                OnChange?.Invoke(this);
            }
        }
        
        public SDFCircle(Vector2 position, float radius, SDFColourProfile sdfColourProfile) {
            this.position = position;
            this.radius = radius;
            this.sdfColourProfile = sdfColourProfile;
        }

        public void Dispose() {
            OnRelease?.Invoke(this);
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct CircleGPUData {
        public Vector2 Position;
        public float Radius;
        public Vector3 MainColour;
        public Vector3 SecondaryColour;
        public Vector3 OutlineColour;
        public float OutlineWidth;
        public float AlternateColourFrequency;
        public float AlternateColourSpeed;
        public float CosTheta;
        public float SinTheta;
    }
}
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Seed.DecalProjector {
    [Serializable]
    public class SDFBox : IDisposable {
        public event Action<SDFBox> OnChange;
        public event Action<SDFBox> OnRelease;
        
        private Vector2 position;
        public Vector2 Position {
            get => position;
            set {
                position = value;
                OnChange?.Invoke(this);
            }
        }
        
        private float rotation;
        public float Rotation {
            get => rotation;
            set {
                rotation = value;
                OnChange?.Invoke(this);
            }
        }

        private Vector2 size;
        public Vector2 Size {
            get => size;
            set {
                size = value;
                OnChange?.Invoke(this);
            }
        }
        
        private Vector4 roundedness;
        public Vector4 Roundedness {
            get => roundedness;
            set {
                roundedness = value;
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
        
        public SDFBox(Vector2 position, float rotation, Vector2 size, Vector4 roundedness, SDFColourProfile sdfColourProfile) {
            this.position = position;
            this.rotation = rotation;
            this.size = size;
            this.roundedness = roundedness;
            this.sdfColourProfile = sdfColourProfile;
        }

        public void Dispose() {
            OnRelease?.Invoke(this);
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct BoxGPUData {
        public Vector2 Position;
        public float Rotation;
        public Vector2 Size;
        public Vector4 Roundedness;
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
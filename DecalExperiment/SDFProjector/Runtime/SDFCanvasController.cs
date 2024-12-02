using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seed.DecalProjector {
    [ExecuteInEditMode]
    public class SDFCanvasController : MonoBehaviour {
        private SDFCanvas canvas;
        public SDFCanvas Canvas {
            get {
                if (canvas == null) {
                    Setup();
                }

                return canvas;
            }
        }
        
        [SerializeField]
        private UnityEngine.Rendering.Universal.DecalProjector decalProjector;
        
        [SerializeField]
        private Color backgroundColour = Color.black;
    
        [SerializeField]
        [Min(0f)]
        private float smoothing = 0.01f;

        [SerializeField]
        [Min(0f)]
        private float colorSmoothing = 0.01f;
        
        private Vector3 cachedBottomLeftCorner;
        private Vector3 cachedTopRightCorner;

        private void Reset() => Setup();

        private void OnValidate() => Setup();
        private void OnEnable() => Setup();

        private void Update() {
            if (decalProjector != null) {
                GetCorners(out var bottomLeft, out var topRight);
                
                if (bottomLeft != cachedBottomLeftCorner || topRight != cachedTopRightCorner) {
                    cachedBottomLeftCorner = bottomLeft;
                    cachedTopRightCorner = topRight;
                    
                    canvas.SetCorners(bottomLeft, topRight);
                }
            }
            
            canvas.Update();
        }
        
        private void OnDisable() {
            canvas?.Dispose();
            canvas = null;
        }
        
        private void Setup() {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
            
            canvas ??= new SDFCanvas();
        
            canvas.SetBackgroundColour(backgroundColour);
            canvas.SetSmoothing(smoothing);
            canvas.SetColorSmoothing(colorSmoothing);
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange stateChange) {
            if (stateChange == PlayModeStateChange.EnteredPlayMode) {
                ForceSetupAll();
            } else if (stateChange == PlayModeStateChange.EnteredEditMode) {
                ForceSetupAll();
            }
        }
#endif

        private void GetCorners(out Vector3 bottomLeft, out Vector3 topRight) {
            var size = decalProjector.size;
            size.z = 0f;
            var halfSize = size * 0.5f;
            bottomLeft = decalProjector.transform.TransformPoint(-halfSize);
            topRight = decalProjector.transform.TransformPoint(halfSize);
        }
        
        [ContextMenu("Clear All")]
        private void ClearAll() {
            canvas?.ClearCircles();
            canvas?.ClearLines();
            canvas?.ClearBoxes();
        }

        private void ForceSetupAll()  {
            // True Unity magic right here.
            if (!this) { 
                return;
            }
            
            var controllers = GetComponentsInChildren<SDFShapeController>();
            foreach (var controller in controllers) {
                if (controller) {
                    controller.Setup();
                }
            }
        }
    }
}
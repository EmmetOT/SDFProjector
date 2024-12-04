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
        
        private static SDFCanvasController instance;
        public static SDFCanvasController Instance {
            get {
                if (instance == null) {
                    instance = FindObjectOfType<SDFCanvasController>();
                }

                return instance;
            }
        }

        [SerializeField]
        [Min(0f)]
        private float margin = 10f;
        
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

        private void LateUpdate() {
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
        
        private void OnEnable() => Setup();

        private void OnDisable() {
            canvas?.Dispose();
            canvas = null;
        }
        
        public void SetProjectorSize(Vector2 size, float depth) {
            decalProjector.size = new(size.x, size.y, depth);
        }

        public void SetRenderingLayer(uint renderingLayerMask) {
            decalProjector.renderingLayerMask = renderingLayerMask;
        }
        
        private void Setup() {
            if (!enabled) {
                return;
            }
            
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
            
            canvas ??= new SDFCanvas();
            
            canvas.SetBackgroundColour(backgroundColour);
            canvas.SetSmoothing(smoothing);
            canvas.SetColourSmoothing(colorSmoothing);
        }

#if UNITY_EDITOR
        private void OnPlayModeStateChanged(PlayModeStateChange stateChange) {
            if (stateChange == PlayModeStateChange.EnteredPlayMode) {
                ForceSetupAll();
            } else if (stateChange == PlayModeStateChange.EnteredEditMode) {
                ForceSetupAll();
            }
        }
        
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded() {
            var controllers = FindObjectsOfType<SDFCanvasController>();
            foreach (var controller in controllers) {
                controller.Setup();
                controller.ForceSetupAll();
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
        public void ClearAll() {
            canvas?.ClearCircles();
            canvas?.ClearLines();
            canvas?.ClearBoxes();
        }

        private void ForceSetupAll()  {
            // True Unity magic right here.
            if (!this) { 
                return;
            }
            
            var controllers = FindObjectsOfType<SDFShapeController>();
            foreach (var controller in controllers) {
                if (controller) {
                    controller.Setup();
                }
            }
        }
    }
}
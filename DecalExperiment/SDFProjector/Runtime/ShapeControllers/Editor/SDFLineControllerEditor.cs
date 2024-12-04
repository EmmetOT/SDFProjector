#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Seed.DecalProjector.Editor {
    [CustomEditor(typeof(SDFLineController))]
    public class SDFLineControllerEditor : UnityEditor.Editor {
        private SDFLineController lineController;

        private readonly List<Vector3> points = new();
        
        private void OnEnable() {
            lineController = (SDFLineController)target;
        }

        private void OnSceneGUI() {
            lineController.GetPoints(points);
            
            if (points == null || points.Count == 0) {
                return;
            }

            var changed = false;

            for (var i = 0; i < points.Count; i++) {
                EditorGUI.BeginChangeCheck();
                var newPoint = Handles.PositionHandle(lineController.transform.TransformPoint(points[i]), Quaternion.identity);

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(this, "Move Point");
                    points[i] = lineController.transform.InverseTransformPoint(newPoint);
                    changed = true;
                }
            }
            
            if (changed) {
                lineController.SetPoints(points);
            }
        }
    }
}

#endif
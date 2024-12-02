using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Seed.DecalProjector {
    public class SDFLineController : SDFShapeController {
        [SerializeField]
        [Min(0f)]
        private float width = 1f;

        [SerializeField]
        private bool isLoop = false;        
        
        [SerializeField]
        private bool alignColourWithLine = false;

        [SerializeField]
        private Vector3[] points = { Vector3.zero, Vector3.right };

        [SerializeField]
        [HideInInspector]
        private Vector2[] tranformedPoints;
        
        private SDFLine sdfLine;

        private void OnDisable() {
            if (Canvas != null && sdfLine != null) {
                Canvas.RemoveLine(sdfLine);
            }
        }

        protected override void OnSetup() {
            TransformPoints();
            
            if (sdfLine == null) {
                sdfLine = Canvas.CreateLine(width, isLoop, alignColourWithLine, SDFColourProfile, tranformedPoints);
            } else {
                sdfLine.Width = width;
                sdfLine.IsLoop = isLoop;
                sdfLine.AlignColourWithLine = alignColourWithLine;
                sdfLine.SDFColourProfile = SDFColourProfile;
                sdfLine.SetPoints(tranformedPoints);
            }
        }

        protected override void OnMoved() {
            TransformPoints();

            if (sdfLine == null) {
                sdfLine = Canvas.CreateLine(width, isLoop, alignColourWithLine, SDFColourProfile, tranformedPoints);
            } else {
                sdfLine.SetPoints(tranformedPoints);
            }
        }
        
        private void TransformPoints() {
            tranformedPoints = new Vector2[points.Length];
            for (var i = 0; i < points.Length; i++) {
                var temp = transform.TransformPoint(points[i]);
                tranformedPoints[i] = new Vector2(temp.x, temp.z);
            }
        }
        
        public void GetPoints(List<Vector3> newPoints) {
            newPoints.Clear();
            newPoints.AddRange(points);
        }
        
        public void SetPoints(IList<Vector3> newPoints) {
            if (newPoints.Count != points.Length) {
                points = new Vector3[newPoints.Count];
            }
            
            for (var i = 0; i < newPoints.Count; i++) {
                points[i] = newPoints[i];
            }
            
            OnMoved();
        }
    }
}
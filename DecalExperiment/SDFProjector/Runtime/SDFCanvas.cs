using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Seed.DecalProjector {
    public class SDFCanvas : IDisposable {
        private Dictionary<SDFCircle, CircleGPUData> circleData = new();
        private List<CircleGPUData> tempCircles = new();
        private ComputeBuffer circleBuffer;
        public int CircleCount => circleData.Count;
        
        private Dictionary<SDFBox, BoxGPUData> boxData = new();
        private List<BoxGPUData> tempBoxes = new();
        private ComputeBuffer boxBuffer;
        public int BoxCount => boxData.Count;
        
        private Dictionary<SDFLine, LineGPUData> lineData = new();
        private List<LineGPUData> tempLines = new();
        private List<LinePointGPUData> linePoints = new();
        private ComputeBuffer lineBuffer;
        private ComputeBuffer linePointBuffer;
        public int LineCount => lineData.Count;
        
        private float smoothing = 0.01f;
        public float Smoothing => smoothing;
        
        private float colourSmoothing = 0.01f;
        public float ColourSmoothing => colourSmoothing;
        
        private Color backgroundColour = Color.black;
        public Color BackgroundColour => backgroundColour;

        public void Update() {
            Shader.SetGlobalFloat(SDFShaderGlobals.ElapsedTime, Time.time);
        }

        public SDFCanvas() {
            Dispose();
            
            UpdateBuffer(circleBuffer, tempCircles, SDFShaderGlobals.CircleBuffer, SDFShaderGlobals.CircleCount);
            UpdateBuffer(boxBuffer, tempBoxes, SDFShaderGlobals.BoxBuffer, SDFShaderGlobals.BoxCount);
            UpdateBuffer(lineBuffer, tempLines, SDFShaderGlobals.LineBuffer, SDFShaderGlobals.LineCount);
            UpdateBuffer(linePointBuffer, linePoints, SDFShaderGlobals.LinePointBuffer, SDFShaderGlobals.LinePointCount);
        }
        
        public void Dispose() {
            ClearCircles();
            ClearBoxes();
            ClearLines();
        }
        
        public void SetBackgroundColour(Color colour) {
            backgroundColour = colour;
            Shader.SetGlobalColor(SDFShaderGlobals.BackgroundColour, backgroundColour);
        }
        
        public void SetSmoothing(float value) {
            smoothing = value;
            Shader.SetGlobalFloat(SDFShaderGlobals.Smoothing, smoothing);
        }
        
        public void SetColourSmoothing(float value) {
            colourSmoothing = value;
            Shader.SetGlobalFloat(SDFShaderGlobals.ColorSmoothing, colourSmoothing);
        }
        
        public void SetCorners(Vector3 bottomLeft, Vector3 topRight) {
            bottomLeft = new Vector2(bottomLeft.x, bottomLeft.z);
            topRight = new Vector2(topRight.x, topRight.z);
            Shader.SetGlobalVector(SDFShaderGlobals.BottomLeft, bottomLeft);
            Shader.SetGlobalVector(SDFShaderGlobals.TopRight, topRight);
        }
        
        public bool HasCircle(SDFCircle circle) {
            return circleData.ContainsKey(circle);
        }
        
        public SDFCircle CreateCircle(Vector2 position, float radius, SDFColourProfile sdfColourProfile) {
            var circle = new SDFCircle(position, radius, sdfColourProfile);
            circle.OnChange += OnCircleChanged;
            circle.OnRelease += OnCircleReleased;
            
            OnCircleChanged(circle);
            return circle;
        }

        public void RemoveCircle(SDFCircle circle) {
            if (!circleData.Remove(circle)) {
                return;
            }
            
            tempCircles.Clear();
            tempCircles.AddRange(circleData.Values);

            UpdateBuffer(circleBuffer, tempCircles, SDFShaderGlobals.CircleBuffer, SDFShaderGlobals.CircleCount);
        }
        
        public bool HasBox(SDFBox box) {
            return boxData.ContainsKey(box);
        }

        public SDFBox CreateBox(Vector2 position, float rotation, Vector2 size, Vector4 roundedness, SDFColourProfile sdfColourProfile) {
            var box = new SDFBox(position, rotation, size, roundedness, sdfColourProfile);
            box.OnChange += OnBoxChanged;
            box.OnRelease += OnBoxReleased;
            
            OnBoxChanged(box);
            return box;
        }
        
        public void RemoveBox(SDFBox box) {
            if (!boxData.Remove(box)) {
                return;
            }
            
            tempBoxes.Clear();
            tempBoxes.AddRange(boxData.Values);

            UpdateBuffer(boxBuffer, tempBoxes, SDFShaderGlobals.BoxBuffer, SDFShaderGlobals.BoxCount);
        }
        
        public bool HasLine(SDFLine line) {
            return lineData.ContainsKey(line);
        }

        public SDFLine CreateLine(float width, bool isLoop, bool alignColourWithLine, SDFColourProfile sdfColourProfile, params Vector2[] points) {
            var line = new SDFLine(width, isLoop, alignColourWithLine, sdfColourProfile, points);
            line.SetIndices(linePoints.Count, linePoints.Count + points.Length - 1);
            line.OnChange += OnLineChanged;
            line.OnRelease += OnLineReleased;
            line.OnPointChanged += OnLinePointChanged;
            line.OnPointsChanged += OnLinePointsChanged;
            
            OnLineChanged(line);
            OnLinePointsChanged(line);
            return line;
        }
        
        public SDFLine CreateLine(float width, bool isLoop, bool alignColourWithLine, SDFColourProfile sdfColourProfile, IList<Vector2> points) {
            return CreateLine(width, isLoop, alignColourWithLine, sdfColourProfile, points.ToArray());
        }
        
        public void RemoveLine(SDFLine line) {
            if (!lineData.Remove(line)) {
                return;
            }
            
            tempLines.Clear();
            tempLines.AddRange(lineData.Values);

            UpdateBuffer(lineBuffer, tempLines, SDFShaderGlobals.LineBuffer, SDFShaderGlobals.LineCount);
            
            var oldStartIndex = line.StartIndex;
            var oldEndIndex = line.EndIndex;
            
            // TODO this does not handle the case where more lines follow this line

            linePoints.RemoveRange(oldStartIndex, oldEndIndex - oldStartIndex + 1);
            UpdateBuffer(linePointBuffer, linePoints, SDFShaderGlobals.LinePointBuffer, SDFShaderGlobals.LinePointCount);
        }
        
        public void ClearCircles() {
            foreach (var circle in circleData.Keys) {
                circle.OnChange -= OnCircleChanged;
                circle.OnRelease -= OnCircleReleased;
            }
            
            circleData.Clear();
            tempCircles.Clear();
            circleBuffer?.Dispose();
            Shader.SetGlobalInt(SDFShaderGlobals.CircleCount, 0);
        }
        
        public void ClearBoxes() {
            foreach (var box in boxData.Keys) {
                box.OnChange -= OnBoxChanged;
                box.OnRelease -= OnBoxReleased;
            }
            
            boxData.Clear();
            tempBoxes.Clear();
            boxBuffer?.Dispose();
            Shader.SetGlobalInt(SDFShaderGlobals.BoxCount, 0);
        }
        
        public void ClearLines() {
            foreach (var line in lineData.Keys) {
                line.OnChange -= OnLineChanged;
                line.OnRelease -= OnLineReleased;
                line.OnPointChanged -= OnLinePointChanged;
                line.OnPointsChanged -= OnLinePointsChanged;
            }
            
            lineData.Clear();
            tempLines.Clear();
            linePoints.Clear();
            lineBuffer?.Dispose();
            linePointBuffer?.Dispose();
            Shader.SetGlobalInt(SDFShaderGlobals.LineCount, 0);
            Shader.SetGlobalInt(SDFShaderGlobals.LinePointCount, 0);
        }
        
        private void OnCircleChanged(SDFCircle circle) {
            circleData[circle] = CreateCircleGPUData(circle);
            
            tempCircles.Clear();
            tempCircles.AddRange(circleData.Values);

            UpdateBuffer(circleBuffer, tempCircles, SDFShaderGlobals.CircleBuffer, SDFShaderGlobals.CircleCount);
        }
        
        private void OnCircleReleased(SDFCircle circle) {
            circle.OnChange -= OnCircleChanged;
            circle.OnRelease -= OnCircleReleased;
            circleData.Remove(circle);
            
            tempCircles.Clear();
            tempCircles.AddRange(circleData.Values);
            
            UpdateBuffer(circleBuffer, tempCircles, SDFShaderGlobals.CircleBuffer, SDFShaderGlobals.CircleCount);
        }

        private static CircleGPUData CreateCircleGPUData(SDFCircle circle) {
            return new CircleGPUData {
                Position = circle.Position,
                Radius = circle.Radius,
                MainColour = circle.SDFColourProfile.MainColour.ToVector3(),
                SecondaryColour = circle.SDFColourProfile.SecondaryColour.ToVector3(),
                OutlineColour = circle.SDFColourProfile.OutlineColour.ToVector3(),
                OutlineWidth = circle.SDFColourProfile.OutlineWidth,
                AlternateColourFrequency = circle.SDFColourProfile.AlternateColourFrequency,
                AlternateColourSpeed = circle.SDFColourProfile.AlternateColourSpeed,
                CosTheta = Mathf.Cos(Mathf.Deg2Rad * circle.SDFColourProfile.AlternateColourAngle),
                SinTheta = Mathf.Sin(Mathf.Deg2Rad * circle.SDFColourProfile.AlternateColourAngle),
            };
        }
        
        private void OnBoxChanged(SDFBox box) {
            boxData[box] = CreateBoxGPUData(box);
            
            tempBoxes.Clear();
            tempBoxes.AddRange(boxData.Values);

            UpdateBuffer(boxBuffer, tempBoxes, SDFShaderGlobals.BoxBuffer, SDFShaderGlobals.BoxCount);
        }
        
        private void OnBoxReleased(SDFBox box) {
            box.OnChange -= OnBoxChanged;
            box.OnRelease -= OnBoxReleased;
            boxData.Remove(box);
            
            tempBoxes.Clear();
            tempBoxes.AddRange(boxData.Values);
            
            UpdateBuffer(boxBuffer, tempBoxes, SDFShaderGlobals.BoxBuffer, SDFShaderGlobals.BoxCount);
        }
        
        private static BoxGPUData CreateBoxGPUData(SDFBox box) {
            return new BoxGPUData {
                Position = box.Position,
                Rotation = box.Rotation,
                Size = box.Size,
                Roundedness = box.Roundedness,
                MainColour = box.SDFColourProfile.MainColour.ToVector3(),
                SecondaryColour = box.SDFColourProfile.SecondaryColour.ToVector3(),
                OutlineColour = box.SDFColourProfile.OutlineColour.ToVector3(),
                OutlineWidth = box.SDFColourProfile.OutlineWidth,
                AlternateColourFrequency = box.SDFColourProfile.AlternateColourFrequency,
                AlternateColourSpeed = box.SDFColourProfile.AlternateColourSpeed,
                CosTheta = Mathf.Cos(Mathf.Deg2Rad * box.SDFColourProfile.AlternateColourAngle),
                SinTheta = Mathf.Sin(Mathf.Deg2Rad * box.SDFColourProfile.AlternateColourAngle),
            };
        }
        
        private void OnLineChanged(SDFLine line) {
            lineData[line] = CreateLineGPUData(line);
            
            tempLines.Clear();
            tempLines.AddRange(lineData.Values);

            UpdateBuffer(lineBuffer, tempLines, SDFShaderGlobals.LineBuffer, SDFShaderGlobals.LineCount);
        }

        private void OnLinePointsChanged(SDFLine line) {
            if (linePoints.Count > 0) {
                var oldStartIndex = line.StartIndex;
                var oldEndIndex = line.EndIndex;
            
                linePoints.RemoveRange(oldStartIndex, oldEndIndex - oldStartIndex + 1);
            }

            foreach (var lP in line.Points) {
                linePoints.Add(new LinePointGPUData {
                    Position = lP.Position,
                    DistanceAlongLine = lP.DistanceAlongLine
                });
            }
            
            // TODO this does not handle the case where more lines follow this line
            
            line.SetIndices(linePoints.Count - line.Points.Count, linePoints.Count - 1);
            UpdateBuffer(linePointBuffer, linePoints, SDFShaderGlobals.LinePointBuffer, SDFShaderGlobals.LinePointCount);
        }

        private void OnLineReleased(SDFLine line) {
            line.OnChange -= OnLineChanged;
            line.OnRelease -= OnLineReleased;
            line.OnPointChanged -= OnLinePointChanged;
            line.OnPointsChanged -= OnLinePointsChanged;
            lineData.Remove(line);
            
            tempLines.Clear();
            tempLines.AddRange(lineData.Values);
            
            UpdateBuffer(lineBuffer, tempLines, SDFShaderGlobals.LineBuffer, SDFShaderGlobals.LineCount);
            
            var oldStartIndex = line.StartIndex;
            var oldEndIndex = line.EndIndex;
            
            // TODO this does not handle the case where more lines follow this line

            linePoints.RemoveRange(oldStartIndex, oldEndIndex - oldStartIndex + 1);
            UpdateBuffer(linePointBuffer, linePoints, SDFShaderGlobals.LinePointBuffer, SDFShaderGlobals.LinePointCount);
        }
        
        private void OnLinePointChanged(SDFLine line, SDFLinePoint point, int index) {
            var startIndex = line.StartIndex;
            linePoints[startIndex + index] = new LinePointGPUData {
                Position = point.Position,
                DistanceAlongLine = point.DistanceAlongLine
            };
            
            UpdateBuffer(linePointBuffer, linePoints, SDFShaderGlobals.LinePointBuffer, SDFShaderGlobals.LinePointCount);
        }
        
        private static LineGPUData CreateLineGPUData(SDFLine line) {
            return new LineGPUData() {
                PointIndexStart = (uint)line.StartIndex,
                PointIndexEnd = (uint)line.EndIndex,
                Width = line.Width,
                TotalLength = line.TotalLength,
                MainColour = line.SDFColourProfile.MainColour.ToVector3(),
                SecondaryColour = line.SDFColourProfile.SecondaryColour.ToVector3(),
                OutlineColour = line.SDFColourProfile.OutlineColour.ToVector3(),
                OutlineWidth = line.SDFColourProfile.OutlineWidth,
                AlternateColourFrequency = line.SDFColourProfile.AlternateColourFrequency,
                AlternateColourSpeed = line.SDFColourProfile.AlternateColourSpeed,
                CosTheta = Mathf.Cos(Mathf.Deg2Rad * line.SDFColourProfile.AlternateColourAngle),
                SinTheta = Mathf.Sin(Mathf.Deg2Rad * line.SDFColourProfile.AlternateColourAngle),
                AlignColourWithLine = line.AlignColourWithLine ? 1 : 0,
                Loop = line.IsLoop ? 1 : 0
            };
        }
        
        private static void UpdateBuffer<T>(ComputeBuffer buffer, List<T> data, int bufferName, int countName) where T : struct {
            if (buffer == null || !buffer.IsValid() || data.Count != buffer.count) {
                buffer?.Dispose();
                buffer = new ComputeBuffer(Mathf.Max(1, data.Count), Marshal.SizeOf(typeof(T)));
            }

            buffer.SetData(data);
            Shader.SetGlobalInt(countName, data.Count);
            Shader.SetGlobalBuffer(bufferName, buffer);
        }
    }
}

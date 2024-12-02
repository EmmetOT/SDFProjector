using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

namespace Seed.DecalProjector {
    [Serializable]
    public class SDFLine : IDisposable {
        public event Action<SDFLine> OnChange;
        public event Action<SDFLine> OnRelease;
        public event Action<SDFLine, SDFLinePoint, int> OnPointChanged;
        public event Action<SDFLine> OnPointsChanged;
        
        private float width;
        public float Width {
            get => width;
            set {
                width = value;
                OnChange?.Invoke(this);
            }
        }
        
        private bool isLoop;
        public bool IsLoop {
            get => isLoop;
            set {
                isLoop = value;
                OnChange?.Invoke(this);
            }
        }
        
        private bool alignColourWithLine;
        public bool AlignColourWithLine {
            get => alignColourWithLine;
            set {
                alignColourWithLine = value;
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
        
        private List<SDFLinePoint> points = new();
        public IReadOnlyCollection<SDFLinePoint> Points => points;
        
        public void SetPoint(int index, Vector2 point) {
            if (index < 0 || index >= points.Count) {
                throw new IndexOutOfRangeException();
            }
            
            points[index] = new SDFLinePoint { Position = point };
            RecalculateDistances();
            OnPointChanged?.Invoke(this, points[index], index);
        }

        public void SetPoints(IList<Vector2> newPoints) {
            points ??= new List<SDFLinePoint>();
            points.Clear();
            points.AddRange(newPoints.Select(p => new SDFLinePoint { Position = p }));
            RecalculateDistances();
            OnPointsChanged?.Invoke(this);
        }

        public void AddPoint(Vector2 point) {
            points.Add(new SDFLinePoint { Position = point });
            RecalculateDistances();
            OnPointsChanged?.Invoke(this);
        }
        
        public void RemovePoint(int index) {
            if (index < 0 || index >= points.Count) {
                throw new IndexOutOfRangeException();
            }
            
            points.RemoveAt(index);
            RecalculateDistances();
            OnPointsChanged?.Invoke(this);
        }

        public float TotalLength { get; private set; } = 0f;

        public int StartIndex { get; private set; } = -1;

        public int EndIndex { get; private set; } = -1;

        public void SetIndices(int start, int end) {
            StartIndex = start;
            EndIndex = end;
        }
        
        private void RecalculateDistances() {
            TotalLength = 0f;
            for (var i = 0; i < points.Count; i++) {
                var point = points[i];
                var distance = i == 0 ? 0f : Vector3.Distance(points[i - 1].Position, point.Position);
                TotalLength += distance;
                points[i] = new SDFLinePoint {
                    Position = point.Position,
                    DistanceAlongLine = TotalLength
                };
            }
        }
        
        public SDFLine(float width, bool isLoop, bool alignColourWithLine, SDFColourProfile sdfColourProfile, params Vector2[] points) {
            this.width = width;
            this.isLoop = isLoop;
            this.alignColourWithLine = alignColourWithLine;
            this.sdfColourProfile = sdfColourProfile;
            this.points.AddRange(points.Select(p => new SDFLinePoint { Position = p }));
            RecalculateDistances();
        }        
        
        public SDFLine(float width, bool isLoop, bool alignColourWithLine, SDFColourProfile sdfColourProfile, IList<Vector2> points) : this(width, isLoop, alignColourWithLine, sdfColourProfile, points.ToArray()) { }
        
        public void Dispose() {
            OnRelease?.Invoke(this);
        }
    }
    
    public struct SDFLinePoint {
        public Vector2 Position;
        public float DistanceAlongLine;
    }
    
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct LineGPUData {
        public uint PointIndexStart;
        public uint PointIndexEnd;
        public float Width;
        public float TotalLength;
        public Vector3 MainColour;
        public Vector3 SecondaryColour;
        public float AlternateColourFrequency;
        public float AlternateColourSpeed;
        public float AlternateColourAngle;
        public int AlignColourWithLine;
        public int Loop;
    }

    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct LinePointGPUData {
        public Vector2 Position;
        public float DistanceAlongLine;
    };
}
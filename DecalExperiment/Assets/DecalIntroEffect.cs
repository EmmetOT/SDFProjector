using System.Collections.Generic;
using Seed.DecalProjector;
using UnityEngine;

public class DecalIntroEffect {
    private const float circleRadius = 10f;
    private const float startPositionOffset = 30f;
    private const float startingSpeed = 200f;
    private const float attractionForce = 100f;
    private const float damping = 0.95f;

    private struct Info {
        public Transform Transform;
        public Vector2 CurrentVelocity;
        public Vector2 CurrentPosition;
    }
    
    private readonly SDFCanvas canvas;
    private readonly Dictionary<SDFCircle, Info> circles = new();
    
    private float timeElapsed = 0f;
    private float totalTime = 0f;
    private bool isPlaying = false;
    
    public DecalIntroEffect(SDFCanvas canvas) {
        this.canvas = canvas;
    }

    public void Update() {
        if (!isPlaying) {
            return;
        }
        
        timeElapsed += Time.deltaTime;
        
        var normalizedTime = timeElapsed / totalTime;
        
        // Move circles towards the seedlings, such that at the end of the animation they are at the seedling positions
        
        var keys = new List<SDFCircle>(circles.Keys);
        
        for (var i = 0; i < keys.Count; i++) {
            var circle = keys[i];
            var info = circles[circle];
            
            var seedlingPos2D = new Vector2(info.Transform.position.x, info.Transform.position.z);
            
            // Add attraction force towards the seedling
            var direction = (seedlingPos2D - info.CurrentPosition).normalized;
            var distance = Vector2.Distance(info.CurrentPosition, seedlingPos2D);
            var attraction = direction * (attractionForce * distance);
            
            var newVelocity = (info.CurrentVelocity + attraction * Time.deltaTime) * damping;
            var newPosition = info.CurrentPosition + newVelocity * Time.deltaTime;
            
            if (distance < 0.01f && newVelocity.magnitude < 0.01f) {
                newVelocity = Vector2.zero;
                newPosition = seedlingPos2D;
            }

            circles[circle] = new() { Transform = info.Transform, CurrentVelocity = newVelocity, CurrentPosition = newPosition };
            circle.Position = newPosition;
        }
    }
    
    public void DrawGizmos() {
        if (!isPlaying) {
            return;
        }
        
        foreach (var (key, val) in circles) {
            Gizmos.color = key.SDFColourProfile.MainColour;
            
            var pos2D = new Vector3(val.CurrentPosition.x, 0f, val.CurrentPosition.y);
            Gizmos.DrawSphere(pos2D, 4f);
            
            var targetPos2D = new Vector3(val.Transform.position.x, 0f, val.Transform.position.z);
            Gizmos.DrawLine(pos2D, targetPos2D);
            Gizmos.DrawSphere(targetPos2D, 1f);
        }
    }
    
    public void Play(IList<Transform> seedlingTransforms, float totalTime) {
        this.totalTime = totalTime;
        
        isPlaying = true;
        timeElapsed = 0f;

        Vector2 midpoint = Vector2.zero;
        foreach (var seedlingTransform in seedlingTransforms) {
            midpoint += new Vector2(seedlingTransform.position.x, seedlingTransform.position.z);
        }
        midpoint /= seedlingTransforms.Count;
        
        foreach (var seedlingTransform in seedlingTransforms) {
            // Gemerate a random point on a circle centered on the midpoint with the radius startPositionOffset
            var startPosition = (Random.insideUnitCircle.normalized * startPositionOffset) + midpoint;
            
            // generate momentum vector at a tangent to the circle
            var direction = (startPosition - midpoint).normalized;
            var startingVelocity = new Vector2(-direction.y, direction.x) * startingSpeed;
            
            var randomColour = new Color(Random.value, Random.value, Random.value);
            
            var circle = canvas.CreateCircle(startPosition, circleRadius, new(randomColour, randomColour, randomColour, 0f, 0f, 0f, 0f));
            circles.Add(circle, new() { Transform = seedlingTransform, CurrentVelocity = startingVelocity, CurrentPosition = startPosition });
        }
    }
    
    public void Stop() {
        isPlaying = false;
        
        foreach (var circle in circles.Keys) {
            canvas.RemoveCircle(circle);
        }
    }
}
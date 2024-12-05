#ifndef SDF_DECALS_SDFS_INCLUDED
#define SDF_DECALS_SDFS_INCLUDED

#include "SDFDecals_Structs.hlsl"
#include "SDFDecals_Maths.hlsl"
#include "SDFDecals_Globals.hlsl"

float SDF_Circle(float2 p, CircleData data) {
    // adding a tiny number prevents NaN
    float2 difference = (data.Position - p) + EPSILON;
    return lerp(length(difference) - data.Radius, MAX_DIST, data.Skip);
}
//
// float SDF_Circle(float2 p, float2 circlePos, float radius) {
//     // adding a tiny number prevents NaN
//     float2 difference = (circlePos - p) + EPSILON;
//     return length(difference) - radius;
// }

float SDF_LineSegment(float2 p, float2 a, float2 b, float width) {
    // https://iquilezles.org/www/articles/distgradfunctions2d/distgradfunctions2d.htm
    float2 pa = p - a, ba = b - a;
    float h = saturate(dot(pa, ba) / dot(ba, ba));
    float2 q = pa - h * ba;
    float d = length(q);

    return d - width;
}

float SDF_Line(float2 p, LineData data) {
    uint startIndex = data.PointIndexStart;
    uint endIndex = data.PointIndexEnd;
    float minDist = 1000000.;

    for (uint i = startIndex; i < endIndex; i++) {
        LinePoint a = _LinePointBuffer[i];
        LinePoint b = _LinePointBuffer[i + 1];
    
        minDist = min(minDist, SDF_LineSegment(p, a.Position, b.Position, data.Width));
    }

    if (data.Loop > 0) {
        LinePoint a = _LinePointBuffer[endIndex];
        LinePoint b = _LinePointBuffer[startIndex];
        minDist = min(minDist, SDF_LineSegment(p, a.Position, b.Position, data.Width));
    }

    return lerp(minDist, MAX_DIST, data.Skip);
}

float SDF_Box(float2 p, BoxData data) {
    p = rotate(data.Position - p, data.Rotation * DEG_TO_RAD);
    float4 r = data.Roundedness;

    r.xy = (p.x > 0.0)?r.xy : r.zw;
    r.x = (p.y > 0.0)?r.x : r.y;
    float2 q = abs(p) - data.Size + r.x;
    return lerp(min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x, MAX_DIST, data.Skip);
}

// smooth min but also smoothly combines associated float4s (e.g. colours)
float SDF_SmoothMin(float d1, float d2, float4 v1, float4 v2, out float4 vResult) {
    float dH = saturate(0.5 + 0.5 * (d2 - d1) / _Smoothing);
    float vH = saturate(0.5 + 0.5 * (d2 - d1) / _ColorSmoothing);

    vResult = lerp(v2, v1, vH);
    return lerp(d2, d1, dH) - _Smoothing * dH * (1.0 - dH);
}

float4 SDF_Colour(float2 p, float3 mainColour, float3 secondaryColour, 
                  float alternateColourFrequency, float alternateColourSpeed, 
                  float cosTheta, float sinTheta) {
    float2 rotatedP = float2(p.x * cosTheta - p.y * sinTheta, p.x * sinTheta + p.y * cosTheta);
    float wave = sin(rotatedP.x * alternateColourFrequency + _ElapsedTime * alternateColourSpeed);
    return float4(lerp(secondaryColour, mainColour, step(0.0, wave)), 1.0);
}

float4 SDF_Colour_Circle(float2 p, CircleData data) {
    float dist = SDF_Circle(p, data);
    
    float4 mainCol = SDF_Colour(
        p, 
        data.MainColour, 
        data.SecondaryColour, 
        data.AlternateColourFrequency, 
        data.AlternateColourSpeed, 
        data.CosTheta, 
        data.SinTheta
    );
    
    float innerDist = abs(dist);
    float outlineFactor = step(innerDist, data.OutlineWidth);
    float4 finalCol = lerp(mainCol, float4(data.OutlineColour, 1.0), outlineFactor);
    return lerp(finalCol, float4(data.OutlineColour, 1.0), step(0.0, dist));
}

float4 SDF_Colour_Box(float2 p, BoxData data) {
    float dist = SDF_Box(p, data);
    
    float4 mainCol = SDF_Colour(
        p, 
        data.MainColour, 
        data.SecondaryColour, 
        data.AlternateColourFrequency, 
        data.AlternateColourSpeed, 
        data.CosTheta, 
        data.SinTheta
    );
    
    float innerDist = abs(dist);
    float outlineFactor = step(innerDist, data.OutlineWidth);
    float4 finalCol = lerp(mainCol, float4(data.OutlineColour, 1.0), outlineFactor);
    return lerp(finalCol, float4(data.OutlineColour, 1.0), step(0.0, dist));
}

float4 SDF_Colour_Line(float2 p, LineData data) {
    float4 mainCol;
    float minDist = 1000000.;
    if (data.AlignColourWithLine <= 0) {
        minDist = SDF_Line(p, data);
        mainCol = SDF_Colour(p, data.MainColour, data.SecondaryColour, 
                          data.AlternateColourFrequency, data.AlternateColourSpeed, 
                          data.CosTheta, data.SinTheta);
    } else {
        uint startIndex = data.PointIndexStart;
        uint endIndex = data.PointIndexEnd;

        int closestSegmentStart = -1;
        int closestSegmentEnd = -1;

        // Find the closest line segment
        for (uint i = startIndex; i < endIndex; i++) {
            LinePoint a = _LinePointBuffer[i];
            LinePoint b = _LinePointBuffer[i + 1];

            float dist = SDF_LineSegment(p, a.Position, b.Position, data.Width);
            if (dist < minDist) {
                minDist = dist;
                closestSegmentStart = i;
                closestSegmentEnd = i + 1;
            }
        }

        if (data.Loop > 0) {
            LinePoint a = _LinePointBuffer[endIndex];
            LinePoint b = _LinePointBuffer[startIndex];
            float dist = SDF_LineSegment(p, a.Position, b.Position, data.Width);
            if (dist < minDist) {
                minDist = dist;
                closestSegmentStart = endIndex;
                closestSegmentEnd = startIndex;
            }
        }

        // Ensure a valid closest segment is found
        if (closestSegmentStart == -1 || closestSegmentEnd == -1) {
            return float4(data.MainColour, 1.0); // Fallback to main colour if no segment is found
        }

        // Get the closest segment's points
        LinePoint a_closest = _LinePointBuffer[closestSegmentStart];
        LinePoint b_closest = _LinePointBuffer[closestSegmentEnd];

        // Calculate the normalized distance (T for length) along the closest segment
        float segmentLength = length(b_closest.Position - a_closest.Position);
        float2 dir = normalize(b_closest.Position - a_closest.Position);
        float tSegment = dot(p - a_closest.Position, dir) / segmentLength;
        tSegment = saturate(tSegment); // Clamp to [0, 1]

        // Total T for the whole line
        float tStart = a_closest.DistanceAlongLine;// / data.TotalLength;
        float tEnd = b_closest.DistanceAlongLine;// / data.TotalLength;
        float T_length = lerp(tStart, tEnd, tSegment);

        // Calculate T for width (perpendicular direction)
        float2 perpDir = float2(-dir.y, dir.x);
        float distToLine = dot(p - a_closest.Position, perpDir);
        float T_width = saturate(0.5 + distToLine / data.Width); // Map to [0, 1]
        float2 uv = float2(T_length, T_width);

        mainCol = SDF_Colour(uv, data.MainColour, data.SecondaryColour, 
                  data.AlternateColourFrequency, data.AlternateColourSpeed, 
                  data.CosTheta, data.SinTheta);
    }

    float innerDist = abs(minDist);
    float outlineFactor = step(innerDist, data.OutlineWidth);
    float4 finalCol = lerp(mainCol, float4(data.OutlineColour, 1.0), outlineFactor);
    return lerp(finalCol, float4(data.OutlineColour, 1.0), step(0.0, minDist));
}

#endif // SDF_DECALS_SDFS_INCLUDED
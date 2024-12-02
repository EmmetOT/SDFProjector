#ifndef SDF_DECALS_STRUCTS_INCLUDED
#define SDF_DECALS_STRUCTS_INCLUDED

struct CircleData {
    float2 Position;
    float Radius;
    float3 MainColour;
    float3 SecondaryColour;
    float AlternateColourFrequency;
    float AlternateColourSpeed;
    float AlternateColourAngle;
};

struct LineData {
    uint PointIndexStart;
    uint PointIndexEnd;
    float Width;
    float TotalLength;
    float3 MainColour;
    float3 SecondaryColour;
    float AlternateColourFrequency;
    float AlternateColourSpeed;
    float AlternateColourAngle;
    int AlignColourWithLine;
    int Loop;
};

struct LinePoint {
    float2 Position;
    float DistanceAlongLine;
};

struct BoxData {
    float2 Position;
    float Rotation;
    float2 Size;
    float4 Roundedness;
    float3 MainColour;
    float3 SecondaryColour;
    float AlternateColourFrequency;
    float AlternateColourSpeed;
    float AlternateColourAngle;
};

#endif // SDF_DECALS_STRUCTS_INCLUDED
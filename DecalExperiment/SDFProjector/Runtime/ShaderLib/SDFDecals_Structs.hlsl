#ifndef SDF_DECALS_STRUCTS_INCLUDED
#define SDF_DECALS_STRUCTS_INCLUDED

// todo: move colour data to a separate struct

struct CircleData {
    float2 Position;
    float Radius;
    float3 MainColour;
    float3 SecondaryColour;
    float3 OutlineColour;
    float OutlineWidth;
    float AlternateColourFrequency;
    float AlternateColourSpeed;
    float CosTheta;
    float SinTheta;
};

struct LineData {
    uint PointIndexStart;
    uint PointIndexEnd;
    float Width;
    float TotalLength;
    float3 MainColour;
    float3 SecondaryColour;
    float3 OutlineColour;
    float OutlineWidth;
    float AlternateColourFrequency;
    float AlternateColourSpeed;
    float CosTheta;
    float SinTheta;
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
    float3 OutlineColour;
    float OutlineWidth;
    float AlternateColourFrequency;
    float AlternateColourSpeed;
    float CosTheta;
    float SinTheta;
};

#endif // SDF_DECALS_STRUCTS_INCLUDED
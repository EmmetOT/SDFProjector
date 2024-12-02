#ifndef SDF_DECALS_GLOBALS_INCLUDED
#define SDF_DECALS_GLOBALS_INCLUDED

#include "SDFDecals_Structs.hlsl"

StructuredBuffer<CircleData> _CircleBuffer;
int _CircleCount;

StructuredBuffer<LineData> _LineBuffer;
int _LineCount;

StructuredBuffer<LinePoint> _LinePointBuffer;
int _LinePointCount;

StructuredBuffer<BoxData> _BoxBuffer;
int _BoxCount;

float2 _BottomLeft;
float2 _TopRight;

float _Smoothing;
float _ColorSmoothing;

float _ElapsedTime;

#endif // SDF_DECALS_GLOBALS_INCLUDED
#ifndef SDF_DECALS_INCLUDED
#define SDF_DECALS_INCLUDED

#include "SDFDecals_SDFs.hlsl"
#include "SDFDecals_Structs.hlsl"
#include "SDFDecals_Globals.hlsl"

void SDFDecals_float(float2 uv, out float4 outputColor) {
    outputColor = _BackgroundColour;
    float minDist = MAX_DIST;

    float2 worldSpacePos = uv * (_TopRight - _BottomLeft) + _BottomLeft;

    if (_CircleCount == 0 && _LineCount == 0 && _BoxCount == 0) {
        outputColor = _BackgroundColour;
        return;
    }
    
    if (_CircleCount > 0) {
        CircleData circleData = _CircleBuffer[0];
        minDist = SDF_Circle(worldSpacePos, circleData);
        outputColor = SDF_Colour_Circle(worldSpacePos, circleData);
    
        for (int i = 1; i < _CircleCount; i++) {
            circleData = _CircleBuffer[i];
            float dist = SDF_Circle(worldSpacePos, circleData);
            minDist = SDF_SmoothMin(minDist, dist, outputColor, SDF_Colour_Circle(worldSpacePos, circleData), outputColor);
        }
    }
    
    if (_LineCount > 0) {
        LineData lineData = _LineBuffer[0];
        float dist = SDF_Line(worldSpacePos, lineData);
    
        if (_CircleCount == 0) {
            minDist = dist;
            outputColor = SDF_Colour_Line(worldSpacePos, lineData);
        } else {
            minDist = SDF_SmoothMin(minDist, dist, outputColor, SDF_Colour_Line(worldSpacePos, lineData), outputColor);
        }
    
        for (int i = 1; i < _LineCount; i++) {
            lineData = _LineBuffer[i];
            dist = SDF_Line(worldSpacePos, lineData);
            minDist = SDF_SmoothMin(minDist, dist, outputColor, SDF_Colour_Line(worldSpacePos, lineData), outputColor);
        }
    }
    
    if (_BoxCount > 0) {
        BoxData boxData = _BoxBuffer[0];
        float dist = SDF_Box(worldSpacePos, boxData);
    
        if (_CircleCount == 0 && _LineCount == 0) {
            minDist = dist;
            outputColor = SDF_Colour_Box(worldSpacePos, boxData);
        } else {
            minDist = SDF_SmoothMin(minDist, dist, outputColor, SDF_Colour_Box(worldSpacePos, boxData), outputColor);
        }
        
        for (int i = 1; i < _BoxCount; i++) {
            boxData = _BoxBuffer[i];
            dist = SDF_Box(worldSpacePos, boxData);
            minDist = SDF_SmoothMin(minDist, dist, outputColor, SDF_Colour_Box(worldSpacePos, boxData), outputColor);
        }
    }

    outputColor = lerp(outputColor, _BackgroundColour, step(0.0, minDist));
}

#endif // SDF_DECALS_INCLUDED
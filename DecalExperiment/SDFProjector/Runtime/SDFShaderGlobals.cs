using UnityEngine;

public static class SDFShaderGlobals {
    public static readonly int CircleBuffer = Shader.PropertyToID("_CircleBuffer");
    public static readonly int CircleCount = Shader.PropertyToID("_CircleCount");

    public static readonly int LineBuffer = Shader.PropertyToID("_LineBuffer");
    public static readonly int LineCount = Shader.PropertyToID("_LineCount");

    public static readonly int LinePointBuffer = Shader.PropertyToID("_LinePointBuffer");
    public static readonly int LinePointCount = Shader.PropertyToID("_LinePointCount");

    public static readonly int BoxBuffer = Shader.PropertyToID("_BoxBuffer");
    public static readonly int BoxCount = Shader.PropertyToID("_BoxCount");

    public static readonly int Smoothing = Shader.PropertyToID("_Smoothing");
    public static readonly int ColorSmoothing = Shader.PropertyToID("_ColorSmoothing");
    public static readonly int BottomLeft = Shader.PropertyToID("_BottomLeft");
    public static readonly int TopRight = Shader.PropertyToID("_TopRight");
    public static readonly int ElapsedTime = Shader.PropertyToID("_ElapsedTime");
    
    public static readonly int BackgroundColour = Shader.PropertyToID("_BackgroundColour");
}
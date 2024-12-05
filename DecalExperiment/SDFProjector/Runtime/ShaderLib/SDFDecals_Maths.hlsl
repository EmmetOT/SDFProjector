#ifndef SDF_DECALS_MATHS_INCLUDED
#define SDF_DECALS_MATHS_INCLUDED

#define MAX_DIST 1000000.
#define EPSILON 0.00001
#define DEG_TO_RAD 0.017453292519943295

int when_eq(float x, float y) {
    return 1 - abs(sign(x - y));
}

int when_neq(float x, float y) {
    return abs(sign(x - y));
}

int when_gt(float x, float y) {
    return max(sign(x - y), 0);
}

int when_lt(float x, float y) {
    return max(sign(y - x), 0);
}

int when_ge(float x, float y) {
    return 1 - when_lt(x, y);
}

int when_le(float x, float y) {
    return 1 - when_gt(x, y);
}

int and(int a, int b) {
    return a * b;
}

int or(int a, int b) {
    return min(a + b, 1);
}

float2 rotate(float2 p, float a) {
    float s = sin(a);
    float c = cos(a);
    return float2(p.x * c - p.y * s, p.x * s + p.y * c);
}

float SideOfLine(float2 p, float2 a, float2 b) {
    float2 ba = b - a;
    float2 pa = p - a;

    // Compute the cross product of the vectors
    float cross = ba.x * pa.y - ba.y * pa.x;

    // Return the result:
    // > 0 : point is on the "left"
    // < 0 : point is on the "right"
    // = 0 : point is on the line
    return cross;
}

#endif // SDF_DECALS_MATHS_INCLUDED
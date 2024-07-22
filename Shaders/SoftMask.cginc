#ifndef UI_SOFT_MASK_INCLUDED
#define UI_SOFT_MASK_INCLUDED
uniform sampler2D _SoftMaskTex;
uniform half4 _SoftMaskColor;
uniform float _AlphaClipThreshold;
uniform half4 _SoftMaskOutsideColor;
uniform int _SoftMaskableEnable;
uniform int _SoftMaskableStereo;
uniform float4x4 _GameVP;
uniform float4x4 _GameTVP;
uniform float4x4 _GameVP_2;
uniform float4x4 _GameTVP_2;

half Approximately(float4x4 a, float4x4 b)
{
    float4x4 d = abs(a - b);
    return step(
        max(d._m00,max(d._m01,max(d._m02,max(d._m03,
        max(d._m10,max(d._m11,max(d._m12,max(d._m13,
        max(d._m20,max(d._m21,max(d._m22,max(d._m23,
        max(d._m30,max(d._m31,max(d._m32,d._m33))))))))))))))),
        0.1);
}

float2 WorldToUv(float4 worldPos)
{
    worldPos = mul(UNITY_MATRIX_M, worldPos);
    float4x4 gameVp = lerp(_GameVP, _GameVP_2, unity_StereoEyeIndex);
    float4x4 gameTvp = lerp(_GameTVP, _GameTVP_2, unity_StereoEyeIndex);

    fixed isSceneView = 1 - Approximately(UNITY_MATRIX_VP, gameVp);

    float4 clipPos = mul(UNITY_MATRIX_VP, worldPos);
    float4 clipPosG = mul(gameTvp, worldPos);
    return lerp(
        clipPos.xy / clipPos.w * 0.5 + 0.5,
        clipPosG.xy / clipPosG.w * 0.5 + 0.5,
        isSceneView);
}

float2 ClipToUv(float4 clipPos)
{
    half2 uv = clipPos.xy / _ScreenParams.xy;
    #if UNITY_UV_STARTS_AT_TOP
    uv.y = lerp(uv.y, 1 - uv.y, step(0, _ProjectionParams.x));
    #endif

    return uv;
}

float SoftMaskSample(float2 uv, float a)
{
    if (_SoftMaskableEnable == 0)
    {
        return 1;
    }

    if (_SoftMaskableStereo)
    {
        uv = lerp(half2(uv.x / 2, uv.y), half2(uv.x / 2 + 0.5, uv.y), unity_StereoEyeIndex);
    }

    half4 mask = tex2D(_SoftMaskTex, uv);
    half4 alpha = saturate(lerp(half4(1, 1, 1, 1),
                                lerp(mask, half4(1, 1, 1, 1) - mask, _SoftMaskColor - half4(1, 1, 1, 1)),
                                _SoftMaskColor));
    #if SOFTMASK_EDITOR
        int inScreen = step(0, uv.x) * step(uv.x, 1) * step(0, uv.y) * step(uv.y, 1);
        alpha = lerp(_SoftMaskOutsideColor, alpha, inScreen);
        clip (a * alpha.x * alpha.y * alpha.z * alpha.w - _AlphaClipThreshold * (1 - inScreen) - 0.001);
    #endif

    return alpha.x * alpha.y * alpha.z * alpha.w;
}

#if SOFTMASK_EDITOR
#define EDITOR_ONLY(x) x
#define SoftMask(_, worldPos, alpha) SoftMaskSample(WorldToUv(worldPos), alpha)
#else
#define EDITOR_ONLY(_)
#define SoftMask(clipPos, _, __) SoftMaskSample(ClipToUv(clipPos), 1)
#endif
#endif // UI_SOFT_MASK_INCLUDED

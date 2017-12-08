Shader "Procedural Blob"
{
    Properties
    {
        _Exponent("Exponent", Range(0.01, 10)) = 1
    }

    CGINCLUDE

    #include "UnityCG.cginc"

    half4 _Color;

    half _Exponent;

    void Vertex(
        float4 position : POSITION,
        float2 texcoord : TEXCOORD,
        half4 color : COLOR,
        out float4 out_position : SV_Position,
        out float2 out_texcoord : TEXCOORD,
        out half4 out_color : COLOR
    ) 
    {
        out_position = UnityObjectToClipPos(position);
        out_texcoord = texcoord;
        out_color = color;
    }

    half4 Fragment(
        float4 sv_position : SV_Position,
        float2 texcoord : TEXCOORD,
        half4 color : COLOR
    ) : SV_Target
    {
        half l = length(texcoord.xyxy - 0.5) * 2;
        return pow(saturate(1 - l), _Exponent) * color;
    }

    ENDCG

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
        Blend One One
        Cull Off Lighting Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            ENDCG
        }
    }
}

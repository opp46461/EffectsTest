Shader "Custom/Dissolve_1"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _DissolveTex("Dissolve Texture", 2D) = "white" {}
        _DissolveThreshold("Dissolve Threshold", Range(0, 1)) = 0
    }
        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct a2v
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                sampler2D _MainTex;
                sampler2D _DissolveTex;
                float _DissolveThreshold;

                v2f vert(a2v v)
                {
                    v2f o;
                    o.position = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_TARGET
                {
                    float4 texColor = tex2D(_MainTex, i.uv);
                    float4 dissolveColor = tex2D(_DissolveTex, i.uv);
                    clip(dissolveColor.rgb - _DissolveThreshold);
                    return texColor;
                }
                ENDCG
            }
        }
}
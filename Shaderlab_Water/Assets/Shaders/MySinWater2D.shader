// 我的正弦波模拟2D水
Shader "Unlit/MySinWater2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color ("叠加色", Color) = (1,1,1,1)
        _WaveSmoothing ("平滑度", Range(0, 1)) = 0.01
        _Speed ("速度", Float) = 1.0
        _Amplitude ("振幅", Float) = 1.0
        _Wavelength ("波长", Float) = 1.0
        _WaterDirector ("水移动方向", Range(-1, 1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct appdata members y)
// #pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Amplitude;
            float _WaterDirector;
            float _Speed;
            float _Wavelength;
            float _WaveSmoothing;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                UNITY_APPLY_FOG(i.fogCoord, col);
                // return _Color * clamp(smoothstep(0 , _WaveSmoothing, ((1 - i.uv.y) - _Amplitude * (sin(_WaterDirector * i.uv.x * _Wavelength + _Time.x * _Speed) + 1))), 0, 1);
                return _Color * clamp(smoothstep(0 , _WaveSmoothing, ((1 - i.uv.y) - _Amplitude * (sin((_WaterDirector * i.uv.x + _Time.x * _Speed) * radians(360) / _Wavelength) + 1))), 0, 1);
                // return _Color * smoothstep(0 , _WaveSmoothing, ((1 - i.uv.y) - _Amplitude * sin((_WaterDirector * i.uv.x + _Time.x * _Speed) * radians(360) / _Wavelength)));
                // return _Color * ((1 - i.uv.y) - _Amplitude * sin((_WaterDirector * i.uv.x + _Time.x * _Speed) * radians(360) / _Wavelength));
                // return _Color * _Amplitude * sin((_WaterDirector * i.uv.x + _Time.x * _Speed) * radians(360) / _Wavelength);
            }
            ENDCG
        }
    }
}

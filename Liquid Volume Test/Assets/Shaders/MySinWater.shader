Shader "Unlit/MySinWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR]_Color ("叠加色", Color) = (1,1,1,1)
        _Amplitude ("振幅", Float) = 1.0
        _WaterDirector ("水移动方向", Range(-1, 1)) = 1.0
        _Speed ("速度", Float) = 1.0
        _Wavelength ("波长", Float) = 1.0
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
                float3 uv : TEXCOORD0;
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

            v2f vert (appdata v)
            {
                v2f o;
                // float phase = _Time.y;
                float phase = _Time.x * _Speed * radians(360) / _Wavelength;
                // y=sin(fmod(v.uv.x,1.0f)*2.0f*3.1415926f+phase);
                // y = sin(fmod(v.uv.x, 1.0) * radians(360) + phase);
                // v.vertex.xy = v.vertex.xy + _Amplitude * sin((v.vertex.xy + float2(unity_DeltaTime.x, unity_DeltaTime.x) * _Speed) * 2 * degrees(1) / _Wavelength);
                // v.vertex.xz = v.vertex.xz + _Amplitude * sin((v.vertex.xz + float2(unity_DeltaTime.x, unity_DeltaTime.x) * _Speed) * 2 * degrees(1) / _Wavelength);
                // v.vertex.xy = v.vertex.xy + _Amplitude * sin((v.vertex.xy + float2(_SinTime.z, _SinTime.z) * _Speed) * 2 * degrees(1) / _Wavelength);
                o.uv.z = _Amplitude * sin((_WaterDirector * v.uv.x + _Time.x * _Speed) * radians(360) / _Wavelength);// 顶点坐标的Y轴方向根据UV的U轴发生变化
                v.vertex.y = o.uv.z;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //float2 hehe = sin((i.vertex.xy + float2(unity_DeltaTime.x, unity_DeltaTime.x) * _Speed));
                // float2 hehe = _Amplitude * sin((i.uv + float2(unity_DeltaTime.x, unity_DeltaTime.x) * _Speed) * 2 * degrees(1) / _Wavelength);
                // float2 hehe = i.vertex.xy + _Amplitude * sin((i.vertex.xy + float2(unity_DeltaTime.x, unity_DeltaTime.x) * _Speed) * 2 * degrees(1) / _Wavelength);
                // float hehe = i.vertex.x + _Amplitude * sin((i.vertex.x + unity_DeltaTime.x * _Speed) * 2 * degrees(1) / _Wavelength);
                
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col * _Color;
                
                //col.a = step(i.vertex.xy, hehe);

                // col.xyz *= i.uv.z;
                // return _Color * i.uv.z;
            }
            ENDCG
        }
    }
}

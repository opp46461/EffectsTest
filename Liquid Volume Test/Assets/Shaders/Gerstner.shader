Shader "My/Gerstner" {
    Properties{
        //�������û���õ�
        _A("A_���,��ˮƽ�浽����ĸ߶�", Vector) = (1, 1, 0, 0)//xy����
        _Q("Q_�������Ʋ��Ķ��ȣ���ֵԽ����Խ��", Vector) = (0.66, 0, 0, 0)//x����
        _S("S_�ٶ�,ÿ���ֲ����ƶ��ľ���", Vector) = (2, 2, 0, 0)//xy����
        _Dx("X_��ֱ�ڲ����ز�ǰ�������ˮƽʸ��X", Vector) = (0.6, 0, 0, 0)//x����
        _Dz("Z_��ֱ�ڲ����ز�ǰ�������ˮƽʸ��Z", Vector) = (0.24, 0, 0, 0)//x����
        _L("L_����,����ռ��в��嵽����֮��ľ���", Vector) = (1, 1, 1, 1)//x���ã�����yzw��������
    }
        SubShader{
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry"}

            Pass {
                Tags { "LightMode" = "ForwardBase" }

                CGPROGRAM

                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"
                #include "Lighting.cginc"
                #include "AutoLight.cginc"


                float4 _A;
                float4 _Q;
                float4 _S;
                float4 _Dx;
                float4 _Dz;
                float4 _L;

                struct a2v {
                    float4 vertex : POSITION;
                    float4 color : TEXCOORD1;
                };

                struct v2f {
                    float4 pos : SV_POSITION;
                    float3 worldPos : TEXCOORD1;
                    float4 color : TEXCOORD2;

                };
                float3 CalculateWavesDisplacement(float3 vert)
                {
                    float PI = 3.141592f;
                    float3 Gpos = float3(0,0,0);
                    float4 w = 2 * PI / _L;
                    float4 psi = _S * 2 * PI / _L;
                    float4 phase = w * _Dx * vert.x + w * _Dz * vert.z + psi * _Time.x;
                    float4 sinp = float4(0,0,0,0), cosp = float4(0,0,0,0);
                    sincos(phase, sinp, cosp);
                    Gpos.x = dot(_Q * _A * _Dx, cosp);
                    Gpos.z = dot(_Q * _A * _Dz, cosp);
                    Gpos.y = dot(_A, sinp);
                    return Gpos;
                }
                v2f vert(a2v v) {
                    v2f o;
                    float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
                    float3 disPos = CalculateWavesDisplacement(worldPos);
                    v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos + disPos, 1));
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    o.color = v.color;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {

                    return i.color;

                }

                ENDCG
            }
    }
}

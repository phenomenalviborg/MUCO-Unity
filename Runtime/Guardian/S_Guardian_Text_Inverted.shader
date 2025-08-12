Shader "MUCO/S_Guardian_Text_Inverted"
{
    Properties
    {
        u_Color("Color", Color) = (0.9, 0.9, 0.9)
        u_Zoom("Zoom", float) = 8.0
        u_Speed("Speed", float) = 2.0
        u_FadeDistance("Fade Distance",float) = 0.75
        _MainTex("Main Texture", 2D) = "white" {}
    }
    SubShader {
        Tags { "Queue" = "Geometry" }
        Cull off
        ZWrite off
        ZTest off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct fragmentInput
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldpos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float u_Zoom;
            float u_Speed;
            float u_FadeDistance;
            half3 u_Color;

            fragmentInput vert(vertexInput v)
            {
                fragmentInput o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float2 movingTiles(float2 _st, float _zoom, float _speed)
            {
                _st *= _zoom;
                float time = _Time.y * _speed;

                if (frac(_st.y * 0.5) > 0.5) {
                    _st.x += frac(time) * 2.0;  
                }
                else {
                    _st.x -= frac(time) * 2.0;
                }
                
                return frac(_st);
            }

            fixed4 frag(fragmentInput i) : SV_Target
            {
                float2 uv = i.uv;
                float2 st = movingTiles(uv, u_Zoom, u_Speed);
                float dist  = distance(_WorldSpaceCameraPos, i.worldpos);
                fixed4 texColor = tex2D(_MainTex, st);
                fixed4 col = fixed4(texColor.rgb, 1.0 - texColor.a);
                col.rgb = lerp(u_Color.rgb, 1.0 - u_Color.rgb, texColor.a);
                clip(col.a - 0.7);
                col.a *= u_FadeDistance - clamp(dist, 0.0f, u_FadeDistance);
                return col;
            }
            ENDCG
        }
    }
}
Shader "Interactible/SimpleLitColor"
{
    Properties
    {
        _Color("color", Color) = (.25, .5, .5, 1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                half3 worldNormal : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            float4 _Color;

            v2f vert(float4 vertex : POSITION, float3 normal : NORMAL)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                o.worldNormal = UnityObjectToWorldNormal(normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 sun_dir = normalize(float3(.2, -1, .5));
                float3 sun = dot(-i.worldNormal, sun_dir);
                sun = clamp(sun, 0, 1);
                sun = lerp(0.5, 1, sun);

                float3 color = _Color * sun;

                return float4(color, 1);
            }
            ENDCG
        }
    }
}
Shader "MUCO/AlwaysOnTopUnlitColor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        // Make the shader render on top of everything
        Tags { "Queue"="Overlay" "RenderType"="Opaque" }

        Pass
        {
            // Disable lighting
            Lighting Off
            // Ensure no culling, meaning both sides of the geometry will be rendered
            Cull Off
            // Set ZTest to Always so it is always rendered
            ZTest Always
            // ZWrite Off because we don't want this writing to the depth buffer
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
Shader "Astro/Circle_test"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Smoothness ("Edge Smoothness", Range(0.0, 0.5)) = 0.02
        _CenterColor ("Center Color", Color) = (0,0,0,0)
        _Thickness ("Ring Thickness", Range(0.0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _Smoothness)
                UNITY_DEFINE_INSTANCED_PROP(float4, _CenterColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _Thickness)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                o.pos = UnityObjectToClipPos(v.vertex);
                
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                float4 color = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
                float smoothness = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Smoothness);
                float4 centerColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _CenterColor);
                float thickness = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Thickness);

                float2 offset = i.uv - 0.5;
                float dist = length(offset); 

                // Ô²»·ÖÐÐÄ°ë¾¶
                float radius = 0.5 - thickness * 0.5;

                float sdf = abs(dist - radius);

                float halfWidth = thickness * 0.5;

                float t = saturate(dist * 2.0);
                float4 finalColor = lerp(centerColor, color, t);

                float alpha = 1.0 - smoothstep(halfWidth - smoothness, halfWidth, sdf);

                if (alpha <= 0.0) 
                    discard;

                return half4(finalColor.rgb, finalColor.a * alpha);
            }
            ENDCG
        }
    }
}
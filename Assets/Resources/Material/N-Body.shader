Shader "DSP/ProceduralStar"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        // 软边缘参数：0.0是硬边，0.5是完全模糊
        _Smoothness ("Edge Smoothness", Range(0.0, 0.5)) = 0.02
    }
    SubShader
    {
        // 设为 Transparent 队列，因为我们需要把正方形的四个角透明掉
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        
        // 开启混合模式 (Alpha Blending)
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off // 关闭深度写入，防止半透明遮挡问题
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0; // 获取 UV 坐标
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
                UNITY_DEFINE_INSTANCED_PROP(float, _Smoothness)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)

            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                
                // 传递 UV
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 color = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _BaseColor);
                float smoothness = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial, _Smoothness);

                // --- 核心数学画圆逻辑 ---
                // UV 默认是 0~1，中心点是 (0.5, 0.5)
                // 算出当前像素距离中心的距离
                float2 offset = input.uv - 0.5;
                float dist = length(offset); // 距离圆心的长度

                // 完美的圆形半径是 0.5
                // 使用 smoothstep 进行边缘抗锯齿处理
                // 如果 dist > 0.5，alpha 就会变成 0 (透明)
                float alpha = 1.0 - smoothstep(0.5 - smoothness, 0.5, dist);

                // 如果 alpha 太小，直接丢弃（虽然 Blend 开启了，但 discard 可以省一点点计算）
                if (alpha <= 0.0) discard;

                return half4(color.rgb, color.a * alpha);
            }
            ENDHLSL
        }
    }
}
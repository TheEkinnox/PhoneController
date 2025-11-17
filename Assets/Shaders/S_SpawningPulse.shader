Shader "Custom/S_SpawningPulse"
{
    Properties
    {
        _PulseColor("PulseColor", Color) = (1, 1, 1, 1)
        _PulseSpeed("Pulse Speed", float) = 1
        _RingSize("Ring Size", float) = 1
        _RingBlur("Ring Blur", float) = 1
        _MaxSize("Ring Max Spread", float) = 1
        _PulseStrength("Pulse Strength", float) = 1
        _PulseOrigin("Pulse Origin", Vector) = (0,0,0,0)
        
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            float _PulseSpeed;
            float _PulseStrength;
            float _RingSize;
            float _RingBlur;
            float _MaxSize;
            half4 _PulseColor;
            float3 _PulseOrigin;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 world = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(world);
                OUT.worldPos = world;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                //get the variables
                half4 color = _PulseColor;
                float blur = _RingBlur;

                //distance between world pos and set origin(how far the ring should be)
                float worldDistance = distance(IN.worldPos, _PulseOrigin);
                
                //The radius the circle should have based on how long was spent between start and now time the speed
                //NEED TO ASK WHY TO JONATHAN
                //Time.y = the time spend since beginning x is /20, z *2 and w *3 
                float radius = _Time.y * _PulseSpeed;

                //Restrt the circle after x units
                radius = radius % _MaxSize;
                // calculate the ring radius and ditance of the world
                float distFromRing = abs(worldDistance - radius);
                //Increase the max size of the inner ring
                float inner = _RingSize - distFromRing;
                //clamp ring form 0 to 1
                float ring = saturate(inner/blur);
                //multiply the strench and color of the circle
                color.rgb += ring * _PulseStrength;
                return color;
            }
            ENDHLSL
        }
    }
}

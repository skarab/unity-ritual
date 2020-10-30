Shader "Demo/CompositingPass"
{
    Properties
    {
        [NoScaleOffset] _ParticlesDepth("ParticlesDepth", 2D) = "white" {}
        [NoScaleOffset] _ParticlesNormals("ParticlesNormals", 2D) = "white" {}
        _LightDirection("LightDirection", Vector) = (0.0, -1.0, 0.0)
        _LightDirection2("LightDirection2", Vector) = (0.0, -1.0, 0.0)
        [NoScaleOffset] _Cubemap("Cubemap", Cube) = "grey" {}
    }

    HLSLINCLUDE

    #pragma vertex Vert

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"
   
    // The PositionInputs struct allow you to retrieve a lot of useful information for your fullScreenShader:
    // struct PositionInputs
    // {
    //     float3 positionWS;  // World space position (could be camera-relative)
    //     float2 positionNDC; // Normalized screen coordinates within the viewport    : [0, 1) (with the half-pixel offset)
    //     uint2  positionSS;  // Screen space pixel coordinates                       : [0, NumPixels)
    //     uint2  tileCoord;   // Screen tile coordinates                              : [0, NumTiles)
    //     float  deviceDepth; // Depth from the depth buffer                          : [0, 1] (typically reversed)
    //     float  linearDepth; // View space Z coordinate                              : [Near, Far]
    // };

    // To sample custom buffers, you have access to these functions:
    // But be careful, on most platforms you can't sample to the bound color buffer. It means that you
    // can't use the SampleCustomColor when the pass color buffer is set to custom (and same for camera the buffer).
    // float4 SampleCustomColor(float2 uv);
    // float4 LoadCustomColor(uint2 pixelCoords);
    // float LoadCustomDepth(uint2 pixelCoords);
    // float SampleCustomDepth(float2 uv);

    // There are also a lot of utility function you can use inside Common.hlsl and Color.hlsl,
    // you can check them out in the source code of the core SRP package.

    sampler2D _ParticlesDepth;
    sampler2D _ParticlesNormals;
    float4 _LightDirection;
    float4 _LightDirection2;
    samplerCUBE _Cubemap;

    float FullScreenPassDepth(Varyings varyings) : SV_Depth
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
       
        return tex2D(_ParticlesDepth, varyings.positionCS.xy * _ScreenSize.zw).x;
    }

    float4 FullScreenPassDiffuse(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
        float depth = LoadCameraDepth(varyings.positionCS.xy);
        float4 color = float4(0.0, 0.0, 0.0, 0.0);

        float particles_depth = tex2D(_ParticlesDepth, varyings.positionCS.xy * _ScreenSize.zw).x;
        if (particles_depth<depth)
            discard;
                
        float3 L = -_LightDirection.xyz;
        float3 normal = normalize(tex2D(_ParticlesNormals, varyings.positionCS.xy * _ScreenSize.zw).xyz);
        float light = saturate(dot(normal, L));

        float3 diffuse = float3(1.0, 0.3, 0.0) * pow(light * 2.0, 4.0); //float3(0.0, 0.5, 1.0)

        PositionInputs posInput = GetPositionInput(varyings.positionCS.xy, _ScreenSize.zw, particles_depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
        float3 viewDirection = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
        float3 R = reflect(-_LightDirection2.xyz, normal);
        /*float3 specular = float3(1.0, 0.5, 0.5) * pow(saturate(dot(R, viewDirection)), 4.0)*50.0;

        float3 env = texCUBE(_Cubemap, reflect(viewDirection, normal)).xyz;

        float4 o = float4(env*0.8 + diffuse + specular, 1.0);
        
        return float4(o.x*0.1, o.y*0.01, o.z*0.01, 1.0);*/
        /*
        float3 specular = float3(1.0, 1.0, 1.0) * pow(saturate(dot(R, viewDirection)), 4.0) * 50.0;

        float3 env = texCUBE(_Cubemap, reflect(viewDirection, normal)).xyz;

        float4 o = float4(env * 0.5 + diffuse*4.0 + specular * 2.0, 1.0);
        float s = 0.2;
        return float4(o.z * 1.2*s+ o.x * 0.02 * s, o.y * 0.08*s, o.x * 0.02 * s, 1.0);*/

        float3 specular = float3(1.0, 1.0, 1.0) * pow(saturate(dot(R, viewDirection)), 4.0) * 50.0;

        float3 env = texCUBE(_Cubemap, reflect(viewDirection, normal)).xyz;

        float4 o = float4((env * 0.8 + diffuse + specular * 4.0)*0.5, 1.0);

        return float4(o.x, o.y, o.z, 1.0);
    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "Depth"

            ZWrite On
            ZTest Less
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPassDepth
            ENDHLSL
        }
       
        Pass
        {
            Name "Diffuse"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPassDiffuse
            ENDHLSL
        }
    }
    Fallback Off
}

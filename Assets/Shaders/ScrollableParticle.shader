﻿// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Scrollable Unlit"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        _ScrollSpeed("Scroll Speed", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _DistortionStrength("Strength", Float) = 1.0
        _DistortionBlend("Blend", Range(0.0, 1.0)) = 0.5

        _SoftParticlesNearFadeDistance("Soft Particles Near Fade", Float) = 0.0
        _SoftParticlesFarFadeDistance("Soft Particles Far Fade", Float) = 1.0
        _CameraNearFadeDistance("Camera Near Fade", Float) = 1.0
        _CameraFarFadeDistance("Camera Far Fade", Float) = 2.0

            // Hidden properties
            [HideInInspector] _Mode("__mode", Float) = 0.0
            [HideInInspector] _ColorMode("__colormode", Float) = 0.0
            [HideInInspector] _FlipbookMode("__flipbookmode", Float) = 0.0
            [HideInInspector] _LightingEnabled("__lightingenabled", Float) = 0.0
            [HideInInspector] _DistortionEnabled("__distortionenabled", Float) = 0.0
            [HideInInspector] _EmissionEnabled("__emissionenabled", Float) = 0.0
            [HideInInspector] _BlendOp("__blendop", Float) = 0.0
            [HideInInspector] _SrcBlend("__src", Float) = 1.0
            [HideInInspector] _DstBlend("__dst", Float) = 0.0
            [HideInInspector] _ZWrite("__zw", Float) = 1.0
            [HideInInspector] _Cull("__cull", Float) = 2.0
            [HideInInspector] _SoftParticlesEnabled("__softparticlesenabled", Float) = 0.0
            [HideInInspector] _CameraFadingEnabled("__camerafadingenabled", Float) = 0.0
            [HideInInspector] _SoftParticleFadeParams("__softparticlefadeparams", Vector) = (0,0,0,0)
            [HideInInspector] _CameraFadeParams("__camerafadeparams", Vector) = (0,0,0,0)
            [HideInInspector] _ColorAddSubDiff("__coloraddsubdiff", Vector) = (0,0,0,0)
            [HideInInspector] _DistortionStrengthScaled("__distortionstrengthscaled", Float) = 0.0
    }

        Category
        {
            SubShader
            {
                Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "PreviewType" = "Plane" "PerformanceChecks" = "False" }

                BlendOp[_BlendOp]
                Blend[_SrcBlend][_DstBlend]
                ZWrite[_ZWrite]
                Cull[_Cull]
                ColorMask RGB

                GrabPass
                {
                    Tags { "LightMode" = "Always" }
                    "_GrabTexture"
                }

                Pass
                {
                    Name "ShadowCaster"
                    Tags { "LightMode" = "ShadowCaster" }

                    BlendOp Add
                    Blend One Zero
                    ZWrite On
                    Cull Off

                    CGPROGRAM
                    #pragma target 2.5

                    #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
                    #pragma shader_feature_local _ _COLOROVERLAY_ON _COLORCOLOR_ON _COLORADDSUBDIFF_ON
                    #pragma shader_feature_local _REQUIRE_UV2
                    #pragma multi_compile_shadowcaster
                    #pragma multi_compile_instancing
                    #pragma instancing_options procedural:vertInstancingSetup

                    #pragma vertex vertParticleShadowCaster
                    #pragma fragment fragParticleShadowCaster

                    #include "UnityStandardParticleShadow.cginc"
                    ENDCG
                }

                Pass
                {
                    Name "SceneSelectionPass"
                    Tags { "LightMode" = "SceneSelectionPass" }

                    BlendOp Add
                    Blend One Zero
                    ZWrite On
                    Cull Off

                    CGPROGRAM
                    #pragma target 2.5

                    #pragma shader_feature_local _ALPHATEST_ON
                    #pragma shader_feature_local _REQUIRE_UV2
                    #pragma multi_compile_instancing
                    #pragma instancing_options procedural:vertInstancingSetup

                    #pragma vertex vertEditorPass
                    #pragma fragment fragSceneHighlightPass

                    #include "UnityStandardParticleEditor.cginc"
                    ENDCG
                }

                    Pass
                {
                    Name "ScenePickingPass"
                    Tags{ "LightMode" = "Picking" }

                    BlendOp Add
                    Blend One Zero
                    ZWrite On
                    Cull Off

                    CGPROGRAM
                    #pragma target 2.5

                    #pragma shader_feature_local _ALPHATEST_ON
                    #pragma shader_feature_local _REQUIRE_UV2
                    #pragma multi_compile_instancing
                    #pragma instancing_options procedural:vertInstancingSetup

                    #pragma vertex vertEditorPass
                    #pragma fragment fragScenePickingPass

                    #include "UnityStandardParticleEditor.cginc"
                    ENDCG
                }

                    Pass
                {
                    Tags { "LightMode" = "ForwardBase" }

                    CGPROGRAM
                    #pragma multi_compile __ SOFTPARTICLES_ON
                    #pragma multi_compile_fog
                    #pragma target 2.5

                    #pragma shader_feature_local _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
                    #pragma shader_feature_local _ _COLOROVERLAY_ON _COLORCOLOR_ON _COLORADDSUBDIFF_ON
                    #pragma shader_feature_local _NORMALMAP
                    #pragma shader_feature _EMISSION
                    #pragma shader_feature_local _FADING_ON
                    #pragma shader_feature_local _REQUIRE_UV2
                    #pragma shader_feature_local EFFECT_BUMP

                    #pragma vertex vertParticleUnlit
                    #pragma fragment fragParticleUnlitOverride
                    #pragma multi_compile_instancing
                    #pragma instancing_options procedural:vertInstancingSetup

                    #include "UnityStandardParticles.cginc"

                    float _ScrollSpeed;

                    half4 fragParticleUnlitOverride(VertexOutput IN) : SV_Target
                    {
                        IN.texcoord.x += _Time.y * 4.0f;
                        half4 albedo = readTexture(_MainTex, IN);
                        albedo *= _Color;

                        fragColorMode(IN);
                        fragSoftParticles(IN);
                        fragCameraFading(IN);

                        #if defined(_NORMALMAP)
                        float3 normal = normalize(UnpackScaleNormal(readTexture(_BumpMap, IN), _BumpScale));
                        #else
                        float3 normal = float3(0,0,1);
                        #endif

                        #if defined(_EMISSION)
                        half3 emission = readTexture(_EmissionMap, IN).rgb;
                        #else
                        half3 emission = 0;
                        #endif

                        fragDistortion(IN);

                        half4 result = albedo;

                        #if defined(_ALPHAMODULATE_ON)
                        result.rgb = lerp(half3(1.0, 1.0, 1.0), albedo.rgb, albedo.a);
                        #endif

                        result.rgb += emission * _EmissionColor * cameraFade * softParticlesFade;

                        #if !defined(_ALPHABLEND_ON) && !defined(_ALPHAPREMULTIPLY_ON) && !defined(_ALPHAOVERLAY_ON)
                        result.a = 1;
                        #endif

                        #if defined(_ALPHATEST_ON)
                        clip(albedo.a - _Cutoff + 0.0001);
                        #endif

                        UNITY_APPLY_FOG_COLOR(IN.fogCoord, result, fixed4(0,0,0,0));
                        return result;
                    }
                    ENDCG
                }
            }
        }

            Fallback "VertexLit"
            CustomEditor "StandardParticlesShaderGUI"
}
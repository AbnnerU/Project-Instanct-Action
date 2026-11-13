// Made with Amplify Shader Editor v1.9.9.12
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/Lite_SH_Vefects_Unlit_Flipbook_HDRP_New"
{
	Properties
	{
		[Space(13)][Header(Main Texture)][Space(13)] _MainTexture( "Main Texture", 2D ) = "white" {}
		_UVS( "UV S", Vector ) = ( 1, 1, 0, 0 )
		_UVP( "UV P", Vector ) = ( 0, 0, 0, 0 )
		[HDR] _R( "R", Color ) = ( 1, 0.9719134, 0.5896226, 0 )
		[HDR] _G( "G", Color ) = ( 1, 0.7230805, 0.25, 0 )
		[HDR] _B( "B", Color ) = ( 0.5943396, 0.259371, 0.09812209, 0 )
		[HDR] _Outline( "Outline", Color ) = ( 0.2169811, 0.03320287, 0.02354041, 0 )
		[Header(TextureProps)][Space(13)] _Intensity( "Intensity", Range( 0, 5 ) ) = 1
		_ErosionSmoothness( "Erosion Smoothness", Range( 0.1, 15 ) ) = 0.1
		_FlatColor( "Flat Color", Range( 0, 1 ) ) = 0
		_UVDS1( "UV D S", Vector ) = ( 1, 1, 0, 0 )
		[Space(13)][Header(Distortion)][Space(13)] _DistortionTexture( "Distortion Texture", 2D ) = "white" {}
		_UVDP1( "UV D P", Vector ) = ( 0.1, -0.2, 0, 0 )
		_DistortionLerp( "Distortion Lerp", Range( 0, 0.1 ) ) = 0
		[Header(SecondDistortion)][Space(13)] _DistortionSecond( "DistortionSecond", 2D ) = "white" {}
		_SecondDistortionLerp( "SecondDistortionLerp", Range( 0.5, 1 ) ) = 0.5
		_UVDS( "UV D S", Vector ) = ( 1, 1, 0, 0 )
		_UVDP( "UV D P", Vector ) = ( 0.1, -0.2, 0, 0 )
		[Space(13)][Header(AR)][Space(13)] _Cull( "Cull", Float ) = 2
		_Src( "Src", Float ) = 5
		_Dst( "Dst", Float ) = 10
		_ZWrite1( "ZWrite1", Float ) = 0
		_ZTest( "ZTest", Float ) = 2

		[HideInInspector] _RenderQueueType("Render Queue Type", Float) = 5
		[HideInInspector][ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1
		//[HideInInspector] _ShadowMatteFilter("Shadow Matte Filter", Float) = 2.006836
		[HideInInspector] _StencilRef("Stencil Ref", Int) = 0 // StencilUsage.Clear
		[HideInInspector] _StencilWriteMask("Stencil Write Mask", Int) = 3 // StencilUsage.RequiresDeferredLighting | StencilUsage.SubsurfaceScattering
		[HideInInspector] _StencilRefDepth("Stencil Ref Depth", Int) = 0 // Nothing
		[HideInInspector] _StencilWriteMaskDepth("Stencil Write Mask Depth", Int) = 8 // StencilUsage.TraceReflectionRay
		[HideInInspector] _StencilRefMV("Stencil Ref MV", Int) = 32 // StencilUsage.ObjectMotionVector
		[HideInInspector] _StencilWriteMaskMV("Stencil Write Mask MV", Int) = 32 // StencilUsage.ObjectMotionVector
		[HideInInspector] _StencilRefDistortionVec("Stencil Ref Distortion Vec", Int) = 2 // StencilUsage.DistortionVectors
		[HideInInspector] _StencilWriteMaskDistortionVec("Stencil Write Mask Distortion Vec", Int) = 2 // StencilUsage.DistortionVectors
		[HideInInspector] _StencilWriteMaskGBuffer("Stencil Write Mask GBuffer", Int) = 3 // StencilUsage.RequiresDeferredLighting | StencilUsage.SubsurfaceScattering
		[HideInInspector] _StencilRefGBuffer("Stencil Ref GBuffer", Int) = 2 // StencilUsage.RequiresDeferredLighting
		[HideInInspector] _ZTestGBuffer("ZTest GBuffer", Int) = 4
		[HideInInspector][ToggleUI] _RequireSplitLighting("Require Split Lighting", Float) = 0
		[HideInInspector][ToggleUI] _ReceivesSSR("Receives SSR", Float) = 1
		[HideInInspector] _SurfaceType("Surface Type", Float) = 1
		[HideInInspector] _BlendMode("Blend Mode", Float) = 0
		[HideInInspector] _SrcBlend("Src Blend", Float) = 1
		[HideInInspector] _DstBlend("Dst Blend", Float) = 0
		[HideInInspector] _AlphaSrcBlend("Alpha Src Blend", Float) = 1
		[HideInInspector] _AlphaDstBlend("Alpha Dst Blend", Float) = 0
		[HideInInspector][ToggleUI] _ZWrite("ZWrite", Float) = 1
		[HideInInspector][ToggleUI] _TransparentZWrite("Transparent ZWrite", Float) = 0
		[HideInInspector] _CullMode("Cull Mode", Float) = 2
		[HideInInspector] _TransparentSortPriority("Transparent Sort Priority", Float) = 0
		[HideInInspector][ToggleUI] _EnableFogOnTransparent("Enable Fog", Float) = 1
		[HideInInspector] _CullModeForward("Cull Mode Forward", Float) = 2 // This mode is dedicated to Forward to correctly handle backface then front face rendering thin transparent
		[HideInInspector][Enum(Back, 2, Front, 1)] _TransparentCullMode("_TransparentCullMode", Int) = 2// Back culling by default
		[HideInInspector] _ZTestDepthEqualForOpaque("ZTest Depth Equal For Opaque", Int) = 4 // Less equal
		[HideInInspector][Enum(UnityEngine.Rendering.CompareFunction)] _ZTestTransparent("ZTest Transparent", Int) = 4// Less equal
		[HideInInspector][ToggleUI] _TransparentBackfaceEnable("Transparent Backface Enable", Float) = 0
		//[HideInInspector][ToggleUI] _AlphaCutoffEnable("Alpha Cutoff Enable", Float) = 1
		[HideInInspector][ToggleUI] _UseShadowThreshold("Use Shadow Threshold", Float) = 1
		[HideInInspector][ToggleUI] _DoubleSidedEnable("Double Sided Enable", Float) = 0
		[HideInInspector][Enum(Flip, 0, Mirror, 1, None, 2)] _DoubleSidedNormalMode("Double Sided Normal Mode", Float) = 1
		[HideInInspector] _DoubleSidedConstants("DoubleSidedConstants", Vector) = (1,1,-1,0)
		[HideInInspector] _DistortionEnable("_DistortionEnable",Float) = 0
		[HideInInspector] _DistortionOnly("_DistortionOnly",Float) = 0

		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector][ToggleUI] _TransparentWritingMotionVec("Transparent Writing MotionVec", Float) = 0
		[HideInInspector][Enum(Back, 2, Front, 1)] _OpaqueCullMode("_OpaqueCullMode", Int) = 2 // Back culling by default
		[HideInInspector][ToggleUI] _SupportDecals("Support Decals", Float) = 1
		[HideInInspector][ToggleUI] _ReceivesSSRTransparent("Receives SSR Transparent", Float) = 0
		[HideInInspector] _EmissionColor("Color", Color) = (1, 1, 1)
		[HideInInspector] _UnlitColorMap_MipInfo("_UnlitColorMap_MipInfo", Vector) = (0, 0, 0, 0)

		[HideInInspector][Enum(Auto, 0, On, 1, Off, 2)] _DoubleSidedGIMode("Double sided GI mode", Float) = 0
	}

	SubShader
	{
		PackageRequirements
		{
			"com.unity.render-pipelines.high-definition": "[15.0,16.0]"
		}

		

		

		Tags { "RenderPipeline"="HDRenderPipeline" "RenderType"="Opaque" "Queue"="Transparent" "ShaderGraphShader"="true" }

	LOD 0

		HLSLINCLUDE
		#pragma target 4.5
		#pragma exclude_renderers glcore gles gles3 

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#define ASE_ADJUST_CLIP_POSITION( x ) x

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlaneASE (float3 pos, float4 plane)
		{
			return dot (float4(pos,1.0f), plane);
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlaneASE(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlaneASE(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlaneASE(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlaneASE(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward Unlit"
			Tags { "LightMode"="ForwardOnly" }

			Blend [_SrcBlend] [_DstBlend], [_AlphaSrcBlend] [_AlphaDstBlend]

			Cull [_CullModeForward]
			ZTest [_ZTestDepthEqualForOpaque]
			ZWrite [_ZWrite1]

			ColorMask [_ColorMaskTransparentVel] 1

			Stencil
			{
				Ref [_StencilRef]
				WriteMask [_StencilWriteMask]
				Comp Always
				Pass Replace
			}


			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma shader_feature_local_fragment _ENABLE_FOG_ON_TRANSPARENT
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 150006


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT

			#pragma multi_compile _ DEBUG_DISPLAY
			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex Vert
			#pragma fragment Frag

	        #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
	        #define _WRITE_TRANSPARENT_MOTION_VECTOR
	        #endif

			#define SHADERPASS SHADERPASS_FORWARD_UNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

			#if defined(_ENABLE_SHADOW_MATTE) && SHADERPASS == SHADERPASS_FORWARD_UNLIT
				#define LIGHTLOOP_DISABLE_TILE_AND_CLUSTER
				#define HAS_LIGHTLOOP
				#define SHADOW_OPTIMIZE_REGISTER_USAGE 1

				#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Shadow/HDShadowContext.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/HDShadow.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/PunctualLightCommon.hlsl"
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/HDShadowLoop.hlsl"
			#endif

			CBUFFER_START( UnityPerMaterial )
			float4 _Outline;
			float4 _B;
			float4 _R;
			float4 _G;
			float2 _UVDP;
			float2 _UVDS1;
			float2 _UVDP1;
			float2 _UVP;
			float2 _UVS;
			float2 _UVDS;
			float _Intensity;
			float _FlatColor;
			float _Src;
			float _SecondDistortionLerp;
			float _Cull;
			float _ZWrite1;
			float _ZTest;
			float _Dst;
			float _DistortionLerp;
			float _ErosionSmoothness;
			float4 _EmissionColor;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			#ifdef _ENABLE_SHADOW_MATTE
			float _ShadowMatteFilter;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _AlphaCutoff;
			float _AlphaCutoffShadow;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			float _EnableBlendModePreserveSpecularLighting;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTexture;
			sampler2D _DistortionTexture;
			sampler2D _DistortionSecond;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariablesFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_COLOR


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			struct SurfaceDescription
			{
				float3 Color;
				float3 Emission;
				float4 ShadowTint;
				float Alpha;
				float AlphaClipThreshold;
				float AlphaClipThresholdShadow;
				float4 VTPackedFeedback;
			};

			void BuildSurfaceData(FragInputs fragInputs, SurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				surfaceData.color = surfaceDescription.Color;
			}

			void GetSurfaceAndBuiltinData(SurfaceDescription surfaceDescription , FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif

				#ifdef _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif

				BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData);

				#ifdef WRITE_NORMAL_BUFFER
				surfaceData.normalWS = fragInputs.tangentToWorld[2];
				#endif

				#if defined(_ENABLE_SHADOW_MATTE) && SHADERPASS == SHADERPASS_FORWARD_UNLIT
					HDShadowContext shadowContext = InitShadowContext();
					float shadow;
					float3 shadow3;
					posInput = GetPositionInput(fragInputs.positionSS.xy, _ScreenSize.zw, fragInputs.positionSS.z, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
					float3 normalWS = normalize(fragInputs.tangentToWorld[1]);
					uint renderingLayers = GetMeshRenderingLayerMask();
					ShadowLoopMin(shadowContext, posInput, normalWS, asuint(_ShadowMatteFilter), renderingLayers, shadow3);
					shadow = dot(shadow3, float3(1.0f/3.0f, 1.0f/3.0f, 1.0f/3.0f));

					float4 shadowColor = (1 - shadow)*surfaceDescription.ShadowTint.rgba;
					float  localAlpha  = saturate(shadowColor.a + surfaceDescription.Alpha);

					#ifdef _SURFACE_TYPE_TRANSPARENT
						surfaceData.color = lerp(shadowColor.rgb*surfaceData.color, lerp(lerp(shadowColor.rgb, surfaceData.color, 1 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow), surfaceDescription.Alpha);
					#else
						surfaceData.color = lerp(lerp(shadowColor.rgb, surfaceData.color, 1 - surfaceDescription.ShadowTint.a), surfaceData.color, shadow);
					#endif
					localAlpha = ApplyBlendMode(surfaceData.color, localAlpha).a;
					surfaceDescription.Alpha = localAlpha;
				#endif

				ZERO_INITIALIZE(BuiltinData, builtinData);
				builtinData.opacity = surfaceDescription.Alpha;

				#if defined(DEBUG_DISPLAY)
					builtinData.renderingLayers = GetMeshRenderingLayerMask();
				#endif

                #ifdef _ALPHATEST_ON
                    builtinData.alphaClipTreshold = surfaceDescription.AlphaClipThreshold;
                #endif

				builtinData.emissiveColor = surfaceDescription.Emission;

				#ifdef UNITY_VIRTUAL_TEXTURING
                builtinData.vtPackedFeedback = surfaceDescription.VTPackedFeedback;
                #endif

				#ifdef _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif

                ApplyDebugToBuiltinData(builtinData);
			}

			float GetDeExposureMultiplier()
			{
			#if defined(DISABLE_UNLIT_DEEXPOSURE)
				return 1.0;
			#else
				return _DeExposureMultiplier;
			#endif
			}

			PackedVaryingsMeshToPS VertexFunction( AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS o;
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_color = inputMesh.ase_color;
				o.ase_texcoord1 = inputMesh.ase_texcoord;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;

				#ifdef ASE_CUSTOM_MOTION_VECTOR
				// Declared so the Velocity output port surfaces on the master node; only consumed by the motion vector pass.
				float3 aseCustomVelocity = float3( 0, 0, 0 );
				#endif

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				o.positionCS = ASE_ADJUST_CLIP_POSITION( TransformWorldToHClip(positionRWS) );
				o.positionRWS = positionRWS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			#ifdef UNITY_VIRTUAL_TEXTURING
			#define VT_BUFFER_TARGET SV_Target1
			#define EXTRA_BUFFER_TARGET SV_Target2
			#else
			#define EXTRA_BUFFER_TARGET SV_Target1
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput,
						out float4 outColor : SV_Target0
						#ifdef UNITY_VIRTUAL_TEXTURING
						,out float4 outVTFeedback : VT_BUFFER_TARGET
						#endif
						#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
						, out float outputDepth : DEPTH_OFFSET_SEMANTIC
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;
				input.positionRWS = packedInput.positionRWS;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 PositionRWS = packedInput.positionRWS;
				float3 V = GetWorldSpaceNormalizeViewDir( packedInput.positionRWS );
				float4 ScreenPosNorm = float4( posInput.positionNDC, packedInput.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, packedInput.positionCS.z ) * packedInput.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos, _ProjectionParams.x );

				float2 texCoord19 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner27 = ( 1.0 * _Time.y * _UVP + ( texCoord19 * _UVS ));
				float2 texCoord11 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner15 = ( 1.0 * _Time.y * _UVDP + ( texCoord11 * _UVDS ));
				float2 lerpResult26 = lerp( float2( 0,0 ) , ( ( (tex2D( _DistortionTexture, panner15 )).rg + -0.5 ) * 2.0 ) , _DistortionLerp);
				float2 DistortionRegister34 = ( panner27 + lerpResult26 );
				float4 tex2DNode45 = tex2D( _MainTexture, DistortionRegister34 );
				float4 lerpResult97 = lerp( _Outline , _B , tex2DNode45.b);
				float4 lerpResult112 = lerp( lerpResult97 , _G , tex2DNode45.g);
				float4 lerpResult111 = lerp( lerpResult112 , _R , tex2DNode45.r);
				float4 lerpResult88 = lerp( ( packedInput.ase_color * lerpResult111 ) , packedInput.ase_color , _FlatColor);
				float2 texCoord95 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner71 = ( 1.0 * _Time.y * _UVDP1 + ( texCoord95 * _UVDS1 ));
				float4 SecondDistortion108 = ( tex2D( _DistortionSecond, panner71 ) + _SecondDistortionLerp );
				float4 temp_output_90_0 = ( ( lerpResult88 * _Intensity ) * SecondDistortion108 );
				
				float mainTex_alpha48 = tex2DNode45.a;
				float smoothstepResult55 = smoothstep( packedInput.ase_texcoord1.z , ( packedInput.ase_texcoord1.z + _ErosionSmoothness ) , mainTex_alpha48);
				float mainTex_VC_alha52 = packedInput.ase_color.a;
				float OpacityRegister61 = ( smoothstepResult55 * mainTex_VC_alha52 );
				

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				surfaceDescription.Color = temp_output_90_0.rgb;
				surfaceDescription.Emission = temp_output_90_0.rgb;
				surfaceDescription.Alpha = OpacityRegister61;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _ALPHATEST_SHADOW_ON
				surfaceDescription.AlphaClipThresholdShadow = _AlphaCutoffShadow;
				surfaceDescription.AlphaClipThresholdShadow = _UseShadowThreshold ? surfaceDescription.AlphaClipThresholdShadow : surfaceDescription.AlphaClipThreshold;
				#endif

				surfaceDescription.ShadowTint = float4( 0, 0, 0, 1 );
				float2 Distortion = float2( 0, 0 );
				float DistortionBlur = 0;

				#if defined( ASE_WRITE_DEPTH )
				posInput.deviceDepth = posInput.deviceDepth;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				surfaceDescription.VTPackedFeedback = float4(1.0f,1.0f,1.0f,1.0f);
				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				BSDFData bsdfData = ConvertSurfaceDataToBSDFData( input.positionSS.xy, surfaceData );

				#if defined(_ENABLE_SHADOW_MATTE)
				bsdfData.color *= GetScreenSpaceAmbientOcclusion(input.positionSS.xy);
				#endif


			#ifdef DEBUG_DISPLAY
				if (_DebugLightingMode >= DEBUGLIGHTINGMODE_DIFFUSE_LIGHTING && _DebugLightingMode <= DEBUGLIGHTINGMODE_EMISSIVE_LIGHTING)
				{
					if (_DebugLightingMode != DEBUGLIGHTINGMODE_EMISSIVE_LIGHTING)
					{
						builtinData.emissiveColor = 0.0;
					}
					else
					{
						bsdfData.color = 0.0;
					}
				}
			#endif

				float4 outResult = ApplyBlendMode(bsdfData.color * GetDeExposureMultiplier() + builtinData.emissiveColor * GetCurrentExposureMultiplier(), builtinData.opacity);
				outResult = EvaluateAtmosphericScattering(posInput, V, outResult);

				#ifdef DEBUG_DISPLAY
					int bufferSize = int(_DebugViewMaterialArray[0].x);
					for (int index = 1; index <= bufferSize; index++)
					{
						int indexMaterialProperty = int(_DebugViewMaterialArray[index].x);
						if (indexMaterialProperty != 0)
						{
							float3 result = float3(1.0, 0.0, 1.0);
							bool needLinearToSRGB = false;

							GetPropertiesDataDebug(indexMaterialProperty, result, needLinearToSRGB);
							GetVaryingsDataDebug(indexMaterialProperty, input, result, needLinearToSRGB);
							GetBuiltinDataDebug(indexMaterialProperty, builtinData, posInput, result, needLinearToSRGB);
							GetSurfaceDataDebug(indexMaterialProperty, surfaceData, result, needLinearToSRGB);
							GetBSDFDataDebug(indexMaterialProperty, bsdfData, result, needLinearToSRGB);

							if (!needLinearToSRGB)
								result = SRGBToLinear(max(0, result));

							outResult = float4(result, 1.0);
						}
					}

					if (_DebugFullScreenMode == FULLSCREENDEBUGMODE_TRANSPARENCY_OVERDRAW)
					{
						float4 result = _DebugTransparencyOverdrawWeight * float4(TRANSPARENCY_OVERDRAW_COST, TRANSPARENCY_OVERDRAW_COST, TRANSPARENCY_OVERDRAW_COST, TRANSPARENCY_OVERDRAW_A);
						outResult = result;
					}
				#endif

				outColor = outResult;

				#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
				outputDepth = posInput.deviceDepth;
				#endif

				#ifdef UNITY_VIRTUAL_TEXTURING
				outVTFeedback = builtinData.vtPackedFeedback;
				#endif
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }

			Cull [_CullMode]
			ZWrite On
			ZClip [_ZClip]
			ColorMask 0

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma shader_feature_local_fragment _ENABLE_FOG_ON_TRANSPARENT
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 150006


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT

			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex Vert
			#pragma fragment Frag

			#if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
			#define _WRITE_TRANSPARENT_MOTION_VECTOR
			#endif

			#define SHADERPASS SHADERPASS_SHADOWS
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START( UnityPerMaterial )
			float4 _Outline;
			float4 _B;
			float4 _R;
			float4 _G;
			float2 _UVDP;
			float2 _UVDS1;
			float2 _UVDP1;
			float2 _UVP;
			float2 _UVS;
			float2 _UVDS;
			float _Intensity;
			float _FlatColor;
			float _Src;
			float _SecondDistortionLerp;
			float _Cull;
			float _ZWrite1;
			float _ZTest;
			float _Dst;
			float _DistortionLerp;
			float _ErosionSmoothness;
			float4 _EmissionColor;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			#ifdef _ENABLE_SHADOW_MATTE
			float _ShadowMatteFilter;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _AlphaCutoff;
			float _AlphaCutoffShadow;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			float _EnableBlendModePreserveSpecularLighting;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTexture;
			sampler2D _DistortionTexture;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
				float AlphaClipThresholdShadow;
			};

			void BuildSurfaceData(FragInputs fragInputs, SurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				#ifdef WRITE_NORMAL_BUFFER
				surfaceData.normalWS = fragInputs.tangentToWorld[2];
				#endif
			}

			void GetSurfaceAndBuiltinData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif

				#if defined( _ALPHATEST_SHADOW_ON )
					DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThresholdShadow );
				#elif defined( _ALPHATEST_ON )
					DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif

				BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData);

				ZERO_INITIALIZE (BuiltinData, builtinData);
				builtinData.opacity = surfaceDescription.Alpha;

				#if defined(DEBUG_DISPLAY)
					builtinData.renderingLayers = GetMeshRenderingLayerMask();
				#endif

				#ifdef _ALPHATEST_ON
                    builtinData.alphaClipTreshold = surfaceDescription.AlphaClipThreshold;
                #endif

                #ifdef _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif

                ApplyDebugToBuiltinData(builtinData);
			}

			PackedVaryingsMeshToPS VertexFunction( AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS o;
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord1 = inputMesh.ase_texcoord;
				o.ase_color = inputMesh.ase_color;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;

				#ifdef ASE_CUSTOM_MOTION_VECTOR
				// Declared so the Velocity output port surfaces on the master node; only consumed by the motion vector pass.
				float3 aseCustomVelocity = float3( 0, 0, 0 );
				#endif

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				o.positionCS = ASE_ADJUST_CLIP_POSITION( TransformWorldToHClip(positionRWS) );
				o.positionRWS = positionRWS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput
						#ifdef WRITE_MSAA_DEPTH
						, out float4 depthColor : SV_Target0
							#ifdef WRITE_NORMAL_BUFFER
							, out float4 outNormalBuffer : SV_Target1
							#endif
						#else
							#ifdef WRITE_NORMAL_BUFFER
							, out float4 outNormalBuffer : SV_Target0
							#endif
						#endif
						#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
						, out float outputDepth : DEPTH_OFFSET_SEMANTIC
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;
				input.positionRWS = packedInput.positionRWS;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 PositionRWS = packedInput.positionRWS;
				float3 V = GetWorldSpaceNormalizeViewDir( packedInput.positionRWS );
				float4 ScreenPosNorm = float4( posInput.positionNDC, packedInput.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, packedInput.positionCS.z ) * packedInput.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos, _ProjectionParams.x );

				float2 texCoord19 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner27 = ( 1.0 * _Time.y * _UVP + ( texCoord19 * _UVS ));
				float2 texCoord11 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner15 = ( 1.0 * _Time.y * _UVDP + ( texCoord11 * _UVDS ));
				float2 lerpResult26 = lerp( float2( 0,0 ) , ( ( (tex2D( _DistortionTexture, panner15 )).rg + -0.5 ) * 2.0 ) , _DistortionLerp);
				float2 DistortionRegister34 = ( panner27 + lerpResult26 );
				float4 tex2DNode45 = tex2D( _MainTexture, DistortionRegister34 );
				float mainTex_alpha48 = tex2DNode45.a;
				float smoothstepResult55 = smoothstep( packedInput.ase_texcoord1.z , ( packedInput.ase_texcoord1.z + _ErosionSmoothness ) , mainTex_alpha48);
				float mainTex_VC_alha52 = packedInput.ase_color.a;
				float OpacityRegister61 = ( smoothstepResult55 * mainTex_VC_alha52 );
				

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				surfaceDescription.Alpha = OpacityRegister61;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _ALPHATEST_SHADOW_ON
				surfaceDescription.AlphaClipThresholdShadow = _AlphaCutoffShadow;
				surfaceDescription.AlphaClipThresholdShadow = _UseShadowThreshold ? surfaceDescription.AlphaClipThresholdShadow : surfaceDescription.AlphaClipThreshold;
				#endif

				#if defined( ASE_WRITE_DEPTH )
				posInput.deviceDepth = posInput.deviceDepth;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription,input, V, posInput, surfaceData, builtinData);

				#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
				outputDepth = posInput.deviceDepth;
				float bias = max(abs(ddx(posInput.deviceDepth)), abs(ddy(posInput.deviceDepth))) * _SlopeScaleDepthBias;
				outputDepth += bias;
				#endif

				#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.vmesh.positionCS.z;
					depthColor.a = SharpenAlpha(builtinData.opacity, builtinData.alphaClipTreshold);
				#endif

				#if defined(WRITE_NORMAL_BUFFER)
				EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), outNormalBuffer);
				#endif

				#if (defined(WRITE_DECAL_BUFFER) && !defined(_DISABLE_DECALS)) || defined(WRITE_RENDERING_LAYER)
					DecalPrepassData decalPrepassData;
					#ifdef _DISABLE_DECALS
					ZERO_INITIALIZE(DecalPrepassData, decalPrepassData);
					#else
					decalPrepassData.geomNormalWS = surfaceData.geomNormalWS;
					#endif
					decalPrepassData.renderingLayerMask = GetMeshRenderingLayerMask();
					EncodeIntoDecalPrepassBuffer(decalPrepassData, outDecalBuffer);
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "META"
			Tags { "LightMode"="Meta" }

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma shader_feature_local_fragment _ENABLE_FOG_ON_TRANSPARENT
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 150006


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT

			#pragma shader_feature EDITOR_VISUALIZATION

			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex Vert
			#pragma fragment Frag

			#if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
			#define _WRITE_TRANSPARENT_MOTION_VECTOR
			#endif

			#define SHADERPASS SHADERPASS_LIGHT_TRANSPORT
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

			CBUFFER_START( UnityPerMaterial )
			float4 _Outline;
			float4 _B;
			float4 _R;
			float4 _G;
			float2 _UVDP;
			float2 _UVDS1;
			float2 _UVDP1;
			float2 _UVP;
			float2 _UVS;
			float2 _UVDS;
			float _Intensity;
			float _FlatColor;
			float _Src;
			float _SecondDistortionLerp;
			float _Cull;
			float _ZWrite1;
			float _ZTest;
			float _Dst;
			float _DistortionLerp;
			float _ErosionSmoothness;
			float4 _EmissionColor;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			#ifdef _ENABLE_SHADOW_MATTE
			float _ShadowMatteFilter;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _AlphaCutoff;
			float _AlphaCutoffShadow;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			float _EnableBlendModePreserveSpecularLighting;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTexture;
			sampler2D _DistortionTexture;
			sampler2D _DistortionSecond;


            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_COLOR


			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 uv3 : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				float4 positionCS : SV_Position;
				#ifdef EDITOR_VISUALIZATION
				float2 VizUV : TEXCOORD0;
				float4 LightCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


			
			struct SurfaceDescription
			{
				float3 Color;
				float3 Emission;
				float Alpha;
				float AlphaClipThreshold;
			};

			void BuildSurfaceData( FragInputs fragInputs, SurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData )
			{
				ZERO_INITIALIZE( SurfaceData, surfaceData );
				surfaceData.color = surfaceDescription.Color;

				#ifdef WRITE_NORMAL_BUFFER
				surfaceData.normalWS = fragInputs.tangentToWorld[2];
				#endif
			}

			void GetSurfaceAndBuiltinData( SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData )
			{
				#ifdef LOD_FADE_CROSSFADE
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif

				#ifdef _ALPHATEST_ON
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif

				BuildSurfaceData( fragInputs, surfaceDescription, V, surfaceData );
				ZERO_INITIALIZE( BuiltinData, builtinData );
				builtinData.opacity = surfaceDescription.Alpha;
				#if defined(DEBUG_DISPLAY)
					builtinData.renderingLayers = GetMeshRenderingLayerMask();
				#endif

				#ifdef _ALPHATEST_ON
                    builtinData.alphaClipTreshold = surfaceDescription.AlphaClipThreshold;
                #endif

				builtinData.emissiveColor = surfaceDescription.Emission;

				#ifdef _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif


                ApplyDebugToBuiltinData(builtinData);
			}

			#define SCENEPICKINGPASS
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/MetaPass.hlsl"

			PackedVaryingsMeshToPS VertexFunction( AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS o;
				UNITY_SETUP_INSTANCE_ID( inputMesh );
				UNITY_TRANSFER_INSTANCE_ID( inputMesh, o );

				o.ase_color = inputMesh.ase_color;
				o.ase_texcoord2 = inputMesh.uv0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;

			#ifdef EDITOR_VISUALIZATION
				float2 vizUV = 0;
				float4 lightCoord = 0;
				UnityEditorVizData(inputMesh.positionOS.xyz, inputMesh.uv0.xy, inputMesh.uv1.xy, inputMesh.uv2.xy, vizUV, lightCoord);
			#endif

				float2 uv = float2( 0.0, 0.0 );
				if( unity_MetaVertexControl.x )
				{
					uv = inputMesh.uv1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				}
				else if( unity_MetaVertexControl.y )
				{
					uv = inputMesh.uv2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
				}

				#ifdef EDITOR_VISUALIZATION
					o.VizUV.xy = vizUV;
					o.LightCoord = lightCoord;
				#endif

				o.positionCS = float4( uv * 2.0 - 1.0, inputMesh.positionOS.z > 0 ? 1.0e-4 : 0.0, 1.0 );
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 uv3 : TEXCOORD3;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.uv0 = v.uv0;
				o.uv1 = v.uv1;
				o.uv2 = v.uv2;
				o.uv3 = v.uv3;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.uv0 = patch[0].uv0 * bary.x + patch[1].uv0 * bary.y + patch[2].uv0 * bary.z;
				o.uv1 = patch[0].uv1 * bary.x + patch[1].uv1 * bary.y + patch[2].uv1 * bary.z;
				o.uv2 = patch[0].uv2 * bary.x + patch[1].uv2 * bary.y + patch[2].uv2 * bary.z;
				o.uv3 = patch[0].uv3 * bary.x + patch[1].uv3 * bary.y + patch[2].uv3 * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			float4 Frag( PackedVaryingsMeshToPS packedInput  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE( FragInputs, input );
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;

				PositionInputs posInput = GetPositionInput( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS );

				float3 V = float3( 1.0, 1.0, 1.0 );

				float2 texCoord19 = packedInput.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner27 = ( 1.0 * _Time.y * _UVP + ( texCoord19 * _UVS ));
				float2 texCoord11 = packedInput.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner15 = ( 1.0 * _Time.y * _UVDP + ( texCoord11 * _UVDS ));
				float2 lerpResult26 = lerp( float2( 0,0 ) , ( ( (tex2D( _DistortionTexture, panner15 )).rg + -0.5 ) * 2.0 ) , _DistortionLerp);
				float2 DistortionRegister34 = ( panner27 + lerpResult26 );
				float4 tex2DNode45 = tex2D( _MainTexture, DistortionRegister34 );
				float4 lerpResult97 = lerp( _Outline , _B , tex2DNode45.b);
				float4 lerpResult112 = lerp( lerpResult97 , _G , tex2DNode45.g);
				float4 lerpResult111 = lerp( lerpResult112 , _R , tex2DNode45.r);
				float4 lerpResult88 = lerp( ( packedInput.ase_color * lerpResult111 ) , packedInput.ase_color , _FlatColor);
				float2 texCoord95 = packedInput.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner71 = ( 1.0 * _Time.y * _UVDP1 + ( texCoord95 * _UVDS1 ));
				float4 SecondDistortion108 = ( tex2D( _DistortionSecond, panner71 ) + _SecondDistortionLerp );
				float4 temp_output_90_0 = ( ( lerpResult88 * _Intensity ) * SecondDistortion108 );
				
				float mainTex_alpha48 = tex2DNode45.a;
				float smoothstepResult55 = smoothstep( packedInput.ase_texcoord2.z , ( packedInput.ase_texcoord2.z + _ErosionSmoothness ) , mainTex_alpha48);
				float mainTex_VC_alha52 = packedInput.ase_color.a;
				float OpacityRegister61 = ( smoothstepResult55 * mainTex_VC_alha52 );
				

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				surfaceDescription.Color = temp_output_90_0.rgb;
				surfaceDescription.Emission = temp_output_90_0.rgb;
				surfaceDescription.Alpha = OpacityRegister61;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData( surfaceDescription,input, V, posInput, surfaceData, builtinData );

				BSDFData bsdfData = ConvertSurfaceDataToBSDFData( input.positionSS.xy, surfaceData );
				LightTransportData lightTransportData = GetLightTransportData( surfaceData, builtinData, bsdfData );

				float4 res = float4( 0.0, 0.0, 0.0, 1.0 );
				UnityMetaInput metaInput;
				metaInput.Albedo = lightTransportData.diffuseColor.rgb;
				metaInput.Emission = lightTransportData.emissiveColor;
			#ifdef EDITOR_VISUALIZATION
				metaInput.VizUV = packedInput.VizUV;
				metaInput.LightCoord = packedInput.LightCoord;
			#endif
				res = UnityMetaFragment(metaInput);

				return res;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma shader_feature_local_fragment _ENABLE_FOG_ON_TRANSPARENT
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 150006


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT

			#pragma editor_sync_compilation

			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex Vert
			#pragma fragment Frag

			#define SHADERPASS SHADERPASS_DEPTH_ONLY
			#define SCENESELECTIONPASS 1

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

			int _ObjectId;
			int _PassValue;

			CBUFFER_START( UnityPerMaterial )
			float4 _Outline;
			float4 _B;
			float4 _R;
			float4 _G;
			float2 _UVDP;
			float2 _UVDS1;
			float2 _UVDP1;
			float2 _UVP;
			float2 _UVS;
			float2 _UVDS;
			float _Intensity;
			float _FlatColor;
			float _Src;
			float _SecondDistortionLerp;
			float _Cull;
			float _ZWrite1;
			float _ZTest;
			float _Dst;
			float _DistortionLerp;
			float _ErosionSmoothness;
			float4 _EmissionColor;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			#ifdef _ENABLE_SHADOW_MATTE
			float _ShadowMatteFilter;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _AlphaCutoff;
			float _AlphaCutoffShadow;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			float _EnableBlendModePreserveSpecularLighting;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTexture;
			sampler2D _DistortionTexture;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			void BuildSurfaceData(FragInputs fragInputs, SurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				#ifdef WRITE_NORMAL_BUFFER
				surfaceData.normalWS = fragInputs.tangentToWorld[2];
				#endif
			}

			void GetSurfaceAndBuiltinData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif

				#ifdef _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData);
				ZERO_INITIALIZE(BuiltinData, builtinData);
				builtinData.opacity =  surfaceDescription.Alpha;

				#ifdef _ALPHATEST_ON
                    builtinData.alphaClipTreshold = surfaceDescription.AlphaClipThreshold;
                #endif

				#ifdef _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif


                ApplyDebugToBuiltinData(builtinData);
			}

			PackedVaryingsMeshToPS VertexFunction( AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS o;
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord1 = inputMesh.ase_texcoord;
				o.ase_color = inputMesh.ase_color;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;

				#ifdef ASE_CUSTOM_MOTION_VECTOR
				// Declared so the Velocity output port surfaces on the master node; only consumed by the motion vector pass.
				float3 aseCustomVelocity = float3( 0, 0, 0 );
				#endif

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				o.positionCS = ASE_ADJUST_CLIP_POSITION( TransformWorldToHClip(positionRWS) );
				o.positionRWS = positionRWS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput
						, out float4 outColor : SV_Target0
						#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
						, out float outputDepth : DEPTH_OFFSET_SEMANTIC
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 PositionRWS = packedInput.positionRWS;
				float3 V = GetWorldSpaceNormalizeViewDir( packedInput.positionRWS );
				float4 ScreenPosNorm = float4( posInput.positionNDC, packedInput.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, packedInput.positionCS.z ) * packedInput.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos, _ProjectionParams.x );

				float2 texCoord19 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner27 = ( 1.0 * _Time.y * _UVP + ( texCoord19 * _UVS ));
				float2 texCoord11 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner15 = ( 1.0 * _Time.y * _UVDP + ( texCoord11 * _UVDS ));
				float2 lerpResult26 = lerp( float2( 0,0 ) , ( ( (tex2D( _DistortionTexture, panner15 )).rg + -0.5 ) * 2.0 ) , _DistortionLerp);
				float2 DistortionRegister34 = ( panner27 + lerpResult26 );
				float4 tex2DNode45 = tex2D( _MainTexture, DistortionRegister34 );
				float mainTex_alpha48 = tex2DNode45.a;
				float smoothstepResult55 = smoothstep( packedInput.ase_texcoord1.z , ( packedInput.ase_texcoord1.z + _ErosionSmoothness ) , mainTex_alpha48);
				float mainTex_VC_alha52 = packedInput.ase_color.a;
				float OpacityRegister61 = ( smoothstepResult55 * mainTex_VC_alha52 );
				

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				surfaceDescription.Alpha = OpacityRegister61;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#if defined( ASE_WRITE_DEPTH )
				posInput.deviceDepth = posInput.deviceDepth;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
				outputDepth = posInput.deviceDepth;
				#endif

				outColor = float4( _ObjectId, _PassValue, 1.0, 1.0 );
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthForwardOnly"
			Tags { "LightMode"="DepthForwardOnly" }

			Cull [_CullMode]
			ZWrite On
			Stencil
			{
				Ref [_StencilRefDepth]
				WriteMask [_StencilWriteMaskDepth]
				Comp Always
				Pass Replace
			}


			ColorMask 0 0

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma shader_feature_local_fragment _ENABLE_FOG_ON_TRANSPARENT
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 150006


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT

			#pragma multi_compile _ WRITE_MSAA_DEPTH

			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex Vert
			#pragma fragment Frag

			#define SHADERPASS SHADERPASS_DEPTH_ONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

			CBUFFER_START( UnityPerMaterial )
			float4 _Outline;
			float4 _B;
			float4 _R;
			float4 _G;
			float2 _UVDP;
			float2 _UVDS1;
			float2 _UVDP1;
			float2 _UVP;
			float2 _UVS;
			float2 _UVDS;
			float _Intensity;
			float _FlatColor;
			float _Src;
			float _SecondDistortionLerp;
			float _Cull;
			float _ZWrite1;
			float _ZTest;
			float _Dst;
			float _DistortionLerp;
			float _ErosionSmoothness;
			float4 _EmissionColor;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			#ifdef _ENABLE_SHADOW_MATTE
			float _ShadowMatteFilter;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _AlphaCutoff;
			float _AlphaCutoffShadow;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			float _EnableBlendModePreserveSpecularLighting;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTexture;
			sampler2D _DistortionTexture;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			void BuildSurfaceData(FragInputs fragInputs, SurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				#ifdef WRITE_NORMAL_BUFFER
				surfaceData.normalWS = fragInputs.tangentToWorld[2];
				#endif
			}

			void GetSurfaceAndBuiltinData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif

				#ifdef _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif

				BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData);
				ZERO_INITIALIZE(BuiltinData, builtinData);
				builtinData.opacity =  surfaceDescription.Alpha;

				#if defined(DEBUG_DISPLAY)
					builtinData.renderingLayers = GetMeshRenderingLayerMask();
				#endif

                #ifdef _ALPHATEST_ON
                    builtinData.alphaClipTreshold = surfaceDescription.AlphaClipThreshold;
                #endif

				#ifdef _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif

                ApplyDebugToBuiltinData(builtinData);
			}

			PackedVaryingsMeshToPS VertexFunction( AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS o;
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord1 = inputMesh.ase_texcoord;
				o.ase_color = inputMesh.ase_color;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;

				#ifdef ASE_CUSTOM_MOTION_VECTOR
				// Declared so the Velocity output port surfaces on the master node; only consumed by the motion vector pass.
				float3 aseCustomVelocity = float3( 0, 0, 0 );
				#endif

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				o.positionCS = ASE_ADJUST_CLIP_POSITION( TransformWorldToHClip(positionRWS) );
				o.positionRWS = positionRWS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput
						#ifdef WRITE_MSAA_DEPTH
						, out float4 depthColor : SV_Target0
							#ifdef WRITE_NORMAL_BUFFER
							, out float4 outNormalBuffer : SV_Target1
							#endif
						#else
							#ifdef WRITE_NORMAL_BUFFER
							, out float4 outNormalBuffer : SV_Target0
							#endif
						#endif
						#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
						, out float outputDepth : DEPTH_OFFSET_SEMANTIC
						#endif
						 )
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;
				input.positionRWS = packedInput.positionRWS;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 PositionRWS = packedInput.positionRWS;
				float3 V = GetWorldSpaceNormalizeViewDir( packedInput.positionRWS );
				float4 ScreenPosNorm = float4( posInput.positionNDC, packedInput.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, packedInput.positionCS.z ) * packedInput.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos, _ProjectionParams.x );

				float2 texCoord19 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner27 = ( 1.0 * _Time.y * _UVP + ( texCoord19 * _UVS ));
				float2 texCoord11 = packedInput.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner15 = ( 1.0 * _Time.y * _UVDP + ( texCoord11 * _UVDS ));
				float2 lerpResult26 = lerp( float2( 0,0 ) , ( ( (tex2D( _DistortionTexture, panner15 )).rg + -0.5 ) * 2.0 ) , _DistortionLerp);
				float2 DistortionRegister34 = ( panner27 + lerpResult26 );
				float4 tex2DNode45 = tex2D( _MainTexture, DistortionRegister34 );
				float mainTex_alpha48 = tex2DNode45.a;
				float smoothstepResult55 = smoothstep( packedInput.ase_texcoord1.z , ( packedInput.ase_texcoord1.z + _ErosionSmoothness ) , mainTex_alpha48);
				float mainTex_VC_alha52 = packedInput.ase_color.a;
				float OpacityRegister61 = ( smoothstepResult55 * mainTex_VC_alha52 );
				

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				surfaceDescription.Alpha = OpacityRegister61;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#if defined( ASE_WRITE_DEPTH )
				posInput.deviceDepth = posInput.deviceDepth;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
				outputDepth = posInput.deviceDepth;
				#endif

				#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.positionCS.z;
					#ifdef _ALPHATOMASK_ON
					depthColor.a = SharpenAlpha(builtinData.opacity, builtinData.alphaClipTreshold);
					#endif
				#endif

				#if defined(WRITE_NORMAL_BUFFER)
					EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), outNormalBuffer);
				#endif
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "MotionVectors"
			Tags { "LightMode"="MotionVectors" }

			Cull [_CullMode]

			ZWrite On

			Stencil
			{
				Ref [_StencilRefMV]
				WriteMask [_StencilWriteMaskMV]
				Comp Always
				Pass Replace
			}


			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma shader_feature_local_fragment _ENABLE_FOG_ON_TRANSPARENT
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 150006


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT

			#pragma multi_compile _ WRITE_MSAA_DEPTH

			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex Vert
			#pragma fragment Frag

			#if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
			#define _WRITE_TRANSPARENT_MOTION_VECTOR
			#endif

			#define SHADERPASS SHADERPASS_MOTION_VECTORS

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

			CBUFFER_START( UnityPerMaterial )
			float4 _Outline;
			float4 _B;
			float4 _R;
			float4 _G;
			float2 _UVDP;
			float2 _UVDS1;
			float2 _UVDP1;
			float2 _UVP;
			float2 _UVS;
			float2 _UVDS;
			float _Intensity;
			float _FlatColor;
			float _Src;
			float _SecondDistortionLerp;
			float _Cull;
			float _ZWrite1;
			float _ZTest;
			float _Dst;
			float _DistortionLerp;
			float _ErosionSmoothness;
			float4 _EmissionColor;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			#ifdef _ENABLE_SHADOW_MATTE
			float _ShadowMatteFilter;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _AlphaCutoff;
			float _AlphaCutoffShadow;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			float _EnableBlendModePreserveSpecularLighting;
			#ifdef ASE_TESSELLATION
			float _TessPhongStrength;
			float _TessValue;
			float _TessMin;
			float _TessMax;
			float _TessEdgeLength;
			float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTexture;
			sampler2D _DistortionTexture;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float3 previousPositionOS : TEXCOORD4;
				float3 precomputedVelocity : TEXCOORD5;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				ASE_SV_POSITION_QUALIFIERS float4 vmeshPositionCS : SV_Position;
				float3 vmeshPositionRWS : TEXCOORD0;
				float4 vpassPositionCS : TEXCOORD1;
				float4 vpassPreviousPositionCS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			void BuildSurfaceData(FragInputs fragInputs, SurfaceDescription surfaceDescription, float3 V, out SurfaceData surfaceData)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
				#ifdef WRITE_NORMAL_BUFFER
				surfaceData.normalWS = fragInputs.tangentToWorld[2];
				#endif
			}

			void GetSurfaceAndBuiltinData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif

				#ifdef _ALPHATEST_ON
				DoAlphaTest ( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif

				BuildSurfaceData(fragInputs, surfaceDescription, V, surfaceData);
				ZERO_INITIALIZE(BuiltinData, builtinData);
				builtinData.opacity =  surfaceDescription.Alpha;

				#if defined(DEBUG_DISPLAY)
                    builtinData.renderingLayers = GetMeshRenderingLayerMask();
                #endif


                #ifdef _ALPHATEST_ON
                    builtinData.alphaClipTreshold = surfaceDescription.AlphaClipThreshold;
                #endif


                #ifdef _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif

                ApplyDebugToBuiltinData(builtinData);
			}

			AttributesMesh ApplyMeshModification(AttributesMesh inputMesh, float3 timeParameters, inout PackedVaryingsMeshToPS o, out float3 customVelocity )
			{
				float3 currentTimeParams = _TimeParameters.xyz;
				_TimeParameters.xyz = timeParameters;

				o.ase_texcoord3 = inputMesh.ase_texcoord;
				o.ase_color = inputMesh.ase_color;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif
				inputMesh.normalOS = inputMesh.normalOS;

				customVelocity = float3( 0, 0, 0 );

				_TimeParameters.xyz = currentTimeParams;
				return inputMesh;
			}

			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh)
			{
				PackedVaryingsMeshToPS o = (PackedVaryingsMeshToPS)0;
				AttributesMesh defaultMesh = inputMesh;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				float3 currentVelocity;
				inputMesh = ApplyMeshModification(inputMesh, _TimeParameters.xyz, o, currentVelocity);

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);

				float3 VMESHpositionRWS = positionRWS;
				float4 VMESHpositionCS = TransformWorldToHClip(positionRWS);

				float4 VPASSpreviousPositionCS;
				float4 VPASSpositionCS = mul(UNITY_MATRIX_UNJITTERED_VP, float4(VMESHpositionRWS, 1.0));

				bool forceNoMotion = unity_MotionVectorsParams.y == 0.0;
				if (forceNoMotion)
				{
					VPASSpreviousPositionCS = float4(0.0, 0.0, 0.0, 1.0);
				}
				else
				{
					bool hasDeformation = unity_MotionVectorsParams.x > 0.0;
					float3 effectivePositionOS = (hasDeformation ? inputMesh.previousPositionOS : defaultMesh.positionOS);

					#if defined(HAVE_MESH_MODIFICATION)
						AttributesMesh previousMesh = defaultMesh;
						previousMesh.positionOS = effectivePositionOS;
						PackedVaryingsMeshToPS test = (PackedVaryingsMeshToPS)0;
						float3 previousVelocity;
						previousMesh = ApplyMeshModification(previousMesh, _LastTimeParameters.xyz, test, previousVelocity);
						effectivePositionOS = previousMesh.positionOS;
					#endif

					#if defined(_ADD_PRECOMPUTED_VELOCITY)
						effectivePositionOS -= inputMesh.precomputedVelocity;
					#endif

					#if defined(ASE_CUSTOM_MOTION_VECTOR)
						effectivePositionOS -= currentVelocity;
					#endif

					float3 previousPositionRWS = TransformPreviousObjectToWorld( effectivePositionOS );

					#ifdef ATTRIBUTES_NEED_NORMAL
						float3 normalWS = TransformPreviousObjectToWorldNormal(defaultMesh.normalOS);
					#else
						float3 normalWS = float3(0.0, 0.0, 0.0);
					#endif

					#if defined(HAVE_VERTEX_MODIFICATION)
						ApplyVertexModification(inputMesh, normalWS, previousPositionRWS, _LastTimeParameters.xyz);
					#endif

					#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
						if (_TransparentCameraOnlyMotionVectors > 0)
						{
							previousPositionRWS = VMESHpositionRWS.xyz;
						}
					#endif

					VPASSpreviousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(previousPositionRWS, 1.0));
				}

				o.vmeshPositionCS = ASE_ADJUST_CLIP_POSITION( VMESHpositionCS );
				o.vmeshPositionRWS = VMESHpositionRWS;

				o.vpassPositionCS = ASE_ADJUST_CLIP_POSITION( VPASSpositionCS );
				o.vpassPreviousPositionCS = ASE_ADJUST_CLIP_POSITION( VPASSpreviousPositionCS );
				return o;
			}

			#if ( 0 ) // TEMPORARY: defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float3 previousPositionOS : TEXCOORD4;
				float3 precomputedVelocity : TEXCOORD5;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.previousPositionOS = v.previousPositionOS;
				#if defined (_ADD_PRECOMPUTED_VELOCITY)
					o.precomputedVelocity = v.precomputedVelocity;
				#endif
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.previousPositionOS = patch[0].previousPositionOS * bary.x + patch[1].previousPositionOS * bary.y + patch[2].previousPositionOS * bary.z;
				#if defined (_ADD_PRECOMPUTED_VELOCITY)
					o.precomputedVelocity = patch[0].precomputedVelocity * bary.x + patch[1].precomputedVelocity * bary.y + patch[2].precomputedVelocity * bary.z;
				#endif
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(WRITE_DECAL_BUFFER) && defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_NORMAL SV_Target3
			#elif defined(WRITE_DECAL_BUFFER) || defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_NORMAL SV_Target2
			#else
			#define SV_TARGET_NORMAL SV_Target1
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput
						#ifdef WRITE_MSAA_DEPTH
						, out float4 depthColor : SV_Target0
						, out float4 outMotionVector : SV_Target1
							#ifdef WRITE_DECAL_BUFFER
							, out float4 outDecalBuffer : SV_Target2
							#endif
						#else
						, out float4 outMotionVector : SV_Target0
							#ifdef WRITE_DECAL_BUFFER
							, out float4 outDecalBuffer : SV_Target1
							#endif
						#endif

						#ifdef WRITE_NORMAL_BUFFER
						, out float4 outNormalBuffer : SV_TARGET_NORMAL
						#endif

						#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
						, out float outputDepth : DEPTH_OFFSET_SEMANTIC
						#endif
						 )
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.vmeshPositionCS;
				input.positionRWS = packedInput.vmeshPositionRWS.xyz;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 PositionRWS = packedInput.vmeshPositionRWS.xyz;
				float3 V = GetWorldSpaceNormalizeViewDir( packedInput.vmeshPositionRWS.xyz );
				float4 ScreenPosNorm = float4( posInput.positionNDC, packedInput.vmeshPositionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, packedInput.vmeshPositionCS.z ) * packedInput.vmeshPositionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos, _ProjectionParams.x );

				float2 texCoord19 = packedInput.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner27 = ( 1.0 * _Time.y * _UVP + ( texCoord19 * _UVS ));
				float2 texCoord11 = packedInput.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner15 = ( 1.0 * _Time.y * _UVDP + ( texCoord11 * _UVDS ));
				float2 lerpResult26 = lerp( float2( 0,0 ) , ( ( (tex2D( _DistortionTexture, panner15 )).rg + -0.5 ) * 2.0 ) , _DistortionLerp);
				float2 DistortionRegister34 = ( panner27 + lerpResult26 );
				float4 tex2DNode45 = tex2D( _MainTexture, DistortionRegister34 );
				float mainTex_alpha48 = tex2DNode45.a;
				float smoothstepResult55 = smoothstep( packedInput.ase_texcoord3.z , ( packedInput.ase_texcoord3.z + _ErosionSmoothness ) , mainTex_alpha48);
				float mainTex_VC_alha52 = packedInput.ase_color.a;
				float OpacityRegister61 = ( smoothstepResult55 * mainTex_VC_alha52 );
				

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				surfaceDescription.Alpha = OpacityRegister61;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#if defined( ASE_WRITE_DEPTH )
				posInput.deviceDepth = posInput.deviceDepth;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				float4 VPASSpositionCS = float4(packedInput.vpassPositionCS.xy, 0.0, packedInput.vpassPositionCS.z);
				float4 VPASSpreviousPositionCS = float4(packedInput.vpassPreviousPositionCS.xy, 0.0, packedInput.vpassPreviousPositionCS.z);

				#ifdef _DEPTHOFFSET_ON
				VPASSpositionCS.w += builtinData.depthOffset;
				VPASSpreviousPositionCS.w += builtinData.depthOffset;
				#endif

				float2 motionVector = CalculateMotionVector( VPASSpositionCS, VPASSpreviousPositionCS );
				EncodeMotionVector( motionVector * 0.5, outMotionVector );

				bool forceNoMotion = unity_MotionVectorsParams.y == 0.0;
				if( forceNoMotion )
					outMotionVector = float4( 2.0, 0.0, 0.0, 0.0 );

				#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.vmeshPositionCS.z;
					depthColor.a = SharpenAlpha(builtinData.opacity, builtinData.alphaClipTreshold);
				#endif

				#ifdef WRITE_NORMAL_BUFFER
					EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), outNormalBuffer);
				#endif

				#if defined(WRITE_DECAL_BUFFER)
					DecalPrepassData decalPrepassData;
					#ifdef _DISABLE_DECALS
					ZERO_INITIALIZE(DecalPrepassData, decalPrepassData);
					#else
					decalPrepassData.geomNormalWS = surfaceData.geomNormalWS;
					#endif
					decalPrepassData.renderingLayerMask = GetMeshRenderingLayerMask();
					EncodeIntoDecalPrepassBuffer(decalPrepassData, outDecalBuffer);
				#endif

				#if defined(_DEPTHOFFSET_ON) || defined(ASE_WRITE_DEPTH)
				outputDepth = posInput.deviceDepth;
				#endif
			}

			ENDHLSL
		}

		
        Pass
		{
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

            Cull [_CullMode]

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#pragma instancing_options renderinglayer
			#pragma shader_feature_local_fragment _ENABLE_FOG_ON_TRANSPARENT
			#define ASE_VERSION 19912
			#define ASE_SRP_VERSION 150006


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _TRANSPARENT_WRITES_MOTION_VEC

			#pragma editor_sync_compilation

			#pragma multi_compile _ DOTS_INSTANCING_ON

			#pragma vertex Vert
			#pragma fragment Frag

			#if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
			#define _WRITE_TRANSPARENT_MOTION_VECTOR
			#endif

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_TANGENT_TO_WORLD

			#define SHADERPASS SHADERPASS_DEPTH_ONLY
			#define SCENEPICKINGPASS 1

			#define SHADER_UNLIT

			float4 _SelectionID;

            CBUFFER_START( UnityPerMaterial )
			float4 _Outline;
			float4 _B;
			float4 _R;
			float4 _G;
			float2 _UVDP;
			float2 _UVDS1;
			float2 _UVDP1;
			float2 _UVP;
			float2 _UVS;
			float2 _UVDS;
			float _Intensity;
			float _FlatColor;
			float _Src;
			float _SecondDistortionLerp;
			float _Cull;
			float _ZWrite1;
			float _ZTest;
			float _Dst;
			float _DistortionLerp;
			float _ErosionSmoothness;
			float4 _EmissionColor;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			#ifdef _ENABLE_SHADOW_MATTE
			float _ShadowMatteFilter;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _AlphaCutoff;
			float _AlphaCutoffShadow;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			float _EnableBlendModePreserveSpecularLighting;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTexture;
			sampler2D _DistortionTexture;


            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_TEXTURE_COORDINATES0
			#define ASE_NEEDS_FRAG_TEXTURE_COORDINATES0


			#if defined(ASE_WRITE_DEPTH_CONSERVATIVE) && (SHADER_TARGET >= 45)
				#define ASE_SV_DEPTH SV_DepthLessEqual
				#define ASE_SV_POSITION_QUALIFIERS linear noperspective centroid
			#else
				#define ASE_SV_DEPTH SV_Depth
				#define ASE_SV_POSITION_QUALIFIERS
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				ASE_SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				float3 positionRWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float4 tangentWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
            struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};


            void GetSurfaceAndBuiltinData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                #ifdef LOD_FADE_CROSSFADE
			        LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif

                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif

                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif


				ZERO_INITIALIZE(SurfaceData, surfaceData);

				ZERO_BUILTIN_INITIALIZE(builtinData);
				builtinData.opacity = surfaceDescription.Alpha;

				#if defined(DEBUG_DISPLAY)
					builtinData.renderingLayers = GetMeshRenderingLayerMask();
				#endif

                #ifdef _ALPHATEST_ON
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif

                #ifdef _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif


                ApplyDebugToBuiltinData(builtinData);

            }


			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh  )
			{

				PackedVaryingsMeshToPS o;
				ZERO_INITIALIZE(PackedVaryingsMeshToPS, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, o );

				o.ase_texcoord3 = inputMesh.ase_texcoord;
				o.ase_color = inputMesh.ase_color;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = inputMesh.normalOS;

				#ifdef ASE_CUSTOM_MOTION_VECTOR
				// Declared so the Velocity output port surfaces on the master node; only consumed by the motion vector pass.
				float3 aseCustomVelocity = float3( 0, 0, 0 );
				#endif

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				o.positionCS = ASE_ADJUST_CLIP_POSITION( TransformWorldToHClip(positionRWS) );
				o.positionRWS = positionRWS;
				o.normalWS.xyz =  normalWS;
				o.tangentWS.xyzw =  tangentWS;
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_color : COLOR;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_color = v.ase_color;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			void Frag(	PackedVaryingsMeshToPS packedInput
						, out float4 outColor : SV_Target0
						
					)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;

				input.tangentToWorld = BuildTangentToWorld(packedInput.tangentWS.xyzw, packedInput.normalWS.xyz);

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 PositionRWS = packedInput.positionRWS;
				float3 V = GetWorldSpaceNormalizeViewDir( packedInput.positionRWS );
				float4 ScreenPosNorm = float4( posInput.positionNDC, packedInput.positionCS.zw );
				float4 ClipPos = ComputeClipSpacePosition( ScreenPosNorm.xy, packedInput.positionCS.z ) * packedInput.positionCS.w;
				float4 ScreenPos = ComputeScreenPos( ClipPos, _ProjectionParams.x );

				float2 texCoord19 = packedInput.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner27 = ( 1.0 * _Time.y * _UVP + ( texCoord19 * _UVS ));
				float2 texCoord11 = packedInput.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float2 panner15 = ( 1.0 * _Time.y * _UVDP + ( texCoord11 * _UVDS ));
				float2 lerpResult26 = lerp( float2( 0,0 ) , ( ( (tex2D( _DistortionTexture, panner15 )).rg + -0.5 ) * 2.0 ) , _DistortionLerp);
				float2 DistortionRegister34 = ( panner27 + lerpResult26 );
				float4 tex2DNode45 = tex2D( _MainTexture, DistortionRegister34 );
				float mainTex_alpha48 = tex2DNode45.a;
				float smoothstepResult55 = smoothstep( packedInput.ase_texcoord3.z , ( packedInput.ase_texcoord3.z + _ErosionSmoothness ) , mainTex_alpha48);
				float mainTex_VC_alha52 = packedInput.ase_color.a;
				float OpacityRegister61 = ( smoothstepResult55 * mainTex_VC_alha52 );
				

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				surfaceDescription.Alpha = OpacityRegister61;

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);
				outColor = unity_SelectionID;
			}

            ENDHLSL
        }

		Pass
		{
			Name "FullScreenDebug"
			Tags
			{
				"LightMode" = "FullScreenDebug"
			}

			Cull [_CullMode]
			ZTest LEqual
			ZWrite Off

			HLSLPROGRAM

			/*ase_pragma_before*/

			#pragma vertex Vert
			#pragma fragment Frag

			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"

			#define SHADERPASS SHADERPASS_FULL_SCREEN_DEBUG

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};

			struct VaryingsMeshToPS
			{
				SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
			};

			struct PackedVaryingsMeshToPS
			{
				SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
					uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
			};

			VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
			{
				VaryingsMeshToPS output;
				output.positionCS = input.positionCS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				return output;
			}

			PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
			{
				PackedVaryingsMeshToPS output;
				ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
				output.positionCS = input.positionCS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				return output;
			}

			FragInputs BuildFragInputs(VaryingsMeshToPS input)
			{
				FragInputs output;
				ZERO_INITIALIZE(FragInputs, output);

				output.tangentToWorld = k_identity3x3;
				output.positionSS = input.positionCS;

				return output;
			}

			FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
			{
				UNITY_SETUP_INSTANCE_ID(input);
				VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
				return BuildFragInputs(unpacked);
			}

			#define DEBUG_DISPLAY
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/FullScreenDebug.hlsl"

			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/VertMesh.hlsl"

			PackedVaryingsType Vert(AttributesMesh inputMesh)
			{
				VaryingsType varyingsType;
				varyingsType.vmesh = VertMesh(inputMesh);
				return PackVaryingsType(varyingsType);
			}

			#if !defined(_DEPTHOFFSET_ON)
			[earlydepthstencil] // quad overshading debug mode writes to UAV
			#endif
			void Frag(PackedVaryingsToPS packedInput)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				FragInputs input = UnpackVaryingsToFragInputs(packedInput);

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

			#ifdef PLATFORM_SUPPORTS_PRIMITIVE_ID_IN_PIXEL_SHADER
				if (_DebugFullScreenMode == FULLSCREENDEBUGMODE_QUAD_OVERDRAW)
				{
					IncrementQuadOverdrawCounter(posInput.positionSS.xy, input.primitiveID);
				}
			#endif
			}

			ENDHLSL
		}
		
	}
	

	

	CustomEditor "Rendering.HighDefinition.HDUnlitGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=19912
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":10,"pos":[-4480,-1728],"params":["Inherit","False","1992","995","Distortion","18","34","31","27","26","25","24","23","22","20","19","18","17","16","15","14","13","12","11","","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":11,"pos":[-4416,-1040],"params":["Inherit","False","0","-1","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":12,"pos":[-4160,-912],"params":["Inherit","False","Property","_UVDS","UV D S","16","0","Create","True","0","0","0","False","0","False","Object","-1","","1,1","1,1","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":13,"pos":[-4160,-1040],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":14,"pos":[-3904,-912],"params":["Inherit","False","Property","_UVDP","UV D P","17","0","Create","True","0","0","0","False","0","False","Object","-1","","0.1,-0.2","0.1,-0.2","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":15,"pos":[-3904,-1040],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":16,"pos":[-3648,-1040],"params":["Inherit","True","Property","_DistortionTexture","Distortion Texture","11","0","Create","True","0","0","0","False","3","Space(13)","Header(Distortion)","Space(13)","False","","-1","None","None","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.ComponentMaskNode, AmplifyShaderEditor","id":17,"pos":[-3264,-1040],"params":["Inherit","False","True","True","False","False","1","0","COLOR","0,0,0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":18,"pos":[-3520,-1552],"params":["Inherit","False","Property","_UVS","UV S","1","0","Create","True","0","0","0","False","0","False","Object","-1","","1,1","1,1","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":19,"pos":[-3776,-1680],"params":["Inherit","False","0","-1","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":20,"pos":[-3264,-1552],"params":["Inherit","False","Property","_UVP","UV P","2","0","Create","True","0","0","0","False","0","False","Object","-1","","0,0","0,0","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":22,"pos":[-3264,-1168],"params":["Inherit","False","Property","_DistortionLerp","Distortion Lerp","13","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0.1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":23,"pos":[-3264,-1296],"params":["Inherit","False","Constant","_Vector0","Vector 0","8","0","Create","True","0","0","0","False","0","False","Object","-1","","0,0","0,0","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.FunctionNode, AmplifyShaderEditor","id":24,"pos":[-3008,-1040],"params":["Inherit","False","ConstantBiasScale","-1","","1","63208df05c83e8e49a48ffbdce2e43a0","0","3","3","FLOAT2","0,0","False","1","FLOAT","-0.5","False","2","FLOAT","2","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":25,"pos":[-3520,-1680],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":26,"pos":[-2880,-1296],"params":["Inherit","False","3","0","FLOAT2","0,0","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":27,"pos":[-3264,-1680],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":31,"pos":[-2880,-1680],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":36,"pos":[-2416,-1728],"params":["Inherit","False","1896","1857.979","Color","16","48","45","84","112","111","106","105","101","97","96","93","92","88","52","47","38","","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":34,"pos":[-2720,-1680],"params":["Inherit","False","DistortionRegister","-1","True","1","0","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":38,"pos":[-2304,-640],"params":["Inherit","False","34","DistortionRegister","1","0","OBJECT","","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":21,"pos":[-4464,-320],"params":["Inherit","False","1538.791","442.8129","Opacity","8","61","58","57","55","53","51","46","28","","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":45,"pos":[-1792,-640],"params":["Inherit","True","Property","_MainTexture","Main Texture","0","0","Create","True","0","0","0","False","3","Space(13)","Header(Main Texture)","Space(13)","False","","-1","None","None","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":46,"pos":[-4160,-208],"params":["Inherit","False","Property","_ErosionSmoothness","Erosion Smoothness","8","0","Create","True","0","0","0","False","0","False","Object","-1","","0.1","1.57","0.1","15","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.VertexColorNode, AmplifyShaderEditor","id":47,"pos":[-1280,-1024],"params":["Inherit","False","0","5","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":48,"pos":[-1408,-640],"params":["Inherit","False","mainTex_alpha","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":51,"pos":[-3856,-128],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":52,"pos":[-1280,-768],"params":["Inherit","False","mainTex_VC_alha","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":53,"pos":[-3968,-272],"params":["Inherit","False","48","mainTex_alpha","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":28,"pos":[-4432,-208],"params":["Inherit","False","0","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":57,"pos":[-3600,-280],"params":["Inherit","False","52","mainTex_VC_alha","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SmoothstepOpNode, AmplifyShaderEditor","id":55,"pos":[-3728,-192],"params":["Inherit","False","3","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","1","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":58,"pos":[-3408,-192],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":63,"pos":[336,-48],"params":["Inherit","False","1243","166","AR","5","110","80","78","82","83","","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":65,"pos":[-4464,-704],"params":["Inherit","False","1665.348","371.0714","SecondDistortion","9","108","104","103","102","98","95","94","85","71","","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":61,"pos":[-3120,-128],"params":["Inherit","False","OpacityRegister","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":71,"pos":[-3904,-624],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":85,"pos":[-3904,-496],"params":["Inherit","False","Property","_UVDP1","UV D P","12","0","Create","True","0","0","0","False","0","False","Object","-1","","0.1,-0.2","0.1,-0.2","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":88,"pos":[-768,-640],"params":["Inherit","True","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":90,"pos":[-384,0],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":92,"pos":[-2304,-1152],"params":["Inherit","False","Property","_B","B","5","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","0.5943396,0.259371,0.09812209,0","0.2641509,0.2616589,0.2554289,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":93,"pos":[-2304,-896],"params":["Inherit","False","Property","_Outline","Outline","6","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","0.2169811,0.03320287,0.02354041,0","0,0,0,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.Vector2Node, AmplifyShaderEditor","id":94,"pos":[-4160,-496],"params":["Inherit","False","Property","_UVDS1","UV D S","10","0","Create","True","0","0","0","False","0","False","Object","-1","","1,1","1,1","0","3","FLOAT2","0","FLOAT","1","FLOAT","2"]}
{"type":"AmplifyShaderEditor.TextureCoordinatesNode, AmplifyShaderEditor","id":95,"pos":[-4416,-624],"params":["Inherit","False","0","-1","2","3","2","SAMPLER2D","","False","0","FLOAT2","1,1","False","1","FLOAT2","0,0","False","5","FLOAT2","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":96,"pos":[-2304,-1408],"params":["Inherit","False","Property","_G","G","4","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","1,0.7230805,0.25,0","1,0.3523919,0,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":97,"pos":[-1792,-1280],"params":["Inherit","True","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":98,"pos":[-4160,-624],"params":["Inherit","False","2","2","0","FLOAT2","0,0","False","1","FLOAT2","0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":99,"pos":[-384,-256],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":101,"pos":[-2304,-1664],"params":["Inherit","False","Property","_R","R","3","1","[HDR]","Create","True","0","0","0","False","0","False","Object","-1","","1,0.9719134,0.5896226,0","0.3679245,0.3679245,0.3679245,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":102,"pos":[-3552,-656],"params":["Inherit","True","Property","_DistortionSecond","DistortionSecond","14","1","[Header]","Create","True","1","SecondDistortion","0","0","False","1","Space(13)","False","","-1","None","None","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":103,"pos":[-3536,-448],"params":["Inherit","False","Property","_SecondDistortionLerp","SecondDistortionLerp","15","0","Create","True","0","0","0","False","0","False","Object","-1","","0.5","0","0.5","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":104,"pos":[-3232,-512],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":105,"pos":[-768,-1152],"params":["Inherit","True","2","2","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":106,"pos":[-1152,-384],"params":["Inherit","False","Property","_FlatColor","Flat Color","9","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":107,"pos":[-768,-128],"params":["Inherit","False","Property","_Intensity","Intensity","7","1","[Header]","Create","True","1","TextureProps","0","0","False","1","Space(13)","False","Object","-1","","1","1","0","5","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":111,"pos":[-1152,-1664],"params":["Inherit","True","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":112,"pos":[-1408,-1408],"params":["Inherit","True","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":78,"pos":[640,0],"params":["Inherit","False","Property","_Src","Src","19","0","Create","True","0","0","0","True","0","False","Object","-1","","5","5","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":110,"pos":[896,0],"params":["Inherit","False","Property","_Dst","Dst","20","0","Create","True","0","0","0","True","0","False","Object","-1","","10","10","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":82,"pos":[1408,0],"params":["Inherit","False","Property","_ZTest","ZTest","22","0","Create","True","0","0","0","True","0","False","Object","-1","","2","2","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":83,"pos":[1152,0],"params":["Inherit","False","Property","_ZWrite1","ZWrite1","21","0","Create","True","0","0","0","True","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":84,"pos":[-768,0],"params":["Inherit","False","108","SecondDistortion","1","0","OBJECT","","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":73,"pos":[-768,128],"params":["Inherit","False","61","OpacityRegister","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":80,"pos":[384,0],"params":["Inherit","False","Property","_Cull","Cull","18","0","Create","True","0","0","0","True","3","Space(13)","Header(AR)","Space(13)","False","Object","-1","","2","2","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":108,"pos":[-3024,-512],"params":["Inherit","False","SecondDistortion","-1","True","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":116,"pos":[0,0],"params":["Float","False","True","-1","3","Rendering.HighDefinition.HDUnlitGUI","0","16","Vefects/Lite_SH_Vefects_Unlit_Flipbook_HDRP_New","7f5cb9c3ea6481f469fdd856555439ef","True","Forward Unlit","0","0","Forward Unlit","13","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","4","RenderPipeline=HDRenderPipeline","RenderType=Opaque=RenderType","Queue=Transparent=Queue=0","ShaderGraphShader=true","True","5","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","False","True","1","0","True","_SrcBlend","0","True","_DstBlend","1","0","True","_AlphaSrcBlend","0","True","_AlphaDstBlend","False","False","False","False","False","False","False","False","False","False","False","False","True","0","True","_CullModeForward","False","False","False","True","True","True","True","True","0","True","_ColorMaskTransparentVel","False","False","False","False","False","True","True","0","True","_StencilRef","255","False","","255","True","_StencilWriteMask","7","False","","3","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","0","True","_ZWrite","True","0","True","_ZTestDepthEqualForOpaque","False","False","True","1","LightMode=ForwardOnly","False","False","0","Hidden/InternalErrorShader","0","0","Standard","35","Surface Type","1","639223358668073083","  Rendering Pass ","0","0","  Rendering Pass","1","0","  Blending Mode","0","0","  Receive Fog","1","0","  Distortion","0","0","    Distortion Mode","0","0","    Distortion Only","1","0","  Depth Write","1","0","  Cull Mode","0","0","  Depth Test","4","0","Double-Sided","0","0","Alpha Clipping","0","0","  Use Shadow Threshold","0","0","Receive Decals","1","0","Motion Vectors","1","0","  Add Precomputed Velocity","0","0","  Add Custom Velocity","0","0","Shadow Matte","0","0","Cast Shadows","1","0","Write Depth","0","0","  Depth Offset","0","0","  Conservative","0","0","GPU Instancing","1","0","Tessellation","0","0","  Phong","0","0","  Strength","0.5,False,","0","  Type","0","0","  Tess","16,False,","0","  Min","10,False,","0","  Max","25,False,","0","  Edge Length","16,False,","0","  Max Displacement","25,False,","0","Vertex Position","1","0","LOD CrossFade","0","0","0","8","True","True","True","True","True","True","False","True","False","","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":117,"pos":[0,0],"params":["Float","False","False","-1","3","Rendering.HighDefinition.HDUnlitGUI","0","1","New Amplify Shader","7f5cb9c3ea6481f469fdd856555439ef","True","ShadowCaster","0","1","ShadowCaster","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","4","RenderPipeline=HDRenderPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","ShaderGraphShader=true","True","5","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","0","True","_CullMode","False","True","False","False","False","False","0","False","","False","False","False","False","False","False","False","False","False","True","1","False","","False","False","True","0","True","_ZClip","True","1","LightMode=ShadowCaster","False","False","0","Hidden/InternalErrorShader","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":118,"pos":[0,0],"params":["Float","False","False","-1","3","Rendering.HighDefinition.HDUnlitGUI","0","1","New Amplify Shader","7f5cb9c3ea6481f469fdd856555439ef","True","META","0","2","META","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","4","RenderPipeline=HDRenderPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","ShaderGraphShader=true","True","5","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","2","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","LightMode=Meta","False","False","0","Hidden/InternalErrorShader","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":119,"pos":[0,0],"params":["Float","False","False","-1","3","Rendering.HighDefinition.HDUnlitGUI","0","1","New Amplify Shader","7f5cb9c3ea6481f469fdd856555439ef","True","SceneSelectionPass","0","3","SceneSelectionPass","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","4","RenderPipeline=HDRenderPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","ShaderGraphShader=true","True","5","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","2","False","","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","1","LightMode=SceneSelectionPass","False","False","0","Hidden/InternalErrorShader","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":120,"pos":[0,0],"params":["Float","False","False","-1","3","Rendering.HighDefinition.HDUnlitGUI","0","1","New Amplify Shader","7f5cb9c3ea6481f469fdd856555439ef","True","DepthForwardOnly","0","4","DepthForwardOnly","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","4","RenderPipeline=HDRenderPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","ShaderGraphShader=true","True","5","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","0","True","_CullMode","False","True","False","False","False","False","0","False","","False","False","False","False","False","False","False","True","True","0","True","_StencilRefDepth","255","False","","255","True","_StencilWriteMaskDepth","7","False","","3","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","1","LightMode=DepthForwardOnly","False","False","0","Hidden/InternalErrorShader","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":121,"pos":[0,0],"params":["Float","False","False","-1","3","Rendering.HighDefinition.HDUnlitGUI","0","1","New Amplify Shader","7f5cb9c3ea6481f469fdd856555439ef","True","MotionVectors","0","5","MotionVectors","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","4","RenderPipeline=HDRenderPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","ShaderGraphShader=true","True","5","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","0","True","_CullMode","False","False","False","False","False","False","False","False","False","True","True","0","True","_StencilRefMV","255","False","","255","True","_StencilWriteMaskMV","7","False","","3","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","1","False","","False","False","False","True","1","LightMode=MotionVectors","False","False","0","Hidden/InternalErrorShader","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":122,"pos":[0,0],"params":["Float","False","False","-1","3","Rendering.HighDefinition.HDUnlitGUI","0","1","New Amplify Shader","7f5cb9c3ea6481f469fdd856555439ef","True","DistortionVectors","0","6","DistortionVectors","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","4","RenderPipeline=HDRenderPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","ShaderGraphShader=true","True","5","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","False","True","4","1","False","","1","False","","4","1","False","","1","False","","True","1","False","","1","False","","False","False","False","False","False","False","False","False","False","False","False","True","0","True","_CullMode","False","False","False","False","False","False","False","False","False","True","True","0","True","_StencilRefDistortionVec","255","False","","255","True","_StencilWriteMaskDistortionVec","7","False","","3","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","True","2","False","","True","3","False","","False","False","True","1","LightMode=DistortionVectors","False","False","0","Hidden/InternalErrorShader","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.TemplateMultiPassMasterNode, AmplifyShaderEditor","id":123,"pos":[0,0],"params":["Float","False","False","-1","3","Rendering.HighDefinition.HDUnlitGUI","0","1","New Amplify Shader","7f5cb9c3ea6481f469fdd856555439ef","True","ScenePickingPass","0","7","ScenePickingPass","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","4","RenderPipeline=HDRenderPipeline","RenderType=Opaque=RenderType","Queue=Geometry=Queue=0","ShaderGraphShader=true","True","5","True","9","d3d11","metal","vulkan","xboxone","xboxseries","playstation","ps4","ps5","switch","0","False","False","False","False","False","False","False","False","False","False","False","False","False","False","True","0","True","_CullMode","False","False","False","False","False","False","False","False","False","False","False","True","2","False","","True","3","False","","False","False","True","1","LightMode=Picking","False","False","0","Hidden/InternalErrorShader","0","0","Standard","0","False","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":114,"pos":[-2329.735,-2158.397],"params":["Inherit","False","100","100","Lite version","0","Lite version","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":113,"pos":[336,-224],"params":["Inherit","False","304","100","Lush was here! <3","0","Lush was here! <3","0,0,0,1","0","0"]}
{"wire":[13,0,11,0]}
{"wire":[13,1,12,0]}
{"wire":[15,0,13,0]}
{"wire":[15,2,14,0]}
{"wire":[16,1,15,0]}
{"wire":[17,0,16,0]}
{"wire":[24,3,17,0]}
{"wire":[25,0,19,0]}
{"wire":[25,1,18,0]}
{"wire":[26,0,23,0]}
{"wire":[26,1,24,0]}
{"wire":[26,2,22,0]}
{"wire":[27,0,25,0]}
{"wire":[27,2,20,0]}
{"wire":[31,0,27,0]}
{"wire":[31,1,26,0]}
{"wire":[34,0,31,0]}
{"wire":[45,1,38,0]}
{"wire":[48,0,45,4]}
{"wire":[51,0,28,3]}
{"wire":[51,1,46,0]}
{"wire":[52,0,47,4]}
{"wire":[55,0,53,0]}
{"wire":[55,1,28,3]}
{"wire":[55,2,51,0]}
{"wire":[58,0,55,0]}
{"wire":[58,1,57,0]}
{"wire":[61,0,58,0]}
{"wire":[71,0,98,0]}
{"wire":[71,2,85,0]}
{"wire":[88,0,105,0]}
{"wire":[88,1,47,0]}
{"wire":[88,2,106,0]}
{"wire":[90,0,99,0]}
{"wire":[90,1,84,0]}
{"wire":[97,0,93,0]}
{"wire":[97,1,92,0]}
{"wire":[97,2,45,3]}
{"wire":[98,0,95,0]}
{"wire":[98,1,94,0]}
{"wire":[99,0,88,0]}
{"wire":[99,1,107,0]}
{"wire":[102,1,71,0]}
{"wire":[104,0,102,0]}
{"wire":[104,1,103,0]}
{"wire":[105,0,47,0]}
{"wire":[105,1,111,0]}
{"wire":[111,0,112,0]}
{"wire":[111,1,101,0]}
{"wire":[111,2,45,1]}
{"wire":[112,0,97,0]}
{"wire":[112,1,96,0]}
{"wire":[112,2,45,2]}
{"wire":[108,0,104,0]}
{"wire":[116,0,90,0]}
{"wire":[116,1,90,0]}
{"wire":[116,2,73,0]}
ASEEND*/
//CHKSM=C4D093285FDF7FF9FF66E3354931A9A514CBD8D7
Shader "NatureManufacture Shaders/Standard Shaders/Standard Metalic Snow"
{
	Properties
	{
		_MainTex("MainTex ", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpScale("BumpScale", Range( 0 , 5)) = 0
		_MetalicRAmbientOcclusionGSmoothnessA("Metalic (R) Ambient Occlusion (G) Smoothness (A)", 2D) = "white" {}
		_MetallicPower("Metallic Power", Range( 0 , 2)) = 1
		_AmbientOcclusionPower("Ambient Occlusion Power", Range( 0 , 1)) = 1
		_SmoothnessPower("Smoothness Power", Range( 0 , 2)) = 1
		_DetailMask("DetailMask", 2D) = "white" {}
		_DetailAlbedoPower("Detail Albedo Power", Range( 0 , 2)) = 0
		_DetailAlbedoMap("DetailAlbedoMap", 2D) = "black" {}
		[NoScaleOffset]_DetailNormalMap("DetailNormalMap", 2D) = "bump" {}
		_DetailNormalMapScale("DetailNormalMapScale", Range( 0 , 5)) = 0
		[Toggle(_USESNOW_ON)] _UseSnow("Use Snow", Float) = 1
		[Toggle(_USEDYNAMICSNOWTSTATICMASKF_ON)] _UseDynamicSnowTStaticMaskF("Use Dynamic Snow (T) Static Mask (F)", Float) = 1
		_SnowMaskB("Snow Mask (B)", 2D) = "white" {}
		_SnowMaskPower("Snow Mask Power", Range( 0 , 10)) = 1
		_Snow_Amount("Snow_Amount", Range( 0 , 2)) = 0.13
		_Snow_AmountGrowSpeed("Snow_Amount Grow Speed", Range( 1 , 3)) = 3
		_TriplanarCoverFalloff("Triplanar Cover Falloff", Range( 1 , 100)) = 8
		_SnowAlbedoRGB("Snow Albedo (RGB)", 2D) = "white" {}
		_SnowTiling("Snow Tiling", Range( 0.0001 , 100)) = 15
		_SnowAlbedoColor("Snow Albedo Color", Color) = (1,1,1,1)
		_SnowNormalRGB("Snow Normal (RGB)", 2D) = "white" {}
		_SnowMetalicRAmbientOcclusionGSmothnessA("Snow Metalic (R) Ambient Occlusion(G) Smothness (A)", 2D) = "white" {}
		_SnowNormalScale("Snow Normal Scale", Range( 0 , 5)) = 0
		_SnowNormalCoverHardness("Snow Normal Cover Hardness", Range( 0 , 10)) = 0
		_SnowMetallicPower("Snow Metallic Power", Range( 0 , 2)) = 1
		_SnowAmbientOcclusionPower("Snow Ambient Occlusion Power", Range( 0 , 1)) = 1
		_SnowSmoothnessPower("Snow Smoothness Power", Range( 0 , 2)) = 1
		_SnowMaxAngle("Snow Max Angle ", Range( 0.001 , 90)) = 90
		_SnowHardness("Snow Hardness", Range( 1 , 10)) = 5
		_Snow_Min_Height("Snow_Min_Height", Range( -1000 , 10000)) = -1000
		_Snow_Min_Height_Blending("Snow_Min_Height_Blending", Range( 0 , 500)) = 1
		_SnowHeightG("Snow Height (G)", 2D) = "white" {}
		_SnowHeightSharpness("Snow Height Sharpness", Range( 0 , 2)) = 0.3
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZTest LEqual
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma shader_feature _USESNOW_ON
		#pragma shader_feature _USEDYNAMICSNOWTSTATICMASKF_ON
		#include "NM_indirect.cginc"
		#pragma multi_compile GPU_FRUSTUM_ON __
		#pragma instancing_options procedural:setup
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _BumpScale;
		uniform sampler2D _BumpMap;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _DetailNormalMapScale;
		uniform sampler2D _DetailNormalMap;
		uniform sampler2D _DetailAlbedoMap;
		uniform float4 _DetailAlbedoMap_ST;
		uniform sampler2D _DetailMask;
		uniform float4 _DetailMask_ST;
		uniform sampler2D _SnowNormalRGB;
		uniform float _SnowTiling;
		uniform float _TriplanarCoverFalloff;
		uniform float _SnowNormalScale;
		uniform sampler2D _SnowMaskB;
		uniform float4 _SnowMaskB_ST;
		uniform float _SnowMaskPower;
		uniform float _SnowNormalCoverHardness;
		uniform float _Snow_Amount;
		uniform float _Snow_AmountGrowSpeed;
		uniform float _SnowMaxAngle;
		uniform float _SnowHardness;
		uniform float _Snow_Min_Height;
		uniform float _Snow_Min_Height_Blending;
		uniform sampler2D _SnowHeightG;
		uniform float _SnowHeightSharpness;
		uniform float4 _Color;
		uniform float _DetailAlbedoPower;
		uniform sampler2D _SnowAlbedoRGB;
		uniform float4 _SnowAlbedoColor;
		uniform sampler2D _MetalicRAmbientOcclusionGSmoothnessA;
		uniform float _MetallicPower;
		uniform sampler2D _SnowMetalicRAmbientOcclusionGSmothnessA;
		uniform float _SnowMetallicPower;
		uniform float _SmoothnessPower;
		uniform float _SnowSmoothnessPower;
		uniform float _AmbientOcclusionPower;
		uniform float _SnowAmbientOcclusionPower;


		inline float3 TriplanarSamplingSNF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			xNorm.xyz = half3( UnpackNormal( xNorm ).xy * float2( nsign.x, 1.0 ) + worldNormal.zy, worldNormal.x ).zyx;
			yNorm.xyz = half3( UnpackNormal( yNorm ).xy * float2( nsign.y, 1.0 ) + worldNormal.xz, worldNormal.y ).xzy;
			zNorm.xyz = half3( UnpackNormal( zNorm ).xy * float2( -nsign.z, 1.0 ) + worldNormal.xy, worldNormal.z ).xyz;
			return normalize( xNorm.xyz * projNormal.x + yNorm.xyz * projNormal.y + zNorm.xyz * projNormal.z );
		}


		inline float4 TriplanarSamplingSF( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float tilling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= projNormal.x + projNormal.y + projNormal.z;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = ( tex2D( topTexMap, tilling * worldPos.zy * float2( nsign.x, 1.0 ) ) );
			yNorm = ( tex2D( topTexMap, tilling * worldPos.xz * float2( nsign.y, 1.0 ) ) );
			zNorm = ( tex2D( topTexMap, tilling * worldPos.xy * float2( -nsign.z, 1.0 ) ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float3 tex2DNode4 = UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ), _BumpScale );
			float2 uv_DetailAlbedoMap = i.uv_texcoord * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
			float3 tex2DNode485 = UnpackScaleNormal( tex2D( _DetailNormalMap, uv_DetailAlbedoMap ), _DetailNormalMapScale );
			float2 uv_DetailMask = i.uv_texcoord * _DetailMask_ST.xy + _DetailMask_ST.zw;
			float4 tex2DNode481 = tex2D( _DetailMask, uv_DetailMask );
			float3 lerpResult479 = lerp( tex2DNode4 , BlendNormals( tex2DNode4 , tex2DNode485 ) , tex2DNode481.a);
			float temp_output_265_0 = ( 1.0 / _SnowTiling );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 triplanar457 = TriplanarSamplingSNF( _SnowNormalRGB, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_265_0, 1.0, 0 );
			float3 tanTriplanarNormal457 = mul( ase_worldToTangent, triplanar457 );
			float3 appendResult458 = (float3(_SnowNormalScale , _SnowNormalScale , 1.0));
			float2 uv_SnowMaskB = i.uv_texcoord * _SnowMaskB_ST.xy + _SnowMaskB_ST.zw;
			float4 tex2DNode494 = tex2D( _SnowMaskB, uv_SnowMaskB );
			float clampResult501 = clamp( ( tex2DNode494.b * _SnowMaskPower ) , 0.0 , 1.0 );
			float3 normalizeResult483 = normalize( BlendNormals( UnpackScaleNormal( tex2D( _BumpMap, uv_MainTex ), _SnowNormalCoverHardness ) , tex2DNode485 ) );
			float temp_output_489_0 = ( 4.0 - _Snow_AmountGrowSpeed );
			float clampResult492 = clamp( pow( ( _Snow_Amount / temp_output_489_0 ) , temp_output_489_0 ) , 0.0 , 2.0 );
			float clampResult87 = clamp( ase_worldNormal.y , 0.0 , 0.999999 );
			float temp_output_85_0 = ( _SnowMaxAngle / 45.0 );
			float clampResult83 = clamp( ( clampResult87 - ( 1.0 - temp_output_85_0 ) ) , 0.0 , 2.0 );
			float temp_output_329_0 = ( ( 1.0 - _Snow_Min_Height ) + ase_worldPos.y );
			float clampResult336 = clamp( ( temp_output_329_0 + 1.0 ) , 0.0 , 1.0 );
			float clampResult335 = clamp( ( ( 1.0 - ( ( temp_output_329_0 + _Snow_Min_Height_Blending ) / temp_output_329_0 ) ) + -0.5 ) , 0.0 , 1.0 );
			float clampResult338 = clamp( ( clampResult336 + clampResult335 ) , 0.0 , 1.0 );
			float temp_output_349_0 = ( pow( ( clampResult83 * ( 1.0 / temp_output_85_0 ) ) , _SnowHardness ) * clampResult338 );
			float3 lerpResult15 = lerp( normalizeResult483 , tanTriplanarNormal457 , ( saturate( ( ase_worldNormal.y * clampResult492 ) ) * temp_output_349_0 ));
			float clampResult368 = clamp( ( ( (WorldNormalVector( i , lerpResult15 )).y * clampResult492 ) * ( ( clampResult492 * _SnowHardness ) * temp_output_349_0 ) ) , 0.0 , 1.0 );
			float4 triplanar460 = TriplanarSamplingSF( _SnowHeightG, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_265_0, 1.0, 0 );
			#ifdef _USEDYNAMICSNOWTSTATICMASKF_ON
				float staticSwitch493 = ( clampResult501 * saturate( ( clampResult368 * pow( triplanar460.y , _SnowHeightSharpness ) ) ) );
			#else
				float staticSwitch493 = clampResult501;
			#endif
			#ifdef _USESNOW_ON
				float staticSwitch497 = staticSwitch493;
			#else
				float staticSwitch497 = 1E-07;
			#endif
			float3 lerpResult369 = lerp( lerpResult479 , ( tanTriplanarNormal457 * appendResult458 ) , staticSwitch497);
			o.Normal = lerpResult369;
			float4 temp_output_77_0 = ( tex2D( _MainTex, uv_MainTex ) * _Color );
			float4 blendOpSrc474 = temp_output_77_0;
			float4 blendOpDest474 = ( _DetailAlbedoPower * tex2D( _DetailAlbedoMap, uv_DetailAlbedoMap ) );
			float4 lerpResult480 = lerp( temp_output_77_0 , (( blendOpDest474 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpDest474 - 0.5 ) ) * ( 1.0 - blendOpSrc474 ) ) : ( 2.0 * blendOpDest474 * blendOpSrc474 ) ) , ( _DetailAlbedoPower * tex2DNode481.a ));
			float4 triplanar455 = TriplanarSamplingSF( _SnowAlbedoRGB, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_265_0, 1.0, 0 );
			float4 lerpResult10 = lerp( lerpResult480 , ( triplanar455 * _SnowAlbedoColor ) , staticSwitch497);
			o.Albedo = lerpResult10.xyz;
			float4 tex2DNode2 = tex2D( _MetalicRAmbientOcclusionGSmoothnessA, uv_MainTex );
			float4 triplanar459 = TriplanarSamplingSF( _SnowMetalicRAmbientOcclusionGSmothnessA, ase_worldPos, ase_worldNormal, _TriplanarCoverFalloff, temp_output_265_0, 1.0, 0 );
			float4 break323 = triplanar459;
			float lerpResult17 = lerp( ( tex2DNode2.r * _MetallicPower ) , ( break323.x * _SnowMetallicPower ) , staticSwitch497);
			o.Metallic = lerpResult17;
			float lerpResult27 = lerp( ( tex2DNode2.a * _SmoothnessPower ) , ( break323.w * _SnowSmoothnessPower ) , staticSwitch497);
			o.Smoothness = lerpResult27;
			float clampResult96 = clamp( tex2DNode2.g , ( 1.0 - _AmbientOcclusionPower ) , 1.0 );
			float clampResult94 = clamp( break323.y , ( 1.0 - _SnowAmbientOcclusionPower ) , 1.0 );
			float lerpResult28 = lerp( clampResult96 , clampResult94 , staticSwitch497);
			o.Occlusion = lerpResult28;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
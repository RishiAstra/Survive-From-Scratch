// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Terrain"
{
	Properties
	{
		[HideInInspector]_TerrainHolesTexture("_TerrainHolesTexture", 2D) = "white" {}
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Normal0("Normal0", 2D) = "white" {}
		[HideInInspector]_Normal1("Normal1", 2D) = "white" {}
		[HideInInspector]_Normal2("Normal2", 2D) = "white" {}
		[HideInInspector]_Normal3("Normal3", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-100" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_local __ _ALPHATEST_ON
		#pragma shader_feature_local _MASKMAP
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Mask1;
		uniform sampler2D _Mask0;
		uniform sampler2D _Mask3;
		uniform sampler2D _Mask2;
		uniform float4 _MaskMapRemapScale1;
		uniform float4 _MaskMapRemapScale0;
		uniform float4 _MaskMapRemapOffset1;
		uniform float4 _MaskMapRemapOffset2;
		uniform float4 _MaskMapRemapScale2;
		uniform float4 _MaskMapRemapScale3;
		uniform float4 _MaskMapRemapOffset0;
		uniform float4 _MaskMapRemapOffset3;
		uniform sampler2D _Control;
		uniform float4 _Control_ST;
		uniform sampler2D _Normal0;
		uniform sampler2D _Splat0;
		uniform float4 _Splat0_ST;
		uniform sampler2D _Normal1;
		uniform sampler2D _Splat1;
		uniform float4 _Splat1_ST;
		uniform sampler2D _Normal2;
		uniform sampler2D _Splat2;
		uniform float4 _Splat2_ST;
		uniform sampler2D _Normal3;
		uniform sampler2D _Splat3;
		uniform float4 _Splat3_ST;
		uniform float _Smoothness0;
		uniform float _Smoothness1;
		uniform float _Smoothness2;
		uniform float _Smoothness3;
		uniform sampler2D _TerrainHolesTexture;
		uniform float4 _TerrainHolesTexture_ST;
		uniform float _Metallic;
		uniform float _Smoothness;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float localCalculateTangentsStandard16_g1 = ( 0.0 );
			{
			v.tangent.xyz = cross ( v.normal, float3( 0, 0, 1 ) );
			v.tangent.w = -1;
			}
			float3 temp_cast_0 = (localCalculateTangentsStandard16_g1).xxx;
			v.vertex.xyz += temp_cast_0;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			float4 tex2DNode5_g1 = tex2D( _Control, uv_Control );
			float dotResult20_g1 = dot( tex2DNode5_g1 , float4(1,1,1,1) );
			float SplatWeight22_g1 = dotResult20_g1;
			float localSplatClip74_g1 = ( SplatWeight22_g1 );
			float SplatWeight74_g1 = SplatWeight22_g1;
			{
			#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight74_g1 == 0.0f ? -1 : 1);
			#endif
			}
			float4 SplatControl26_g1 = ( tex2DNode5_g1 / ( localSplatClip74_g1 + 0.001 ) );
			float4 temp_output_59_0_g1 = SplatControl26_g1;
			float2 uv_Splat0 = i.uv_texcoord * _Splat0_ST.xy + _Splat0_ST.zw;
			float2 uv_Splat1 = i.uv_texcoord * _Splat1_ST.xy + _Splat1_ST.zw;
			float2 uv_Splat2 = i.uv_texcoord * _Splat2_ST.xy + _Splat2_ST.zw;
			float2 uv_Splat3 = i.uv_texcoord * _Splat3_ST.xy + _Splat3_ST.zw;
			float4 weightedBlendVar8_g1 = temp_output_59_0_g1;
			float4 weightedBlend8_g1 = ( weightedBlendVar8_g1.x*tex2D( _Normal0, uv_Splat0 ) + weightedBlendVar8_g1.y*tex2D( _Normal1, uv_Splat1 ) + weightedBlendVar8_g1.z*tex2D( _Normal2, uv_Splat2 ) + weightedBlendVar8_g1.w*tex2D( _Normal3, uv_Splat3 ) );
			float3 temp_output_61_0_g1 = UnpackNormal( weightedBlend8_g1 );
			o.Normal = temp_output_61_0_g1;
			float4 appendResult33_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
			float4 tex2DNode4_g1 = tex2D( _Splat0, uv_Splat0 );
			float4 appendResult36_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
			float4 tex2DNode3_g1 = tex2D( _Splat1, uv_Splat1 );
			float4 appendResult39_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
			float4 tex2DNode6_g1 = tex2D( _Splat2, uv_Splat2 );
			float4 appendResult42_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
			float4 tex2DNode7_g1 = tex2D( _Splat3, uv_Splat3 );
			float4 weightedBlendVar9_g1 = temp_output_59_0_g1;
			float4 weightedBlend9_g1 = ( weightedBlendVar9_g1.x*( appendResult33_g1 * tex2DNode4_g1 ) + weightedBlendVar9_g1.y*( appendResult36_g1 * tex2DNode3_g1 ) + weightedBlendVar9_g1.z*( appendResult39_g1 * tex2DNode6_g1 ) + weightedBlendVar9_g1.w*( appendResult42_g1 * tex2DNode7_g1 ) );
			float4 MixDiffuse28_g1 = weightedBlend9_g1;
			float4 temp_output_60_0_g1 = MixDiffuse28_g1;
			float4 localClipHoles100_g1 = ( temp_output_60_0_g1 );
			float2 uv_TerrainHolesTexture = i.uv_texcoord * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
			float holeClipValue99_g1 = tex2D( _TerrainHolesTexture, uv_TerrainHolesTexture ).r;
			float Hole100_g1 = holeClipValue99_g1;
			{
			#ifdef _ALPHATEST_ON
				clip(Hole100_g1 == 0.0f ? -1 : 1);
			#endif
			}
			o.Albedo = localClipHoles100_g1.xyz;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}

	Dependency "BaseMapShader"="ASESampleShaders/SimpleTerrainBase"
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
299;146;1301;693;1227.805;198.0299;1;True;True
Node;AmplifyShaderEditor.FunctionNode;1;-630.5,-263.5;Inherit;False;Four Splats First Pass Terrain;0;;1;37452fdfb732e1443b7e39720d05b708;2,85,0,102,1;7;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;201;FLOAT;0;False;62;FLOAT;0;False;7;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;200;FLOAT;19;FLOAT;17
Node;AmplifyShaderEditor.RangedFloatNode;3;-166.5,-101.5;Inherit;False;Property;_Smoothness;Smoothness;24;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-173.5,-182.5;Inherit;False;Property;_Metallic;Metallic;25;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;4;-817.2911,-18.85504;Inherit;True;Property;_Splat4;Splat 0;26;0;Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;217,-274;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Terrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;-100;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;1;BaseMapShader=ASESampleShaders/SimpleTerrainBase;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;0;0;1;0
WireConnection;0;1;1;14
WireConnection;0;3;2;0
WireConnection;0;4;3;0
WireConnection;0;11;1;17
ASEEND*/
//CHKSM=095714BCDBD2E54CB09D24411CA1F073F9C03A16
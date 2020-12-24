Shader "test/test"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Fade("Fade", Range(0.0,1.0)) = 1.0
		_Tile("Tile", Vector) = (1.0,1.0,1.0,1.0)
	}


		HLSLINCLUDE
#pragma target 4.5
#define MESH_HAS_UV
#define SHADER_CUSTOM_VERTEX customVert
#include "CoreRP/ShaderLibrary/common.hlsl"
#include "HDRP/ShaderVariables.hlsl"
#include "EasyHDRP.hlsl"

			//#include "HDRP/Lighting/ScreenSpaceLighting/ScreenSpaceTracing.hlsl"
			//#include "HDRP/Lighting/ScreenSpaceLighting/ScreenSpaceLighting.cs.hlsl"
//#include "HDRP/Lighting/ScreenSpaceLighting/"


	sampler2D _MainTex;
	float _Fade;
	float4 _Tile;
	v2f customVert(v2f i)
	{
		i.uv.xy *= _Tile.xy;
		return i;
	}
	float4 frag(v2f i) : SV_Target
	{
		float4 col = tex2D(_MainTex, i.uv.xy);
		return col;// *_Fade;
	}

	ENDHLSL

	SubShader
	{
		Tags{ 
			"Queue" = "Geometry" 
			//"LightMode" = "Forward"
		}
		Pass
		{
			Name ""
			Tags{ "LightMode" = "ForwardOnly" }
			Blend Off
			ZWrite off
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
		Pass
		{
			Name ""
			Tags{ "LightMode" = "DepthForwardOnly" }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDHLSL
		}
	}
}
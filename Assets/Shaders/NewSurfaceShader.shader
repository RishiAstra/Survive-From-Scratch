// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/NewSurfaceShader2"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque"}
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma instancing_options procedural:setup
        #pragma multi_compile GPU_FRUSTUM_ON __

        #include "VS_indirect.cginc"
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }

        //struct v2f {
        //    float3 worldPos : TEXCOORD0;
        //    half3 tspace0 : TEXCOORD1;
        //    half3 tspace1 : TEXCOORD2;
        //    half3 tspace2 : TEXCOORD3;
        //    float2 uv : TEXCOORD4;
        //    float4 pos : SV_POSITION;
        //};
        //v2f vert (float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0)
        //{
        //    v2f o;
        //    o.pos = UnityObjectToClipPos(vertex);
        //    o.worldPos = mul(unity_ObjectToWorld, vertex).xyz;
        //    half3 wNormal = UnityObjectToWorldNormal(normal);
        //    half3 wTangent = UnityObjectToWorldDir(tangent.xyz);
        //    half tangentSign = tangent.w * unity_WorldTransformParams.w;
        //    half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
        //    o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
        //    o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
        //    o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
        //    o.uv = uv;
        //    return o;
        //}

        //// textures from shader properties
        ////sampler2D _MainTex;
        //sampler2D _OcclusionMap;
        //sampler2D _BumpMap;
        
        //fixed4 frag (v2f i) : SV_Target
        //{
        //    // same as from previous shader...
        //    half3 tnormal = UnpackNormal(tex2D(_BumpMap, i.uv));
        //    half3 worldNormal;
        //    worldNormal.x = dot(i.tspace0, tnormal);
        //    worldNormal.y = dot(i.tspace1, tnormal);
        //    worldNormal.z = dot(i.tspace2, tnormal);
        //    half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
        //    half3 worldRefl = reflect(-worldViewDir, worldNormal);
        //    half4 skyData = UNITY_SAMPLE_TEXCUBE(unity_SpecCube0, worldRefl);
        //    half3 skyColor = DecodeHDR (skyData, unity_SpecCube0_HDR);                
        //    fixed4 c = 0;
        //    c.rgb = skyColor;

        //    // modulate sky color with the base texture, and the occlusion map
        //    fixed3 baseColor = tex2D(_MainTex, i.uv).rgb;
        //    fixed occlusion = tex2D(_OcclusionMap, i.uv).r;
        //    c.rgb *= baseColor;
        //    c.rgb *= occlusion;

        //    return c;
        //}

        ENDCG
    }
    FallBack "Diffuse"
}

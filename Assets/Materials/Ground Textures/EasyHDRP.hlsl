struct appdata
{
	float4 vertex : POSITION;
#ifdef MESH_HAS_UV
	float4 uv : TEXCOORD0;
#endif
#ifdef MESH_HAS_UV2
	float4 uv2 : TEXCOORD1;
#endif
#ifdef MESH_HAS_UV3
	float4 uv3 : TEXCOORD2;
#endif
#ifdef MESH_HAS_UV4
	float4 uv3 : TEXCOORD3;
#endif
#ifdef MESH_HAS_NORMALS
	float3 normal : NORMAL;
#endif
#ifdef MESH_HAS_TANGENT
	float4 tangent : TANGENT;
#endif
#ifdef MESH_HAS_COLOR
	float4 color : COLOR;
#endif
};
struct v2f
{
	float4 vertex : SV_POSITION;
#ifdef MESH_HAS_UV
	float4 uv : TEXCOORD0;
#endif
#ifdef MESH_HAS_UV2
	float4 uv2 : TEXCOORD1;
#endif
#ifdef MESH_HAS_UV3
	float4 uv3 : TEXCOORD2;
#endif
#ifdef MESH_HAS_UV4
	float4 uv4 : TEXCOORD3;
#endif
#ifdef MESH_HAS_NORMALS
	float3 normal : NORMAL;
#endif
#ifdef MESH_HAS_TANGENT
	float4 tangent : TANGENT;
	float4 bitangent : TEXCOORD5;
#endif
#ifdef MESH_HAS_COLOR
	float4 color : TEXCOORD4;
#endif
};
#ifdef SHADER_CUSTOM_VERTEX
v2f SHADER_CUSTOM_VERTEX(v2f i);
#endif
v2f vert(appdata v)
{
	v2f o;
#ifdef MESH_HAS_UV
	o.uv = v.uv;
#endif
#ifdef MESH_HAS_UV2
	o.uv2 = v.uv2;
#endif
#ifdef MESH_HAS_UV3
	o.uv3 = v.uv3;
#endif
#ifdef MESH_HAS_UV4
	o.uv3 = v.uv3;
#endif
#ifdef MESH_HAS_NORMALS
	o.normal = TransformObjectToWorldDir(v.normal);
#endif
#ifdef MESH_HAS_TANGENT
	o.tangent = float4(TransformObjectToWorldDir(v.tangent.xyz), v.tangent.w);
	o.bitangent = cross(o.normal, o.tangent);
#endif
#ifdef MESH_HAS_COLOR
	o.color = v.color;
#endif
	// Transform local to world before custom vertex code
	o.vertex.xyz = TransformObjectToWorld(v.vertex);
#ifdef SHADER_CUSTOM_VERTEX
	o = SHADER_CUSTOM_VERTEX(o);
#endif
	o.vertex.xyz = GetCameraRelativePositionWS(o.vertex.xyz);
	o.vertex = TransformWorldToHClip(o.vertex.xyz);
	return o;
}
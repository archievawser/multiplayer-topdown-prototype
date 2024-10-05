#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float4x4 mvp;

struct VertexInput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

struct FragmentInput
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
};

FragmentInput VertexMain(VertexInput v)
{
	FragmentInput output;

	output.Position = mul(v.Position, mvp);
	output.TexCoord = v.TexCoord;
	
	return output;
}

float4 FragmentMain(FragmentInput input) : COLOR
{
	return tex2D(SpriteTextureSampler, input.TexCoord);
}

technique SpriteDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexMain();
		PixelShader = compile PS_SHADERMODEL FragmentMain();
	}
};
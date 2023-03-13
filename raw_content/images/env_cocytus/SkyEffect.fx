#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float     GlobalElapsedTime;
Texture2D SkyTexture;
sampler2D SkyTextureSampler = sampler_state {
	Texture = <SkyTexture>;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Normal : NORMAL0;
	float2 UV     : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float3 Normal: NORMAL0;
	float2 UV : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = input.Position;
	output.UV = input.UV;
	output.Color = input.Color;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 UV = input.UV + float2(GlobalElapsedTime * 0.3123, sin(GlobalElapsedTime*0.2) * 0.15);
	UV.x = fmod(UV.x, 1.0);
	float4 Color = input.Color;
	float alpha = clamp((sin(GlobalElapsedTime * 1.85) + 1.0) / 2.0 + 0.1, 0.4, 1.1);
	Color.r = Color.g = Color.b = alpha;
	return tex2D(SkyTextureSampler, UV) * Color;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
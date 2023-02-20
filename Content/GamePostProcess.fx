#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D FramebufferTexture;
float GlobalElapsedTime;
sampler2D FramebufferTextureSampler = sampler_state {
	Texture = <FramebufferTexture>;
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

float RoundToNearestSortOf(float a, int n) {
	int X = a * n;
	float R = X / n;
	return R;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{	
	float2 UV = input.UV;
	UV.x = 1 - UV.x;
	float4 Color = input.Color;

	return tex2D(FramebufferTextureSampler, UV) * Color * 1.1 + 0.010 * sin(GlobalElapsedTime*10+500);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
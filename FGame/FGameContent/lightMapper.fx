sampler TextureSampler : register(s0);
uniform texture2D lightMap;
uniform float2 screenSize;

sampler lightMapSampler = sampler_state
{
	Texture = <lightMap>;
	Filter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

float4 main(float2 texCoord : TEXCOORD0) : COLOR0
{

	return tex2D(TextureSampler, texCoord) * tex2D(lightMapSampler, texCoord);
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}

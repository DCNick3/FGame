sampler TextureSampler : register(s0);
uniform float4 liteSource[5];
uniform float2 screenSize;

//#define USE_ADDITIVE_LIGHT

#define USE_LIGHT_MULTIPLIER

float dist(float2 x, float2 y)
{
	float2 lng = abs(x - y);
	return sqrt(pow(lng.x, 2) + pow(lng.y, 2));
}


float4 main(float2 texCoord : TEXCOORD0) : COLOR0
{
	float2 pixPos = screenSize * texCoord;
	//float3 slctdSource = 0;
	//float l = 100000; // liteSource[0].xy - pixPos;
	float lght = 0;
	[loop]
	for (int i = 0; i < 5; i++)
	{
		if (liteSource[i].z <= 0)
			break;
		float nlght =
#ifdef USE_LIGHT_MULTIPLIER
			liteSource[i].w *
#endif
			(liteSource[i].z - dist(liteSource[i], pixPos)) / liteSource[i].z;

#ifdef USE_ADDITIVE_LIGHT
		lght += nlght;
#else
		if (nlght > lght)
		{
			lght = nlght;
		}
#endif
	}
	//lght /= 32.0;
	//if (lght < 0) lght = 0;
	float ll = tex2D(TextureSampler, texCoord).r;

	float mx = 
#ifdef USE_ADDITIVE_LIGHT
		ll + lght;
#else
		ll > lght ? ll : lght;
#endif
	return float4(mx, mx, mx, 1);
	// float4(1.0,1.0,0.5,0.5);
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 main();
	}
}

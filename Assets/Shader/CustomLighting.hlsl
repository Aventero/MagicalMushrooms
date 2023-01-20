#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

struct CustomLightingData 
{

	// Position and orientation
	float3 normalWS; // WorldSpace

	// Surface attributes
	float3 albedo; // Unlit color
};

// Don't show this in the ShaderGraph Preview mode -> Fixes error
#ifndef SHADERGRAPH_PREVIEW 
float3 CustomLightHandling(CustomLightingData d, Light light)
{
	// radiance is Light strength
	float3 radiance = light.color;
	
	// saturate clamps to 0-1
	// dot product of normal & lightdirection, gives the surface the color or not.
	float3 diffuse = saturate(dot(d.normalWS, light.direction));

	// everything multiplied
	float3 color = d.albedo * radiance * diffuse;

	return color;
}
#endif

float3 CalculateCustomLighting(CustomLightingData d)
{
#ifdef SHADERGRAPH_PREVIEW

	// In preview, estimate diffuse + specular
	float lightDir = float3(0.5, 0.5, 0);
	float intensity = saturate(dot(d.normalWS, lightDir));
	return d.albedo * intensity;
#else
	// Get the main light, this is in URP/ShaderLibrary/Lighting.hlsl
	Light mainLight = GetMainLight();
	float3 color = 0;

	// shade the main light
	color += CustomLightHandling(d, mainLight);
	return color;
#endif
}

// Custom function wrapper
// _float is the precision suffix -> This is called in the Subgraph
// returns a Vector3 Color!
void CalculateCustomLighting_float(float3 Normal, float3 Albedo, out float3 Color) 
{
	// Set the Struct with the given parameters
	CustomLightingData d;
	d.normalWS = Normal;
	d.albedo = Albedo;

	// "Return" it
	Color = CalculateCustomLighting(d);
}

#endif
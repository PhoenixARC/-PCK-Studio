#version 330 core

layout (triangles) in;
layout (triangle_strip, max_vertices=3) out;

uniform vec2 TexSize;

out vec2 o_TexCoord;
out vec2 o_TillingFactor;

in geometryData
{
	vec2 TexCoord;
} dataIn[];

void FixUV()
{
	bool isXBad =
		dataIn[0].TexCoord.x >= TexSize.x &&
		dataIn[1].TexCoord.x >= TexSize.x &&
		dataIn[2].TexCoord.x >= TexSize.x;

	gl_Position = gl_in[0].gl_Position;
	o_TexCoord = dataIn[0].TexCoord;
	if (isXBad)
		o_TexCoord.x = mod(o_TexCoord.x, TexSize.x);
	EmitVertex();
		
	gl_Position = gl_in[1].gl_Position;
	o_TexCoord = dataIn[1].TexCoord;
	if (isXBad)
		o_TexCoord.x = mod(o_TexCoord.x, TexSize.x);
	EmitVertex();
		
	gl_Position = gl_in[2].gl_Position;
	o_TexCoord = dataIn[2].TexCoord;
	if (isXBad)
		o_TexCoord.x = mod(o_TexCoord.x, TexSize.x);
	EmitVertex();
}

void main()
{
	o_TillingFactor = 1.0 / TexSize;
	FixUV();
	EndPrimitive();
};
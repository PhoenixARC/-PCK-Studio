#version 330 core

layout(location = 0) out vec4 color;

uniform sampler2D Texture;

in vec2 o_TillingFactor;
in vec2 o_TexCoord;

void main()
{
	color = texture(Texture, o_TexCoord * o_TillingFactor);
};
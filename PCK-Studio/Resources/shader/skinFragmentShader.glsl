#version 330 core

layout(location = 0) out vec4 color;

uniform sampler2D u_Texture;

in vec2 o_TexScale;
in vec2 o_TexCoord;

void main()
{
	color = texture(u_Texture, o_TexCoord * o_TexScale);
};
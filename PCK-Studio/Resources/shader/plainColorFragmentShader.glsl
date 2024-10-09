#version 330 core

layout(location = 0) out vec4 FragColor;

uniform vec4 BlendColor;
uniform float Intensity;

in vec4 color;

void main()
{
	FragColor = vec4((color * BlendColor).rgb, Intensity);
}
#version 330 core

layout(location = 0) out vec4 FragColor;

uniform vec4 baseColor;
uniform float intensity;

in vec4 color;

void main()
{
	FragColor = vec4((color * baseColor).rgb, intensity);
}
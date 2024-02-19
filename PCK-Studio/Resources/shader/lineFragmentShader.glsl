#version 330 core

layout(location = 0) out vec4 FragColor;

uniform vec4 baseColor;

in vec4 color;

void main()
{
	FragColor = color * baseColor;
}
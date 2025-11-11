#version 330 core

layout(location = 0) out vec4 color;

uniform sampler2D screenTexture;

in vec2 texCoords;

void main()
{
	vec3 texColor = texture(screenTexture, texCoords).rgb;
	color = vec4(texColor, 1.0);
}
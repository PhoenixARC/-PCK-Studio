#version 330 core

layout(location = 0) out vec4 color;

uniform samplerCube skybox;
uniform float brightness;

in vec3 texCoords;

void main()
{
	color = texture(skybox, texCoords) * vec4(vec3(clamp(brightness, 0.0, 1.0)), 1.0);
}
#version 330 core

layout(location = 0) out vec4 color;

uniform samplerCube skybox;

in vec3 texCoords;

void main()
{
	color = texture(skybox, texCoords);
}
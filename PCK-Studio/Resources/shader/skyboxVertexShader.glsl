#version 330 core

layout(location = 0) in vec4 a_Pos;

uniform mat4 viewProjection;

out vec3 texCoords;

void main()
{
	vec4 pos = viewProjection * a_Pos;
	gl_Position = vec4(pos.x, pos.y, pos.ww);
	texCoords = vec3(a_Pos.x, a_Pos.y, -a_Pos.z);
};
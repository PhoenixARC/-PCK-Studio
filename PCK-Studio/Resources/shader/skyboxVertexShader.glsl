#version 330 core

layout(location = 0) in vec4 a_Pos;

uniform mat4 ViewProjection;

out vec3 texCoords;

void main()
{
	vec4 pos = ViewProjection * a_Pos;
	gl_Position = vec4(pos.x, pos.y, pos.ww);
	texCoords = vec3(a_Pos.xy, -a_Pos.z);
};
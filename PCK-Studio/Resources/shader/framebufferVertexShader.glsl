#version 330 core

layout(location = 0) in vec4 a_PosAndTexCoord;

out vec2 texCoords;

void main()
{
	vec2 pos = a_PosAndTexCoord.xy;
	texCoords = a_PosAndTexCoord.zw;
	gl_Position = vec4(pos, 0.0, 1.0);
};
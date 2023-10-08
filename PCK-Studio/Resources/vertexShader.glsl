#version 330 core

layout(location = 0) in vec4 vertexPosition;
layout(location = 1) in vec2 texCoord;

uniform mat4 u_MVP;

out vec2 v_TexCoord;

void main()
{
	gl_Position = u_MVP * vertexPosition;
	v_TexCoord = texCoord;
};
#version 330 core

layout(location = 0) in vec4 vertexPosition;
layout(location = 1) in vec4 color;
layout(location = 2) in vec2 texCoord;

uniform mat4 u_MVP;

out vec2 v_TexCoord;
out vec4 v_VertexPos;
out vec4 v_Color;

void main()
{
	v_TexCoord = texCoord;
	v_Color = color;
	v_VertexPos = vertexPosition;
	gl_Position = u_MVP * vertexPosition;
};
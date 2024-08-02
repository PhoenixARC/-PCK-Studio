#version 330 core

layout(location = 0) in vec4 a_Pos;
layout(location = 1) in vec4 a_Color;

uniform mat4 ViewProjection;
uniform mat4 Transform;

out vec4 color;

void main()
{
	color = a_Color;
	gl_Position = ViewProjection * Transform * vec4(a_Pos.xyz, 1.0);
};
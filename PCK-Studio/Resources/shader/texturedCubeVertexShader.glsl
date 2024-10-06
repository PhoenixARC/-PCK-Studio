#version 330 core

layout(location = 0) in vec3 vertexPosition;
layout(location = 1) in vec2 texCoord;

uniform mat4 ViewProjection;
uniform mat4 Transform;

out geometryData
{
	vec2 TexCoord;
} dataOut;

void main()
{
	dataOut.TexCoord = texCoord;
	gl_Position = ViewProjection * Transform * vec4(vertexPosition, 1.0);
};
#version 330 core

layout(location = 0) in vec4 vertexPosition;
layout(location = 1) in vec2 texCoord;
layout(location = 2) in float scale;

uniform mat4 u_ViewProjection;
uniform mat4 u_Model;

out geometryData
{
	vec2 TexCoord;
} dataOut;

void main()
{
	dataOut.TexCoord = texCoord;
	vec4 scaledVertex = scale * vertexPosition;
	vec4 invertedVertex = vec4(scaledVertex.x, scaledVertex.y * -1.0, scaledVertex.z * -1.0, 1.0);
	gl_Position = u_ViewProjection * u_Model * invertedVertex;
};
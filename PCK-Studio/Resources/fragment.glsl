#version 330 core

layout(location = 0) out vec4 color;

uniform sampler2D u_Texture;

in vec2 v_TexCoord;
in vec4 v_VertexPos;
in vec4 v_Color;

void main()
{
	vec4 texColor = texture(u_Texture, v_TexCoord);
	// color = vec4(v_VertexPos.xyz, 1.0);
	// color = vec4(v_TexCoord, 0.0, 1.0) * vec4(v_VertexPos.xyz, 1.0);
	// color = v_Color;
	// color = vec4(v_TexCoord, 0.0, 1.0);
	color = texColor;
};
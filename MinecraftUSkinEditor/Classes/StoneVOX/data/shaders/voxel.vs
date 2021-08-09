#version 130

in vec3 position;
in float color;

out vec4 _color;

uniform mat4 modelview;
uniform vec3 highlight;
uniform vec3 normal;
uniform float light = 1.0;

uniform vec3 lowclamp = vec3(0,0,0);
uniform vec3 highclamp = vec3(1,1,1);

uniform float alpha = 1.0;

uniform vec3 colors[128];

void main()
{
	vec4 v = vec4(position,1.0);
	gl_Position =  modelview * v;
	vec3 _step = clamp(colors[int(color)]*highlight, lowclamp, highclamp);
	_color = vec4(_step * light,alpha);
}
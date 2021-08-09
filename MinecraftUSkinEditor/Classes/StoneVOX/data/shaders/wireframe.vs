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

uniform vec3 vHSV = vec3(0,0,0);

vec3 rgb2hsv(vec3 c)
{
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 hsv2rgb(vec3 c)
{
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void main()
{
	vec4 v = vec4(position,1.0);
	gl_Position =  modelview * v;

	vec3 fragRGB = colors[int(color)]*light;
	vec3 fragHSV = rgb2hsv(fragRGB);
	float h = vHSV.x / 1.0;
	fragHSV.x *= h;
	fragHSV.yz *= vHSV.yz;
	fragHSV.x = mod(fragHSV.x, 1.0);
	fragHSV.y = clamp(fragHSV.y, 0.0, 1.0);
	fragHSV.z = clamp(fragHSV.z, 0.0, 1.0);
	fragRGB = hsv2rgb(fragHSV);

	_color = vec4(fragRGB,alpha);
}
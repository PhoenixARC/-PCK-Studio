#version 150
in vec2 position;
in vec2 in_uv;
in vec3 in_color;
uniform mat4 mvp;
 
out vec2 uv;
out vec4 color;
 
void main()
{
  gl_Position = mvp *  vec4(position, 1, 1);
  uv = in_uv;
  color = vec4(in_color,1);
}
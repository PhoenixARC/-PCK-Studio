#version 330                                                                       

uniform sampler2D gAOMap;
uniform vec2 gScreenSize;

in vec4 _color;
out vec4 FragColor;  

vec2 CalcScreenTexCoord()
{
    return gl_FragCoord.xy / gScreenSize;
}

void main()
{
    vec4 AmbientColor = vec4(vec3(1,1,1) * 0.6, 1.0f);
    AmbientColor *= texture(gAOMap, CalcScreenTexCoord()).r;
    FragColor = _color * AmbientColor;
}

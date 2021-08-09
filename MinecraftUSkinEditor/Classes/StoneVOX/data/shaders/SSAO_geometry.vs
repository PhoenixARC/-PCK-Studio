#version 330                                                                        
                                                                                    
in vec3 Position;                                             
in float color;

uniform mat4 gWVP;
uniform mat4 gWV;

out vec3 ViewPos;                                        

void main()
{       
    gl_Position = gWVP * vec4(Position, 1.0);
    ViewPos     = (gWV * vec4(Position, 1.0)).xyz;
}

#version 130
 
in vec2 uv;
in vec4 color;

out vec4 colorResult;

void main()
{
    float uvX = 1.0 - uv.st.x;
    float uvY = uv.st.y;

    vec4 white = vec4(1, 1, 1, 1);
    vec4 black = vec4(0, 0, 0, 1);

    colorResult = mix(mix(color, white, uvX), black, uvY);
}

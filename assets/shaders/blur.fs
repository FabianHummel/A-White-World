#version 330

// Input vertex attributes (from vertex shader)
in vec2 fragTexCoord;
in vec4 fragColor;

// Output fragment color
out vec4 finalColor;

uniform sampler2D texture0;
uniform float radius;

uniform int width;
uniform int height;

void main() {
    finalColor = texture(texture0, fragTexCoord);

    int size = int(radius);
    if (size <= 0) { return; }

    finalColor.rgb = vec3(0);

    int count = 0;

    for (int i = -size; i <= size; ++i) {
        for (int j = -size; j <= size; ++j) {
            finalColor.rgb += texture(texture0, (fragTexCoord + vec2(i, j) * 4 / vec2(width, height))).rgb;
            count += 1;
        }
    }

    finalColor.rgb /= float(count);
}
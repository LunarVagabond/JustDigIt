shader_type canvas_item;

uniform vec2 player_screen_pos;
uniform float radius = 0.2;
uniform float softness = 0.05;
uniform float aspect_ratio;

void fragment() {
    vec2 uv = SCREEN_UV;

    // Adjust UV and player position for aspect ratio (make space square)
    vec2 corrected_uv = vec2(uv.x * aspect_ratio, uv.y);
    vec2 corrected_player_pos = vec2(player_screen_pos.x * aspect_ratio, player_screen_pos.y);

    // Also scale radius so it matches corrected space
    float corrected_radius = radius * aspect_ratio;

    // Calculate distance in corrected space
    float dist = distance(corrected_uv, corrected_player_pos);

    // Smoothstep for alpha: transparent inside radius, fading out outside
    float alpha = smoothstep(corrected_radius - softness, corrected_radius, dist);

    COLOR = vec4(0.0, 0.0, 0.0, alpha);
}

using System.Collections.Generic;

namespace ReMod.Core.VRChat
{
    public class VRCConfig
    {
        public List<string> betas;
        public int ps_max_particles;
        public int ps_max_systems;
        public int ps_max_emission;
        public int ps_max_total_emission;
        public int ps_mesh_particle_divider;
        public int ps_mesh_particle_poly_limit;
        public int ps_collision_penalty_high;
        public int ps_collision_penalty_med;
        public int ps_collision_penalty_low;
        public int ps_trails_penalty;
        public int camera_res_height;
        public int camera_res_width;
        public int screenshot_res_height;
        public int screenshot_res_width;
        public int dynamic_bone_max_affected_transform_count;
        public int dynamic_bone_max_collider_check_count;
        public string cache_directory;
        public int cache_size;
        public int cache_expiry_delay;
        public bool disableRichPresence;
    }
}

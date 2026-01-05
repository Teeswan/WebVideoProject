using Youtube_Entertainment_Project.Data.Entity;

namespace Youtube_Entertainment_Project.DTOs
{
    public class SuperAdminDashboardDto
    {
        public int TotalAdmins { get; set; }
        public int TotalUsers { get; set; }
        public List<AppUser> AdminUsers { get; set; }
    }
}

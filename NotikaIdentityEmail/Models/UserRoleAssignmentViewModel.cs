
namespace NotikaIdentityEmail.Models
{
    public class UserRoleAssignmentViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<RoleAssignViewModel> Roles { get; set; } = new List<RoleAssignViewModel>();
    }
}

using Microsoft.AspNetCore.Identity;
namespace Crm.Domain.Entities;
/// <summary>
/// Thực thể User định danh, sử dụng khóa chính GUID/UUID.
/// Dữ liệu nghiệp vụ chi tiết được quản lý tại UserProfile.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid();
    }
}

using PhungNoiProject.Models;

namespace PhungNoiProject.Services
{
    public interface IAuthentication
    {
        bool CreateUserAndAccount(Member user);
        bool Login(LoginModel model);
        bool Logout();
        void AddUserToRole(string userName, string rolesName);
        void DeleteUserAccount(Member user);
    }
}

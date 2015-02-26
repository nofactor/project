﻿using PhungNoiProject.Models;
using System;
using System.Linq;
using System.Web.Security;
using WebMatrix.WebData;

namespace PhungNoiProject.Services
{
    public class Authentication : IAuthentication 
    {

        public phungnoiDBEntities db = new phungnoiDBEntities();
        public bool Login(LoginModel model)
        {
            if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                try
                {
                    Member user = db.Member.FirstOrDefault(x => x.UserName == model.UserName);
                    user.Userpassword = model.Password;
                    user.lastSignInDate = DateTime.Now;
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    
                }

                return true;
            }
            return false;
        }

        public bool Logout()
        {
            WebSecurity.Logout();
            return true;
        }

        public bool CreateUserAndAccount(Member user)
        {
            WebSecurity.CreateUserAndAccount(user.UserName, user.Userpassword);
            return true;
        }
        public void AddUserToRole(string userName, string rolesName)
        {
            Roles.AddUserToRole(userName, rolesName);
        }


        public void DeleteUserAccount(Member user)
        {
            string [] roles = Roles.GetRolesForUser(user.UserName);
            if(roles.Count() > 0)
                Roles.RemoveUserFromRoles(user.UserName,roles);

            ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(user.UserName); // deletes record from webpages_Membership table
            ((SimpleMembershipProvider)Membership.Provider).DeleteUser(user.UserName, true);
        }
    }
}
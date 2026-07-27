
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AskalePortal.BLL;
using Microsoft.AspNetCore.Authorization;
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AutoMapper;



namespace AskalePortal.API.Security
{
    public static class TokenHandler
    {
      
        public static Token CreateToken(IConfiguration configuration,IWebHostEnvironment env,IMapper mapper, string? username, string? password, string? ip)
        {
            if(username!=null && password!=null && ip != null)
            {
                Token token;
                List<Claim> listClaims;

                BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(configuration, env,mapper);
                BLLActions.RoleDetails bllRoleDetail = new BLLActions.RoleDetails(configuration, env, mapper);
                AdminUser? adminUser = bllAdminUser.getUser(username, password);
                if (adminUser == null)
                {
                    BLLActions.LoginLogs bllLoginLogs = new BLLActions.LoginLogs(configuration, env);
                    LoginLog loginlogs = new LoginLog();
                    loginlogs.iP=(ip); loginlogs.enabled = true;
                    loginlogs.createdDate = DateTime.Now;
                    loginlogs.createdUserId = 0;
                    loginlogs.username=(username);
                    loginlogs.isSuccess=(false);
                    bllLoginLogs.Add(loginlogs);
                    token = new() { AccessToken = "kullanıcı yok", RefreshToken = "" };

                    return token;
                }
                else
                {
                    listClaims = new List<Claim>();
                    List<RoleDetail> roleDetails = bllRoleDetail.GetByRoleID(adminUser.roleId);
                    foreach (var item in roleDetails)
                    {
                        if (item.canAdd)
                        {
                            listClaims.Add(new Claim(ClaimTypes.Role, "ROLE_" + item.moduleId.ToString() + "_" + "ADD"));
                        }
                        if (item.canDelete)
                        {
                            listClaims.Add(new Claim(ClaimTypes.Role, "ROLE_" + item.moduleId.ToString() + "_" + "DELETE"));
                        }
                        if (item.canEdit)
                        {
                            listClaims.Add(new Claim(ClaimTypes.Role, "ROLE_" + item.moduleId.ToString() + "_" + "EDIT"));
                        }
                        if (item.canSee)
                        {
                            listClaims.Add(new Claim(ClaimTypes.Role, "ROLE_" + item.moduleId.ToString() + "_" + "SEE"));
                        }
                        if (item.canSeeLogs)
                        {
                            listClaims.Add(new Claim(ClaimTypes.Role, "ROLE_" + item.moduleId.ToString() + "_" + "LOGS"));
                        }
                    }
                    listClaims.Add(new Claim("userId", adminUser.Id.ToString()));
                    listClaims.Add(new Claim("name", adminUser.name));


                    DateTime expirationTime = DateTime.Now.AddDays(Convert.ToInt16(configuration["Token:Expiration"]));

                    SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]!));
                    SigningCredentials signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha512);

                    JwtSecurityToken tokenSecurityToken = new JwtSecurityToken(issuer: configuration["Token:Issuer"], audience: configuration["Token:Audience"], expires: expirationTime, signingCredentials: signingCredentials, notBefore: DateTime.Now, claims: listClaims.AsEnumerable());
                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();


                    byte[] numbers = new byte[32];
                    using RandomNumberGenerator random = RandomNumberGenerator.Create();
                    random.GetBytes(numbers);

                    token = new()
                    {
                        AccessToken = tokenHandler.WriteToken(tokenSecurityToken),
                        RefreshToken = Convert.ToBase64String(numbers),

                    };
                    token.Expiration = expirationTime;
                    BLLActions.LoginLogs bllLoginLogs = new BLLActions.LoginLogs(configuration, env);
                    LoginLog loginlogs = new LoginLog();
                    loginlogs.iP=(ip);
                    loginlogs.enabled = true;
                    loginlogs.createdDate= DateTime.Now;
                    loginlogs.createdUserId = adminUser.Id;
                    loginlogs.username=(username);
                    loginlogs.isSuccess=(true);
                    bllLoginLogs.Add(loginlogs);

                    return token;



                }
            }
            else
            {
                Token token = new() { AccessToken = "kullanıcı yok", RefreshToken = "" };
                return token;
            }

            

        }
    }
}

using AutoMapper;
using DB.EFModel;
using DB.Entity;
using DB.Migrations.Helpers;
using DB.Model;
using DB.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net;
using System.Net.Mail;

namespace DB.Repositories
{
    public class UserRepository : RepositoryBase<User, UserDTO>, IUserRepository
    {
        public UserRepository(ProcuraDbContext context, IMapper mapper,IHttpContextAccessor httpContextAccessor) : base(context, mapper, httpContextAccessor) { }
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            }
            catch (Exception ex)
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            }
        }
        public User GetUserByUsernameAsync(string username, bool includeInactive = false)
        {




            return SelectUserAsUserObject(
                QueryUsers(includeInactive).Where(x => x.StaffId == username.ToLower())
                ).FirstOrDefault();
        }


        private IQueryable<User> SelectUserAsUserObject(IQueryable<EFModel.User> query)
        {
            return query.Select(u => new User
            {
                Id = u.Id,
                EmailAddress = u.EmailAddress,
                FullName = u.FullName,
                StaffId = u.StaffId,
                SiteOffice=u.SiteOffice,
                SiteLevelId=u.SiteLevelId,
                UserName = u.UserName,
                //CommunityId=u.CommunityId,
                RoleId =u.RoleId,

            });
        }
        private IQueryable<EFModel.User> QueryUsers(bool includeInactive)
        {
            return _context.Users;
        }
        public void LogAuthAttempt(string username, string ip, string response, DateTime? jwtExpiryDate, bool isOnline)
        {
            var loginHistory = new EFModel.LoginHistory
            {
                Date = DateTime.Now,
                Ip = ip,
                UserName = username,
                Response = response,
                RecaptchaScore = "0",
                JwtTokenExpiryDate = jwtExpiryDate,
                Online = isOnline
            };
            _context.LoginHistories.Add(loginHistory);
            _context.SaveChanges();
        }

        public async Task<bool> CheckPassword(string userName,string Password,int roleId)
        {
            if (roleId == 0)
            {
                var user = await _context.Users.Where(x => x.Password == Password && x.StaffId==userName).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception("Wrong username or password");
                }
                if (user != null)
                    return true;
                else
                    return false;
            }
            else
            {
                var user = await _context.Vendors.Where(x => x.PasswordHash == Password && x.RocNumber == userName).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new Exception("Wrong username or password");
                }
                if (user != null)
                    return true;
                else
                    return false;
            }
                

        }

        
        public async Task SendResetEmailAsync(string toEmail, string residentFullName, string tempPassword, string residentPageUrl, string community)
        {
            var fromEmail = "absec.demo@gmail.com";
            var subject = $"CSA {community} | Password reset request";
            var body = EmailHelper.GetResetPasswordEmailBody(residentFullName, toEmail, tempPassword, residentPageUrl, community);
            using var client = new SmtpClient("smtp.gmail.com") // Replace with your SMTP
            {
                Port = 587,
                Credentials = new NetworkCredential("absec.demo@gmail.com", "qhogkbdwobdoznyx"),
                EnableSsl = true,
            };
            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "CSA Team"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            try
            {
                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                //throw(ex);
            }
        }


        public int? RoleForUser(int userId)
        {
            return _context.Users.Include(x=>x.Role).Where(x => x.Id == userId).Select(x => x.RoleId).FirstOrDefault();
        }

        //public async Task<RoleDTO?> GetRoleAsync(int userId) // Nullable return type
        //{
        //    return await _context.Users
        //        .Where(c => c.Id == userId) // Filter first for efficiency
        //        .Select(c => new RoleDTO
        //        {
        //            Id = c.Id,
        //            Name = c.Name ?? "" // Ensure Name is not null
        //        })
        //        .FirstOrDefaultAsync();
        //}

        public async Task<IEnumerable<UserDTO>> GetUserListAsync()
        {
            return await _context.Users.Include(x=>x.State).Include(x=>x.SiteLevel)
                .Select(c => new UserDTO
                {
                    Id = c.Id,
                    RoleName = c.Role == null ? "" : c.Role.Name,
                    FirstName = c.FullName,
                    LastName = c.FullName,
                    RoleId = c.RoleId,
                    Status = c.IsActive==true?"Active":"InActive",
                    Email = c.EmailAddress,
                    //LastLogin = c.la,
                    UserName = c.UserName,
                    SiteLevel=c.SiteLevel,
                    SiteOffice=c.State,
                    IsEvaluationCommittee=c.IsEvaluationCommittee,
                    IsNegotiationCommittee=c.IsNegotiationCommittee,
                    IsOpeningCommittee=c.IsOpeningCommittee

                })
                .ToListAsync();

        }

        public async Task<IEnumerable<UserDTO>> GetBidderUserListAsync(int roleId)
        {
            return await _context.Users.Where(x=>x.RoleId== roleId).Include(x => x.State).Include(x => x.SiteLevel)
                .Select(c => new UserDTO
                {
                    Id = c.Id,
                    RoleName = c.Role == null ? "" : c.Role.Name,
                    FirstName = c.FullName,
                    LastName = c.FullName,
                    RoleId = c.RoleId,
                    Status = c.IsActive == true ? "Active" : "InActive",
                    Email = c.EmailAddress,
                    //LastLogin = c.la,
                    UserName = c.UserName,
                    SiteLevel = c.SiteLevel,
                    SiteOffice = c.State,
                    IsEvaluationCommittee = c.IsEvaluationCommittee,
                    IsNegotiationCommittee = c.IsNegotiationCommittee,
                    IsOpeningCommittee = c.IsOpeningCommittee

                })
                .ToListAsync();

        }

        public async Task<UserDTO> SaveUserAsync(UserDTO user)
        {
            if (StaffExist(user.StaffId))
            {
                throw new Exception("User Already Exist with Same StaffId");
            }

            user.Name = user.FullName;
            var entity = _mapper.Map<EFModel.User>(user);
            entity.EmailAddress = user.Email;
            entity.MobileNo = user.Mobile;
            entity.SiteOffice = user.SiteOfficeId.Value;
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            var password = EmailHelper.GenerateRandomPassword();
            var userDetails = _context.Users.Where(x => x.Id == entity.Id).FirstOrDefault();
            if (userDetails != null)
            {
                await SendWelcomeEmailAsync(
                            toEmail: user.Email ?? "",
                            residentFullName: user.Name ?? "",
                            tempPassword: password,
                            residentPageUrl: "http://103.27.86.226/Procura",
                            userDetails.FullName,
                            userDetails.StaffId
                        );
            }

            return await GetByIdAsync(entity.Id);
        }

        public async Task SendWelcomeEmailAsync(
    string toEmail, string residentFullName, string tempPassword,
    string residentPageUrl, string community, string userName)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var fromEmail = "absec.demo@gmail.com";

            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, "qhogkbdwobdoznyx")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, "Procura Team"),
                Subject = "Welcome to Procura - Your Account Has Been Created",
                Body = EmailHelper.GetWelcomeEmailBody(residentFullName, userName, tempPassword, residentPageUrl, community),
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email Error: " + ex.Message);
                throw;
            }
        }


        private bool userExist(string? userName)
        {
            return _context.Users.Any(u => u.UserName == userName);
        }

        private bool StaffExist(string? staffId)
        {
            return _context.Users.Any(u => u.StaffId == staffId);
        }

        public async Task UpdateUserAsync(int userId, UserDTO user)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(c => c.Id == userId);
            if (entity != null)
            {
                entity.FullName = user.FullName;
                entity.EmailAddress = user.Email;
                entity.MobileNo = user.Mobile;
                entity.SiteLevelId = user.SiteLevelId.Value;
                entity.RoleId = user.RoleId.Value;
                entity.SiteOffice = user.SiteOfficeId.Value;
                entity.DesignationId = user.DesignationId;
                entity.IsOpeningCommittee = user.IsOpeningCommittee;
                entity.IsEvaluationCommittee = user.IsEvaluationCommittee;
                entity.IsNegotiationCommittee = user.IsNegotiationCommittee;
            }
            await _context.SaveChangesAsync();
        }
        

        //private string IsValidPassword(int userId, string password)
        //{
        //    bool isValid = true;
        //    string validationError = string.Empty;
        //    PasswordPolicyModel policy = new PasswordPolicyModel();


        //    var passwordRules = _context.PasswordPolicy.ToList();
        //    foreach (var rule in passwordRules)
        //    {
        //        switch (rule.Code)
        //        {
        //            case "MIN_PWD_LENGTH":
        //                policy.MinimumPasswordLength = Convert.ToInt32(rule.Value);
        //                break;
        //            case "MIN_NUM_CHAR_PWD":
        //                policy.MinimumNumericCharacters = Convert.ToInt32(rule.Value);
        //                break;
        //            case "MIN_ALPHA_CHAR_PWD":
        //                policy.MinimumAlphaCharacters = Convert.ToInt32(rule.Value);
        //                break;
        //            case "MIN_UPER_CHAR_IN_PWD":
        //                policy.MinimumUppercaseCharacters = Convert.ToInt32(rule.Value);
        //                break;
        //            case "MIN_LOWER_CHAR_IN_PWD":
        //                policy.MinimumLowercaseCharacters = Convert.ToInt32(rule.Value);
        //                break;
        //            case "PWD_HISTORY":
        //                policy.PassWordHistory = Convert.ToInt32(rule.Value);
        //                break;
        //        }
        //    }

        //    int alp, digit, splch, i, l;
        //    alp = digit = splch = i = 0;

        //    l = password.Length;

        //    while (i < l)
        //    {
        //        if ((password[i] >= 'a' && password[i] <= 'z') || (password[i] >= 'A' && password[i] <= 'Z'))
        //        {
        //            alp++;
        //        }
        //        else if (password[i] >= '0' && password[i] <= '9')
        //        {
        //            digit++;
        //        }
        //        else
        //        {
        //            splch++;
        //        }

        //        i++;
        //    }

        //    if (policy.MinimumPasswordLength > l || policy.MinimumNumericCharacters > digit || policy.MinimumAlphaCharacters > alp)
        //    {
        //        isValid = false;
        //        validationError = $"Passsword length should be at least {policy.MinimumPasswordLength} letters and contain at least {policy.MinimumNumericCharacters} numbers , {policy.MinimumAlphaCharacters} characters.";
        //    }
        //    if (isValid && IsPreviousPassword(userId, password, policy.PassWordHistory))
        //    {
        //        validationError = "Password is Already used.";
        //    }

        //    return validationError;

        //}
        //private bool IsPreviousPassword(int userId, string password, int noOfPasswords)
        //{
        //    var user = _context.Users.FirstOrDefault(x => x.Id == userId);
        //    if (user != null && user.Password == password)
        //    {
        //        return true;
        //    }
        //    ///var previousPasswords = _context.PasswordHistory.Where(x => x.USER_ID == userId)?.OrderByDescending(x => x.CHANGED_DATE)?.ToList()?.Take(noOfPasswords);
        //    return previousPasswords != null && previousPasswords.Any(x => x.PASSWORD == password);

        //}


        public async Task<bool> IsPasswordReusedAsync(int userId, string newHash, int historyCount)
        {
            return await _context.UserPasswordHistories
                .Where(x => x.ResidentId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .Take(historyCount)
                .AnyAsync(x => x.PasswordHash == newHash);
        }
        public async Task SaveHistoryAsync(int userId, string passwordHash, int historyCount)
        {
            _context.UserPasswordHistories.Add(new UserPasswordHistory
            {
                ResidentId = userId,
                PasswordHash = passwordHash,
                CreatedDate = DateTime.UtcNow
            });

            var oldPasswords = await _context.UserPasswordHistories
                .Where(x => x.ResidentId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .Skip(historyCount)
                .ToListAsync();

            _context.UserPasswordHistories.RemoveRange(oldPasswords);

            await _context.SaveChangesAsync();
        }


        private void ValidatePassword(string password)
        {

            var passwordPolicy= _context.passwordPolicies.ToList();

            var policy = new
            {
                MinPasswordLength = _context.passwordPolicies.Where(x=>x.Code== "MIN_PWD_LENGTH").Select(x=>x.Value).FirstOrDefault(),
                MinNumericChars = _context.passwordPolicies.Where(x => x.Code == "MIN_NUM_CHAR_PWD").Select(x => x.Value).FirstOrDefault(),
                MinAlphaChars = _context.passwordPolicies.Where(x => x.Code == "MIN_ALPHA_CHAR_PWD").Select(x => x.Value).FirstOrDefault(),
            };
            if (password.Length <Convert.ToInt32(policy.MinPasswordLength))
                throw new Exception("Password atleast "+ policy.MinPasswordLength+" characters length");

            if (password.Count(char.IsDigit) < Convert.ToInt32(policy.MinNumericChars))
                throw new Exception("Password should contains atleast " + policy.MinNumericChars + " number");

            if (password.Count(char.IsLetter) < Convert.ToInt32(policy.MinAlphaChars))
                throw new Exception("Password should contains atleast " + policy.MinAlphaChars + " character");
        }




        public async Task<IEnumerable<UserDTO>> GetUserListAsync(int? siteLevelId, int? siteOfficeId, bool? status)
        {
            var query = _context.Users
                .Include(x => x.State)
                .Include(x => x.SiteLevel)
                .AsQueryable();

            // Site Level filter
            if (siteLevelId.HasValue && siteLevelId!=0)
            {
                query = query.Where(x => x.SiteLevelId == siteLevelId);
            }

            // Site Office filter
            if (siteOfficeId.HasValue && siteOfficeId !=0)
            {
                query = query.Where(x => x.SiteOffice == siteOfficeId);
            }

            // Status filter
            if (status.HasValue)
            {
                query = query.Where(x => x.IsActive == status);
            }

            return await query
                .Select(c => new UserDTO
                {
                    Id = c.Id,
                    RoleName = c.Role == null ? "" : c.Role.Name,
                    FirstName = c.FullName,
                    LastName = c.FullName,
                    RoleId = c.RoleId,
                    Status = c.IsActive ? "Active" : "InActive",
                    Email = c.EmailAddress,
                    UserName = c.UserName,
                    SiteLevel = c.SiteLevel,
                    SiteOffice = c.State,
                    IsEvaluationCommittee = c.IsEvaluationCommittee,
                    IsNegotiationCommittee = c.IsNegotiationCommittee,
                    IsOpeningCommittee = c.IsOpeningCommittee
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<UserDTO>> GetBidderUserListAsync(int? siteLevelId, int? siteOfficeId, bool? status)
        {
            var query = _context.Users
                .Where(x => x.RoleId == 5)
                .Include(x => x.State)
                .Include(x => x.SiteLevel)
                .AsQueryable();

            if (siteLevelId.HasValue)
            {
                query = query.Where(x => x.SiteLevelId == siteLevelId);
            }

            if (siteOfficeId.HasValue)
            {
                query = query.Where(x => x.SiteOffice == siteOfficeId);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.IsActive == status);
            }

            return await query
                .Select(c => new UserDTO
                {
                    Id = c.Id,
                    RoleName = c.Role == null ? "" : c.Role.Name,
                    FirstName = c.FullName,
                    LastName = c.FullName,
                    RoleId = c.RoleId,
                    Status = c.IsActive ? "Active" : "InActive",
                    Email = c.EmailAddress,
                    UserName = c.UserName,
                    SiteLevel = c.SiteLevel,
                    SiteOffice = c.State,
                    IsEvaluationCommittee = c.IsEvaluationCommittee,
                    IsNegotiationCommittee = c.IsNegotiationCommittee,
                    IsOpeningCommittee = c.IsOpeningCommittee
                })
                .ToListAsync();
        }

    }
}

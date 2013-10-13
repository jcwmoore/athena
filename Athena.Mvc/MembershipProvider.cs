using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Data.Entity
{
    public abstract class MembershipContext : DbContext
    {
        public MembershipContext(System.Data.Common.DbConnection conn)
            : base(conn, true)
        {
        }

        public DbSet<DbUser> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DbUserConfiguration());
        }

        private class DbUserConfiguration : ModelConfiguration.EntityTypeConfiguration<DbUser>
        {
            public DbUserConfiguration()
            {
                this.ToTable("User");
                this.HasKey(k => k.UserId);

                this.Property(p => p.CreatedBy).HasMaxLength(255).HasColumnName("CreatedBy").IsRequired();
                this.Property(p => p.CreatedOn).HasColumnName("CreatedOnUtc").IsRequired();
                this.Property(p => p.EmailAddress).HasMaxLength(255).HasColumnName("EmailAddress").IsRequired();
                this.Property(p => p.IsApproved).HasColumnName("IsApproved").IsRequired();
                this.Property(p => p.IsLockedOut).HasColumnName("IsLockedOut").IsRequired();
                this.Property(p => p.LastLogin).HasColumnName("LastLoginUtc").IsRequired();
                this.Property(p => p.LastSeen).HasColumnName("LastSeenUtc").IsRequired();
                this.Property(p => p.LockedOutOn).HasColumnName("LockedOutOnUtc").IsRequired();
                this.Property(p => p.Password).HasMaxLength(2000).HasColumnName("Password").IsRequired();
                this.Property(p => p.PasswordAnswer).HasMaxLength(2000).HasColumnName("PasswordAnswer");
                this.Property(p => p.PasswordQuestion).HasMaxLength(2000).HasColumnName("PasswordQuestion");
                this.Property(p => p.PasswordChangeDate).HasColumnName("PasswordChangeDateUtc").IsRequired();
                this.Property(p => p.PasswordExpireDate).HasColumnName("PasswordExpireDateUtc").IsRequired();
                this.Property(p => p.UpdatedBy).HasMaxLength(255).HasColumnName("UpdatedBy").IsRequired();
                this.Property(p => p.UpdatedOn).HasColumnName("UpdatedOnUtc").IsRequired();
                this.Property(p => p.UserGuid).HasColumnName("UserGuid").IsRequired();
                this.Property(p => p.UserId).HasColumnName("UserId").IsRequired();
                this.Property(p => p.UserName).HasColumnName("UserName").IsRequired();

            }
        }
    }

    public class DbUser
    {
        public int UserId { get; set; }
        public Guid UserGuid { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public DateTime PasswordChangeDate { get; set; }
        public DateTime PasswordExpireDate { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime LastSeen { get; set; }
        public string EmailAddress { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LockedOutOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class MembershipProvider : System.Web.Security.MembershipProvider
    {
        private const string PROVIDER_NAME = "My Custom Provider";
        private const string DEFAULT_PASSWORD = "Password,123";
        private readonly IoC.Container _container;

        // TODO: you will need to provide an IOC container
        public MembershipProvider() //: this(null)
        {
        }

        public MembershipProvider(IoC.Container container)
        {
            _container = container;
        }

        public override string ApplicationName { get; set; }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var oldHash = GetHash(oldPassword);
            var newHash = GetHash(newPassword);
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == username && u.Password == oldHash);
                if (user != null)
                {
                    user.Password = newHash;
                    ctx.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            var hash = GetHash(password);
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == username && u.Password == hash);
                if (user != null)
                {
                    user.PasswordAnswer = GetHash(newPasswordAnswer);
                    user.PasswordQuestion = newPasswordQuestion;
                    ctx.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            System.Web.Security.MembershipUser result = null;
            if (providerUserKey is Guid)
            {
                var id = (Guid)providerUserKey;
                using (var ctx = _container.Resolve<MembershipContext>())
                {
                    var user = ctx.Users.FirstOrDefault(u => u.UserName == username);
                    if (user != null)
                    {
                        user = new DbUser();
                        user.CreatedBy = username;
                        user.CreatedOn = DateTime.UtcNow;
                        user.EmailAddress = email;
                        user.IsApproved = isApproved;
                        user.IsLockedOut = false;
                        user.LastLogin = DateTime.UtcNow;
                        user.LastSeen = DateTime.UtcNow;
                        user.LockedOutOn = DateTime.UtcNow;
                        user.Password = GetHash(password);
                        user.PasswordAnswer = GetHash(passwordAnswer);
                        user.PasswordChangeDate = DateTime.UtcNow;
                        user.PasswordExpireDate = DateTime.UtcNow.AddYears(100);
                        user.PasswordQuestion = passwordQuestion;
                        user.UpdatedBy = username;
                        user.UserGuid = id;
                        user.UpdatedOn = DateTime.UtcNow;
                        user.UserName = username;
                        ctx.Users.Add(user);
                        ctx.SaveChanges();
                        status = Web.Security.MembershipCreateStatus.Success;
                        result = new Web.Security.MembershipUser(PROVIDER_NAME, username, id, email, passwordQuestion, string.Empty, isApproved, false, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);
                    }
                    else
                    {
                        status = System.Web.Security.MembershipCreateStatus.DuplicateUserName;
                    }
                }
            }
            else
            {
                status = Web.Security.MembershipCreateStatus.InvalidProviderUserKey;
            }
            return result;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    ctx.Users.Remove(user);
                    ctx.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        private System.Web.Security.MembershipUser Map(DbUser record)
        {
            return new Web.Security.MembershipUser(PROVIDER_NAME, record.UserName, record.UserGuid, record.EmailAddress, record.PasswordQuestion, string.Empty, record.IsApproved, record.IsLockedOut, record.CreatedOn, record.LastLogin, record.LastSeen, record.PasswordChangeDate, record.LockedOutOn);
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var result = new System.Web.Security.MembershipUserCollection();
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                totalRecords = ctx.Users.Count();
                var records =
                    from u in ctx.Users
                    where u.EmailAddress == emailToMatch
                    select u;
                foreach (var record in records.Skip(pageIndex * pageSize).Take(pageSize))
                {
                    result.Add(Map(record));
                }
            }
            return result;
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var result = new System.Web.Security.MembershipUserCollection();
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                totalRecords = ctx.Users.Count();
                var records =
                    from u in ctx.Users
                    where u.UserName == usernameToMatch
                    select u;
                foreach (var record in records.Skip(pageIndex * pageSize).Take(pageSize))
                {
                    result.Add(Map(record));
                }
            }
            return result;
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var result = new System.Web.Security.MembershipUserCollection();
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                totalRecords = ctx.Users.Count();
                var records =
                    from u in ctx.Users
                    select u;
                foreach (var record in records.Skip(pageIndex * pageSize).Take(pageSize))
                {
                    result.Add(Map(record));
                }
            }
            return result;
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    return Map(user);
                }
                return null;
            }
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                if (providerUserKey is Guid)
                {
                    var id = (Guid)providerUserKey;
                    var user = ctx.Users.FirstOrDefault(u => u.UserGuid == id);
                    if (user != null)
                    {
                        return Map(user);
                    }
                }
                return null;
            }
        }

        public override string GetUserNameByEmail(string email)
        {
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var user = ctx.Users.FirstOrDefault(u => u.EmailAddress == email);
                if (user != null)
                {
                    return user.UserName;
                }
                return null;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 5; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 5; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 10; }
        }

        public override System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get { return Web.Security.MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return false; }
        }

        public override string ResetPassword(string username, string answer)
        {
            var hash = GetHash(answer);
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == username);
                if (user != null)
                {
                    user.PasswordAnswer = GetHash(DEFAULT_PASSWORD);
                    ctx.SaveChanges();
                    return DEFAULT_PASSWORD;
                }
                return null;
            }
        }

        public override bool UnlockUser(string userName)
        {
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == userName);
                if (user != null)
                {
                    user.IsLockedOut = false;
                    ctx.SaveChanges();
                    return true;
                }
                return false;
            }
        }

        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var update = ctx.Users.FirstOrDefault(u => u.UserName == user.UserName);
                if (update == null)
                {
                    update.UpdatedBy = user.UserName;
                    update.UpdatedOn = DateTime.UtcNow;
                    update.EmailAddress = user.Email;
                    update.PasswordQuestion = user.PasswordQuestion;
                    ctx.SaveChanges();
                }
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            using (var ctx = _container.Resolve<MembershipContext>())
            {
                var user = ctx.Users.FirstOrDefault(u => u.UserName == username);
                if (user == null)
                {
                    return true;
                }
                return false;
            }
        }

        public static string GetHash(string value)
        {
            var md5Hasher = SHA1.Create(PROVIDER_NAME);
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}

using OAuth.Domain.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace OAuth.Data.Mapping
{
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("OP_Users")
                .HasKey(c => c.Id);

            Property(c => c.Id).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Id).HasColumnName("UserID_int");
            Property(t => t.UserName).HasColumnName("User_nvarchar");
            Property(t => t.Password).HasColumnName("Password_nvarchar");
            Property(t => t.PhoneNumber).HasColumnName("Phone_nvarchar");
            Property(t => t.FullName).HasColumnName("FullName_nvarchar");
            Property(t => t.Status).HasColumnName("Status_tinyint");
            Property(t => t.AddDate).HasColumnName("addtime_datetime");
            Property(t => t.LastLogonDate).HasColumnName("LastAccess_datetime");
            Property(t => t.DigitalCertificate).HasColumnName("DigitalCertificate_nvarchar");

            //关系一对多，一个用户可拥有多个角色
            HasMany<UserRole>(u => u.UserRoles).WithRequired();

            //关系一对多，一个用户可拥有多个项目
            HasMany<UserProject>(u => u.UserProjects).WithRequired();
        }
    }
}

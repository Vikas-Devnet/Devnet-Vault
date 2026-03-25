using Domain.Common.Models;

namespace Domain.Entities.Account;

public class UserKey : AuditProperty
{
    public int Id { get; set; }
    public Guid UserId { get; set; }

    public byte[] PrimaryEncryptedDK { get; set; } = null!;
    public byte[] PrimaryDK_IV { get; set; } = null!;
    public byte[] PrimaryDK_Tag { get; set; } = null!;

    public byte[] KdfSalt { get; set; } = null!;

    public byte[] BackupEncryptedDK { get; set; } = null!;
    public byte[] BackupDK_IV { get; set; } = null!;
    public byte[] BackupDK_Tag { get; set; } = null!;
    public byte[] BackupKdfSalt { get; set; } = null!;
    public UserMaster UserMaster { get; set; } = null!;
}

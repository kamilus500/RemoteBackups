using System;

namespace RemoteBackups.Api.Entities
{
    public class AuditLog
    {
        public Guid Id { get; private set; }
        public string Action { get; private set; }
        public DateTime Timestamp { get; private set; }

        public Guid UserId { get; private set; }
        public string Login { get; private set; }

        public AuditLog()
        {

        }

        public AuditLog(string action, Guid userId,  string login)
        {
            Id = Guid.NewGuid();
            Action = action;
            Timestamp = DateTime.UtcNow;
            UserId = userId;
            Login = login;
        }
    }
}
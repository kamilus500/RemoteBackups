namespace RemoteBackups.Api.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Login { get; private set; }
        public string HashedPassword { get; private set; }

        public virtual List<FileMetaData> Files { get; set; }

        public User()
        {

        }

        public User(string login, string hashedPassword)
        {
            Id = Guid.NewGuid();
            Login = login;
            HashedPassword = hashedPassword;
        }

        public string GetLogin()
            => $"{Login}";
    }
}

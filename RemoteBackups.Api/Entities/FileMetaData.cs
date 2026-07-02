namespace RemoteBackups.Api.Entities
{
    public class FileMetaData
    {
        public Guid Id { get; private set; }
        public string OriginalName { get; private set; }
        public string ContentType { get; private set; }
        public DateTime UploadAt { get; private set; }

        public string Path { get; private set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; }

        public FileMetaData()
        {
            
        }

        public FileMetaData(string orginalName, string contentType)
        {
            OriginalName = orginalName;
            ContentType = contentType;
            UploadAt = DateTime.UtcNow;
        }

        public void SetPath(string path)
        {
            Path = path;
        }
    }
}

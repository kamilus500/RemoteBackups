namespace RemoteBackups.Api.Entities
{
    public class FileMetaData
    {
        public Guid Id { get; private set; }
        public string OriginalName { get; private set; }
        public string ContentType { get; private set; }
        public long SizeInBytes { get; private set; }
        public DateTime UploadAt { get; private set; }
        public string Path { get; private set; }

        public Guid UserId { get; private set; }
        public virtual User User { get; private set; }

        protected FileMetaData() { }

        public FileMetaData(string originalName, string contentType, long sizeInBytes, Guid userId)
        {
            Id = Guid.NewGuid();
            OriginalName = originalName;
            ContentType = contentType;
            SizeInBytes = sizeInBytes;
            UserId = userId;
            UploadAt = DateTime.UtcNow;
        }

        public void SetPath(string path)
        {
            Path = path;
        }
    }
}
namespace FileContextCore.FileManager
{
    public interface IFileManager
    {
        string GetFileName();

        string LoadContent();

        void SaveContent(string content);

        bool Clear();

        bool FileExists();
    }
}

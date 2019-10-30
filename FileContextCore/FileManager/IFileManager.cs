namespace FileContextCore.FileManager
{
    interface IFileManager
    {
        string GetFileName();

        string LoadContent();

        void SaveContent(string content);

        bool Clear();

        bool FileExists();
    }
}

using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FileContextCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;

namespace FileContextCore.FileManager
{
    public class EncryptedFileManager : IFileManager
    {
        private readonly object _thisLock = new object();

        IEntityType _type;
        private string _filetype;
        private string _key;
		private string _databasename;
        private string _location;

        public void Initialize(IFileContextScopedOptions options, IEntityType entityType, string fileType)
        {
            _type = entityType;
            _filetype = fileType;
            _key = options.Password;
            _databasename = options.DatabaseName ?? "";
            _location = options.Location;
        }
        
        public string GetFileName()
        {
            string name = _type.GetTableName().GetValidFileName();

            string path = string.IsNullOrEmpty(_location)
                ? Path.Combine(AppContext.BaseDirectory, "appdata", _databasename)
                : _location;

            Directory.CreateDirectory(path);

            return Path.Combine(path, name + "." + _filetype + ".encrypted");
        }

        public string LoadContent()
        {
            lock (_thisLock)
            {
                string path = GetFileName();

                if (File.Exists(path))
                {
                    return Decrypt(File.ReadAllText(path));
                }

                return "";
            }
        }

        public void SaveContent(string content)
        {
            lock (_thisLock)
            {
                string path = GetFileName();
                File.WriteAllText(path, Encrypt(content));
            }
        }

        public bool Clear()
        {
            lock (_thisLock)
            {
                FileInfo fi = new FileInfo(GetFileName());

                if (fi.Exists)
                {
                    fi.Delete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool FileExists()
        {
            lock (_thisLock)
            {
                FileInfo fi = new FileInfo(GetFileName());

                return fi.Exists;
            }
        }

        private string Decrypt(string str)
        {
            try
            {
                str = str.Replace(" ", "+");
                return UseAesDecryptor(str);
            }
            catch
            {
                return "";
            }
        }

        private string UseAesDecryptor(string str)
        {
            byte[] cipherBytes = Convert.FromBase64String(str);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Dispose();
                    }
                    str = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return str;
        }

        private string Encrypt(string str)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(str);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(_key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Dispose();
                    }
                    str = Convert.ToBase64String(ms.ToArray());
                }
            }
            return str;
        }
    }
}

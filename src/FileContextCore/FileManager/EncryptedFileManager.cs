using FileContextCore.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileContextCore.FileManager
{
    public class EncryptedFileManager : IFileManager
    {
        private string Key = "45Mm9yeh8d";

        public EncryptedFileManager()
        {

        }

        public EncryptedFileManager(string _key)
        {
            Key = _key;
        }

        public string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(str);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
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

        public string Encrypt(string str)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(str);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
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

        private object thisLock = new object();

        public string GetFileName<T>(string fileType)
        {
            return GetFilePath(typeof(T).Name + "." + fileType + "." + "crypt");
        }

        public string GetFilePath(string fileName)
        {
            string path = Path.Combine(AppContext.BaseDirectory, "appdata");
            Directory.CreateDirectory(path);

            return Path.Combine(path, fileName);
        }

        public string LoadContent<T>(string fileType)
        {
            return LoadContent(GetFileName<T>(fileType));
        }

        public string LoadContent(string fileName)
        {
            lock (thisLock)
            {
                string path = GetFilePath(fileName);

                if (File.Exists(path))
                {
                    return Decrypt(File.ReadAllText(path));
                }

                return "";
            }
        }

        public void SaveContent<T>(string fileType, string content)
        {
            SaveContent(GetFileName<T>(fileType), content);
        }

        public void SaveContent(string fileName, string content)
        {
            lock (thisLock)
            {
                string path = GetFilePath(fileName);

                File.WriteAllText(path, Encrypt(content));
            }
        }

        public bool Clear()
        {
            lock (thisLock)
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "appdata"));

                if (di.Exists)
                {
                    di.Delete(true);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DatabaseExists()
        {
            lock (thisLock)
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "appdata"));
                return di.Exists;
            }
        }
    }
}

using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace FileContextCore.Storage
{
    class FileContextTransaction : IDbContextTransaction
    {
        public FileContextTransaction()
        {

        }

        public void Commit()
        {

        }

        public void Dispose()
        {
            
        }

        public void Rollback()
        {
            
        }
    }
}

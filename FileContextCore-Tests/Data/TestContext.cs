using System;
using System.Collections.Generic;
using System.Text;
using FileContextCore.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FileContextCore_Tests.Data
{
    public class TestContext : DbContext
    {
        private readonly string _serializer = "json";
        private readonly string _filemanager = "default";

        public TestContext(string serializer, string filemanager)
        {
            _serializer = serializer;
            _filemanager = filemanager;
        }

        public TestContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Entry> Entries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseFileContext(_serializer, _filemanager);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileContextCore_Tests.Data;
using FileContextCore_Tests.Helper;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace FileContextCore_Tests
{
    public class TransactionTests
    {
        [Fact]
        public void TransactionSuccess()
        {
            TestContext db = new TestContext("json", "default");
            db.Entries.RemoveRange(db.Entries);
            db.SaveChanges();

            Assert.Empty(db.Entries);

            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.SaveChanges();

                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }

            Assert.Equal(6, db.Entries.Count());
        }

        [Fact]
        public void TransactionFailureRollback()
        {
            TestContext db = new TestContext("json", "default");
            db.Entries.RemoveRange(db.Entries);
            db.SaveChanges();

            Assert.Empty(db.Entries);

            using (IDbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.SaveChanges();

                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.Entries.Add(ModelCreator.GenerateEntry());
                    db.SaveChanges();

                    throw new Exception("Should perform rollback");

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    db.SaveChanges();
                }
            }

            Assert.Equal(6, db.Entries.Count());
        }
    }
}

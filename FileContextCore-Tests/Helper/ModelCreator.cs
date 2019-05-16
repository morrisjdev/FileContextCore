using System;
using System.Collections.Generic;
using System.Text;
using Bogus;
using FileContextCore_Tests.Data;

namespace FileContextCore_Tests.Helper
{
    public static class ModelCreator
    {
        private static readonly Faker<Entry> entryFaker;

        static ModelCreator()
        {
            entryFaker = new Faker<Entry>()
                .RuleFor(u => u.Content, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.SomeInt, (f, u) => f.Random.Int())
                .RuleFor(u => u.SomeDouble, (f, u) => f.Random.Double());
        }

        public static Entry GenerateEntry()
        {
            return entryFaker.Generate();
        }
    }
}

using Bogus;
using H.Necessaire;
using H.Playground.Streaming.Core.Model;
using System;
using System.Linq;

namespace H.Playground.Streaming.Core
{
    public class DataEntryProvider
    {
        static readonly Faker faker = new Faker();
        static readonly Random random = new Random();

        public DataEntry NewRandomEntry()
        {
            return new DataEntry
            {
                Name = faker.Company.CompanyName(),
                Description = faker.Company.CatchPhrase(),
                Attributes = Enumerable.Range(0, random.Next(1, 4)).Select(i => faker.Hacker.Adjective().NoteAs($"Attribute{i + 1}")).ToArray(),
            };
        }
    }
}

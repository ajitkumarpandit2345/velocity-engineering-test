using System;
using System.Collections.Generic; // Fixed typo issue: Collegctions -> Collections
using System.Linq;

// Standardized namespace to reflect domain context and dropped redundant 'Utility' prefix
namespace Valocity.ProfileHelper
{
    // Renamed to singular 'Person' per .NET naming conventions for single entities
    public class Person
    {
        public string Name { get; private set; }
        public DateTimeOffset DOB { get; private set; }

        // Compute dynamic UTC timestamp instead of using a stale static field evaluated once at startup
        public Person(string name) : this(name, DateTimeOffset.UtcNow.AddYears(-15)) { }

        // Use DateTimeOffset to match property type and eliminate timezone ambiguity
        public Person(string name, DateTimeOffset dob)
        {
            Name = name;
            DOB = dob;
        }
    }

    // Renamed from 'BirthingUnit' to 'PersonFactory' to clearly describe intent
    public class PersonFactory
    {
        private readonly List<Person> _people;

        public PersonFactory()
        {
            _people = new List<Person>();
        }

        /// <summary>
        /// Generates a specified count of random people.
        /// </summary>
        /// <param name="count">Number of people to create.</param>
        /// <returns>A list of newly generated Person objects.</returns>
        public List<Person> GetPeople(int count)
        {
            // Use local list so each call returns only the requested batch rather than accumulating indefinitely
            var generatedPeople = new List<Person>(count);

            for (int i = 0; i < count; i++)
            {
                // Random.Shared avoids allocating a new Random() each loop and prevents duplicate seeds
                // Next(0, 2) has exclusive upper bound so "Betty" is reachable (Next(0, 1) always returned 0)
                string name = Random.Shared.Next(0, 2) == 0 ? "Bob" : "Betty";

                // Use 365 days/year (was 356) to avoid age calculation drift
                int randomAge = Random.Shared.Next(18, 85);
                var dob = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromDays(randomAge * 365));

                generatedPeople.Add(new Person(name, dob));
            }

            // Track in internal store while returning only the newly created batch
            _people.AddRange(generatedPeople);
            return generatedPeople;
        }

        /// <summary>
        /// Retrieves people named Bob, optionally filtered to those older than 30.
        /// </summary>
        // Made public (was dead private code) and fixed comparison: older than 30 means DOB <= cutoff
        public IEnumerable<Person> GetBobs(bool olderThan30)
        {
            var cutoffDate = DateTimeOffset.UtcNow.AddYears(-30);

            return olderThan30
                ? _people.Where(x => x.Name == "Bob" && x.DOB <= cutoffDate)
                : _people.Where(x => x.Name == "Bob");
        }

        public string GetMarried(Person p, string lastName)
        {
            // Guard clauses against null arguments
            ArgumentNullException.ThrowIfNull(p);
            ArgumentNullException.ThrowIfNull(lastName);

            // Removed test guard ("test" string check) that leaked into production code
            string fullName = $"{p.Name} {lastName}";

            // Fixed length comparison (was int.Length) and return the truncated string
            return fullName.Length > 255 ? fullName.Substring(0, 255) : fullName;
        }
    }
}
using System;
using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using ObjectFilter.Objects;
using ObjectFilter.Filters;
using System.Linq.Expressions;
using ObjectFilter.LinqExtensions;

namespace ObjectFilter.Tests
{
    [TestFixture]
    public class FilterTests
    {
        static Guid secondGuid = Guid.NewGuid();
        List<AObject> aobjs = new List<AObject>()
            {
                new AObject(){
                    AProperty = "One",
                    BProperty = Guid.NewGuid(),
                    CProperty = true,
                    Active = true
                },
                new AObject(){
                    AProperty = "Two",
                    BProperty = secondGuid,
                    CProperty = false,
                    Active = true
                },
                new AObject(){
                    AProperty = "Three",
                    BProperty = Guid.NewGuid(),
                    CProperty = false,
                    Active = false
                }
            };
        [Test]
        public void TestFilterability()
        {
            List<AObject> aobjs = new List<AObject>()
            {
                new AObject(){
                    AProperty = "One",
                    BProperty = Guid.NewGuid(),
                    CProperty = true
                },
                new AObject(){
                    AProperty = "Two",
                    BProperty = Guid.NewGuid(),
                    CProperty = false
                },
                new AObject(){
                    AProperty = "Three",
                    BProperty = Guid.NewGuid(),
                    CProperty = false
                }
            };
            var filter = Filter<AObject>.WhereIn(a => a.CProperty, true);
            var result = aobjs.Where(filter.Finder);
            Assert.IsTrue(result.FirstOrDefault().CProperty);
            filter = Filter<AObject>.WhereIn(a => a.CProperty, false);
            filter = filter.And(Filter<AObject>.WhereIn(x => x.AProperty, "One", "Two"));
            result = aobjs.Where(filter.Finder);
            Assert.AreEqual(result.FirstOrDefault().AProperty, "Two");
            Assert.IsFalse(result.FirstOrDefault().CProperty);
            // var filt = ObjectFilter.Filters.ObjectFilter<AObject>.Finder<String>(x => x.AProperty, "Three");
            // var filter1 = ObjectFilter.Filters.ObjectFilter.Finder<AObject, String>(x => x.AProperty, new List<String>(){"One", "Two"}.AsEnumerable());
            // var filter2 = ObjectFilter.Filters.ObjectFilter.Finder<AObject, String>(x => x.AProperty, "Three");
            //var filterOne = new ObjectFilter.Filters.ObjectSpecFilter<AObject>(filter1);
            // var filterTwo = new ObjectFilter.Filters.ObjectSpecFilter<AObject>(filter2);
            //var filter = ObjectFilter.Filters.ObjectFilter<AObject>.FindSome<String>(x => x.AProperty, new List<String>() { "One", "Two" });
            // var filterx = ObjectFilter.Filters.ObjectFilter<AObject>.FindSome<bool>(x => x.CProperty, false.YieldOne());
            
            
            //var result2 = aobjs.Where(filter);
            //Assert.AreEqual(result2.FirstOrDefault().AProperty, "Three");
        }

        [Test]
        public void TestFilteredObject()
        {
            var filter = Filter<AObject>.WhereIn(a => a.Active, true);
            filter = filter.And(Filter<AObject>.WhereIn(a => a.CProperty, false));
            var result = aobjs.Where(filter.Finder).FirstOrDefault();
            Assert.AreEqual(result.BProperty, secondGuid);
            Assert.IsTrue(result.Active);
            Assert.AreEqual(result.AProperty, "Two");
        }

        [Test]
        public void TestConvertToString()
        {
            List<Guid> guids = new List<Guid>()
            {
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()
            };
            var result = guids.ConvertToString();
            Assert.IsTrue(result.GetType() == typeof(String));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvariantDictionary()
        {
            Dictionary<String, String> dict = new Dictionary<string, string>
                (StringComparer.InvariantCultureIgnoreCase);
            dict.Add("limit", "Some Value");
            dict.Add("lImIt", "Some Value");
        }

        [Test]  
        public void TestInvariantDictionaryDiffKeys()
        {
            Dictionary<String, String> dict = new Dictionary<string, string>
                (StringComparer.InvariantCultureIgnoreCase);
            dict.Add("limit", "Some Value");
            dict.Add("alimit", "Some Value");
            foreach (var kvp in dict)
            {
                System.Diagnostics.Debug.WriteLine(kvp.Key);
            }
            Assert.IsTrue(dict.Count == 2);
        }
        [Test]
        public void TestNonInvariantDictionary()
        {

            Dictionary<String, String> dict = new Dictionary<string, string>();
            dict.Add("limit", "Some Value");
            dict.Add("lImIt", "Some Value");
        }

        [Test]
        public void TestSearchObject_Constructor()
        {
            Guid bprop = Guid.NewGuid();
            Guid bpropsecond = Guid.NewGuid();
            List<String> apropsearch = new List<string>(){ "One"};
            List<Guid> bpropsearch = new List<Guid>() { bprop, bpropsecond};
            List<bool> cpropsearch = new List<bool>(){ true, true, false };
            List<bool> activesearch = new List<bool>() { false, false, false };
            AObjectSearch search = new AObjectSearch();
            Assert.AreEqual(false, search.Ascending);
            Assert.AreEqual(10, search.Limit);
            Assert.AreEqual(0, search.Offset);
            Assert.AreEqual(null, search.OrderBy);
            search = new AObjectSearch(9, 1, true, "AProperty");
            Assert.AreEqual(true, search.Ascending);
            Assert.AreEqual(9, search.Limit);
            Assert.AreEqual(1, search.Offset);
            Assert.AreEqual("AProperty", search.OrderBy);
            search = new AObjectSearch(9, 1, true, "AProperty", apropsearch, bpropsearch, cpropsearch, activesearch);
            Assert.AreEqual(true, search.Ascending);
            Assert.AreEqual(9, search.Limit);
            Assert.AreEqual(1, search.Offset);
            Assert.AreEqual("AProperty", search.OrderBy);
            Assert.AreEqual(Enumerable.Empty<bool>(), search.CPropertyFilters);
            Assert.IsTrue(search.ActiveFilters.Count() == 1);
            Assert.IsTrue(search.APropertyFilters.Contains("One"));
            Assert.AreEqual(1, search.APropertyFilters.Count());
            Assert.IsTrue(search.BPropertyFilters.Contains(bprop));
            Assert.IsTrue(search.BPropertyFilters.Contains(bpropsecond));
            Assert.AreEqual(2, search.BPropertyFilters.Count());
        }

        [Test]
        public void TestSearchObjectFilter()
        {
            List<String> apropsearch = new List<string>() { "One" };
            List<Guid> bpropsearch = new List<Guid>() { secondGuid };
            List<bool> cpropsearch = new List<bool>() { true, true, false };
            List<bool> activesearch = new List<bool>() { true, true, true };
            var search = new AObjectSearch(1, 0, false, null, apropsearch, bpropsearch, cpropsearch, activesearch);



            var filter = Filter<AObject>.WhereIn(a => a.Active, search.ActiveFilters.ToArray());
            filter = filter.And(Filter<AObject>.WhereIn(a => a.AProperty, search.APropertyFilters.ToArray()));
            filter = filter.And(Filter<AObject>.WhereIn(a => a.BProperty, search.BPropertyFilters.ToArray()));
            filter = filter.And(Filter<AObject>.WhereIn(a => a.CProperty, search.CPropertyFilters.ToArray()));
            var result = aobjs.Where(filter.Finder);
            Assert.AreEqual(0, result.Count());

            search = new AObjectSearch(1, 0, false, null, "Two".YieldOne(), bpropsearch, cpropsearch, activesearch);
            filter = Filter<AObject>.WhereIn(a => a.Active, search.ActiveFilters.ToArray());
            filter = filter.And(Filter<AObject>.WhereIn(a => a.AProperty, search.APropertyFilters.ToArray()));
            filter = filter.And(Filter<AObject>.WhereIn(a => a.BProperty, search.BPropertyFilters.ToArray()));
            filter = filter.And(Filter<AObject>.WhereIn(a => a.CProperty, search.CPropertyFilters.ToArray()));
            result = aobjs.Where(filter.Finder);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(secondGuid, result.FirstOrDefault().BProperty);
            Assert.AreEqual(true, result.FirstOrDefault().Active);
        }
    }
}

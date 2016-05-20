using System;
using System.Collections.Generic;
using System.Threading;
using ElasticsearchCRUD.ContextAddDeleteUpdate.CoreTypeAttributes;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.MappingModel;
using ElasticsearchCRUD.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using ElasticsearchCRUD.Model;
using ElasticsearchCRUD.Tracing;
using ElasticsearchCRUD.Utils;
using Xunit;

namespace ElasticsearchCRUD.Integration.Test
{
    public class JsonSerializeDeserializeTests : IDisposable
    {
        private readonly IElasticsearchMappingResolver _elasticsearchMappingResolver = new ElasticsearchMappingResolver();
        private const string ConnectionString = "http://localhost:9200";

        [Fact]
        public void IndexAndMappingClassWithListOfStrings()
        {
            var classWithListOfStrings = new ClassWithListOfStrings
            {
                Id = 3,
                Jobs = new List<string> { "one", "two"}
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(classWithListOfStrings, classWithListOfStrings.Id);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<ClassWithListOfStrings>(classWithListOfStrings.Id);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void IndexAndMappingClassWithArrayOfStrings()
        {
            var classWithArrayOfStrings = new ClassWithArrayOfStrings
            {
                Id = 2,
                Jobs = new List<string> { "one", "two" }.ToArray()
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(classWithArrayOfStrings, classWithArrayOfStrings.Id);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<ClassWithArrayOfStrings>(classWithArrayOfStrings.Id);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void IndexAndMappingClassWithListOfObjects()
        {
            var classWithListOfObjects = new ClassWithListOfObjects
            {
                Id = 2,
                Jobs = new List<ClassChild>
                {
                    new ClassChild
                    {
                        Id = 1,
                        Job= "one"
                    },
                    new ClassChild
                    {
                        Id = 2,
                        Job= "two"
                    },
                }
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(classWithListOfObjects, classWithListOfObjects.Id);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<ClassWithListOfObjects>(classWithListOfObjects.Id);

                Assert.NotNull(doc);
            }
        }

        [Fact]
        public void IndexAndMappingClassWithArrayOfObjects()
        {
            var classWithArrayOfObjects = new ClassWithArrayOfObjects
            {
                Id = 2,
                Jobs = new List<ClassChild>
                {
                    new ClassChild
                    {
                        Id = 1,
                        Job= "one"
                    },
                    new ClassChild
                    {
                        Id = 2,
                        Job= "two"
                    },
                }.ToArray()
            };

            using (
                var context = new ElasticsearchContext(ConnectionString,
                    new ElasticsearchSerializerConfiguration(_elasticsearchMappingResolver)))
            {
                context.TraceProvider = new ConsoleTraceProvider();
                context.AddUpdateDocument(classWithArrayOfObjects, classWithArrayOfObjects.Id);
                context.SaveChangesAndInitMappings();

                Thread.Sleep(1500);
                var doc = context.GetDocument<ClassWithArrayOfObjects>(classWithArrayOfObjects.Id);

                Assert.NotNull(doc);
            }
        }

        public void Dispose()
        {
            using (var context = new ElasticsearchContext(ConnectionString, _elasticsearchMappingResolver))
            {
                //if (context.IndexExists<ClassWithListOfStrings>())
                //{
                //    context.AllowDeleteForIndex = true;
                //    context.DeleteIndex<ClassWithListOfStrings>();
                //}

                //if (context.IndexExists<ClassWithArrayOfStrings>())
                //{
                //    context.AllowDeleteForIndex = true;
                //    context.DeleteIndex<ClassWithArrayOfStrings>();
                //}

                //if (context.IndexExists<ClassWithListOfObjects>())
                //{
                //    context.AllowDeleteForIndex = true;
                //    context.DeleteIndex<ClassWithListOfObjects>();
                //}

                //if (context.IndexExists<ClassWithArrayOfObjects>())
                //{
                //    context.AllowDeleteForIndex = true;
                //    context.DeleteIndex<ClassWithArrayOfObjects>();
                //}


            }
        }
    }

    public class ClassWithListOfStrings {
        public int Id { get; set; }

        public List<string> Jobs { get; set; }
    }

    public class ClassWithArrayOfStrings
    {
        public int Id { get; set; }

        public string[] Jobs { get; set; }
    }

    public class ClassChild
    {
        public int Id { get; set; }

        public string Job { get; set; }
    }

    public class ClassWithListOfObjects
    {
        public int Id { get; set; }

        public List<ClassChild> Jobs { get; set; }
    }

    public class ClassWithArrayOfObjects
    {
        public int Id { get; set; }

        public ClassChild[] Jobs { get; set; }
    }
}

ElasticsearchCRUD [![NuGet Status](http://img.shields.io/nuget/v/ElasticsearchCRUD.svg?style=flat-square)](https://www.nuget.org/packages/ElasticsearchCRUD/)
========================
<strong>Documentation:</strong>
 http://damienbod.wordpress.com/2014/09/22/elasticsearch-crud-net-provider/

<strong>Code:</strong> 
https://github.com/damienbod/ElasticsearchCRUD

<strong>NuGet Package:</strong> 
https://www.nuget.org/packages/ElasticsearchCRUD/

========================

<strong>Tutorials:</strong>

<a href="http://damienbod.wordpress.com/2014/09/22/elasticsearch-crud-net-provider/">Part 1: ElasticsearchCRUD introduction</a>

<a href="http://damienbod.wordpress.com/2014/10/01/full-text-search-with-asp-net-mvc-jquery-autocomplete-and-elasticsearch/">Part 2: MVC application search with simple documents using autocomplete, jQuery and jTable</a>

<a href="http://damienbod.wordpress.com/2014/10/08/mvc-crud-with-elasticsearch-nested-documents/">Part 3: MVC Elasticsearch CRUD with nested documents</a>

<a href="http://damienbod.wordpress.com/2014/10/14/transferring-data-to-elasticsearch-from-ms-sql-server-using-elasticsearchcrud-and-entity-framework/">Part 4: Data Transfer from MS SQL Server using Entity Framework to Elasticsearch</a>

<a href="http://damienbod.wordpress.com/2014/10/26/mvc-crud-with-elasticsearch-child-parent-documents/">Part 5: MVC Elasticsearch with child, parent documents</a>

<a href="http://damienbod.wordpress.com/2014/11/02/mvc-application-with-entity-framework-and-elasticsearch/">Part 6: MVC application with Entity Framework and Elasticsearch</a>

<a href="http://damienbod.wordpress.com/2014/11/07/live-reindex-in-elasticsearch/">Part 7: Live Reindex in Elasticsearch</a>

<a href="http://damienbod.wordpress.com/2014/11/13/csv-export-using-elasticsearch-and-web-api/">Part 8: CSV export using Elasticsearch and Web API</a>

<a href="http://damienbod.wordpress.com/2014/11/23/elasticsearch-parent-child-grandchild-documents-and-routing/">Part 9: Elasticsearch Parent, Child, Grandchild Documents and Routing</a>

<a href="http://damienbod.wordpress.com/2014/11/24/elasticsearch-type-mappings-with-elasticsearchcrud/">Part 10: Elasticsearch Type mappings with ElasticsearchCRUD</a>

<a href="https://damienbod.wordpress.com/2014/12/12/elasticsearch-synonym-analyzer-using-elasticsearchcrud/">Part 11: Elasticsearch Synonym Analyzer using ElasticsearchCRUD</a>

<a href="https://damienbod.wordpress.com/2014/12/20/using-elasticsearch-german-analyzer/">Part 12: Using Elasticsearch German Analyzer</a>

<a href="https://damienbod.wordpress.com/2015/01/07/mvc-google-maps-search-using-elasticsearch/">Part 13: MVC google maps search using Elasticsearch</a>

========================

<strong>Examples:</strong>

<a href="https://github.com/damienbod/WebSearchWithElasticsearch">Simple autocomplete search </a>

This examples shows how to do a simple search using an MVC application with jQuery autocomplete and Elasticsearch simple documents . 

<a href="https://github.com/damienbod/WebSearchWithElasticsearchNestedDocuments">Using with NESTED documents (NEST for search)</a>

This example uses Elasticsearch nested documents. The documents can be created, deleted, updated or searched for. The autocomplete search searches the documents as well as the nested objects.

<a href="https://github.com/damienbod/WebSearchWithElasticsearchChildDocuments">Elasticsearch child, parent documents in a MVC application</a>

This example uses Elasticsearch child/parent documents. All documents are saved inside the same index each with a different type. The child and parent documents are saved on the same shard. It is possible to do CRUD operations with all child documents or search for child/parent documents.

<a href="https://github.com/damienbod/DataTransferSQLWithEntityFrameworkToElasticsearch">Data Transfer MS SQLServer 2014 With EntityFramework To Elasticsearch</a>

This examples show how to transfer entities to documents in Elasticsearch. The entities are saved as nested documents.

<a href="https://github.com/damienbod/WebSearchWithElasticsearchEntityFrameworkAsPrimary">MVC application with Entity Framework and Elasticsearch</a>

This example demonstrates how to use Entity Framework as you primary database and Elasticsearch for the search in an MVC application. The application needs to create, update, delete documents in the search engine when ever Entity Framework changes, deletes or updates an entity.

<a href="https://github.com/damienbod/LiveReindexInElasticsearch">Live Reindexing in Elasticsearch</a>

This example shows how to do a live reindex in Elasticsearch. There is no downtime. The old index is accessed using an alias. The new index is created from the old index using scan and scroll and a document mapper. Then the alias to switched to access the new index. Then if required, the old index could be removed.

<a href="https://github.com/damienbod/WebApiCSVExportFromElasticsearch">Web API CSV Export using Elasticsearch (scan and scroll)</a>

This example shows how to export data from Elasticsearch ( _search scan and scroll) to Web API as a CSV file (using WebApiContrib.Formatting.Xlsx). The export is displayed in real time using SignalR. The example also provides a SignalR TraceProvider for ElasticsearchCRUD.

<a href="https://github.com/damienbod/ElasticsearchCRUD/tree/master/ConsoleElasticsearchCrudExample">ConsoleElasticsearchCrudExample</a>

A basic CRUD example.

<a href="https://github.com/damienbod/ElasticsearchCRUD/tree/master/ElasticsearchCRUD.Integration.Test">ElasticsearchCRUD.Integration.Test</a>

The integration tests shows lots of examples for ElasticsearchCRUD.

<a href="https://github.com/damienbod/ElasticsearchCRUD/tree/master/Damienbod.AnimalProvider">Damienbod.AnimalProvider</a>

Example showing mapping configuration.


========================
<strong>History</strong>

<strong>Version 1.0.26</strong><em> 2015.01.24</em>
- Support for core type geometrycollection
- Support for nested filter and query
- Support mapping for nested objects
- Add more search queries:
Common Terms Query, Function Score Query, GeoShape Query, Has Child Query, Has Parent Query, Ids Query, More Like This Query, Nested Query, Prefix Query, Query String Query, Simple Query String Query, Regexp Query, Span First Query, Span Multi Term Query, Span Near Query, Span Not Query, Span Or Query, Span Term Query, Top Children Query, Wildcard Query

<strong>Version 1.0.25</strong><em> 2015.01.18</em>
- Support for search filters:
And Filter, Bool Filter, Exists Filter, Geo Bounding Box Filter, Geo Distance Filter, Geo Distance Range Filter, Geo Polygon Filter, GeoShape Filter, GeoShape Indexed Filter, Geohash Cell Filter, Has Child Filter, Has Parent Filter, Ids Filter, Limit Filter, Match All Filter, Missing Filter, Not Filter, Or Filter, Prefix Filter, Query Filter, Range Filter, Regexp Filter, Script Filter, Term Filter, Terms Filter
- support for sort
- support for Filter in Alias
- Support for Queries in Scan and Scroll,
- support search objects
- support for basic queries:
Match Query, Multi Match Query, Bool Query, Boosting Query, Constant Score Query, Dis Max Query, Filtered Query, Fuzzy Like This Query, Fuzzy Like This Field Query, Fuzzy Query, Match All Query, Range Query, Term Query, Terms Query

<strong>Version 1.0.24</strong><em> 2015.01.05</em>
- Support for geo_point index and mapping
- Support for geo_shape index and mapping
- Spporting the following Geo Shape Types:
point, linestring, polygon, multipoint, multilinestring, multipolygon, envelope, circle 

<strong>Version 1.0.23</strong><em> 2015.01.02</em>
- Added search highlighting and refactored the hits results
- Support for alias in create index
- Added a missing const icu_tokenizer 
- Support for alias routing and filter parameters 
- Added _id attribute which can be used instead of the Key Data Annotations attribute 

<strong>Version 1.0.22</strong><em> 2014.12.15</em>
- support for custom char_filters 
- support for custom similarity 
- added all DateTime format options

<strong>Version 1.0.21</strong><em> 2014.12.09</em>
- support for index Token filters, custom filters
- support for index Tokenizers, custom tokenizers
- support for index Analyzers, custom analyzers
- support for _all and _source mappings
- support for support analysis mappings, settings
- support update index analysis settings
- support for mapping Analyzer
- refactored the search results to conform with the search API hits/hit etc

<strong>Version 1.0.20</strong><em> 2014.11.28</em>
- support optimize index 
- support close index
- support open index
- support update index settings 
- support CreateMapping for existing index
- support specific routing in mappings

<strong>Version 1.0.19</strong><em> 2014.11.22</em>
- support for Delete mapping (Index Type)
- support for sync delete index
- bug fix: bool types should not require an [ElasticsearchBoolean] attribute to create mapping 
- DeleteDocument, add support/tests for explicit routed documents
- IndexType Mapping fix for grandchild documents serialization

<strong>Version 1.0.18</strong><em> 2014.11.21</em>
- support for Elasticsearch Core Types mappings as Attribute definitions
(string, float, double, byte, short, integer,long, date, boolean, binary)
- support for  similarity mappings
- support copy_to mappings
- support fields mappings (multi-fields)
- support CreateIndex simple, nested or child/parent document with or without routing
- support for auto init mapping, simple, nested or child/parent document with or without routing

<strong>Version 1.0.17</strong><em> 2014.11.14</em>
- Added support for search exists
- Added support for document routing child/parent/grandchild or whatever
- Used routing pro code configuration
- Bug Fix Grandchildren documents are not always saved to the proper shard

<strong>Version 1.0.16</strong><em> 2014.11.09</em>
- Added live Reindex support for child parent indexes
- Added support for index exists
- Added support for Alias exists
- Added support for IndexType exists
- Add support for Exists with any URI
- Fixed Console TraceProvider Bug

<strong>Version 1.0.14</strong><em> 2014.11.06</em>
- Added live Reindex support
- Added index and indexType mapping utilities
- Added console min trace level to ConsoleTraceLogger
- Fix for scan and scroll implementation 

<strong>Version 1.0.13</strong><em> 2014.11.05</em>
- Support for alias
- Support for _search scan and scroll

<strong>Version 1.0.12</strong><em> 2014.10.31</em>
- Bug fix SearchById

<strong>Version 1.0.11</strong><em> 2014.10.29</em>
- Added ClearCache API support
- Added HTTP Head request to test if a document exists DocumentExists<T>
- Added DeleteByQuery API support 

<strong>Version 1.0.10</strong><em> 2014.10.25</em>
- Support for Elasticsearch Count API
- Return hits/total in search results
- Added code documentation and included in NuGet deployment
- Removed search for child documents per parent

<strong>Version 1.0.9</strong><em> 2014.10.22</em>
- Added Get child document for parent Id method to context
- Add SearchById<T> method to context
- Create a Child document possible
- It is possible now to update, or index child documents which belong to a parent document 
- Added non-functional tests for child documents, parent documents
- Added Search for child documents of a document
- Added Search with Json String for type T
- Initial Mappings for child parent type relationships
- Using key attribute to identity ids

<strong>Version 1.0.8</strong><em> 2014.10.17</em>
- Support Collection of Objects Property in entity as child documents
- Support Single Object Property in entity as child document
- Support Array of Objects Property in entity as child documents
- Add JsonIgnore Property Attribute for Elasticsearch
- Add Configuration/Mapping for child Object definition NESTED or document 
- Bug Fix Only the first child object is processed if it is defined in a parent List 

<strong>Version 1.0.7</strong><em> 2014.10.12</em>
- Bug in 1-n-m-1 mapping for EF entities
- Added diagnostics for HttpClient Request and response
- Added diagnostics for JsonWriter
- Added Entity Framework Data Transfer Tests
- Added Configuration to turn on/off Nested objects IncludeChildObjectsInDocument 

<strong>Version 1.0.6</strong><em> 2014.10.12</em>
- Add support for Entity Framework dynamic proxy entities
- Add Error Handling when child Entity has a reference to its parent Entity 
- support for HashSet<T> properties
- Exception Handling for M-N relationships, and circular relationships detection
- Support for 1 to N Entity Framework entities

<strong>Version 1.0.5</strong><em> 2014.10.05</em>
- Support Array of Objects Property in entity as NESTED document
- Support Collection of Objects Property in entity as NESTED documents 
- Support Single Object Property in entity as NESTED document
- Support for simple type List properties as NESTED document
- Support for simple type Array properties as NESTED document

<strong>Version 1.0.4</strong><em> 2014.10.02</em>
- sync / async methods added for CRUD
- Better Error handling
- Improvement Tracing, added System.Diagnostics Tracing support
- More Tests
- URL bug fix for GetEntity

<strong>Version 1.0.3</strong><em> 2014.09.30</em>
- sync save changes added
- default mapping settings changed, lowercase for everything and index = plural 

<strong>Version 1.0.2</strong><em> 2014.09.27</em>
- support for multiple Entity/Document Types
- Error handling improvements 
- Delete index

<strong>Version 1.0.1</strong>
Support for single entity, initial version.


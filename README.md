ElasticsearchCRUD
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


<a href=" https://github.com/damienbod/WebSearchWithElasticsearchEntityFrameworkAsPrimary">MVC application with Entity Framework and Elasticsearch</a>

This example demonstrates how to use Entity Framework as you primary database and Elasticsearch for the search in an MVC application. The application needs to create, update, delete documents in the search engine when ever Entity Framework changes, deletes or updates an entity.

<a href="https://github.com/damienbod/ElasticsearchCRUD/tree/master/ConsoleElasticsearchCrudExample">ConsoleElasticsearchCrudExample</a>

A basic CRUD example.

<a href="https://github.com/damienbod/ElasticsearchCRUD/tree/master/ElasticsearchCRUD.Integration.Test">ElasticsearchCRUD.Integration.Test</a>

The integration tests shows lots of examples for ElasticsearchCRUD.

<a href="https://github.com/damienbod/ElasticsearchCRUD/tree/master/Damienbod.AnimalProvider">Damienbod.AnimalProvider</a>

Example showing mapping configuration.


========================
<strong>History</strong>

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


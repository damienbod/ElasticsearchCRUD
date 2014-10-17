ElasticsearchCRUD
========================
<strong>Documentation:</strong>
 http://damienbod.wordpress.com/2014/09/22/elasticsearch-crud-net-provider/

<strong>Code:</strong> 
https://github.com/damienbod/ElasticsearchCRUD

<strong>NuGet Package:</strong> 
https://www.nuget.org/packages/ElasticsearchCRUD/


========================

<strong>Examples:</strong> 

https://github.com/damienbod/ElasticsearchCRUD/tree/master/ConsoleElasticsearchCrudExample

https://github.com/damienbod/ElasticsearchCRUD/tree/master/ElasticsearchCRUD.Integration.Test

https://github.com/damienbod/ElasticsearchCRUD/tree/master/Damienbod.AnimalProvider

<a href="https://github.com/damienbod/DataTransferSQLWithEntityFrameworkToElasticsearch">Data Transfer MS SQLServer 2014 With EntityFramework To Elasticsearch</a>

<a href="https://github.com/damienbod/WebSearchWithElasticsearchNestedDocuments">Using with NESTED documents (NEST for search)</a>

<a href="https://github.com/damienbod/WebSearchWithElasticsearch">Simple autocomplete search (ElasticLINQ for search)</a>

========================
<strong>History</strong>

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


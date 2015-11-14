plugin install elasticsearch/elasticsearch-lang-groovy/2.0.0

https://www.elastic.co/guide/en/elasticsearch/plugins/2.0/plugins-delete-by-query.html
bin/plugin install delete-by-query

plugin install lmenezes/elasticsearch-kopf
http://localhost:9200/_plugin/kopf


plugin install mobz/elasticsearch-head


plugin install royrusso/elasticsearch-HQ
http://localhost:9200/_plugin/HQ/

The script tests now require the following configuration in the elasticsearch.yml file. 
------------------------------------
script.groovy.sandbox.enabled: true


------------------------------------
other cluster settings which can be used for testing:
------------------------------------

cluster.name: bodone

node.name: "bodthree"

discovery.zen.ping.unicast.hosts: [ "localhost:9202", "localhost:9201"]

index.routing.allocation.disable_allocation: false

cluster.routing.allocation.disk.threshold_enabled : false

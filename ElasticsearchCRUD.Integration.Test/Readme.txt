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

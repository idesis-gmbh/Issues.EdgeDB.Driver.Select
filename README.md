# Issues.EdgeDB.Driver.Select

A showcase to reproduce and demonstrate strange behaviour in EdgeDB.Net.Driver.

## Environment

* macOS Monterey 12.5.1
* EdgeDB 2.14+5561c9b
* EdgeDB (CLI) 2.3.1+5d93f42
* EdgeDB.Net.Driver 1.1.3
* Microsoft.NetCore.App 6.0.16

## Steps to reproduce

* Create a new database with schema from /init_db/default.esdl, which is almost what can be found at the [object types cheatsheet](https://www.edgedb.com/docs/guides/cheatsheet/objects).
* Add data from file /init_db/content.eql
* Run Program.cs and verify that you can read values from EdgeDB
* Modify Program.cs / selectMovies(): Set _query_ to _queryFail_ and verify that the query's result cannot be converted successfully.

It looks like __the order of data fields in shapes__ with the same base type, like _director_ and _actor_ being both _crew_ / _person_, __must differ__ from each other.


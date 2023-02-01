# Polygons
A tool for defining and manipulating polygons under the set of given constraints of two kinds:
* any number of edges can be specified as being parallel to each other;
such edges will remain parallel no matter what transformations are taking place.
* any number of edges can be specified to have fixed lengths;
the lengths of such edges won't change under any transformation.
## Program architecture
The program can be in many different states (drawing, defining costraints, etc.) and in each one of them user input is interpreted differently.
The _state_ design pattern proved to be a quite elegant solution to this problem.
In order to make the program easily extensible with regard to adding new drawing line and circle algorithms, the _visitor_ design pattern has been emplyed; concrete drawing methods are substituted at run-time (cf. the use of function objects in the `Algorithm` class).

## Constraints algorithm
The detailed description of the algorithm can be found (in Polish) in README.pl.md.
The English translation is coming soon.

# PS4

Luke Ludlow

CS 3500



---

2019-09-27 through 2019-09-30


when the spreadsheet is created, i combine the provided IsValid delegate with the spreadsheet's own 
varaible name requirements to create a more fool-proof validator. 
this is not explicitly required by the specification, it's my design choice because it will catch 
some errors earlier and avoid unnecessary formula evaluation.


the cell factory creates new cells via the given name and content, and it also 
sets the value of the cell, evaluating the cell formula if necessary. 

the spreadsheet helper contains lots of smaller helper methods. its given protected access to the
spreadsheet, it's basically like a 'friend' class in C++. 

the spreadsheet writer handles responsibility for reading and writing xml files.

when reading xml files, there is extensive error checking and exception handling. this is annoying but necessary,
because there's a lot that can go wrong when reading files from the "outside world".


cell values are lazily instantiated and recursively calculated. 


---



2019-09-20

---

design details:

- cells are represented by their own class. each cell has a name, content, and value, as per the specifications.
- currently, the content and value are generic object types. the spreadsheet class is responsible for verifying and
  typecasting content types. in the future, i will the cell class to use custom abstract types. that way the different
  cell content and value types will be handled by polymorphism.
- the spreadsheet is backed by a Dictionary mapping strings to cells (cell name to the cell object).
- a DependencyGraph is used to keep track of cell dependencies. any time a formula cell is added or a formula cell is
  removed, we make sure to check and update the dependency graph.

library versions:

- PS2 DependencyGraph library: building against the same version as was originally submitted. see commit id
  `015bf26a0a0515b46fb54c4b34be5e0860e3540c`
- PS3 Formula library: building against the version that i fixed after i submitted. one grading test failed, so i fixed
  the division bug, and that fixed version of the Formula library is what i'm using. see commit id
  `2bbdaed760c37773aa81585c7dda9fec3cd028df`

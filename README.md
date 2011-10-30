ALite V4.0
==========

ALite is a small, lightweight and unobtrusive library for constructing
data-aware objects using the .NET platform.  It follows the active record
pattern but can be used in other ways.


Features
--------

 * Abstracted data access class to speed up the creation of database
   interaction code;
 * ALite will not attempt to generate any code or second-guess your intentions;
 * SQL is used to interact with the database;
 * Base classes for data-aware objects and collections of objects;
 * Rule system for validating object properties;
 * Undo system to revert objects to their previous state;
 * Object new/modified/etc status tracking enables objects to automatically
   choose the correct method for saving their data (ie. upserts);
 * Does not enforce any design patterns on the developer.
   

Requirements
------------

This library is designed for .NET4.
  
  
Links
-----

 * [Development blog][1]
 * [Bitbucket page][2]

  [1]: http://ant.simianzombie.com
  [2]: http://bitbucket.org/ant512/alite


Email
-----

Contact me at <ant@simianzombie.com>.

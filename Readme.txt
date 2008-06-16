* Copyright (c) 2008, Antony Dzeryn
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the names "ALite", "Simian Zombie" nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Antony Dzeryn ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Antony Dzeryn BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

  
ALite V2.1
----------

  ALite is a small, lightweight and unobtrusive framework for constructing
  data-aware objects using the .NET platform.
  

Features
--------

 - Abstracted data access class to speed up the creation of database
   interaction code;
 - Base classes for data-aware objects and collections of objects;
 - Reflection-based data rule system;
 - Delegate-based validation system;
 - Optional transactional database access;
 - Transactional undo to restore all properties to previous values;
 - Object new/dirty/etc status tracking enables objects to automatically choose
   the correct method for saving their data;
 - Does not enforce any design patterns on the developer.


Not Features
------------

  The following features are not implemented by this framework:
  
   - Multiple undos;
   - Remoting;
   - Caching;
   - Anything else not listed in the "Features" section.
   

Requirements
------------

  This library is designed for .NET2.  Although written in C#.NET, the compiled
  library will also work with VB.NET.
  
  
The Author
----------

  Written by Antony Dzeryn.  For more info, email me at spam_mail250@yahoo.com
  (that *is* a real email address).
  
  Check http://ant.simianzombie.com for other programs, or my blog at
  http://ant.simianzombie.com/blog


Usage
-----

  Including ALite in a Project
  ----------------------------

  Using ALite is simple.  To include it in your project, simply add a reference
  to the compiled DLL in the "Alite\bin\Release" directory of this archive.
  
  
  Creating a Data-Aware Object
  ----------------------------
  
  A typical data-aware object has four elements - member variables, properties
  to access those variables, constructors and methods.  The methods include the
  data access functions, which will perform all of the database interaction.
  
  Database interactions are typically limited to 4 functions:
  
   - Fetching data;
   - Inserting new data;
   - Updating old data;
   - Deleting data.
   
  ALite caters for all of these interactions.
 
  This is a typical data-aware object:
  
	using System;
	using System.Collections.Generic;
	using System.Text;
	using ALite;

	namespace Test
	{
		class TestObject : DBObject
		{
			#region Members

			private Guid mId;
			private string mName;

			#endregion

			#region Properties

			public Guid Id {
				get { return mId; }
				set { SetProperty<Guid>("Id", ref mId, value); }
			}

			public string Name
			{
				get { return mName; }
				set { SetProperty<string>("Name", ref mName, value); }
			}

			#endregion

			#region Constructors

			public TestObject()
			{
				mId = Guid.NewGuid();
				
				AddRule(ValidationRule.RuleType.MaxLength, 2, "Name");
			}

			#endregion
			
			#region Methods
			
			public override DBErrorCode Fetch()
			{
				DataAccess data = new DataAccess();
				DBErrorCode status = DBErrorCode.Ok;

				// Prepare data access object
				data.Procedure = "uspTestObjectSelect";
				data.AddParameter("@ObjectID", mId);
				
				// Attempt to retrieve data
				if (data.Fetch())
				{
					mName = data.GetString("ObjectName");
					
					// Call base method
					base.Fetch();
				} else {
				
					// Fetch failed; update status
					status = DBErrorCode.Failed;
				}
				
				data.Dispose();
				
				return status;
			}
			
			protected override DBErrorCode Create()
			{
				DataAccess data = new DataAccess();
				DBErrorCode status = DBErrorCode.Ok;

				// Save to database
				data.Procedure = "uspTestObjectCreate";
				data.AddParameter("@ObjectID", mId);
				data.AddParameter("@ObjectName", mName);
				data.Save();
				
				// Call base method
				base.Create();
				
				data.Dispose();
				
				return status;
			}
			
			protected override DBErrorCode Update()
			{
				DataAccess data = new DataAccess();
				DBErrorCode status = DBErrorCode.Ok;

				// Update database
				data.Procedure = "uspTestObjectUpdate";
				data.AddParameter("@ObjectID", mId);
				data.AddParameter("@ObjectName", mName);
				data.Save();
				
				// Call base method
				base.Update();
				
				data.Dispose();
				
				return status;
			}
			
			protected override DBErrorCode Delete()
			{
				DataAccess data = new DataAccess();
				DBErrorCode status = DBErrorCode.Ok;

				// Delete from database
				data.Procedure = "uspTestObjectDelete";
				data.AddParameter("@ObjectID", mId);
				data.Save();
				
				// Call base method
				base.Delete();
				
				data.Dispose();
				
				return status;
			}
			
			#endregion
		}
	}

  This code illustrates the main features of a data-aware object and the ALite
  framework.  To begin with, let's examine the four database interaction
  methods.
  
  
  Data Access Methods - The DataAccess Class
  ------------------------------------------

  All of the data access methods use the DataAccess object to interact with a
  database.  A DataAccess object is created like this:
  
  DataAccess data = new DataAccess();
  
  The connection string for the object should be set in the application's config
  file like this:
  
	<connectionStrings>
		<add name="DB" connectionString="Data Source=SERVER;Initial
			Catalog=DATABASE;User ID=USER;Password=PASSWORD"/>
	</connectionStrings>
	
  It is a standard connection string; the only limitation is that the key's name
  must be "DB".  You can alternatively use the second constructor, which allows
  a connection string to be supplied as a parameter.
  
  The "data" object has six main methods/properties of interest.  These are:
  
   - data.Procedure
     Sets the stored procedure name to call (ignores the SQLCode property).
 
   - data.SQLCode
     Sets the inline SQL to run (ignores the Procedure property).

   - data.AddParameter(string name, object data)
     Add a parameter to the stored procedure call.

   - data.Save()
     Call stored procedure with supplied parameter list; does not retrieve any
     data.
	 
   - data.Fetch()
     Call stored procedure with supplied parameter list; retrieves any data
     returned by the stored procedure.
	 
   - data.Dispose()
     Delete the DataAccess object.
	 
  The Fetch() method has a few related methods which are only used when
  retrieving data.  These are:

   - data.GetBoolean(string ordinal)
     Returns the bool represented by the supplied ordinal name.
  
   - data.GetByte(string ordinal)
     Returns the byte represented by the supplied ordinal name.
  
   - data.GetDateTime(string ordinal)
     Returns the date represented by the supplied ordinal name.
	 
   - data.Get[DataType](string ordinal)
     Following the pattern of the above methods, there are getters for each of
     the datatypes that SQL can return.

   - data.Read()
     Moves to the next result if the dataset returned by the Fetch() method
	 returned more than one result.

  Using the DataAccess class is simply a matter of:

   - Creating the object;
   - Setting the stored procedure;
   - Adding any parameters;
   - Calling the appropriate Save() or Fetch() method;
   - Retrieving data if necessary;
   - Disposing the object.
   
   
  Data Access Methods - Status Tracking
  -------------------------------------
  
  You may have noticed that the Create(), Update() and Delete() methods are all
  protected, whilst the Fetch() method is public.  This is intentional.  The
  DBObject base class keeps track of the object's status - whether it is a new
  object, an existing object that has been modified, or a deleted object - and
  calls the correct internal data access method when its external Save() method
  is called.
  
  To illustrate this, the following code will update the database:
  
	// Create an object
	TestObject myObject = new TestObject();
	
	// Fetch an existing object from the database
	myObject.Id = "00000000-0000-0000-0000-000000000001";
	myObject.Fetch();
	
	// Change the object's name and save to the database
	myObject.Name = "New Name";
	myObject.Save();
	
  In contrast, the following code will create a new object:
  
	// Create an object
	TestObject myObject = new TestObject();
	
	// Set the object's name
	myObject.Name = "New Name";
	
	// Save to the database
	myObject.Save();
	
  Note that the same Save() method is called on both occasions; the object
  automatically tracks whether it contains existing or new data.
  
  The following code will delete existing data:
  
	// Create an object
	TestObject myObject = new TestObject();
	myObject.Id = "00000000-0000-0000-0000-000000000001";
	myObject.MarkOld();
	
	// Mark the object for deletion
	myObject.MarkDeleted();
	
	// Delete from the database
	myObject.Save();
	
  Status tracking is mainly handled automatically within a data-aware object.
  There are a few responsibilities remaining for the developer:
  
   - Each data access call (Fetch(), Create(), Update() and Delete()) must
     call the matching base method if the access was successful.
	 
   - The class' setter must set the internal value using the SetProperty()
     method rather than just setting the value directly.
	 
  Both of these requirements are illustrated in the example class above.
  
  The SetProperty() method is smart enough to determine whether or not a
  property has actually changed.  If a property has not changed it will not be
  updated, which means that the object will not change status.  Objects that
  contain no changes will not be saved to the database if their Save() method
  is called, reducing DB access overheads.
  
  
  Undo
  ----
  
  ALite has a very simple method for undoing changes made to a DBObject.  It
  uses a SortedList object to maintain a list of internal values.  Before any
  changes are made to an object's properties, "ResetUndo()" should be called.
  Every time a property is changed, the old value is automatically stored in the
  backup list.  Calling "Undo()" will reset all of the values that have changed
  since "ResetUndo()" was called.  For example:
  
  TestObject myObject = new TestObject();
  
    // Set name to "Test"
    myObject.Name = "Test";
  
    // Prepare the undo system
    myObject.ResetUndo();
  
    // Change name to "Updated"
    myObject.Name = "Updated";
  
    // Restore the previous state
    myObject.Undo();
 
  At this point, myObject.Name equals "Test", the value that it held when we
  reset the undo system, as we have undone the change made.


  Rules
  -----
  
  ALite has two rule/validation systems.  The simple system has half a dozen or
  so basic rules that values can be checked against, such as maximum/minimum
  string lengths, maximum values, etc.  The second system uses delegates to
  allow more complex validation to be added into data-aware objects in a
  formalised way.
  
  
  Basic Rules
  -----------
  
  ALite's basic rule system uses reflection to determine whether or not an
  attempt to set a property is valid or not.  The TestObject class has a rule
  that specifies "2" as the minimum length of the "Name" property.  The
  following code would throw an exception because it violates that rule:
  
	// Create object
	TestObject myObject = new TestObject();
	
	// Attempt to set name to invalid value
	myObject.Name = "a";
	
  ALite currently supports the following rules:
  
   - Maximum string length;
   - Minimum string length;
   - Maximum integer value;
   - Minimum integer value;
   - Cannot be NULL.
   
  Rules should be set up in the constructor like this:
  
	public SomeObject()
	{
		AddRule(ValidationRule.RuleType.MaxLength, 2, "Name");
	}
   
  This functionality comes for free as part of the SetProperty() status tracking
  system.
  
  
  Delegate Rules
  --------------
  
  Validation delegates are functions in the format:
  
    public bool MyFunction(string propertyName, ref string errorMessage,
        object oldValue, object newValue);

  They should return true if the new value for the specified property is valid,
  or false if not.  Here is an example function that validates the "Name"
  property of the TestObject.  It ensures that the property cannot equal "go":
  
	private bool ValidateString(string propertyName, ref string errorMessage,
		object oldValue, object newValue)
	{
		switch (propertyName)
		{
			case "Name":
				if ((string)newValue == "go")
				{
					errorMessage = "Name cannot be 'go'";
					return false;
				}
				break;
			default:
				errorMessage = "Unhandled property name";
				return false;
		}

		// Valid
		return true;
	}
  
  Validation delegates should be wired into the rule system in the object's
  constructor using the "AddRule()" :method
  
	public TestObject()
	{
		AddRule(ValidateString, "Name");
	}
  
  
  Collections
  -----------
  
  Along-side individual data-aware objects that inherit from the DBObject class,
  ALite supports collections of data-aware objects.  These must inherit from the
  DBObjectCollection class, which provides basic functionality for a generic
  list of data-aware objects.
  
  Data-aware collections have much the same features as individual objects:
  
   - Status tracking of the items in the list;
   - Undo for all list items;
   - Fetch() and Save() methods to retrieve a set of items or save all items
     to the database.
	 
  A basic collection looks like this:
  
	using System;
	using System.Collections.Generic;
	using System.Text;
	using ALite;

	public class TestObjectCollection : DBObjectCollection<TestObject>
	{
		#region Methods

		public bool overrides Fetch()
		{
			DataAccess data = new DataAccess();
			bool status = false;

			try
			{
				data.Procedure = "uspTestObjectList";

				if (data.Fetch())
				{
					do
					{
						TestObject newObject = new TestObject();

						newObject.Id = data.GetGuid("ObjectID");
						newObject.Name = data.GetString("ObjectName").Trim();

						newObject.MarkOld();

						Add(newObject);
					}
					while (data.Read());
					
					status = true;
				}

				data.Dispose();
			}
			catch
			{
				throw;
			}
			finally
			{
				data.Dispose();
			}

			return status;
		}

		#endregion
	}

  
  Collections - Status Tracking
  -----------------------------
  
  Objects within a DBObjectCollection automatically update the list's status
  when their properties change via an event system built into the DBObject's
  SetProperty() method.  In the following code, a collection is fetched from the
  database, a single object is altered, and then the whole collection is saved.
  The collection knows that it contains a changed object, and it will only
  save that specific object.
  
	// Fetch collection
	TestObjectCollection collection = new TestObjectCollection();
	collection.Fetch();
	
	// Set the 3rd object's name
	collection[2].Name = "New Name";
	
	// Update the database
	collection.Save();
	

  Collections - Retrieving Data
  -----------------------------
  
  The simplest way to retrieve data is to override the base Fetch() method.  A
  basic Fetch() method would look like this:
  
	public bool overrides Fetch()
	{
		DataAccess data = new DataAccess();
		bool status = false;

		try
		{
			data.Procedure = "uspTestObjectList";

			if (data.Fetch())
			{
				do
				{
					TestObject newObject = new TestObject();

					newObject.Id = data.GetGuid("ObjectID");
					newObject.Name = data.GetString("ObjectName").Trim();

					newObject.MarkOld();

					Add(newObject);
				}
				while (data.Read());
				
				status = true;
			}

			data.Dispose();
		}
		catch
		{
			throw;
		}
		finally
		{
			data.Dispose();
		}

		return status;
	}
  
  This function retrieves data from the record set row by row, creating objects
  and inserting them into the collection as it goes.  Note the use of a do/while
  loop and "data.Read()" method in order to move from row to row.
  
  It is perfectly acceptible to use methods other than the default Fetch()
  function to retrieve data from the database.  A collection could include
  multiple data retrieval methods to filter, sort or otherwise manipulate the
  collection.
  
  
  Collections - Deletion
  ----------------------
  
  Whole collections of objects can be deleted very simply, using code like this:
  
	// Fetch collection
	TestObjectCollection collection = new TestObjectCollection();
	collection.Fetch();
	
	// Mark all objects as deleted
	collection.MarkDeleted();
	
	// Update the database
	collection.Save();
	
  Deleted objects are automatically removed from the collection when the
  collection's Save() method is called.
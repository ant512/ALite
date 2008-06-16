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
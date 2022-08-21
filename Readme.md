
![Logo](Graphics/icon128.png)

Reflection
==========


This Library Contains some of Reflection-related codes I usually get around in different projects. Currently the library contains
tools to help  

 * Instantiation and initialization
 * Browsing/reading and writing objects members tree-wise
 * Exploring object types characteristics
 
 
  Attributes
  ==========
  
  Following attributes are defined to mark classes and their properties with their data-related behaviors. This can be useful 
  for any data-binding usage.
  
  
  |     Attribute    | Applies On   |   Description                      |
  |:----------------:|:------------:|:----------------------------------:|
  | AutoValuedMember | Properties   | Represents a property that it's value is once generated from a data source per entity. Like Identity fields in databases.|
  | MemberName       | Properties   | It can be used to add an alternative name for a property other than The Property.Name.|
  | OwnerName        | Class        | Same as MemberName, But for classes|
  | UniqueMember     | Properties   | Represents a property that it's value is unique among all possible instances of the class.|
  
  
 ```ModelBuilder```
 ------------------
   
   This class helps you create and instantiate a Model class dynamically during the runtime.    


Object Tree
============

```DataPoint```
--------------------
This is a simple structure of an string identifier and a value (object). Such class is helpful to store all the data of a data-structure into 
small points that it has been broken into. 

```Record```
--------------------

A Record is a List of Data-points. It's helpful to keep related data-points together. 

```ObjectEvaluator```
--------------------

This class Will help working with models in runtime. It can flatten the whole object tree, or read/write data into it. an ```ObjectEvaluator``` can address 
any member using a so called "Standard" address/key. You can instantiate ```ObjectEvaluator```s in two ways. 

 1. __By Passing the target object into constructor__
 2. __By Passing the target type into constructor__   
 
 The first method can be used to read all members and their sub-members into a flat structure, from an existing model. on the other hand
 with second method, you can create an object from scratch and set the value for any of it's member and their sub-members easily.
 
 |  Method/Property            | Description |
 |:-------------------------------------------------------|:------------------:|
 |``` object Read(string address)```| This method reads the value from the ObjectEvaluator's target object, by the given address.|
 |``` void Write(string address,object value)``` | Writes the given value at given address, into the ObjectEvaluator's target object|
 |```TModel As<TModel>()```| Returns the ObjectEvaluator's target object casted into given type.|
 | ```object RootObject { get;}``` | Returns ObjectEvaluator's target object without casting|
 |```Record ToStandardFlatData()```| Creates a data point per each terminal member of the whole object tree of ObjectEvaluator's target object (root), the ```Datapoint.Identifier``` being the "Standard" address of the terminal member. And returns them all as a single Record|
 |```void LoadStandardData(Record record)``` | This will write all the data points inside the given record, into the ObjectEvaluator's target object (root).|
 
 Standard Addresses
 ===================
 
 An standard address in this library, is an string that points to a specific part of an object, Following these rules:
 
  1. Standard address of the root object is the name of it's type.
  2. The Standard address of any property of an object, is the Standard address of the object plus a "." and then the name of the property.
  3. If the object being pointed at, is any kind of collection, Its address will be followed by "[-1]". Changing the -1, to N >= 0, would mean that the address is pointing to the N'th value of collection.
  
  Some Examples:
  -------------
  
  Consider these classes:
  
  ```c#

    class Child {
     
        public string Name {get;set;}
        public int Id {get;set;}
    }

    class Information   {

        public string Content {get;set;}
        public string Title {get;set;}
    }

    class Parent {
        
        public int Id {get;set;}
        public List<Child> Children {get;set;}
        public Information Information {get;set;}
    }
  ```    

For these examples, consider the Parent class, for the top level object. The object three will be something like:

```text
---Parent
 |       
 +-------Id                                     [T]
 +-------[Children]
 |     |
 |     +--------------Children[N]
 |               |
 |               +-------------------Name       [T]
 |               +-------------------Id         [T]
 |
 |-------Information
 |    |
 |    +--------------Content                    [T]
 |    +--------------Title                      [T]
  
```

The fields that have [T] mark in front of theme, are the leaves of the whole object's tree structure. 

For this example, the addresses of nodes will be as following:

|   Address     | Type of member being pointed by the address |  Member being a terminal (leaf) |
|:---------------------------------------------------------|:--------------------------:|:-----:|
| Parent                                                   |    Parent                  |       |
| Parent.Id                                                |    int                     | [T]   |
| Parent.Children                                          |    List<Child>             |       |
| Parent.Children[n]                     *                 |    Child                   |       |
| Parent.Children[n].Name                *                 |    string                  | [T]   |
| Parent.Children[n].Id                  *                 |    int                     | [T]   | 
| Parent.Information                                       |    Information             |       |
| Parent.Information.Title                                 |    string                  | [T]   |
| Parent.Information.Content                               |    string                  | [T]   | 

__```*```__: If in actual data, there is a n'th member in Parent.Children list. but for a type without ay data, 
n would be -1.  ```Parent.Children[n]```, still would be of type ```Child```, but such member does not exists yet.


Type Helpers
============

```TypeCheck```
------------

Is A static class providing some useful methods for a given Type.



Tests
=======


```RAssert```
------------

This class works somehow like most _Assert_ implementations in test frameworks. 
By providing a ```Equal(object,object)``` method. This method performs a deep member-by-member equality check 
between to given objects, disregarding their type. It checks to see if two given 
objects have the same corresponding members, and if all of these members have the same value.



Object Extension Methods
===================


Using this library, you will get some extension methods added to any object. these methods are useful
 for comparing and copying objects without knowing their type. These methods will explore the whole object-tree 
 to perform their tasks.
 
 
 |  Method/Property            | Description |
 |:-------------------------------------------------------|:------------------:|
 |``` AreEqualAsNullables(o)```| will be true if both are null or both are not null. |
 |``` Clone()``` | will create a new instance of given object fully filled with source objects values. |
 |```AreEquivalentsWith(o)```| compares all properties of source and given object in whole tree, and returns true if all are equal.|
 | ```CopyInto(o)``` | copies all data from source object into given object through the whole structure. This method can take any number of standard addresses to be excluded from this process for cases that you want copy all data except a number of fields. |
 
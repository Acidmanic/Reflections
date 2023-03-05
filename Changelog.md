

1.0.5
-----

  * Add TreatAsLeaf attribute to mark properties to be treated as leafs even if they are not
  * Use Pluralizer for nouns in ```PluralDataOwnerNameProvider```
  * Fixed the issue of undesired instantiations fot null properties in object.clone
  * Fixed Casting issue in evaluator's write method for CoInvariant assignable types
  * optional variables order for ObjectEvaluator.ToStandardFlatData() has been corrected (backward compatibility)


1.0.6
-----
  * property Attributes would be accessible via access-nodes
  * TypeCheck has new method: IsIntegral(t)
  * TypeCheck has new method: IsNonIntegral(t)
  * TypeCheck has new method: IsNumerical(t)
  * TypeExtensions has new method: GetBaseTypeHierarchy(t)
  * TypeCheck has new method: IsNewable(t)
  * Infinite recursions for self referencing types has been avoided in ObjectInstantiator
  * Infinite recursions for __TreatAsLeaf__ nodes would be avoided.
  * ObjectInstantiator can instantiate ampty objects of models with effectively-primitive arguments 

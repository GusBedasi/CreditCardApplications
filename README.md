# Unit Test

This repo objective is to store my learnings about unit test, state-based and behavior-based.

## What do we know?

Unit test is mainly used to obviously test a unit, a unit can be literaly a single class or a group of class which works together, quoting Martin fowler
> "It's situational thing - the team decides what makes sense to be a unit for the purposes of their undestanding of the system and it's testing"
so the unit is isolated and tested, this part is divided in three different parts: Arrange, Act and Assert.

* Arrange
  Is the part where you setup your testing case to provide what is needed.
  
* Act
  Once is everything done you call the part that you're testing
  
* Assert
  Is the part where you verify based on what you expect the result to be, with what acctually was
  
## Why do we prefer test method with interfaces?

It's rather good test an class that was based on a interface because it implies if you're gonna test a mock version of a method wich you can manipulate or a real implementation that cannot be mocked and makes real things, for example a API call. Make use of real implementations implies negativaly on unit test since it can have a cost (money and performance).

## Solutions used

 * Xuni
 * Moq

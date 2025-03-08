## Procedurally Generated Dungeon

This project is a C# implementation of a procedurally generated dungeon using .NET 8. The dungeon layout is randomly generated, creating unique structures each time the program runs. The comments were added with the use of AI

## Features

Random Dungeon Generation: Uses procedural algorithms to create varied dungeon layouts.

Customizable Parameters: Modify generation settings to control dungeon size, complexity, and structure.

Efficient Randomization: Implements an IRandomize interface for flexible and reusable random number generation.


## Prerequisites

.NET 8 SDK

Visual Studio or any C#-compatible IDE

## Example Dungeon
![Dungeon](https://github.com/user-attachments/assets/9b44d5d8-09da-452d-8396-70af53bfc7e6)



## Dungeon Symbols
```C#
'█'  → Unused space  
'█'  → Dirt Wall  
' '  → Dirt Floor  
'S'  → Stone Wall  
' '  → Corridor  
'D'  → Door  
'+'  → Upstairs  
'-'  → Downstairs  
'C'  → Chests
'T'  → Traps
```

if you want to change the size and play with the dimensions mess around with these lines

```C#
 // Call dungeon generation method
 dungeon.CreateDungeon(100, 100, 100); // dimensions and object count
```

```C#
  // max size of the map
  int xmax = 80; //columns
  int ymax = 50; //rows
```




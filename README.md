## Procedurally Generated Dungeon

This project is a C# implementation of a procedurally generated dungeon using .NET 8. The dungeon layout is randomly generated, creating unique structures each time the program runs.

## Features

Random Dungeon Generation: Uses procedural algorithms to create varied dungeon layouts.

Customizable Parameters: Modify generation settings to control dungeon size, complexity, and structure.

Efficient Randomization: Implements an IRandomize interface for flexible and reusable random number generation.


## Prerequisites

.NET 8 SDK

Visual Studio or any C#-compatible IDE

## Example Dungeon

![Dungeon](https://github.com/user-attachments/assets/2e279561-a5ea-45c2-b394-2fee6511d290)


## Dungeon Symbols
```C#
' '  → Unused space  
'#'  → Dirt Wall  
'.'  → Dirt Floor  
'S'  → Stone Wall  
'='  → Corridor  
'D'  → Door  
'+'  → Upstairs  
'-'  → Downstairs  
'C'  → Chests
'T'  → Traps
```

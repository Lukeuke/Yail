# Yail (Yet another interpreted language)
Created out of pure fun. Not for real use.

## Samples:

### Variables

```js
var x = 3;
var y = x + 3;
println(y);
// output: 6
```

### Data types
- i32 -> Integer
- any -> dynamically sets the proper return type

> To disable type checking use ``#use disable-type-checking`` on top of the file.

### I/O

```js
var x = "";

println("type something:");
x = input();

println(x);
```

#### Parsing
```js
var x = "5";

x = parseInt(x);

println(x + 6);
// output: 11
```

#### Casting
```js
funky test() double {
    return 1.45;
}

var x = test();
x += 1.0;

println(typeof((string)x));
```

### Iterators
While loops
```js
var x = 5;

while x >= 0 {
    println(x);
    x--;
}

while true {
    println("break example");
    break;
}
```

For loops
```js
for var i = 0; i < 10; i++ {
    println(i);
}
```

### If-statements
```js
var x = 5;

if x > 10 {
  println(":D");
}
else if x < 10 {
  println(":(");
}
else {
  println("???");
}
```

### Functions

> More about functions here: [Docs - functions](https://github.com/Lukeuke/Yail/docs/functions.md)

```js
var x = 5;

funky addNumbers(a i32, b i32) i32 {

    println(x); // <- NOTE: value not accessable here

    return a + b;
}

x = addNumbers(3, 5);

println(x);
```

### Access modifiers

```js
package test

funky test() string {
  return "called from test()";
}

pub funky test2() string {
  println("called test2()");
  return test();
}

package main

// Exception: Cannot call private function 'test'.
println(test());
// output: 
// called test2()
// called from test()
println(test2());
```

### Arrays
```js
package main

var x = "hello";

for (var i = 0; i <= 4; i++) {
    println(x[i]);
}
```

```js
var x = [1, 2] i32;

println(x[-1]); // 2

x[-1] = 0;

println(x[-1]); // 0
```

### Libraries

```js
using math // library import

package main // necessary to define main package

println(abs(-5)); // function inside imported package
```

### Supports

- [x] Int, bool, char, string, double, null
- [x] Single-line, multi-line comments
- [x] Arrhythmic operators
- [x] Console outputs ``print()`` / ``println()`` 
- [x] Console input ``input()``
- [x] Parsing ex.: ``parseInt()``
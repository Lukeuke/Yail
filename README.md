# Yail (Yet another interpreted language)
Created out of pure fun. Not for real use.

**Solving real coding problems**
- [Codewars](https://github.com/Lukeuke/Yail/blob/main/.github/codewars/codewars.md)

## Conventions
<!> **Package names must be equal to its file names**!

## Examples:

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

Foreach
```js
package main

var x = ["test", "ad"] string;

foreach (var a in x) {
    println(a);
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

> More about functions here: [Docs - functions](https://github.com/Lukeuke/Yail/blob/main/docs/functions.md)

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

### Dictionaries
```js
package main

var dict = {"key1": 1, "key2": 2};
var x = dict["key1"];

println(x); // 1
dict["key1"] = 2;
print(dict["key1"]); // 2
```


### Reference types
Reference works kinda like in C, declare it by '&amp;' 

> <!> Reference works only on ***IAccessible*** types (Arrays, Dictionaries)

```js
package main

var dict = {"key1": 1, "key2": 2};

var x = dict["key1"];

println(x); // 1

dict["key1"] = 2;

println(x); // 1

// to fix, you need to tell that is a reference

var x1 = &dict["key1"]; // <-

println(x1); // 2

dict["key1"] = 3;

println(x1); // 3
```

### Libraries
> <!> Package name must be equal to the file name: ```package main``` in ```main.yail```
```js
using math // library import

package main // necessary to define main package

println(abs(-5)); // function inside imported package
```

#### External libraries
> <!> Package name must be equal to the file name: ```package main``` in ```main.yail```

> <!> Interpreter is getting path from the project root file.

```
├── main.yail
├── src
│   ├── test.yail
```

test.yail
```js
package test

pub funky publicFn() void {
  println("calling publicFn");
}
```

main.yail
```js
using "./src/test.yail" // only windows support

package main

test::publicFn();
```

### Structs
```js
package main

pub struct Point {
    var x i32;
    var y i32;
}

var p = new Point();
p.x = 2;
p.y = 2;

print(p.x); // 2
```

### Supports

- [x] Int, bool, char, string, double, null
- [x] Single-line, multi-line comments
- [x] Arrhythmic operators
- [x] Console outputs ``print()`` / ``println()`` 
- [x] Console input ``input()``
- [x] Parsing ex.: ``parseInt()``

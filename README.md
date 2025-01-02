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

## I/O

```js
var x = "";

println("type something:");
x = input();

println(x);
```

### Parsing
```js
var x = "5";

x = parseInt(x);

println(x + 6);
// output: 11
```

### While-loops
```js
var x = 5;

while x >= 0 {
    println(x);
    x = x - 1;
}

while true {
    println("break example");
    break;
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
````

### Functions

```js
var x = 5;

funky addNumbers(i32 a, i32 b) i32 {

    println(x); // <- NOTE: value not accessable here

    return a + b;
}

x = addNumbers(3, 5);

println(x);
```

### Supports

- [x] Int, bool, char, string, double, null
- [x] '+' and '-' operators
- [x] Console outputs ``print()`` / ``println()`` 

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

### While-loops
```js
var x = 5;

while x <= 0 {
    println(x);
    x = x - 1;
}

while true {
    println("break example");
    break;
}
```

### Supports

- [x] Int, bool, char, string, double, null
- [x] '+' and '-' operators
- [x] Console outputs ``print()`` / ``println()`` 

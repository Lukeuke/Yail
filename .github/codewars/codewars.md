# Codewars problems solved using Yail

- [Square(n) Sum](https://www.codewars.com/kata/515e271a311df0350d00000f/train/python) <8 kyu>
```js
using math

package codewars

pub funky solve(arr []i32) i32 {
    var output = 0;
    
    foreach (var x in arr) {
        output = output + math::pow(x, 2);
    }
    
    return output;
}

package main

var arr = [1, 2, 2] i32;
var arr2 = [0, 3, 4, 5] i32;
var arr3 = [-1, -2] i32;
println(codewars::solve(arr));
println(codewars::solve(arr2));
println(codewars::solve(arr3));
```
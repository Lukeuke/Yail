## Functions

### Function declaration

- private
```js
package main

funky addNumbers(a i32, b i32) i32 {
    return a + b;
}

var x = addNumbers(3, 5);

println(x);

// output:
// 8
```

- public
```js
package main

pub funky addNumbers(a i32, b i32) i32 { // 'pub' keyword
    return a + b;
}

var x = addNumbers(3, 5);

println(x);

// output:
// 8
```

### Calling function from the same package
```js
package main

funky privateFn() void {
  println("calling privateFn");
}

pub funky publicFn() void {
  println("calling publicFn");
  main::privateFn(); // package needs to be specified here also.
}
```

> <!> Here is a catch, you can disable this by telling the interpreter when parsing include package names for you (Experimental)

```bash
yail <path> disable-explicit-function-calls
```

this will procude correct ``<package>::<function>`` for you.

### Calling function from a different package

Explicitly telling the interpreter what function from what package is called to avoid collisions.
Grammar:
``
<package>::<function>
``
```js
using math // library import

package main

println(math::abs(-1)); // <package>::<function>

// output:
// 1
```

```js
package test

funky privateFn() void {
  println("calling privateFn");
}

pub funky publicFn() void {
  println("calling publicFn");
  test::publicFn(); // specify the package
}

package main

// this will throw an error, because of its protection level.
test::privateFn();

// This is ok
test::publicFn();
```
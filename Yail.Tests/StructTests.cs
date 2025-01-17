using Yail.Tests.Shared;

namespace Yail.Tests;

public class StructTests : BaseYailTest
{
    [Test]
    public void CreateStructWithOnePackage()
    {
        var code = @"
                package main

                pub struct Point {
                    var x i32;
                    var y i32;
                }

                var p = new Point();
                p.x = 2;
                p.y = 2;

                println(p.x);
                println(p.y);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("2\n2\n"));
    }
    
    [Test]
    public void CreateStructWithOnePackage_ExplicitPackageUse()
    {
        var code = @"
                package main

                pub struct Point {
                    var x i32;
                    var y i32;
                }

                var p = new main::Point();
                p.x = 2;
                p.y = 2;

                println(p.x);
                println(p.y);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("2\n2\n"));
    }
    
    [Test]
    public void CreateStructWithOnePackage_WrongPackageUse_ShouldThrowError()
    {
        var code = @"
                package main

                pub struct Point {
                    var x i32;
                    var y i32;
                }

                var p = new test::Point();
                p.x = 2;
                p.y = 2;

                println(p.x);
                println(p.y);
            ";

        try
        {
            RunCode(code);
            Assert.Fail();
        }
        catch
        {
            Assert.Pass();
        }
    }

    [Test]
    public void CreateStructWithTwoPackages_ShouldThrowError()
    {
        var code = @"
                package test

                pub struct Point {
                    var x i32;
                    var y i32;
                }

                package main

                pub struct Point {
                    var x i32;
                    var y i32;
                }

                var p = new Point();
                p.x = 2;
                p.y = 2;

                println(p.x);
                println(p.y);
            ";
        
        try
        {
            RunCode(code);
            Assert.Fail();
        }
        catch
        {
            Assert.Pass();
        }
    }
    
    [Test]
    public void CreateStructWithTwoPackages_ShouldSuccess()
    {
        var code = @"
                package test

                pub struct Point {
                    var x i32;
                    var y i32;
                }

                package main

                pub struct Point {
                    var x i32;
                    var y i32;
                }

                var p = new test::Point();
                p.x = 2;
                p.y = 2;

                println(p.x);
                println(p.y);
            ";
        
        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("2\n2\n"));
    }

    [Test]
    public void CreateStructWithTwoPackages_CallFieldThatDoesNotExist_ShouldThrowError()
    {
        var code = @"
                package main

                pub struct Point {
                    var x i32;
                    var y i32;
                }

                var p = new test::Point();
                p.z = 2;

                println(p.z);
            ";

        try
        {
            RunCode(code);
            Assert.Fail();
        }
        catch
        {
            Assert.Pass();
        }
    }
}
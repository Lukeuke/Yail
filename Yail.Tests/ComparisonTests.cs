using Yail.Tests.Shared;

namespace Yail.Tests;

public class ComparisonTests : BaseYailTest
{
    [Test]
    public void CompareTwoConstInts_Equal_True()
    {
        var code = @"
                    package main
                    
                    print(2 == 2);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("True"));
    }
    
    [Test]
    public void CompareTwoConstInts_NotEqual_False()
    {
        var code = @"
                    package main
                    
                    print(2 != 2);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("False"));
    }
    
    [Test]
    public void CompareTwoConstInts_Greater_True()
    {
        var code = @"
                    package main
                    
                    print(3 > 2);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("True"));
    }
    
    [Test]
    public void CompareTwoConstInts_Less_True()
    {
        var code = @"
                    package main
                    
                    print(1 < 2);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("True"));
    }

    [Test]
    public void CompareTwoConstInts_Less_False()
    {
        var code = @"
                    package main
                    
                    print(3 < 2);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("False"));
    }

    [Test]
    public void CompareTwoConstInts_GreaterOrEqual_True()
    {
        var code = @"
                    package main
                    
                    print(3 >= 3);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("True"));
    }

    [Test]
    public void CompareTwoConstInts_GreaterOrEqual_False()
    {
        var code = @"
                    package main
                    
                    print(2 >= 3);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("False"));
    }

    [Test]
    public void CompareTwoConstInts_LessOrEqual_True()
    {
        var code = @"
                    package main
                    
                    print(2 <= 2);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("True"));
    }

    [Test]
    public void CompareTwoConstInts_LessOrEqual_False()
    {
        var code = @"
                    package main
                    
                    print(5 <= 4);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("False"));
    }

    [Test]
    public void CompareVarToConstInts_LessOrEqual_False()
    {
        var code = @"
                    package main
                    
                    var x = 2;

                    print(x == 2);
                    ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("True"));
    }
}
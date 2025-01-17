using Yail.Tests.Shared;

namespace Yail.Tests;

public class ScalarAdditionsTest : BaseYailTest
{
    [Test]
    public void PreAddition_Int_On_IntArr()
    {
        var code = @"
                package main

                var a = [1, 2, 3] i32;

                var x = 1 + a;

                print(x);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("[2, 3, 4]"));
    }
    
    [Test]
    public void PostAddition_Int_On_IntArr()
    {
        var code = @"
                package main

                var a = [1, 2, 3] i32;

                var x = a + 1;

                print(x);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo("[2, 3, 4]"));
    }
    
    [Test]
    public void PreAddition_StringArr_On_String()
    {
        var code = @"
                package main

                var a = [""foo"", ""bar""] string;

                var x = ""foo"" + a;

                print(x);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo(@"[""foofoo"", ""foobar""]"));
    }
    
    [Test]
    public void PostAddition_StringArr_On_String()
    {
        var code = @"
                package main

                var a = [""foo"", ""bar""] string;

                var x = a + ""foo"";

                print(x);
            ";

        var actual = RunCode(code);
        
        Assert.That(actual, Is.EqualTo(@"[""foofoo"", ""barfoo""]"));
    }
    
    [Test]
    public void PreAddition_Int_On_StringArr_ShouldFail()
    {
        var code = @"
                package main

                var a = [""foo"", ""bar""] string;

                var x = ""foo"" + 1;

                print(x);
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
    public void PostAddition_Int_On_StringArr_ShouldFail()
    {
        var code = @"
                package main

                var a = [""foo"", ""bar""] string;

                var x = a + 1;

                print(x);
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
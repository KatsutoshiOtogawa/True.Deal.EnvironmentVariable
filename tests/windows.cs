
using System;
using System.IO;
using System.Collections;
using System.Runtime.Versioning;

#if NET6_0_OR_GREATER
using NUnit.Framework;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace tests
{

#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#elif NETSTANDARD2_0_OR_GREATER
#endif
#if NET6_0_OR_GREATER
#else
        [TestClass]
#endif
    public class Tests
    {
#if NET6_0_OR_GREATER
        [SetUp]
#else
        [TestInitialize]
#endif
        public void Setup()
        {
            
            // if (OperatingSystem.IsWindows())
            // {
            // } else
            // {
                // True.Deal.EnvironmentVariable.Environment.SetEnvironmentVariable("foo", processguid.ToString());
            // }
        }

#if NET6_0_OR_GREATER
        [TearDown]
#else
        [TestCleanup]
#endif
        public void Clean()
        {
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("foo", null, EnvironmentVariableTarget.Process);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("foo", null, EnvironmentVariableTarget.User);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("bar", null, EnvironmentVariableTarget.Process);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("bar", null, EnvironmentVariableTarget.User);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("Multiple", null, EnvironmentVariableTarget.Process);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("Multiple", null, EnvironmentVariableTarget.User);

        }

        /// <summary>
        /// GetEnvironmentVariable returns different environment variables are referenced for each Target.
        /// </summary>
#if NET6_0_OR_GREATER
        [Test]
#else
        [TestMethod]
#endif
        public void Test1()
        {
            Guid processguid = Guid.NewGuid();
            Guid userguid = Guid.NewGuid();
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("foo", processguid.ToString(), EnvironmentVariableTarget.Process);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("foo", userguid.ToString(), EnvironmentVariableTarget.User);
            var processfoo = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable("foo", EnvironmentVariableTarget.Process);
            var userfoo = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable("foo", EnvironmentVariableTarget.User);
#if NET6_0_OR_GREATER
            Assert.That(processfoo, Is.Not.EqualTo(userfoo));
#else
            Assert.AreNotEqual(processfoo, userfoo);
#endif
        }

        /// <summary>
        /// GetEnvironmentVariable returns different environment variables are referenced for each Target.
        /// </summary>
        /// <param name="target"></param>
#if NET6_0_OR_GREATER
        [Test]
        [TestCase("foo", EnvironmentVariableTarget.Process)]
        [TestCase("foo", EnvironmentVariableTarget.User)]
#else
        [DataTestMethod]
        [DataRow("foo", EnvironmentVariableTarget.Process)]
        [DataRow("foo", EnvironmentVariableTarget.User)]
#endif
        public void Test3(string variable, EnvironmentVariableTarget target)
        {
            Guid guid = Guid.NewGuid();
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, guid.ToString(), target);
            var value = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable(variable, target);
#if NET6_0_OR_GREATER
            Assert.That(guid.ToString(), Is.EqualTo(value));
#else
            Assert.AreEqual(guid.ToString(), value);
#endif
        }

        /// <summary>
        /// GetEnvironmentVariable returns null if a not exists environment variable is specified.
        /// </summary>
        /// <param name="target"></param>
#if NET6_0_OR_GREATER
        [Test]
        [TestCase(EnvironmentVariableTarget.Process)]
        [TestCase(EnvironmentVariableTarget.User)]
        [TestCase(EnvironmentVariableTarget.Machine)]
#else
        [DataTestMethod]
        [DataRow(EnvironmentVariableTarget.Process)]
        [DataRow(EnvironmentVariableTarget.User)]
        [DataRow(EnvironmentVariableTarget.Machine)]
#endif
        public void Test4(EnvironmentVariableTarget target)
        {
            var value = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable("NotExistsEnvironmentVariable", target);
#if NET6_0_OR_GREATER
            Assert.That(value, Is.Null);
#else
            Assert.IsNull(value);
#endif
        }

        /// <summary>
        /// Environment variables can be deleted by giving null as an argument to the SetEnvironmentVariable function.
        /// </summary>
        /// <param name="target"></param>
#if NET6_0_OR_GREATER
        [Test]
        [TestCase(EnvironmentVariableTarget.Process)]
        [TestCase(EnvironmentVariableTarget.User)]
#else
        [DataTestMethod]
        [DataRow(EnvironmentVariableTarget.Process)]
        [DataRow(EnvironmentVariableTarget.User)]
#endif
        public void Test5(EnvironmentVariableTarget target)
        {
#if NET6_0_OR_GREATER
            Assert.That(
                () => True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("NotExistsEnvironmentVariable", "")
                ,Throws.Nothing
            );
#else
            try
            {
                // これ単体の方がいいかも。
                True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("NotExistsEnvironmentVariable", "");
            }catch(Exception ex)
            {
                
                // exceptionの種類が変わるのでよくないかも。
                Assert.Fail(ex.Message); 
            }
#endif

        }

        /// <summary>
        /// Performing a WinGetEnvironmentValueKind on EnvironmentVariableTarget.Process results in an error.
        /// </summary>
        /// <param name="target"></param>
#if NET6_0_OR_GREATER
        [Test]
#else
        [TestMethod]
#endif
        public void Test51()
        {
#if NET6_0_OR_GREATER
            Assert.That(
                () => True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentValueKind("foo", EnvironmentVariableTarget.Process)
                ,Throws.TypeOf<ArgumentException>()
            );
#else
            Assert.ThrowsException<ArgumentException>(
                () => True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentValueKind("foo", EnvironmentVariableTarget.Process)
            );
#endif
        }

        /// <summary>
        /// System.IO.Exception when performing a WinGetEnvironmentValueKind on an environment variable that does not exist.
        /// </summary>
        /// <param name="target"></param>
#if NET6_0_OR_GREATER
        [Test]
#else
        [TestMethod]
#endif
        public void Test512()
        {
#if NET6_0_OR_GREATER
            Assert.That(
                () => True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentValueKind("NotExistsEnvironmentVariable", EnvironmentVariableTarget.User)
                ,Throws.TypeOf<IOException>()
            );
#else
            Assert.ThrowsException<IOException>(
                () => True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentValueKind("NotExistsEnvironmentVariable", EnvironmentVariableTarget.User)
            );
#endif
        }

        /// <summary>
        /// Environment variables can be deleted by giving null as an argument to the SetEnvironmentVariable function.
        /// </summary>
        /// <param name="target"></param>
#if NET6_0_OR_GREATER
        [Test]
        [TestCase(EnvironmentVariableTarget.Process)]
        [TestCase(EnvironmentVariableTarget.User)]
#else
        [DataTestMethod]
        [DataRow(EnvironmentVariableTarget.Process)]
        [DataRow(EnvironmentVariableTarget.User)]
#endif
        public void Test55(EnvironmentVariableTarget target)
        {

            var before = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariables(target);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("Multiple", "value", target);
            var value = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariables(target);
#if NET6_0_OR_GREATER
            Assert.That(value, Does.ContainKey("Multiple"));
#else

            CollectionAssert.Contains(value.Keys, "Multiple");

#endif

            // 他の環境変数に影響を与えていないことの確認。
            value.Remove("Multiple");
            foreach (DictionaryEntry kvp in value)
            {
#if NET6_0_OR_GREATER
                Assert.That(before[kvp.Key?.ToString() ?? ""], Is.EqualTo(kvp.Value?.ToString()));
#else
                Assert.AreEqual(before[kvp.Key?.ToString() ?? ""], kvp.Value?.ToString());
#endif
            }
        }

#if !CI
        /// <summary>
        /// Set EnvironmentVariable MachineTarget require Administrator authentication.
        /// </summary>
#if NET6_0_OR_GREATER
        [Test]
        [TestCase("bar")]
#else
        [DataTestMethod]
        [DataRow("bar")]
#endif
        public void Test6(string variable)
        {
            Guid machineguid = Guid.NewGuid();
#if NET6_0_OR_GREATER
            Assert.That(
                () => True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, machineguid.ToString(), EnvironmentVariableTarget.Machine)
                ,Throws.TypeOf<UnauthorizedAccessException>()
            );
#else
            Assert.ThrowsException<UnauthorizedAccessException>(
                () => True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, machineguid.ToString(), EnvironmentVariableTarget.Machine)
            );
#endif
        }
#endif

        /// <summary>
        /// SetEnvironmentVariable removes the environment variable if its value is given as null.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <param name="target"></param>
#if NET6_0_OR_GREATER
        [Test]
        [TestCase("bar", "hello", EnvironmentVariableTarget.Process)]
        [TestCase("bar", "hello", EnvironmentVariableTarget.User)]
#else
        [DataTestMethod]
        [DataRow("bar", "hello", EnvironmentVariableTarget.Process)]
        [DataRow("bar", "hello", EnvironmentVariableTarget.User)]
#endif
        public void Test7(string variable, string value, EnvironmentVariableTarget target)
        {
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, value, target);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, null, target);
            var deletedvalue = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable(variable, target);

#if NET6_0_OR_GREATER
            Assert.That(deletedvalue, Is.Null);
#else
            Assert.IsNull(deletedvalue);
#endif
        }

        /// <summary>
        /// GetEnvironmentVariables does not get the defaults or empty String key for the registry.
        /// </summary>
        /// <param name="target"></param>
#if NET6_0_OR_GREATER
        [Test]
        [TestCase(EnvironmentVariableTarget.Process)]
        [TestCase(EnvironmentVariableTarget.User)]
        [TestCase(EnvironmentVariableTarget.Machine)]
#else
        [DataTestMethod]
        [DataRow(EnvironmentVariableTarget.Process)]
        [DataRow(EnvironmentVariableTarget.User)]
        [DataRow(EnvironmentVariableTarget.Machine)]
#endif
        public void Test8(EnvironmentVariableTarget target)
        {
            var value = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariables(target);

#if NET6_0_OR_GREATER
            Assert.That(value, Does.Not.ContainKey(""));
            Assert.That(value, Does.Not.ContainKey("(Default)"));
#else
#endif
        }

    /*
                It 'Set-WinEnvironmentVariable, if -Value is "", remove EnvironmentVariable' {
            'hello' | Set-WinEnvironmentVariable -Name bar -Target Process
            Set-WinEnvironmentVariable -Value "" -Name bar -Target Process

            { Get-WinEnvironmentVariable -Name bar -Target Process } |
                Should -Throw -ErrorId "EnvironmentVariableNotFound,Microsoft.PowerShell.Commands.GetWinEnvironmentVariable"
        }

    BeforeAll {
        $processguid = New-Guid
        $userguid = New-Guid
        Set-WinEnvironmentVariable -Value $processguid -Name foo -Target Process
        Set-WinEnvironmentVariable -Value $userguid -Name foo -Target User -Force
}

AfterAll {
        Set-WinEnvironmentVariable -Value "" -Name foo -Target Process
        Set-WinEnvironmentVariable -Value "" -Name foo -Target User -Force
}
It 'Get-WinEnvironmentVariable returns different environment variables are referenced for each Target.' {

        $processfoo = Get - WinEnvironmentVariable - Name foo - Target Process
        $userfoo = Get - WinEnvironmentVariable - Name foo - Target User
        $processfoo | Should - Not - Be $userfoo
    }
    */
    }
}

using System;
using System.IO;
using System.Collections;
using System.Runtime.Versioning;
using NUnit.Framework;

namespace tests
{

#if NET6_0_OR_GREATER
    [SupportedOSPlatform("windows")]
#elif NETSTANDARD2_0_OR_GREATER
#endif
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
            // if (OperatingSystem.IsWindows())
            // {
            // } else
            // {
                // True.Deal.EnvironmentVariable.Environment.SetEnvironmentVariable("foo", processguid.ToString());
            // }
        }

        [TearDown]
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
        [Test]
        public void Test1()
        {
            Guid processguid = Guid.NewGuid();
            Guid userguid = Guid.NewGuid();
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("foo", processguid.ToString(), EnvironmentVariableTarget.Process);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("foo", userguid.ToString(), EnvironmentVariableTarget.User);
            var processfoo = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable("foo", EnvironmentVariableTarget.Process);
            var userfoo = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable("foo", EnvironmentVariableTarget.User);
            Assert.That(processfoo, Is.Not.EqualTo(userfoo));
        }

        /// <summary>
        /// GetEnvironmentVariable returns different environment variables are referenced for each Target.
        /// </summary>
        /// <param name="target"></param>
        [Test]
        [TestCase("foo", EnvironmentVariableTarget.Process)]
        [TestCase("foo", EnvironmentVariableTarget.User)]
        public void Test3(string variable, EnvironmentVariableTarget target)
        {
            Guid guid = Guid.NewGuid();
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, guid.ToString(), target);
            var value = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable(variable, target);
            Assert.That(guid.ToString(), Is.EqualTo(value));
        }

        /// <summary>
        /// GetEnvironmentVariable returns null if a not exists environment variable is specified.
        /// </summary>
        /// <param name="target"></param>
        [Test]
        [TestCase(EnvironmentVariableTarget.Process)]
        [TestCase(EnvironmentVariableTarget.User)]
        [TestCase(EnvironmentVariableTarget.Machine)]
        public void Test4(EnvironmentVariableTarget target)
        {
            var value = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable("NotExistsEnvironmentVariable", target);
            Assert.That(value, Is.Null);
        }

        /// <summary>
        /// Environment variables can be deleted by giving null as an argument to the SetEnvironmentVariable function.
        /// </summary>
        /// <param name="target"></param>
        [Test]
        [TestCase(EnvironmentVariableTarget.Process)]
        [TestCase(EnvironmentVariableTarget.User)]
        public void Test5(EnvironmentVariableTarget target)
        {
            Assert.That(
                () => True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("NotExistsEnvironmentVariable", "")
                ,Throws.Nothing
            );
        }

        /// <summary>
        /// Performing a WinGetEnvironmentValueKind on EnvironmentVariableTarget.Process results in an error.
        /// </summary>
        /// <param name="target"></param>
        [Test]
        public void Test51()
        {
            Assert.That(
                () => True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentValueKind("foo", EnvironmentVariableTarget.Process)
                ,Throws.TypeOf<ArgumentException>()
            );
        }

        /// <summary>
        /// System.IO.Exception when performing a WinGetEnvironmentValueKind on an environment variable that does not exist.
        /// </summary>
        /// <param name="target"></param>
        [Test]
        public void Test512()
        {
            Assert.That(
                () => True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentValueKind("NotExistsEnvironmentVariable", EnvironmentVariableTarget.User)
                ,Throws.TypeOf<IOException>()
            );
        }

        /// <summary>
        /// Environment variables can be deleted by giving null as an argument to the SetEnvironmentVariable function.
        /// </summary>
        /// <param name="target"></param>
        [Test]
        [TestCase(EnvironmentVariableTarget.Process)]
        [TestCase(EnvironmentVariableTarget.User)]
        public void Test55(EnvironmentVariableTarget target)
        {

            var before = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariables(target);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable("Multiple", "value", target);
            var value = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariables(target);
            Assert.That(value, Does.ContainKey("Multiple"));

            // 他の環境変数に影響を与えていないことの確認。
            value.Remove("Multiple");
            foreach (DictionaryEntry kvp in value)
            {
                Assert.That(before[kvp.Key?.ToString() ?? ""], Is.EqualTo(kvp.Value?.ToString()));
            }
        }

        /// <summary>
        /// Set EnvironmentVariable MachineTarget require Administrator authentication.
        /// </summary>
        [Test]
        [TestCase("bar")]
        public void Test6(string variable)
        {
            Guid machineguid = Guid.NewGuid();
            Assert.That(
                () => True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, machineguid.ToString(), EnvironmentVariableTarget.Machine)
                ,Throws.TypeOf<UnauthorizedAccessException>()
            );
        }

        /// <summary>
        /// SetEnvironmentVariable removes the environment variable if its value is given as null.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <param name="target"></param>
        [Test]
        [TestCase("bar", "hello", EnvironmentVariableTarget.Process)]
        [TestCase("bar", "hello", EnvironmentVariableTarget.User)]
        public void Test7(string variable, string value, EnvironmentVariableTarget target)
        {
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, value, target);
            True.Deal.EnvironmentVariable.Environment.WinSetEnvironmentVariable(variable, null, target);
            var deletedvalue = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariable(variable, target);
            Assert.That(deletedvalue, Is.Null);
        }

        /// <summary>
        /// GetEnvironmentVariables does not get the defaults or empty String key for the registry.
        /// </summary>
        /// <param name="target"></param>
        [Test]
        [TestCase(EnvironmentVariableTarget.Process)]
        [TestCase(EnvironmentVariableTarget.User)]
        [TestCase(EnvironmentVariableTarget.Machine)]
        public void Test8(EnvironmentVariableTarget target)
        {
            var value = True.Deal.EnvironmentVariable.Environment.WinGetEnvironmentVariables(target);

            Assert.That(value, Does.Not.ContainKey(""));
            Assert.That(value, Does.Not.ContainKey("(Default)"));
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
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Xml.Linq;

namespace True.Deal.EnvironmentVariable
{
    public class Environment
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        [SupportedOSPlatform("windows")]
        public static string? WinGetEnvironmentVariable(string variable, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {

            if (target == EnvironmentVariableTarget.Process)
            {
                var value = System.Environment.GetEnvironmentVariable(variable, target);

                return value;
            }

            RegistryKey? regkey = null;

            try
            {
                regkey = target switch
                {
                    EnvironmentVariableTarget.User => Registry.CurrentUser.OpenSubKey(@"Environment"),
                    EnvironmentVariableTarget.Machine => Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment"),
                    _ => throw new ArgumentException("ありえあに"),
                };
                // ありうるexpection追加
                return (regkey?.GetValue(variable))?.ToString();

            }
            finally
            {
                regkey?.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        [SupportedOSPlatform("windows")]
        public static IDictionary WinGetEnvironmentVariables(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {

            if (target == EnvironmentVariableTarget.Process)
            {
                // 空のでぃくとが替えることもある。
                return System.Environment.GetEnvironmentVariables(target);
            }

            IDictionary dict = new Dictionary<string, string>();
            RegistryKey? regkey = null;

            try
            {
                regkey = target switch
                {
                    EnvironmentVariableTarget.User => Registry.CurrentUser.OpenSubKey(@"Environment"),
                    EnvironmentVariableTarget.Machine => Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment"),
                    _ => throw new ArgumentException("ありえない"),
                };
                foreach (string name in regkey?.GetValueNames()!)
                {
                    // キーとして扱っているので"(Default)"が混じる。
                    if (!String.IsNullOrEmpty(name))
                    {
                        dict.Add(name, (regkey?.GetValue(name))?.ToString());
                    }
                }
                // ありうるexpection追加
                return dict;

            }
            finally
            {
                regkey?.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.Security.SecurityException">管理者権限が無い。</exception>
        [SupportedOSPlatform("windows")]
        public static RegistryValueKind? WinGetEnvironmentValueKind(string variable, EnvironmentVariableTarget target)
        {
            RegistryKey? regkey = null;

            try
            {
                regkey = target switch
                {
                    EnvironmentVariableTarget.User => Registry.CurrentUser.OpenSubKey(@"Environment"),
                    EnvironmentVariableTarget.Machine => Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment"),
                    _ => throw new ArgumentException("ありえあに"),
                };
                return regkey?.GetValueKind(variable);
            }
            finally
            {
                regkey?.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <param name="valueKind"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.Security.SecurityException">管理者権限が無い。</exception>
        /// <exception cref="UnauthorizedAccessException">管理者権限が無い。</exception>
        [SupportedOSPlatform("windows")]
        public static void WinSetEnvironmentVariable(string variable,string? value, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process, RegistryValueKind valueKind = RegistryValueKind.None)
        {

            if (target == EnvironmentVariableTarget.Process)
            {
                System.Environment.SetEnvironmentVariable(variable,value, target);
                return;
            }

            RegistryKey? regkey = null;

            try
            {
                regkey = target switch
                {
                    EnvironmentVariableTarget.User => Registry.CurrentUser.CreateSubKey(@"Environment"),
                    EnvironmentVariableTarget.Machine => Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment"),
                    _ => throw new ArgumentException("ありえないEnvironmentVariabletargetの選択"),
                };

                // 新規に変数を作成するとき値が設定されていなかったらString型として登録する
                if (RegistryValueKind.None == valueKind && regkey.GetValue(variable) is null)
                {
                    valueKind = RegistryValueKind.String;
                }

                valueKind = valueKind switch
                {
                    RegistryValueKind.None => regkey.GetValueKind(variable),
                    RegistryValueKind.String => RegistryValueKind.String,
                    RegistryValueKind.ExpandString => RegistryValueKind.ExpandString,
                    _ => throw new ArgumentException("ありえない環境変数の型の選択"),
                };

                if (String.IsNullOrEmpty(value))
                {
                    regkey.DeleteValue(variable, false);
                } else
                {
                    regkey.SetValue(variable, value, valueKind);
                }

            }
            finally
            {
                regkey?.Close();
            }
        }


        // これ以降は必要ないかも。

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <exception cref="ArgumentException"></exception>
        [UnsupportedOSPlatform("windows")]
        public static string? GetEnvironmentVariable(string variable)
        {
            return System.Environment.GetEnvironmentVariable(variable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <exception cref="ArgumentException"></exception>
        [UnsupportedOSPlatform("windows")]
        public static IDictionary GetEnvironmentVariables()
        {
            return System.Environment.GetEnvironmentVariables();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentException"></exception>
        [UnsupportedOSPlatform("windows")]
        public static void SetEnvironmentVariable(string variable, string? value)
        {
            System.Environment.SetEnvironmentVariable(variable, value);
        }
    }
}
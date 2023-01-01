using System;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;

#if NET6_0_OR_GREATER
using System.Runtime.Versioning;
#endif

namespace True.Deal.EnvironmentVariable
{
    public class Environment
    {
#if NET6_0_OR_GREATER
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

                // 空は削除
                if (string.IsNullOrEmpty(value))
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

        /// <summary>
        /// 環境変数を削除する
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.Security.SecurityException">管理者権限が無い。</exception>
        /// <exception cref="UnauthorizedAccessException">管理者権限が無い。</exception>
        [SupportedOSPlatform("windows")]
        public static void WinRemoveEnvironmentVariable(string variable, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {

            if (target == EnvironmentVariableTarget.Process)
            {
                System.Environment.SetEnvironmentVariable(variable, "", target);
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
                regkey.DeleteValue(variable, false);
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
#elif (NET35 || NET40 ||  NET452 || NET462 || NET472 || NETSTANDARD2_0_OR_GREATER)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="IOException"></exception>
        public static string WinGetEnvironmentVariable(string variable, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {

            if (target == EnvironmentVariableTarget.Process)
            {
                var value = System.Environment.GetEnvironmentVariable(variable, target);

                return value;
            }

            if (variable is null)
            {
                throw new ArgumentException("variableにはnullを選択しないでください。");
            }

            RegistryKey regkey = null;

            try
            {
                switch (target)
                {
                    case System.EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.OpenSubKey(@"Environment");
                        break;
                    case System.EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        throw new ArgumentException("ありえあに");
                }
                // ありうるexpection追加
                // return (regkey?.GetValue(variable))?.ToString();
                // 値が無い時の処理追加
                var value = regkey.GetValue(variable);
                if (value is null)
                {
                    return null;
                } else
                {
                    return value.ToString();
                }

            }
            finally
            {
                if (regkey != null)
                {
                    regkey.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public static IDictionary WinGetEnvironmentVariables(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {

            if (target == EnvironmentVariableTarget.Process)
            {
                // 空のでぃくとが替えることもある。
                return System.Environment.GetEnvironmentVariables(target);
            }

            IDictionary dict = new Dictionary<string, string>();
            RegistryKey regkey = null;

            try
            {
                switch (target)
                {
                    case EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.OpenSubKey(@"Environment");
                        break;
                    case EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        throw new ArgumentException("ありえあに");
                }

                foreach (string name in regkey.GetValueNames())
                {
                    // キーとして扱っているので"(Default)"が混じる。
                    if (!String.IsNullOrEmpty(name))
                    {
                        dict.Add(name, (regkey.GetValue(name)).ToString());
                    }
                }
                // ありうるexpection追加
                return dict;

            }
            finally
            {
                if (regkey != null)
                {
                    regkey.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.Security.SecurityException">管理者権限が無い。</exception>
        public static RegistryValueKind WinGetEnvironmentValueKind(string variable, EnvironmentVariableTarget target)
        {
            RegistryKey regkey = null;

            if (variable is null)
            {
                throw new ArgumentException("variableにはnullを選択してください。");
            }

            try
            {
                switch (target)
                {
                    case EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.OpenSubKey(@"Environment");
                        break;
                    case EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        throw new ArgumentException("ありえあに");
                }
                // 変数が無ければexception
                return regkey.GetValueKind(variable);
            }
            finally
            {
                if (regkey != null)
                {
                    regkey.Close();
                }
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
#if NET35
        public static void WinSetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process, RegistryValueKind valueKind = RegistryValueKind.Unknown)
#else
        public static void WinSetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process, RegistryValueKind valueKind = RegistryValueKind.None)
#endif
        {

            if (target == EnvironmentVariableTarget.Process)
            {
                System.Environment.SetEnvironmentVariable(variable,value, target);
                return;
            }

            RegistryKey regkey = null;

            try
            {
                switch (target)
                {
                    case EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.CreateSubKey(@"Environment");
                        break;
                    case EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        throw new ArgumentException("ありえあに");
                }

                // 新規に変数を作成するとき値が設定されていなかったらString型として登録する
#if NET35
                if (RegistryValueKind.Unknown == valueKind && regkey.GetValue(variable) is null)
#else
                if (RegistryValueKind.None == valueKind && regkey.GetValue(variable) is null)
#endif
                {
                    valueKind = RegistryValueKind.String;
                }

                switch (valueKind)
                {
#if NET35
                    case RegistryValueKind.Unknown:
#else
                    case RegistryValueKind.None:
#endif
                        valueKind = regkey.GetValueKind(variable);
                        break;
                    case RegistryValueKind.String:
                        valueKind = RegistryValueKind.String;
                        break;
                    case RegistryValueKind.ExpandString:
                        valueKind = RegistryValueKind.ExpandString;
                        break;
                    default:
                        throw new ArgumentException("ありえない環境変数の型の選択");
                }

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
                if (regkey != null)
                {
                    regkey.Close();
                }
            }
        }

        /// <summary>
        /// 環境変数を削除する
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="target"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.Security.SecurityException">管理者権限が無い。</exception>
        /// <exception cref="UnauthorizedAccessException">管理者権限が無い。</exception>
        public static void WinRemoveEnvironmentVariable(string variable, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {

            if (target == EnvironmentVariableTarget.Process)
            {
                System.Environment.SetEnvironmentVariable(variable, "", target);
                return;
            }

            RegistryKey regkey = null;

            try
            {
                switch (target)
                {
                    case EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.CreateSubKey(@"Environment");
                        break;
                    case EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        throw new ArgumentException("ありえないEnvironmentVariabletargetの選択");
                }
                regkey.DeleteValue(variable, false);
            }
            finally
            {
                if (regkey != null)
                {
                    regkey.Close();
                }
            }
        }
#endif
    }
}

using System;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

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
                switch (target)
                {
                    case System.EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.OpenSubKey(@"Environment");
                        break;
                    case System.EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
                        throw new ArgumentException(message);
                }
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
                switch (target)
                {
                    case System.EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.OpenSubKey(@"Environment");
                        break;
                    case System.EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
                        throw new ArgumentException(message);
                }
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
                switch (target)
                {
                    case System.EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.OpenSubKey(@"Environment");
                        break;
                    case System.EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
                        throw new ArgumentException(message);
                }
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
                switch (target)
                {
                    case System.EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.CreateSubKey(@"Environment");
                        break;
                    case System.EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
                        throw new ArgumentException(message);
                }

                // 新規に変数を作成するとき値が設定されていなかったらString型として登録する
                if (RegistryValueKind.None == valueKind && regkey.GetValue(variable) is null)
                {
                    valueKind = RegistryValueKind.String;
                }

                switch (valueKind)
                {
                    case RegistryValueKind.None:
                        valueKind = regkey.GetValueKind(variable);
                        break;
                    case RegistryValueKind.String:
                        valueKind = RegistryValueKind.String;
                        break;
                    case RegistryValueKind.ExpandString:
                        valueKind = RegistryValueKind.ExpandString;
                        break;
                    default:
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedRegistryValueKind
                        );
                        throw new ArgumentException(message);
                }

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
                switch (target)
                {
                    case System.EnvironmentVariableTarget.User:
                        regkey = Registry.CurrentUser.CreateSubKey(@"Environment");
                        break;
                    case System.EnvironmentVariableTarget.Machine:
                        regkey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment");
                        break;
                    default:
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
                        throw new ArgumentException(message);
                }
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
#elif (NET35 || NET40 ||  NET452 || NET462 || NET472 || NET48 || NET481 || NETSTANDARD2_0_OR_GREATER)

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
#if NET35
                var message = string.Format(CultureInfo.CurrentCulture, "You cannot assign null to argument '{0}'", "variable");
#else
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "variable",
                    WinEnvironmentVariableResources.VariableNotNullable
                );
#endif
                throw new ArgumentException(message);
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
#if NET35
                        var message = string.Format(CultureInfo.CurrentCulture, "An unexpected value was assigned to '{0}'.", "target");
#else
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
#endif
                        throw new ArgumentException(message);
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
#if NET35
                        var message = string.Format(CultureInfo.CurrentCulture, "An unexpected value was assigned to '{0}'.", "target");
#else
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
#endif
                        throw new ArgumentException(message);
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
#if NET35
                var message = string.Format(CultureInfo.CurrentCulture, "You cannot assign null to argument '{0}'", "variable");
#else
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "variable",
                    WinEnvironmentVariableResources.VariableNotNullable
                );
#endif
                throw new ArgumentException(message);
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
#if NET35
                        var message = string.Format(CultureInfo.CurrentCulture, "An unexpected value was assigned to '{0}'.", "target");
#else
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
#endif
                        throw new ArgumentException(message);
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
#if NET35
                        var message = string.Format(CultureInfo.CurrentCulture, "An unexpected value was assigned to '{0}'.", "target");
#else
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
#endif
                        throw new ArgumentException(message);
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
#if NET35
                        var message = string.Format(CultureInfo.CurrentCulture, "An unexpected value was assigned to '{0}'.", "target");
#else
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedRegistryValueKind
                        );
#endif
                        throw new ArgumentException(message);
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
#if NET35
                        var message = string.Format(CultureInfo.CurrentCulture, "An unexpected value was assigned to '{0}'.", "target");
#else
                        var message = string.Format(
                            CultureInfo.CurrentCulture,
                            "target",
                            WinEnvironmentVariableResources.UnexpectedEnvironmentVariableTarget
                        );
#endif
                        throw new ArgumentException(message);
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

using Microsoft.Win32;

using System;
using System.Security.AccessControl;

namespace Utilities
{
    public static class Registry
    {
        private static RegistryKey RegistryBase(RegistryHive Hive)
        {
            // Open the base key differently based on OS architecture.  We need the system native keys to avoid the registry redirection.
            RegistryKey baseRegistryKey;

            if (System.Environment.Is64BitOperatingSystem)
            {
                baseRegistryKey = RegistryKey.OpenBaseKey(Hive, RegistryView.Registry64);
            }
            else
            {
                baseRegistryKey = RegistryKey.OpenBaseKey(Hive, RegistryView.Default);
            }

            return baseRegistryKey;
        }

        #region Public Functions

        /// <summary>
        /// Takes a backup of the key value provided, if the restore parameter is set to true then the value is copied from the backup to the SourceValues.
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="sourceKeyName"></param>
        /// <param name="sourceDefaultValue"></param>
        /// <param name="destinationPath"></param>
        /// <param name="destinationKeyName"></param>
        /// <param name="restore"></param>
        /// <returns>Whether the backup was successful.</returns>
        public static bool BackupValue(RegistryHive Hive, string sourcePath, string sourceKeyName, string sourceDefaultValue, string destinationPath, string destinationKeyName, bool restore)
        {
            bool isUpdated = false;

            try
            {
                string destinationRegistryKeyValue = GetString(Hive, destinationPath, destinationKeyName);
                string sourceRegistryKeyValue = GetString(Hive, sourcePath, sourceKeyName);

                if (sourceRegistryKeyValue != null && destinationRegistryKeyValue != null)
                {
                    string newRegistryValue;
                    if (sourceRegistryKeyValue.Trim().Length == 0)
                    {
                        newRegistryValue = sourceDefaultValue;
                    }
                    else
                    {
                        newRegistryValue = sourceRegistryKeyValue;
                    }

                    if (sourceRegistryKeyValue != destinationRegistryKeyValue)
                    {
                        if (restore)
                        {
                            // If the backup value is blank, no point using it to restore.
                            if (destinationRegistryKeyValue.Trim().Length > 0)
                            {
                                isUpdated = SaveString(Hive, sourcePath, sourceKeyName, destinationRegistryKeyValue);
                            }
                        }
                        else
                        {
                            isUpdated = SaveString(Hive, destinationPath, destinationKeyName, newRegistryValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return isUpdated;
        }

        ///// <summary>
        ///// Removes a key from the registry.
        ///// </summary>
        ///// <param name="registryPath">Path of the registry key.</param>
        ///// <param name="registryKeyName">Name of the registry key.</param>
        //public static void DeleteUserValue(RegistryHive Hive, string registryPath, string registryKeyName)
        //{
        //    string currentValue;
        //    RegistryKey registryKey = null;

        //    try
        //    {
        //        switch (Hive)
        //        {
        //            case RegistryHive.LocalMachine:
        //                registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
        //                break;
        //            case RegistryHive.CurrentUser:
        //                registryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
        //                break;
        //            case RegistryHive.ClassesRoot:
        //                registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
        //                break;
        //            case RegistryHive.CurrentConfig:
        //                registryKey = Microsoft.Win32.Registry.CurrentConfig.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
        //                break;
        //            //case RegistryHive.DynData:
        //            //    registryKey = Microsoft.Win32.Registry.DynData.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
        //            //    break;
        //            case RegistryHive.PerformanceData:
        //                registryKey = Microsoft.Win32.Registry.PerformanceData.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
        //                break;
        //            case RegistryHive.Users:
        //                registryKey = Microsoft.Win32.Registry.Users.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
        //                break;
        //        }
        //        //RegistryKey registryKey = Microsoft.Win32.Registry.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

        //        if (registryKey != null)
        //        {
        //            registryKey.DeleteValue(registryKeyName);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        /// <summary>
        /// Removes a key from the registry.
        /// </summary>
        /// <param name="Path">Path of the registry key.</param>
        public static void DeleteKey(string Path)
        {
            try
            {
                Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(Path);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        /// <summary>
        /// Removes a value from the registry.
        /// </summary>
        /// <param name="Path">Path of the registry key.</param>
        /// <param name="ValueName">Name of the registry key.</param>
        public static void DeleteValue(RegistryHive Hive, string Path, string ValueName)
        {
            try
            {
                RegistryKey Base = RegistryBase(Hive);

                RegistryKey registryKey = Base.OpenSubKey(Path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

                if (registryKey != null)
                {
                    if (registryKey.GetValue(ValueName) != null)
                    {
                        registryKey.DeleteValue(ValueName);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }
        }

        /// <summary>
        /// Gets a DWORD value from the registry.
        /// </summary>
        /// <param name="registryPath">Path to the location of the Dword</param>
        /// <param name="registryKeyName">Key that contains the Dword.</param>
        /// <returns></returns>
        public static int GetInt(RegistryHive Hive, string registryPath, string registryKeyName)
        {
            int currentValue = 0;

            try
            {
                RegistryKey Base = RegistryBase(Hive);

                RegistryKey registryKey = Base.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                if (registryKey != null)
                {
                    currentValue = (int)registryKey.GetValue(registryKeyName, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return currentValue;
        }

        /// <summary>
        /// Gets the current value for a specified key from the registry.
        /// </summary>
        /// <param name="registryPath">Example: SOFTWARE\SOE\Notiphi</param>
        /// <param name="registryKeyName">Example: DisableNotiphi</param>
        /// <param name="defaultValue"></param>
        /// <returns>The value of the key.</returns>
        public static object GetObject(RegistryHive Hive, string registryPath, string registryKeyName, object defaultValue)
        {
            string currentValue = null;

            try
            {
                RegistryKey Base = RegistryBase(Hive);

                RegistryKey registryKey = Base.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                if (registryKey != null)
                {
                    currentValue = (string)registryKey.GetValue(registryKeyName, defaultValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return currentValue;
        }

        /// <summary>
        /// Gets the current value for a specified key from the registry.
        /// </summary>
        /// <param name="registryPath">Example: SOFTWARE\SOE\Notiphi</param>
        /// <param name="registryKeyName">Example: DisableNotiphi</param>
        /// <returns>The value of the key.</returns>
        public static string GetString(RegistryHive Hive, string registryPath, string registryKeyName)
        {
            string currentValue = null;

            try
            {
                RegistryKey Base = RegistryBase(Hive);

                RegistryKey registryKey = Base.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                if (registryKey != null)
                {
                    currentValue = (string)registryKey.GetValue(registryKeyName, string.Empty);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return currentValue;
        }

        /// <summary>
        /// Gets the current value for a specified key from the registry. If the key does not exist, it is created.
        /// </summary>
        /// <param name="registryRootPath">Example: SOFTWARE\SOE\Notiphi</param>
        /// <param name="registryKeyName">Example: DisableNotiphi</param>
        /// <returns>The value of the key.</returns>
        public static string GetStringCreateIfEmpty(RegistryHive Hive, string registryRootPath, string registrySubFolder, string registryKeyName)
        {
            // var functionName = "RegistryAccess::GetRegistryValueFromLocalMachineCreateIfEmpty(string registryRootPath, string registrySubFolder, string registryKeyName)";
            string currentValue = null;

            try
            {
                RegistryKey Base = RegistryBase(Hive);

                RegistryKey registryKey = Base.OpenSubKey(registryRootPath + registrySubFolder, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                if (registryKey != null)
                {
                    currentValue = (string)registryKey.GetValue(registryKeyName, string.Empty);
                }
                else
                {
                    RegistryKey registryRootKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(registryRootPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);
                    RegistryKey unused = registryRootKey.CreateSubKey(registrySubFolder);
                    currentValue = GetStringCreateIfEmpty(Hive, registryRootPath, registrySubFolder, registryKeyName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return currentValue;
        }

        /// <summary>
        /// Gets the current value for a specified key from the registry.
        /// </summary>
        /// <param name="registryPath">Path of the registry key.</param>
        /// <param name="registryKeyName">Name of the registry key.</param>
        /// <returns>The value of the key.</returns>
        public static string GetUserObject(string registryPath, string registryKeyName, object defaultValue)
        {
            // var functionName = "RegistryAccess::GetRegistryValueFromUsers(string registryPath, string registryKeyName)";

            string currentValue = null;

            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.Users.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ReadKey);

                if (registryKey != null)
                {
                    currentValue = (string)registryKey.GetValue(registryKeyName, defaultValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return currentValue;
        }

        /// <summary>
        /// Sets a DWORD in the registry.
        /// </summary>
        /// <param name="registryPath">Path to the location of dword.</param>
        /// <param name="registryKeyName">Key that contains the dword.</param>
        /// <param name="newValue">Value to set the dword to.</param>
        /// <returns>Success or failure.</returns>
        public static bool SaveInt(RegistryHive Hive, string registryPath, string registryKeyName, int newValue)
        {
            // var functionName = "RegistryAccess::GetRegistryValueFromLocalMachine(string registryPath, string registryKeyName, string newValue)";
            bool isUpdated = false;

            try
            {
                RegistryKey Base = RegistryBase(Hive);

                RegistryKey registryKey = Base.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

                if (registryKey == null)
                {
                    registryKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None);
                    registryKey.SetValue(registryKeyName, 0, RegistryValueKind.DWord);
                    registryKey.Flush();
                }

                int currentValue = (int)registryKey.GetValue(registryKeyName, 0);

                if (currentValue != newValue)
                {
                    registryKey.SetValue(registryKeyName, newValue, RegistryValueKind.DWord);
                    isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return isUpdated;
        }

        /// <summary>
        /// Sets values in the system registry.
        /// </summary>
        /// <param name="registryBase">Example: HKEY_LOCAL_MACHINE</param>
        /// <param name="registryPath">Example: SOFTWARE\SOE\Notiphi</param>
        /// <param name="registryKeyName">Example: DisableNotiphi</param>
        /// <returns>Whether the update was successful.</returns>
        public static bool SaveString(RegistryHive Hive, string registryPath, string registryKeyName, string newValue)
        {
            // var functionName = "RegistryAccess::GetRegistryValueFromLocalMachine(string registryPath, string registryKeyName, string newValue)";

            bool isUpdated = false;

            try
            {
                RegistryKey Base = RegistryBase(Hive);

                RegistryKey registryKey = Base.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

                if (registryKey == null)
                {
                    registryKey = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None);
                    registryKey.SetValue(registryKeyName, string.Empty, RegistryValueKind.String);
                    registryKey.Flush();
                }

                string currentValue = (string)registryKey.GetValue(registryKeyName, string.Empty);

                if (currentValue != newValue)
                {
                    registryKey.SetValue(registryKeyName, newValue, RegistryValueKind.String);
                    isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return isUpdated;
        }

        /// <summary>
        /// Sets values in the system registry.
        /// </summary>
        /// <param name="registryPath">Path of the registry key.</param>
        /// <param name="registryKeyName">Name of the registry key.</param>
        /// <param name="newValue">New value to set against this registry key.</param>
        /// <returns>Whether the update was successful.</returns>
        public static bool SaveUserString(string registryPath, string registryKeyName, string newValue)
        {
            // var functionName = "RegistryAccess::SetRegistryValueToUsers(string registryPath, string registryKeyName, string newValue)";
            bool isUpdated = false;

            try
            {
                RegistryKey registryKey = Microsoft.Win32.Registry.Users.OpenSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

                if (registryKey == null)
                {
                    registryKey = Microsoft.Win32.Registry.Users.CreateSubKey(registryPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None);
                    registryKey.SetValue(registryKeyName, string.Empty, RegistryValueKind.String);
                    registryKey.Flush();
                }

                string currentValue = (string)registryKey.GetValue(registryKeyName, string.Empty);

                if (currentValue != newValue)
                {
                    registryKey.SetValue(registryKeyName, newValue, RegistryValueKind.String);
                    isUpdated = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            return isUpdated;
        }

        #endregion
    }
}

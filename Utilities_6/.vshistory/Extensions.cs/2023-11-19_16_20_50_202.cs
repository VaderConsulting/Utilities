
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

using ProtoBuf;

using static System.String;

namespace Utilities
{
    public static class Extensions
    {
        #region Constants 

        private static long OneKb = 1024;
        private static long OneMb = OneKb * 1024;
        private static long OneGb = OneMb * 1024;
        private static long OneTb = OneGb * 1024;
        private static long OnePb = OneTb * 1024;
        private static long OneEb = OnePb * 1024;
        private static System.Numerics.BigInteger OneZb = OneEb * 1024;
        private static System.Numerics.BigInteger OneYb = OneZb * 1024;
        private static int WM_SETREDRAW = 0xB;
        private const int MUST_BE_LESS_THAN = 10000000; // 8 decimal digits

        #region GetWindowLong

        // offset of window style value
        public const int GWL_STYLE = -16;

        // window style constants for scrollbars
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;

        #endregion

        #endregion

        #region Events 

        #endregion

        #region Enums 

        #endregion

        #region DLL Imports 

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        #endregion

        #region Fields 

        #endregion

        #region Properties 

        #endregion

        #region Constructors 

        #endregion

        #region Event Handlers 

        #endregion

        #region Private Methods 

        /// <summary>
        /// Compares two values and returns if they are the same.
        /// </summary>
        /// <param name="valueA">The first value to compare.</param>
        /// <param name="valueB">The second value to compare.</param>
        /// <returns><c>true</c> if both values match, otherwise <c>false</c>.</returns>
        private static bool AreValuesEqual(this object? valueA, object? valueB)
        {
            if (valueA == null && valueB == null)
            {
                return true;
            }

            if (valueA == null || valueB == null)
            {
                return false;
            }

            return valueA == valueB;

            //bool result;
            //IComparable selfValueComparer;

            //if ((valueA == null || valueB == null) && (valueA != null && valueB != null))
            //{
            //    throw new ArgumentNullException("The value cannot be null");
            //}

            //selfValueComparer = valueA as IComparable;

            //if (valueA == null && valueB != null || valueA != null && valueB == null)
            //{
            //    result = false; // one of the values is null
            //}
            //else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
            //{
            //    result = false; // the comparison using IComparable failed
            //}
            //else if (!object.Equals(valueA, valueB))
            //{
            //    result = false; // the comparison using Equals failed
            //}
            //else
            //{
            //    result = true; // match
            //}

            //return result;
        }

        /// <summary>
        /// Determines whether value instances of the specified type can be directly compared.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if this value instances of the specified type can be directly compared; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string CurrentMethodName()
        {
            StackTrace trace = new StackTrace();
            StackFrame frame;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            frame = trace.GetFrame(1);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return frame.GetMethod().Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private static byte[]? Hash(byte[] Value, byte[] Salt)
        {
            try
            {
                byte[] saltedValue = Value.Concat(Salt).ToArray();

                return new SHA512Managed().ComputeHash(saltedValue);
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Public Methods 

        #region Assembly 

        /// <summary>
        /// From https://stackoverflow.com/questions/1600962/displaying-the-build-date
        /// </summary>
        /// <param name="Assembly">An assembly to query</param>
        /// <param name="TimeZone">A TimeZoneInfo parameter</param>
        /// <returns>The BuildDate of the Assembly as a DateTime</returns>
        public static DateTime GetBuildDateTime(this Assembly Assembly, TimeZoneInfo TimeZone)
        {
            // Constants related to the Windows PE file format. 
            const int PE_HEADER_OFFSET = 60;
            const int LINKER_TIMESTAMP_OFFSET = 8;

            // Discover the base memory address where our assembly is loaded 
            Module entryModule = Assembly.ManifestModule;
            IntPtr hMod = Marshal.GetHINSTANCE(entryModule);

            if (hMod == IntPtr.Zero - 1)
            {
                throw new Exception("Failed to get HINSTANCE.");
            }

            // Read the linker timestamp 
            int offset = Marshal.ReadInt32(hMod, PE_HEADER_OFFSET);
            int secondsSince1970 = Marshal.ReadInt32(hMod, offset + LINKER_TIMESTAMP_OFFSET);

            // Convert the timestamp to a DateTime 
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime linkTimeUtc = epoch.AddSeconds(secondsSince1970);
            DateTime Result = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, TimeZone ?? TimeZoneInfo.Local);

            return Result;
        }

        #endregion

        #region Bitmap 

        //public static Bitmap Rotate(this Bitmap bitmap, float angle) 
        //{ 
        //    using (Graphics graphics = Graphics.FromImage(bitmap)) 
        //    { 
        //        graphics.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2); 
        //        graphics.RotateTransform(angle); 
        //        graphics.TranslateTransform(-(float)bitmap.Width / 2, -(float)bitmap.Height / 2); 

        //        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic; 
        //        graphics.DrawImage(bitmap, new Point(0, 0)); 
        //    } 

        //    return bitmap; 
        //} 

        #endregion

        #region bool

        /// <summary>
        /// Convert to a Database bit
        /// </summary>
        /// <param name="Value"></param>
        /// <returns><see cref="short"/></returns>
        public static short ToBit(this bool Value)
        {
            if (Value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        #endregion

        #region byte[] 

#if Windows
        /// <summary>
        /// Convert an array of <see cref="byte"/> to an image
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>Image contained within the input array of <see cref="byte"/></returns>
        public static Image ToImage(this byte[] Value)
        {
            MemoryStream ms = new MemoryStream(Value);
            using (Image returnImage = Image.FromStream(ms))
            {
                return returnImage;
            }
        }
#endif

        /// <summary>
        /// Convert to the given type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] Value)
        {
            //MemoryStream Stream = new MemoryStream();
            //BinaryFormatter Formatter = new BinaryFormatter();
            //T Result = default(T);

            //Stream.Write(Value, 0, Value.Length);
            //Stream.Seek(0, SeekOrigin.Begin);

            //Result = (T)Formatter.Deserialize(Stream);

            //Stream.Close();

            //return Result;

            using MemoryStream stream = new MemoryStream();

            // Ensure that our stream is at the beginning.
            stream.Write(Value, 0, Value.Length);
            _ = stream.Seek(0, SeekOrigin.Begin);

            return Serializer.Deserialize<T>(stream);
        }

        /// <summary>
        /// Convert an array of <see cref="byte"/> to a string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>String version of the input array of <see cref="byte"/></returns>
        public static string ToString(this byte[] Value)
        {
            string Result = Encoding.ASCII.GetString(Value);

            return Result;
        }

        /// <summary>
        /// Convert an array of <see cref="byte"/> to a string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>String version of the input array of <see cref="byte"/></returns>
        public static string ToString(this byte[] Value, int Start, int Length)
        {
            byte[] Bytes = new byte[Length];
            string Result = "";
            Array.Copy(Value, Start, Bytes, 0, Length);

            for (int i = 0; i < Length; i++)
            {
                if (Bytes[i] != 0)
                {
                    Result += ((char)Bytes[i]).ToString();
                }
            }

            return Result.Trim();
        }

        /// <summary>
        /// Convert an array of <see cref="byte"/> to a string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>String version of the input array of <see cref="byte"/></returns>
        public static string? ToText(this byte[] Value)
        {
            if (Value == null)
            {
                return null;
            }

            byte[] Bytes = new byte[Value.Length];
            string Result = "";
            Array.Copy(Value, 0, Bytes, 0, Value.Length);

            for (int i = 0; i < Value.Length; i++)
            {
                if (Bytes[i] != 0)
                {
                    Result += ((char)Bytes[i]).ToString();
                }
            }

            return Result.Trim();
        }

        #endregion

        #region Control 

        /// <summary>
        /// Set the DoubleBuffered property against the target Control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="enable"><see cref="bool"/> to enable (true) or disable (false) DoubleBuffering</param>
        public static void DoubleBuffered(this Control control, bool enable)
        {
            PropertyInfo? doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

            if (doubleBufferPropertyInfo == null)
            {
                return;
            }

            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }

        public static Screen GetScreen(this Control target)
        {
            Screen Result = Screen.FromControl(target);

            return Result;
        }

        /// <summary>
        /// Execute the Action on the UI thread.
        /// Usage:  this.OnUIThread(() => this.myLabel.Text = "Text Goes Here");
        /// </summary>
        /// <param name="control">The Control to perform the Code against</param>
        /// <param name="code">The Code to perform</param>
        public static void OnUIThread(this Control control, Action code)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(code);
            }
            else
            {
                code.Invoke();
            }
        }

        ///// <summary>
        ///// Execute the Action asynchronously on the UI thread, does not block execution on the calling thread.
        ///// Usage:  this.AsyncOnUIThread(() => this.myLabel.Text = "Text Goes Here");
        ///// </summary>
        ///// <param name="control">The Control to perform the Code against</param>
        ///// <param name="code">The Code to perform</param>
        //public static void OnUIThreadAsync(this Control control, Action code)
        //{
        //    if (control.InvokeRequired)
        //    {
        //        control.BeginInvoke(code);
        //    }
        //    else
        //    {
        //        //Cursor.Current = Cursors.AppStarting;
        //        code.Invoke();
        //        //Cursor.Current = Cursors.Default;
        //    }
        //}

        public static void ResumeDrawing(this Control target)
        {
            _ = ResumeDrawing(target, true);
        }

        public static int ResumeDrawing(this Control target, bool redraw)
        {
            int Result = SendMessage(target.Handle, WM_SETREDRAW, 1, 0);

            if (redraw)
            {
                target.Refresh();
            }

            return Result;
        }

        public static int SuspendDrawing(this Control target)
        {
            return SendMessage(target.Handle, WM_SETREDRAW, 0, 0);
        }

        #endregion

        #region DataRow

        public static bool GetBoolField(this System.Data.DataRow Row, string Field)
        {
            bool Result = false;

            if (Row.Table.Columns.Contains(Field))
            {
                Result = (bool)Row[Field];
            }

            return Result;
        }

        public static DateTime GetDateField(this System.Data.DataRow Row, string Field)
        {
            DateTime Result = DateTime.MinValue;

            if (Row.Table.Columns.Contains(Field))
            {
                Result = (DateTime)Row[Field];
            }

            return Result;
        }

        public static int GetIntField(this System.Data.DataRow Row, string Field)
        {
            int Result = -1;

            if (Row.Table.Columns.Contains(Field))
            {
                Result = Convert.ToInt32(Row[Field].ToString());
            }

            return Result;
        }

        public static string GetStringField(this System.Data.DataRow Row, string Field)
        {
            string Result = "";

            if (Row.Table.Columns.Contains(Field))
            {
                Result += Row[Field].ToString();
            }

            return Result;
        }

        #endregion

        #region DateTime

        public static int Age(this DateTime DateOfBirth)
        {
            // Calculate the preliminary age
            int age = DateTime.Now.Year - DateOfBirth.Year;

            // Adjust the age if the current date is before the birthday in the current year
            if (DateTime.Now < DateOfBirth.AddYears(age))
            {
                age--;
            }

            // Safeguard against future dates which would result in a negative age
            if (age < 0)
            {
                age = 0;
            }

            return age;
        }

        public static string CalendarQuarter(this DateTime value)
        {
            // Determines the calendar quarter of the year based on the month.
            if (value.Month is >= 1 and <= 3) // First quarter (Q1)
            {
                return "January to March " + value.Year;
            }
            else if (value.Month is >= 4 and <= 6) // Second quarter (Q2)
            {
                return "April to June " + value.Year;
            }
            else if (value.Month is >= 7 and <= 9) // Third quarter (Q3)
            {
                return "July to September " + value.Year;
            }
            else // Fourth quarter (Q4)
            {
                return "October to December " + value.Year;
            }
        }

        public static string FinancialQuarter(this DateTime value, CultureInfo culture)
        {
            // Mapping culture to fiscal year start month
            Dictionary<string, int> fiscalYearStarts = new Dictionary<string, int>
            {
                // Previously listed countries
                { "en-AU", 7 }, // Australia: July
                { "en-US", 10 }, // United States: October
                { "en-GB", 4 }, // United Kingdom: April
                { "en-CA", 4 }, // Canada: April
                { "en-IN", 4 }, // India: April
                { "ja-JP", 4 }, // Japan: April
                { "zh-CN", 1 }, // China: January
                { "de-DE", 1 }, // Germany: January
                { "fr-FR", 1 }, // France: January
                { "es-ES", 1 }, // Spain: January
                { "it-IT", 1 }, // Italy: January
                { "ru-RU", 1 }, // Russia: January
                { "ko-KR", 1 }, // South Korea: January
                { "pt-BR", 1 }, // Brazil: January
                { "ar-SA", 1 }, // Saudi Arabia: January
                { "nl-NL", 1 }, // Netherlands: January
                { "tr-TR", 1 }, // Turkey: January
                { "sv-SE", 1 }, // Sweden: January
                { "pl-PL", 1 }, // Poland: January
                { "id-ID", 1 }, // Indonesia: January

                // European countries
                { "de-DE", 1 },  // Germany: January
                { "fr-FR", 1 },  // France: January
                { "es-ES", 1 },  // Spain: January
                { "it-IT", 1 },  // Italy: January
                { "nl-NL", 1 },  // Netherlands: January
                { "sv-SE", 1 },  // Sweden: January
                { "pl-PL", 1 },  // Poland: January
                { "ru-RU", 1 },  // Russia: January
                { "tr-TR", 1 },  // Turkey: January
                { "pt-PT", 1 },  // Portugal: January
                { "da-DK", 1 },  // Denmark: January
                { "fi-FI", 1 },  // Finland: January
                { "el-GR", 1 },  // Greece: January
                { "cs-CZ", 1 },  // Czech Republic: January
                { "hu-HU", 1 },  // Hungary: January
                { "ro-RO", 1 },  // Romania: January
                { "bg-BG", 1 },  // Bulgaria: January
                { "hr-HR", 1 },  // Croatia: January
                { "sk-SK", 1 },  // Slovakia: January
                { "sl-SI", 1 },  // Slovenia: January
                { "et-EE", 1 },  // Estonia: January
                { "lv-LV", 1 },  // Latvia: January
                { "lt-LT", 1 },  // Lithuania: January
                { "mt-MT", 1 },  // Malta: January
                { "cy-CY", 1 },  // Cyprus: January
                { "is-IS", 1 },  // Iceland: January
                { "no-NO", 1 },  // Norway: January
                { "ch-DE", 1 },  // Switzerland (German-speaking): January
                { "ch-FR", 1 },  // Switzerland (French-speaking): January
                { "ch-IT", 1 },  // Switzerland (Italian-speaking): January
                { "be-NL", 1 },  // Belgium (Dutch-speaking): January
                { "be-FR", 1 },  // Belgium (French-speaking): January
                { "lu-LU", 1 },  // Luxembourg: January
                { "li-LI", 1 },  // Liechtenstein: January
                { "al-AL", 1 },  // Albania: January
                { "ba-BA", 1 },  // Bosnia and Herzegovina: January
                { "mk-MK", 1 },  // North Macedonia: January
                { "me-ME", 1 },  // Montenegro: January
                { "rs-RS", 1 },  // Serbia: January
                { "ua-UA", 1 },  // Ukraine: January
                { "by-BY", 1 },  // Belarus: January
                { "md-MD", 1 },  // Moldova: January
                { "am-AM", 1 },  // Armenia: January
                { "ge-GE", 1 },  // Georgia: January
                { "az-AZ", 1 },  // Azerbaijan: January
                // ... additional cultures as needed
            };

            // Determine the start of the fiscal year for the given culture
            int fiscalYearStartMonth = fiscalYearStarts.TryGetValue(culture.Name, out int startMonth) ? startMonth : 1; // Default to January

            // Calculate the fiscal year
            int fiscalYear = (value.Month < fiscalYearStartMonth) ? value.Year - 1 : value.Year;

            // Determine the quarter
            int adjustedMonth = ((value.Month - fiscalYearStartMonth + 12) % 12) + 1;
            string quarter = adjustedMonth switch
            {
                >= 1 and <= 3 => "First quarter (Q1)",
                >= 4 and <= 6 => "Second quarter (Q2)",
                >= 7 and <= 9 => "Third quarter (Q3)",
                _ => "Fourth quarter (Q4)"
            };

            return $"{quarter}: {fiscalYearStartMonth} to {fiscalYearStartMonth + 2} " + fiscalYear;
        }

        /// <summary>
        /// Return a time-appropriate greeting
        /// Note: It sounds weird to start a greeting with "Good night", so if the periodname is "night", change it back to "evening".
        /// </summary>
        /// <param name="LocalTime"></param>
        /// <returns></returns>
        public static string Greeting(this DateTime LocalTime, CultureInfo Culture)
        {
            string _greeting;

            if (LocalTime.IsMorning(Culture))
            {
                _greeting = "Good morning";
            }
            else if (LocalTime.IsAfternoon(Culture))
            {
                _greeting = "Good afternoon";
            }
            else if (LocalTime.IsEvening(Culture))
            {
                _greeting = "Good evening";
            }
            else if (LocalTime.IsNight(Culture))
            {
                // "Good night" is typically used as a farewell greeting
                // Adjust the greeting as per your application's requirement
                _greeting = "Good night";
            }
            else
            {
                // Fallback greeting, in case none of the conditions are met
                _greeting = "Hello";
            }

            return _greeting;
        }

        public static bool IsMorning(this DateTime _localTime, CultureInfo _culture)
        {
            int _hour = Convert.ToInt32(_localTime.ToString("HH", _culture), _culture);
            return _hour is >= 5 and < 12;
        }

        public static bool IsAfternoon(this DateTime _localTime, CultureInfo _culture)
        {
            int _hour = Convert.ToInt32(_localTime.ToString("HH", _culture), _culture);
            return _hour is >= 12 and < 17;
        }

        public static bool IsEvening(this DateTime _localTime, CultureInfo _culture)
        {
            int _hour = Convert.ToInt32(_localTime.ToString("HH", _culture), _culture);
            return _hour is >= 17 and < 20; // Adjust the upper limit to 21 for a later evening
        }

        public static bool IsNight(this DateTime _localTime, CultureInfo _culture)
        {
            int _hour = Convert.ToInt32(_localTime.ToString("HH", _culture), _culture);
            return _hour is >= 20 or < 5; // Adjust the lower limit to 21 for a later evening start
        }

        public static string TimePeriodName(this DateTime LocalTime, CultureInfo Culture)
        {
            string Result = "";

            if (LocalTime.IsMorning(Culture))
            {
                Result = "morning";
            }
            else if (LocalTime.IsAfternoon(Culture))
            {
                Result = "afternoon";
            }
            else if (LocalTime.IsEvening(Culture))
            {
                Result = "evening";
            }
            else if (LocalTime.IsNight(Culture))
            {
                Result = "night";
            }

            return Result;
        }

        #endregion

        #region Enum

        /// <summary>
        /// Determines whether the enum value contains a specific value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        ///     <c>true</c> if value contains the specified value; otherwise, <c>false</c>.
        /// </returns>
        /// <example>
        /// <code>
        /// EnumExample dummy = EnumExample.Combi;
        /// if (dummy.Contains<EnumExample>(EnumExample.ValueA))
        /// {
        ///     Console.WriteLine("dummy contains EnumExample.ValueA");
        /// }
        /// </code>
        /// </example>
        public static bool Contains<T>(this Enum value, T request)
        {
            int valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            int requestAsInt = Convert.ToInt32(request, CultureInfo.InvariantCulture);

            if (requestAsInt == (valueAsInt & requestAsInt))
            {
                return true;
            }

            return false;
        }

        ///// <summary> 
        ///// Gets all items for an enum value. 
        ///// </summary> 
        ///// <typeparam name="T"></typeparam> 
        ///// <param name="value">The value.</param> 
        ///// <returns></returns> 
        //public static IEnumerable<T> GetAllItems<T>(this Enum value)
        //{
        //    foreach (object item in Enum.GetValues(typeof(T)))
        //    {
        //        yield return (T)item;
        //    }
        //}

        /// <summary>
        /// Gets all combined items from an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <example>
        /// Displays ValueA and ValueB.
        /// <code>
        /// EnumExample dummy = EnumExample.Combi;
        /// foreach (var item in dummy.GetAllSelectedItems<EnumExample>())
        /// {
        ///    Console.WriteLine(item);
        /// }
        /// </code>
        /// </example>
        public static IEnumerable<T> GetAllSelectedItems<T>(this Enum value)
        {
            int valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);

            foreach (object item in Enum.GetValues(typeof(T)))
            {
                int itemAsInt = Convert.ToInt32(item, CultureInfo.InvariantCulture);

                if (itemAsInt == (valueAsInt & itemAsInt))
                {
                    yield return (T)item;
                }
            }
        }

        /// <summary>
        /// Gets an item for an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetItem<T>(this Enum Type, int Value)
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                if (Convert.ToInt32(Type) == Value)
                {
                    yield return (T)item;
                }
            }
        }

        #endregion

        #region Expando

        public static void AddProperty(this ExpandoObject expando, string propertyName, object propertyValue)
        {
            if (expando != null)
            {
                // ExpandoObject supports IDictionary so we can extend it like this
                IDictionary<string, object> expandoDict = expando;

                if (expandoDict.ContainsKey(propertyName))
                {
                    expandoDict[propertyName] = propertyValue;
                }
                else
                {
                    expandoDict.Add(propertyName, propertyValue);
                }
            }
        }

        #endregion

        #region FileInfo 

        //public static void CopyTo(this FileInfo SourceInfo, FileInfo DestinationInfo, Action<int> ProgressCallbackMethod) 
        //{ 
        //    const int BUFFERSIZE = 1024 * 1024;    // 1MB 
        //    byte[] Buffer = new byte[BUFFERSIZE]; 
        //    byte[] Buffer2 = new byte[BUFFERSIZE]; 
        //    bool Swap = false; 
        //    int Progress = 0; 
        //    int ReportedProgress = 0; 
        //    int read = 0; 
        //    long len = SourceInfo.Length; 
        //    float flen = len; 
        //    Task writer = null; 

        //    using (FileStream SourceStream = SourceInfo.OpenRead()) 
        //    using (FileStream DestinationStream = DestinationInfo.OpenWrite()) 
        //    { 
        //        DestinationStream.SetLength(SourceStream.Length); 

        //        for (long size = 0; size < len; size += read) 
        //        { 
        //            if ((Progress = ((int)((size / flen) * 100))) != ReportedProgress) 
        //            { 
        //                ProgressCallbackMethod(ReportedProgress = Progress); 
        //            } 

        //            read = SourceStream.Read(Swap ? Buffer : Buffer2, 0, BUFFERSIZE); 

        //            writer?.Wait(); 

        //            writer = DestinationStream.WriteAsync(Swap ? Buffer : Buffer2, 0, read); 

        //            Swap = !Swap; 
        //        } 

        //        writer?.Wait(); 
        //    } 
        //} 

        //public static Task<Utilities.CopyResult> CopyTo(this FileInfo SourceInfo, FileInfo DestinationInfo, Action<int> ProgressCallback) 
        //{ 
        //    const int BUFFERSIZE = 1024 * 1024;    // 1MB 
        //    byte[] Buffer = new byte[BUFFERSIZE]; 
        //    byte[] Buffer2 = new byte[BUFFERSIZE]; 
        //    bool Swap = false; 
        //    int Progress = 0; 
        //    int ReportedProgress = 0; 
        //    long BytesRead = 0; 
        //    long SourceLength = SourceInfo.Length; 
        //    float SourceLength2 = SourceLength; 
        //    Task WriteTask = null; 
        //    TaskCompletionSource<Utilities.CopyResult> tcs = new TaskCompletionSource<Utilities.CopyResult>(); 

        //    try 
        //    { 
        //        using (FileStream SourceStream = SourceInfo.OpenRead()) 
        //        using (FileStream DestinationStream = DestinationInfo.OpenWrite()) 
        //        { 
        //            DestinationStream.SetLength(SourceStream.Length); 

        //            for (long ReadPosition = 0; ReadPosition < SourceLength; ReadPosition += BytesRead) 
        //            { 
        //                if ((Progress = ((int)((ReadPosition / SourceLength2) * 100))) != ReportedProgress) 
        //                { 
        //                    ProgressCallback(ReportedProgress = Progress); 
        //                } 

        //                BytesRead = SourceStream.Read(Swap ? Buffer : Buffer2, 0, BUFFERSIZE); 

        //                WriteTask?.Wait(); 

        //                WriteTask = DestinationStream.WriteAsync(Swap ? Buffer : Buffer2, 0, (int)BytesRead); 

        //                Swap = !Swap; 
        //            } 

        //            WriteTask?.Wait(); 

        //        } 

        //        ProgressCallback(100); 

        //        DestinationInfo.CreationTime = SourceInfo.CreationTime; 
        //        DestinationInfo.LastAccessTime = SourceInfo.LastAccessTime; 
        //        DestinationInfo.LastWriteTime = SourceInfo.LastWriteTime; 

        //        tcs.TrySetResult(Utilities.CopyResult.Success); 
        //    } 
        //    catch //(Exception e) 
        //    { 
        //        tcs.TrySetResult(Utilities.CopyResult.UnexpectedException); 
        //    } 

        //    return tcs.Task; 
        //} 

        /// <summary>
        /// Permanently delete the file, attempting to do so MaximumAttempts time, with a pause of PauseTime (Milliseconds) between each attempt
        /// </summary>
        /// <param name="Instance">The FileInfo instance to delete</param>
        /// <param name="MaximumAttempts">The maximum number of deletion attempts</param>
        /// <param name="PauseTime">The time in Milliseconds to pause between deletion attempts</param>
        /// <returns>True = Success</returns>
        public static bool DeleteWithRetry(this System.IO.FileInfo Instance, int MaximumAttempts = 3, int PauseTime = 250)
        {
            bool Result = false;
            int DeleteCounter = 1;
            string Path = Instance.FullName;

            do
            {
                try
                {
                    System.IO.File.Delete(Path);
                    Result = true;
                }
                catch
                {
                    DeleteCounter++;
                    System.Threading.Thread.Sleep(PauseTime);
                }
            }
            while (Result == false && DeleteCounter < MaximumAttempts);

            return Result;
        }

        /// <summary>
        /// Print the target file on the default printer
        /// </summary>
        /// <param name="value"></param>
        public static void Print(this FileInfo value)
        {
            Process p = new Process();
            p.StartInfo.FileName = value.FullName;
            p.StartInfo.Verb = "Print";
            _ = p.Start();
        }

        #endregion

        #region Form

        public static void SendToDisplay(this Form WindowsForm, int DisplayIndex)
        {
            Screen[] screens = Screen.AllScreens;

            if (DisplayIndex >= 0 && DisplayIndex < screens.Length)
            {
                bool StartingMaximisedState = false;

                if (WindowsForm.WindowState == FormWindowState.Maximized)
                {
                    WindowsForm.WindowState = FormWindowState.Normal;
                    StartingMaximisedState = true;
                }

                WindowsForm.Location = screens[DisplayIndex].WorkingArea.Location;

                if (StartingMaximisedState)
                {
                    WindowsForm.WindowState = FormWindowState.Maximized;
                }
            }
        }

        #endregion

        #region Icon 

#if Windows
        public static void Destroy(this Icon i)
        {
            if (i != null)
            {
                DestroyIcon(i.Handle);
            }
        } 
#endif

#if Windows
        public static Icon? ToIcon(this Image i, Size OutputSize)
        {
            if (i != null)
            {
                Bitmap NewBitmap = new Bitmap(i, OutputSize);
                IntPtr Hicon = NewBitmap.GetHicon();// Get an Hicon for myBitmap. 

                return Icon.FromHandle(Hicon);// Create a new icon from the handle 
            }

            return null;
            // ==================================================================== 

            //Bitmap NewBitmap = new Bitmap(i); 

            //Bitmap SmallImage = (Bitmap)NewBitmap.GetThumbnailImage(32, 32, null, IntPtr.Zero); 

            //SmallImage.MakeTransparent(); 
            //return Icon.FromHandle(SmallImage.GetHicon()); 

        }
#endif

        #endregion

        #region IDictionary<string, T>

        public static void AddValue<T>(this IDictionary<string, T> Source, string Key, T Value)
        {
            if (Source == null)
            {
                return;
            }

            if (Source.ContainsKey(Key))
            {
                _ = Source.Remove(Key);
                Source.Add(Key, Value);
            }
            else
            {
                Source.Add(Key, Value);
            }
        }

        public static void RemoveValue<T>(this IDictionary<string, T> Source, string Key)
        {
            if (Source == null)
            {
                return;
            }

            if (Source.ContainsKey(Key))
            {
                _ = Source.Remove(Key);
            }
        }

        #endregion

        #region IDictionary<string, object>

        public static dynamic? DictionaryToObject(this IDictionary<string, object> dictionary)
        {
            ExpandoObject expandoObj = new ExpandoObject();
            ICollection<KeyValuePair<string, object>> expandoObjCollection = expandoObj;

            if (dictionary == null)
            {
                return null;
            }

            foreach (KeyValuePair<string, object> keyValuePair in dictionary)
            {
                expandoObjCollection.Add(keyValuePair);
            }

            dynamic eoDynamic = expandoObj;

            return eoDynamic;
        }

        public static T? DictionaryToObject<T>(this IDictionary<string, object> dictionary) where T : class
        {
            return DictionaryToObject(dictionary) as T;
        }

        #endregion

        #region IDictionary<string, string>

        public static T DictionaryToObject<T>(this IDictionary<string, string> dict) where T : new()
        {
            return DictionaryToObject<T>(dict, CultureInfo.CurrentCulture);
        }

        public static T DictionaryToObject<T>(this IDictionary<string, string> dict, CultureInfo Culture) where T : new()
        {
            T t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                KeyValuePair<string, string> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

                // ...and change the type
                object newA = Convert.ChangeType(item.Value, newT, Culture);

                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }

            return t;
        }

        #endregion

        #region Image 

#if Windows
        public static Image? MakeTransparentRange(this Image img, Color ReplacementColor, int RedTolerance, int GreenTolerance, int BlueTolerance, int AlphaRange = 255)
        {
            if (img != null)
            {
                //create an empty Bitmap image 
                Bitmap bmp = (Bitmap)img;

                for (int r = ReplacementColor.R - RedTolerance; r < ReplacementColor.R + RedTolerance; r++)
                {
                    for (int g = ReplacementColor.G - GreenTolerance; g < ReplacementColor.G + GreenTolerance; g++)
                    {
                        for (int b = ReplacementColor.B - BlueTolerance; b < ReplacementColor.B + BlueTolerance; b++)
                        {
                            for (int a = ReplacementColor.A - AlphaRange; a < ReplacementColor.A + AlphaRange; a++)
                            {
                                Color ColorToReplace = Color.FromArgb(AlphaRange, r, g, b);

                                bmp.MakeTransparent(ColorToReplace);
                            }
                        }
                    }
                }

                return bmp;
            }

            return null;
        } 
#endif

#if Windows
        /// <summary> 
        /// Print the target image on the default printer 
        /// </summary> 
        /// <param name="Image"></param> 
        public static void Print(this Image Image)
        {
            PrintDocument pd = new PrintDocument();

            pd.OriginAtMargins = true;
            pd.DefaultPageSettings.Landscape = false;

            pd.PrintPage += (sender, args) =>
            {
                float newWidth = Image.Width * 100 / Image.HorizontalResolution;
                float newHeight = Image.Height * 100 / Image.VerticalResolution;

                float widthFactor = newWidth / args.MarginBounds.Width;
                float heightFactor = newHeight / args.MarginBounds.Height;

                if (widthFactor > 1 | heightFactor > 1)
                {
                    if (widthFactor > heightFactor)
                    {
                        newWidth = newWidth / widthFactor;
                        newHeight = newHeight / widthFactor;
                    }
                    else
                    {
                        newWidth = newWidth / heightFactor;
                        newHeight = newHeight / heightFactor;
                    }
                }

                //System.Windows.Forms.MessageBox.Show("Pre quality adjustment"); 

                args.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                args.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                //System.Windows.Forms.MessageBox.Show("Pre DrawImage"); 

                args.Graphics.DrawImage(Image, 0, 0, (int)newWidth, (int)newHeight);

                //System.Windows.Forms.MessageBox.Show("Post DrawImage"); 

            };

            //System.Windows.Forms.MessageBox.Show("Pre print"); 

            pd.Print();

            //System.Windows.Forms.MessageBox.Show("Post Print"); 

            pd.Dispose();

            //System.Windows.Forms.MessageBox.Show("Post Dispose"); 

            pd = null;

            //System.Windows.Forms.MessageBox.Show("Post Null"); 
        } 
#endif

#if Windows
        /// <summary> 
        /// method to rotate an image either clockwise or counter-clockwise 
        /// </summary> 
        /// <param name="img">the image to be rotated</param> 
        /// <param name="Angle">the angle (in degrees). 
        /// NOTE: 
        /// Positive values will rotate clockwise 
        /// negative values will rotate counter-clockwise 
        /// </param> 
        /// <returns></returns> 
        public static Image? Rotate(this Image img, float Angle)
        {
            if (img != null)
            {
                //create an empty Bitmap image 
                Bitmap bmp = new Bitmap(img.Width, img.Height);

                // Set the resolution so that different DPI screens display the resultant image correctly 
                bmp.SetResolution(img.Width, img.Height);

                //turn the Bitmap into a Graphics object 
                Graphics gfx = Graphics.FromImage(bmp);

                //now we set the rotation point to the center of our image 
                gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

                //now rotate the image 
                gfx.RotateTransform(Angle);

                gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

                //set the InterpolationMode to HighQualityBicubic so to ensure a high 
                //quality image once it is transformed to the specified size 
                gfx.InterpolationMode = InterpolationMode.HighQualityBilinear; // HighQualityBicubic 

                //now draw our new image onto the graphics object 
                gfx.DrawImage(img, new Point(0, 0));

                //dispose of our Graphics object 
                gfx.Dispose();

                //return the image 
                return bmp;
            }

            return null;
        } 
#endif

#if Windows
        /// <summary>
        /// Convert supplied Image to ByteArray
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            return ms.ToArray();
        } 
#endif

#if Windows
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap Resize(this Image image, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        } 
#endif

        #endregion

        #region int 

        public static string CalendarQuarter(this int Value)
        {
            return Value switch
            {
                1 => "January",
                2 => "April",
                3 => "July",
                4 => "October",
                _ => "Error:  Invalid quarter for " + Value.ToString(),
            };
        }

        public static string FinancialQuarter(this int Value)
        {
            return Value switch
            {
                1 => "July",
                2 => "October",
                3 => "January",
                4 => "April",
                _ => "Error:  Invalid quarter for " + Value.ToString(),
            };
        }

        public static string Ordinal(this int Number)
        {
            string Result = Number.ToString();

            Number %= 100;

            if (Number is >= 11 and <= 13)
            {
                return Result + "th";
            }

            return (Number % 10) switch
            {
                1 => Result + "st",
                2 => Result + "nd",
                3 => Result + "rd",
                _ => Result + "th",
            };
        }

        public static string ToMemorySize(this int value, int decimalPlaces = 0)
        {
            return ((long)value).ToMemorySize(decimalPlaces);
        }

        public static string ToStringEx(this int Input, CultureInfo Culture)
        {
            string Result = Input.ToString(Culture);

            return Result;
        }

        #endregion

        #region List<string> 

        /// <summary>
        /// Converts a <see cref="List{string}"/> containing string values to a comma separated value.
        /// </summary>
        /// <param name="Values">Refers to this (current List object)</param>
        /// <returns>A string containing the CSV</returns>
        public static string ToCSV(this List<string> Values)
        {
            string returnValue = String.Empty;

            foreach (string value in Values)
            {
                returnValue += String.Format("{0},", value);
            }

            return returnValue.Remove(returnValue.Length - 1);
        }

        #endregion

        #region List<T> 

        public static bool Approximates<T>(this List<T> Values, List<T> Comparison)
        {
            return Values.Count == Comparison.Count &&
                   Values.All(Comparison.Contains) &&
                   Comparison.All(Values.Contains);
        }

        /// <summary>
        /// A generic function that loops a List of any class type, looking for a boolean field value.
        /// If it finds a TRUE value, then exit the loop and return TRUE.
        /// </summary>
        /// <typeparam name="T">Generic reference</typeparam>
        /// <param name="Values">Refers to this (current List object)</param>
        /// <param name="FieldName">The field containing the boolean value</param>
        /// <returns>Boolean True or False</returns>
        public static bool AreAnyOfTheseRequired<T>(this List<T> Values, string FieldName)
        {
            bool returnValue = false;

            if (Values == null || Values.Count == 0 || String.IsNullOrEmpty(FieldName))
            {
                return false;
            }

            foreach (T? item in Values)
            {
                if (item != null)
                {
                    if ((bool)item.GetType().GetProperty(FieldName).GetValue(item, null))
                    {
                        returnValue = true;
                        break;
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Compares two lists and returns a new List containing all values from both sets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source">List One</param>
        /// <param name="Compare">List Two</param>
        /// <returns>List of values</returns>
        public static List<T> Combine<T>(this List<T> Source, List<T> Compare)
        {
            return Source.Concat(Compare)
                         .ToList<T>();
        }

        /// <summary>
        /// Compares two lists and returns a new List containing distinct values from both sets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source">List One</param>
        /// <param name="Compare">List Two</param>
        /// <returns>List of values</returns>
        public static List<T> CombineDistinct<T>(this List<T> Source, List<T> Compare)
        {
            // When...
            // Source  = 1,2,3,4,5
            // Compare = 4,5,6
            return Source.Union(Compare) // Result = 1, 2, 3, 4, 5, 6
                         .ToList<T>();

            // NOTE:  This is the same as:
            // Source.Concat(Compare).Distinct().ToList<T>; 
        }

        /// <summary>
        /// Compares two lists and returns a new List containing values that are not in both sets
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source">List One</param>
        /// <param name="Compare">List Two</param>
        /// <returns>List of values</returns>
        public static List<T> Difference<T>(this List<T> Source, List<T> Compare)
        {
            // When...
            // Source  = 1,2,3,4,5
            // Compare = 4,5,6
            return Source.Except(Compare)             // Result = 1, 2, 3
                          .Union(
                                 Compare.Except(Source) // Result = 6
                                 )                      // Result = 1, 2, 3, 6
                          .ToList<T>();
        }

        /// <summary>
        /// Compares two lists and returns a new List containing values that are only in the first set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source">List One</param>
        /// <param name="Compare">List Two</param>
        /// <returns>List of values</returns>
        public static List<T> OnlyInFirstSet<T>(this List<T> Source, List<T> Compare)
        {
            return Source.Except(Compare).ToList<T>();
        }

        /// <summary>
        /// Compares two lists and returns a new List containing values that are only in the second set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Source">List One</param>
        /// <param name="Compare">List Two</param>
        /// <returns>List of values</returns>
        public static List<T> OnlyInSecondSet<T>(this List<T> Source, List<T> Compare)
        {
            return Compare.Except(Source).ToList<T>();
        }

        public static DataTable ToDataTable<T>(this List<T> Values)
        {
            DataTable table = new DataTable();

            string ValueFieldName = "Value";

            //Create the DataTable with the desired columns:
            _ = table.Columns.Add(ValueFieldName, typeof(T));

            //Add the items from the enum:
            foreach (T Value in Values)
            {
                _ = table.Rows.Add(Value);
            }

            return table;
        }

        #endregion

        #region ListBox.ObjectCollection 

        public static List<string> ToList(this ListBox.ObjectCollection List)
        {
            List<string> Result = new List<string>();

            foreach (object Item in List)
            {
                string s = (string)Item;

                Result.Add(s);
            }

            return Result;
        }

        public static List<T> ToList<T>(this ListBox.ObjectCollection List)
        {
            List<T> Result = new List<T>();

            foreach (object Item in List)
            {
                Result.Add((T)Item);
            }

            return Result;
        }

        #endregion

        #region ListBox.SelectedObjectCollection

        public static List<string> ToList(this ListBox.SelectedObjectCollection List)
        {
            List<string> Result = new List<string>();

            foreach (object Item in List)
            {
                string s = (string)Item;

                Result.Add(s);
            }

            return Result;
        }

        public static List<T> ToList<T>(this ListBox.SelectedObjectCollection List)
        {
            List<T> Result = new List<T>();

            foreach (object Item in List)
            {
                Result.Add((T)Item);
            }

            return Result;
        }

        #endregion

        #region ListView.ListViewItemCollection 

        /// <summary>
        /// Provides access to ListViewItemCollection for Cross-Thread operations
        /// </summary>
        /// <param name="listView">The ListView to query</param>
        /// <returns>ListViewItemCollection</returns>
        public static ListView.ListViewItemCollection GetItems(this ListView listView)
        {
            ListView.ListViewItemCollection temp = new ListView.ListViewItemCollection(new ListView());
            if (!listView.InvokeRequired)
            {
                foreach (ListViewItem item in listView.Items)
                {
                    _ = temp.Add((ListViewItem)item.Clone());
                }

                return temp;
            }
            else
            {
                return (ListView.ListViewItemCollection)listView.Parent.Invoke(new DelegateGetItems(GetItems), new object[] { listView });
            }
        }

        #endregion

        public static ScrollBars GetVisibleScrollbars(this Control ctl)
        {
            int wndStyle = GetWindowLong(ctl.Handle, GWL_STYLE);
            bool hsVisible = (wndStyle & WS_HSCROLL) != 0;
            bool vsVisible = (wndStyle & WS_VSCROLL) != 0;

            if (hsVisible)
            {
                if (vsVisible)
                {
                    return ScrollBars.Both;
                }
                else
                {
                    return ScrollBars.Horizontal;
                }
            }
            else
            {
                if (vsVisible)
                {
                    return ScrollBars.Vertical;
                }
                else
                {
                    return ScrollBars.None;
                }
            }
        }

        #region long 

        public static string ToMemorySize(this long value, int decimalPlaces = 0)
        {
            double asTb = Math.Round((double)value / OneTb, decimalPlaces);
            double asGb = Math.Round((double)value / OneGb, decimalPlaces);
            double asMb = Math.Round((double)value / OneMb, decimalPlaces);
            double asKb = Math.Round((double)value / OneKb, decimalPlaces);

            string chosenValue = asTb > 1 ? string.Format("{0} Tb", asTb)
                : asGb > 1 ? string.Format("{0} Gb", asGb)
                : asMb > 1 ? string.Format("{0} Mb", asMb)
                : asKb > 1 ? string.Format("{0} Kb", asKb)
                : string.Format("{0}B", Math.Round((double)value, decimalPlaces));
            return chosenValue;
        }

        #endregion

        #region ManagementClass 

        public static bool ContainsProperty(this System.Management.ManagementClass Class, string PropertyName)
        {
            PropertyInfo? Info = Class.GetType().GetProperty(PropertyName);

            return Info != null;
        }

        #endregion

#if Windows
        #region ManagementObject 

        public static bool Contains(this ManagementObject Value, string Name)
        {
            if (Value == null)
            {
                return false;
            }

            try
            {
                PropertyDataCollection Properties = Value.Properties;

                foreach (PropertyData Data in Properties)
                {
                    if ((string)Data.Name == Name)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        public static bool ValueToBool(this ManagementObject Value, string Name)
        {
            bool Result = false;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (bool)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static bool[] ValueToBoolArray(this ManagementObject Value, string Name)
        {
            bool[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (bool[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static DateTime ValueToDateTime(this ManagementObject Value, string Name)
        {
            DateTime Result = DateTime.MinValue;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    string IntermediateResult = (string)Value.GetPropertyValue(Name);

                    if (IntermediateResult != "00000000000000.000000+000")
                    {
                        Result = System.Management.ManagementDateTimeConverter.ToDateTime(IntermediateResult);
                    }
                }
            }
            catch (ManagementException)
            {
            }
            catch (Exception)
            {
            }

            return Result;
        }

        public static DateTime[] ValueToDateTimeArray(this ManagementObject Value, string Name)
        {
            string[] IntermediateResults = { };
            List<DateTime> Results = new List<DateTime>();
            DateTime[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                IntermediateResults = (string[])Value.GetPropertyValue(Name);

                foreach (string TimeStamp in IntermediateResults)
                {
                    Results.Add(System.Management.ManagementDateTimeConverter.ToDateTime(TimeStamp));
                }

                Result = Results.ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static int ValueToInt(this ManagementObject Value, string Name)
        {
            int Result = 0;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (int)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static short ValueToInt16(this ManagementObject Value, string Name)
        {
            short Result = 0;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (short)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static short[] ValueToInt16Array(this ManagementObject Value, string Name)
        {
            short[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (short[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static int ValueToInt32(this ManagementObject Value, string Name)
        {
            int Result = 0;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (int)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static int[] ValueToInt32Array(this ManagementObject Value, string Name)
        {
            int[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (int[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static long ValueToInt64(this ManagementObject Value, string Name)
        {
            long Result = 0;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (long)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static long[] ValueToInt64Array(this ManagementObject Value, string Name)
        {
            long[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (long[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static int[] ValueToIntArray(this ManagementObject Value, string Name)
        {
            int[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (int[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static string ValueToString(this ManagementObject Value, string Name)
        {
            string Result = "";

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (string)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static string[] ValueToStringArray(this ManagementObject Value, string Name)
        {
            string[] Result = { };

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (string[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static uint ValueToUInt(this ManagementObject Value, string Name)
        {
            uint Result = 0;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (uint)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static ushort ValueToUInt16(this ManagementObject Value, string Name)
        {
            ushort Result = 0;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (ushort)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static ushort[] ValueToUInt16Array(this ManagementObject Value, string Name)
        {
            ushort[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (ushort[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static uint ValueToUInt32(this ManagementObject Value, string Name)
        {
            uint Result = 0;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (uint)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static uint[] ValueToUInt32Array(this ManagementObject Value, string Name)
        {
            uint[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (uint[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static ulong ValueToUInt64(this ManagementObject Value, string Name)
        {
            ulong Result = 0;

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (ulong)Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static ulong[] ValueToUInt64Array(this ManagementObject Value, string Name)
        {
            ulong[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (ulong[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        public static uint[] ValueToUIntArray(this ManagementObject Value, string Name)
        {
            uint[] Result = { };

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value.GetPropertyValue(Name) != null)
                {
                    Result = (uint[])Value.GetPropertyValue(Name);
                }
            }
            catch
            {
            }

            return Result;
        }

        #endregion
#endif

        #region Object 

        /// <summary>
        /// Remove the specified Event from the target Object
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="EventName">The Event name to remove</param>
        public static void ClearEventInvocations(this object Value, string EventName)
        {
            if (Value != null)
            {
                FieldInfo? fi = Value.GetType().GetEventField(EventName);

                if (fi == null)
                {
                    return;
                }

                fi.SetValue(Value, null);
            }
        }

        /// <summary>
        /// Determines if the supplied value is an array
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>bool indicating Success</returns>
        public static bool IsArray(this object Value)
        {
            return Value is Array;
        }

        /// <summary>
        /// Convert the target object into an array of <see cref="byte"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>Array of <see cref="byte"/></returns>
        public static byte[]? ToByteArray(this object Value)
        {
            if (Value == null)
            {
                return null;
            }

            //BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            Serializer.Serialize(ms, Value);

            return ms.ToArray();
        }

        #endregion

        #region PropertyCollection 

        //public static DateTime[] AttributeValueToDateTimeArray(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    DateTime[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (DateTime[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

#if Windows
public static bool AttributeValueToBool(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            bool Result = false;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = Convert.ToBoolean(Value[Name].Value);
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

        //public static int[] AttributeValueToIntArray(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    int[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (int[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

        //public static UInt32[] AttributeValueToUInt32Array(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    UInt32[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (UInt32[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

        //public static uint[] AttributeValueToUIntArray(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    uint[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (uint[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

#if Windows
        public static DateTime AttributeValueToDateTime(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            DateTime Result = DateTime.MinValue;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = Convert.ToDateTime(Value[Name].Value);
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

#if Windows
        public static int AttributeValueToInt(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            int Result = 0;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = Convert.ToInt32(Value[Name].Value);
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

#if Windows
        public static short AttributeValueToInt16(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            short Result = 0;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = (short)Value[Name].Value;
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

#if Windows
        public static int AttributeValueToInt32(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            int Result = 0;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = Convert.ToInt32(Value[Name].Value);
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

        //public static Int32[] AttributeValueToInt32Array(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    Int32[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = Convert.ToInt32(Value[Name].Value); 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

#if Windows
        public static long AttributeValueToInt64(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            long Result = 0;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = Convert.ToInt64(Value[Name].Value);
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

#if Windows
        public static string AttributeValueToString(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            string Result = "";

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result += (string)Value[Name].Value;
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

        //public static UInt64[] AttributeValueToUInt64Array(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    UInt64[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (UInt64[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

#if Windows
        public static uint AttributeValueToUInt(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            uint Result = 0;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = Convert.ToUInt32(Value[Name].Value);
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

        //public static string[] AttributeValueToStringArray(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    string[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (string[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

#if Windows
        public static ushort AttributeValueToUInt16(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            ushort Result = 0;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = (ushort)Value[Name].Value;
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

        //public static Int16[] AttributeValueToInt16Array(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    Int16[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (Int16[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

        //public static UInt16[] AttributeValueToUInt16Array(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    UInt16[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (UInt16[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

#if Windows
        public static uint AttributeValueToUInt32(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            uint Result = 0;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = (uint)Value[Name].Value;
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

        //public static Int64[] AttributeValueToInt64Array(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    Int64[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (Int64[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

#if Windows
        public static ulong AttributeValueToUInt64(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            ulong Result = 0;

            if (Value == null)
            {
                return Result;
            }

            if (!Value.Contains(Name))
            {
                return Result;
            }

            try
            {
                if (Value[Name].Value != null)
                {
                    Result = Convert.ToUInt64(Value[Name].Value);
                }
            }
            catch
            {
            }

            return Result;
        }  
#endif

        public static bool Contains(this System.DirectoryServices.PropertyCollection Value, string Name)
        {
            if (Value == null)
            {
                return false;
            }

            try
            {
                foreach (string Data in Value)
                {
                    if (Data == Name)
                    {
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        //public static bool[] AttributeValueToBoolArray(this System.DirectoryServices.PropertyCollection Value, string Name) 
        //{ 
        //    bool[] Result = { }; 

        //    if (Value == null) 
        //    { 
        //        return Result; 
        //    } 

        //    if (!Value.Contains(Name)) 
        //    { 
        //        return Result; 
        //    } 

        //    try 
        //    { 
        //        if (Value[Name].Value != null) 
        //        { 
        //            Result = (bool[])Value[Name].Value; 
        //        } 
        //    } 
        //    catch 
        //    { 
        //    } 

        //    return Result; 
        //} 

        #endregion

        #region SerialPort

        //public static byte[] Bytes(this System.IO.Ports.SerialPort port, byte[] ReceivedData)
        //{
        //    byte[] Result = null;

        //    return Result;
        //}

        #endregion

        #region string 

#if Windows
        /// <summary>
        /// Convert the target <see cref="string"/> into a <see cref="DateTime"/> and add the current TimeZone offset
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>The current <see cref="Datetime"/> plus the current Timezone offset</returns>
        public static DateTime AddTimeZone(this string? Value)
        {
            DateTime localTime = ManagementDateTimeConverter.ToDateTime(Value);

            // Add the current timezone offset 
            localTime += System.TimeZoneInfo.Utc.GetUtcOffset(DateTime.Now); //System.TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

            return localTime;
        }
#endif

        /// <summary>
        /// Convert the given <see cref="string"/> into the target type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns>The target value, as the target type</returns>
        public static T? As<T>(this string? Value)
        {
            return As(Value, default(T));
        }

        /// <summary>
        /// Convert the given <see cref="string"/> into the target type.  If the value is null or Empty, return the default value instead
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="DefaultValue">The value to return if the target value is null or Empty</param>
        /// <returns>The target value, as the target type</returns>
        public static T? As<T>(this string? Value, T DefaultValue)
        {
            if (DefaultValue is null)
            {
                return default(T);
            }

            if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(AsBool(Value,
                                                    Convert.ToBoolean(DefaultValue)),
                                                    typeof(T));
            }

            T? result = default(T);

            if (String.IsNullOrEmpty(Value))
            {
                return DefaultValue;
            }

            try
            {
                Type? underlyingType = Nullable.GetUnderlyingType(typeof(T));

                if (underlyingType == null)
                {
                    result = (T)Convert.ChangeType(Value, typeof(T));
                }
                else if (underlyingType == typeof(bool))
                {
                    result = (T)Convert.ChangeType(AsBool(Value,
                                                    Convert.ToBoolean(DefaultValue)),
                                                    typeof(T));
                }
                else
                {
                    result = (T)Convert.ChangeType(Value, underlyingType);
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// Convert the given <see cref="string"/> into a <see cref="bool"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>The target value, as a <see cref="bool"/></returns>
        public static bool AsBool(this string? Value)
        {
            return AsBool(Value, false);
        }

        /// <summary>
        /// Convert the given <see cref="string"/> into a <see cref="bool"/>.  If the value is null or Empty, return the default value instead
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="DefaultValue">The value to return if the target value is null or Empty</param>
        /// <returns>The target value, as a <see cref="bool"/></returns>
        public static bool AsBool(this string? Value, bool DefaultValue)
        {
            if (String.IsNullOrEmpty(Value))
            {
                return DefaultValue;
            }

            return Value.ToLower() switch
            {
                "1" or "t" or "true" => true,
                "0" or "f" or "false" => false,
                _ => DefaultValue,
            };
        }

        public static string? Capitalise(this string InputString)
        {
            string Result = InputString;

            if (InputString == null)
            {
                return null;
            }

            InputString = InputString.ToLower();

            if (InputString.Length > 1)
            {   // Capitalise everything to the right of a space or hyphen
                StringBuilder r = new StringBuilder();

                // First character is always uppercase
                _ = r.Append(InputString[0].ToString().ToUpper());

                for (int i = 1; i < InputString.Length; i++)
                {
                    if (InputString[i - 1] == (char)32 || InputString[i - 1] == Convert.ToChar("-"))
                    {
                        _ = r.Append(InputString[i].ToString().ToUpper());
                    }
                    else
                    {
                        _ = r.Append(InputString[i]);
                    }
                }

                Result = r.ToString();
            }
            else
            {
                return Result.ToUpper();
            }

            return Result;
        }

        public static double CompareWith(this string OriginalValue, string ComparisonValue)
        {
            double Result = 0.0;
            string[] OriginalWords = Array.Empty<string>();
            string[] CompareWords = Array.Empty<string>();

            if (OriginalWords.Length == 0 || CompareWords.Length == 0)
            {
                return 0;
            }

            OriginalWords = OriginalValue.Words();
            CompareWords = ComparisonValue.Words();

            // Determine how many words from the Original are in the Comparison, case-insensitive
            IEnumerable<string>? CommonWords = OriginalWords!.Intersect(CompareWords!, StringComparer.OrdinalIgnoreCase);

            double CountOfCommonWords = CommonWords.Count();
            double CountOfCompareWords = CompareWords.Length;
            double CountOfOriginalWords = OriginalWords.Length;

            if (CountOfCommonWords == CountOfOriginalWords && CountOfCompareWords == CountOfOriginalWords) // All of the original words are in the compare string
            {
                Result = 100;
            }
            else // Result is a percentage.  100% is equal
            {
                if (CountOfOriginalWords == 0 || CountOfCommonWords == 0 || CountOfCompareWords == 0)
                {
                    Result = 0;
                }
                else
                {
                    if (CountOfOriginalWords >= CountOfCompareWords)
                    {
                        Result = CountOfCommonWords / CountOfOriginalWords * 100;
                    }
                    else if (CountOfCompareWords > CountOfOriginalWords)
                    {
                        Result = CountOfCommonWords / CountOfCompareWords * 100;
                    }
                }
            }

            return Result;
        }

        public static double CompareWith(this string OriginalValue, string ComparisonValue, List<string> CommonWordsToRemove, bool IgnoreCase)
        {
            if (OriginalValue == ComparisonValue)
            {
                return 100.0;
            }

            if (OriginalValue == null || ComparisonValue == null)
            {
                return 0.0;
            }

            if (IgnoreCase)
            {
                OriginalValue = OriginalValue.ToLowerEx();
                ComparisonValue = ComparisonValue.ToLowerEx();
            }

            if (CommonWordsToRemove != null && CommonWordsToRemove.Count > 0)
            {
                foreach (string CommonWord in CommonWordsToRemove)
                {
                    if (IgnoreCase)
                    {
                        string? LowerCommonWord = CommonWord.ToLowerEx();

                        if (LowerCommonWord != null)
                        {
                            OriginalValue = OriginalValue!.ToLowerEx().Replace(LowerCommonWord, "").Trim(); // , System.StringComparison.OrdinalIgnoreCase
                            ComparisonValue = ComparisonValue!.ToLowerEx().Replace(LowerCommonWord, "").Trim(); // , System.StringComparison.OrdinalIgnoreCase
                        }
                    }
                    else
                    {
                        OriginalValue = OriginalValue.Replace(CommonWord, "").Trim(); // , System.StringComparison.OrdinalIgnoreCase
                        ComparisonValue = ComparisonValue.Replace(CommonWord, "").Trim(); // , System.StringComparison.OrdinalIgnoreCase
                    }
                }
            }

            return CompareWith(OriginalValue, ComparisonValue);
        }

        // https://stackoverflow.com/questions/30092463/hash-function-to-generate-16-alphanumerical-characters-from-input-string-in-c-sh
        public static string ConstantLengthHash(this string input, string chars = "234679ACDEFGHJKLMNPQRTUVWXYZ")
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);

            using (SHA256 hashstring = SHA256.Create())
            {
                byte[] hash = hashstring.ComputeHash(bytes);

                char[] hash2 = new char[16];

                // Note: There is a loss of information due to converting each byte to a character from 'chars'
                for (int i = 0; i < hash2.Length; i++)
                {
                    hash2[i] = chars[hash[i] % chars.Length];
                }

                return new string(hash2);
            }
        }

        public static bool ContainsEx(this string? Input, string Value)
        {
            bool Result = false;

            if (Input == null)
            {
                return false;
            }

#if NETCOREAPP || NETFRAMEWORK
            Result = Input.Contains(Value, StringComparison.CurrentCultureIgnoreCase);
#else
                Result = Input.ToLower().Contains(Value.ToLower());
#endif

            return Result;
        }

        public static bool EndsWithEx(this string? Input, string Value, bool IgnoreCase)
        {
            CultureInfo provider = CultureInfo.CurrentCulture;

            if (Input == null || Value == null)
            {
                return false;
            }

            return Input.EndsWith(Value, IgnoreCase, provider);
        }

        /// <summary>
        /// Search within the target <see cref="string"/> for the specified value
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchString">The <see cref="string"/> to search for</param>
        /// <returns><see cref="bool"/> indicating success (true) or failure (false)</returns>
        public static bool Exists(this string? Value, string SearchString)
        {
            bool Result = false;
            string[] StringToArray = Value.Split(',');

            if (Array.IndexOf(StringToArray, SearchString) > -1)
            {
                Result = true;
            }

            return Result;
        }

        // https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
        public static string FromBase64(this string Input)
        {
            byte[] base64EncodedBytes = System.Convert.FromBase64String(Input);

            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Hash(this string? Input)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Input.ToLower());

            string Converted = Convert.ToBase64String(plainTextBytes);
            return ConstantLengthHash(Converted);
        }

        // http://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp 
        public static byte[]? HashWithSalt(this string Value, byte[] salt)
        {
            return Hash(Encoding.UTF8.GetBytes(Value), salt);

            //return Encoding.UTF8.GetBytes(value).Hash(salt); 
        }

        /// <summary>
        /// Determines if the supplied string represents a numeric value
        /// </summary>
        /// <param name="Number"></param>
        /// <returns>bool indicating Success</returns>
        public static bool IsNumeric(this string? Number)
        {
            if (string.IsNullOrEmpty(Number))
            {
                return false;
            }
            else
            {
                int numberOfChar = Number.Length;

                if (numberOfChar > 0)
                {
                    bool r =
                        double.TryParse(Number, out _) ||
                        int.TryParse(Number, out _) ||
                        float.TryParse(Number, out _) ||
                        decimal.TryParse(Number, out _) ||
                        Number.All(char.IsNumber);

                    return r;
                }
                else
                {
                    return false;
                }
            }
        }

        public static string? Left(this string? Input, int Length)
        {
            if (Input == null)
            {
                return null;
            }

            if (Length < 1)
            {
                return "";
            }

            if (Input.Length < Length)
            {
                return Input;
            }

            return Input.Substring(0, Length);
        }

        /// <summary>
        /// Return the length of the target <see cref="string"/> after performing a null coalescing check and Trim()
        /// </summary>
        /// <param name="Value">The <see cref="string"/> to query</param>
        /// <returns>An <see cref="int"/> representing the length</returns>
        public static int LengthOf(this string? Value)
        {
            return (Value ?? string.Empty).Trim().Length;
        }

        // https://dev.to/elemarjr/computing-the-levenshtein-edit-distance-of-two-strings-using-c-6a8
        /// <summary>
        /// Compute the Levenshtein distance between two strings
        /// </summary>
        /// <param name="Original">The first string</param>
        /// <param name="Compare">The string to compare against</param>
        /// <returns>Integer representing the number of edits required to transform the first string to the second</returns>
        public static int LevenshteinDistance(this string? Original, string Compare)
        {
            if (Original == null && Compare == null)
            {
                return 0;
            }

            if (Original == null)
            {
                return Compare.Length;
            }

            if (Compare == null)
            {
                return Original.Length;
            }

            if (Original.Length == 0)
            {
                return Compare.Length;
            }

            if (Compare.Length == 0)
            {
                return Original.Length;
            }

            int[,] Distance = new int[Original.Length + 1, Compare.Length + 1];
            for (int Loop = 0; Loop <= Original.Length; Loop++)
            {
                Distance[Loop, 0] = Loop;
            }

            for (int Loop = 0; Loop <= Compare.Length; Loop++)
            {
                Distance[0, Loop] = Loop;
            }

            for (int Loop = 1; Loop <= Original.Length; Loop++)
            {
                for (int j = 1; j <= Compare.Length; j++)
                {
                    int cost = (Compare[j - 1] == Original[Loop - 1]) ? 0 : 1;
                    Distance[Loop, j] = Min(Distance[Loop - 1, j] + 1, Distance[Loop, j - 1] + 1, Distance[Loop - 1, j - 1] + cost);
                }
            }

            return Distance[Original.Length, Compare.Length];
        }

        public static string? Mid(this string? Input, int Start, int Length)
        {
            if (Input == null)
            {
                return null;
            }

            if (Length < 1)
            {
                return "";
            }

            if (Input.Length < Length)
            {
                return Input;
            }

            return Input.Substring(Start, Length);
        }

        private static int Min(int e1, int e2, int e3)
        {
            return Math.Min(Math.Min(e1, e2), e3);
        }

        public static string RemoveAlphaCharacters(this string Value)
        {
            return new string(Value.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }

        public static string? RemoveEmptyLines(this string? Text)
        {
            if (Text == null)
            {
                return null;
            }

            return Regex.Replace(Text, @"^\s*$\n|\r", string.Empty, RegexOptions.Multiline).TrimEnd();
        }

        public static string? RemoveEOL(this string? Text)
        {
            if (Text == null)
            {
                return null;
            }

            return Text.Replace("\n", "").Replace("\r", "");
        }

        public static string RemoveNumericCharacters(this string Value)
        {
            //return new string(Value.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-').ToArray());
            return new string(Value.Where(c => !char.IsDigit(c) && c != '-').ToArray());
        }

        public static string ReplaceEx(this string? Input, string Original, string NewValue)
        {
            return ReplaceEx(Input, Original, NewValue, StringComparison.CurrentCulture);
        }

        //        public static string ReplaceEx(this string? Input, string Original, string NewValue, bool IgnoreCase)
        //        {
        //            string Result = Input;

        //            if (Input == null)
        //            {
        //                return null;
        //            }

        //#if NETCOREAPP || NETFRAMEWORK
        //        if (IgnoreCase)            
        //        {
        //            Result = Input.Replace(Original, NewValue, StringComparison.CurrentCultureIgnoreCase);
        //        }
        //        else
        //        {
        //            Result = Input.Replace(Original, NewValue, StringComparison.CurrentCulturePreserveCase);
        //        }
        //#else
        //            Result = Input.Replace(Original, NewValue);
        //#endif

        //            return Result;
        //        }

        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another
        /// specified string according the type of search to use for the specified string.
        /// </summary>
        /// <param name="Input">The string performing the replace method.</param>
        /// <param name="Original">The string to be replaced.</param>
        /// <param name="NewValue">The string replace all occurrences of <paramref name="Original"/>.
        /// If value is equal to <c>null</c>, than all occurrences of <paramref name="Original"/> will be removed from the <paramref name="Input"/>.</param>
        /// <param name="ComparisonType">One of the enumeration values that specifies the rules for the search.</param>
        /// <returns>A string that is equivalent to the current string except that all instances of <paramref name="Original"/> are replaced with <paramref name="NewValue"/>.
        /// If <paramref name="Original"/> is not found in the current instance, the method returns the current instance unchanged.</returns>
        [DebuggerStepThrough]
        public static string ReplaceEx(this string? Input, string? Original, string? NewValue, StringComparison ComparisonType)
        {
            // Check inputs.
            if (Input == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException(nameof(Input));
            }

            if (Input.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                return Input;
            }

            if (Original == null)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentNullException(nameof(Original));
            }

            if (Original.Length == 0)
            {
                // Same as original .NET C# string.Replace behavior.
                throw new ArgumentException("String cannot be of zero length.");
            }

            StringBuilder resultStringBuilder = new StringBuilder(Input.Length);

            // Analyze the replacement: replace or remove.
            bool isReplacementNullOrEmpty = string.IsNullOrEmpty(NewValue);

            // Replace all values.
            const int valueNotFound = -1;
            int foundAt;
            int startSearchFromIndex = 0;

            while ((foundAt = Input.IndexOf(Original, startSearchFromIndex, ComparisonType)) != valueNotFound)
            {
                // Append all characters until the found replacement.
                int charsUntilReplacment = foundAt - startSearchFromIndex;
                bool isNothingToAppend = charsUntilReplacment == 0;

                if (!isNothingToAppend)
                {
                    _ = resultStringBuilder.Append(Input, startSearchFromIndex, charsUntilReplacment);
                }

                // Process the replacement.
                if (!isReplacementNullOrEmpty)
                {
                    _ = resultStringBuilder.Append(NewValue);
                }

                // Prepare start index for the next search.
                // This needed to prevent infinite loop, otherwise method always start search 
                // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
                // and comparisonType == "any ignore case" will conquer to replacing:
                // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
                startSearchFromIndex = foundAt + Original.Length;

                if (startSearchFromIndex == Input.Length)
                {
                    // It is end of the input string: no more space for the next search.
                    // The input string ends with a value that has already been replaced. 
                    // Therefore, the string builder with the result is complete and no further action is required.
                    return resultStringBuilder.ToString();
                }
            }

            // Append the last part to the result.
            int charsUntilStringEnd = Input.Length - startSearchFromIndex;

            _ = resultStringBuilder.Append(Input, startSearchFromIndex, charsUntilStringEnd);

            return resultStringBuilder.ToString();
        }

        public static string? Right(this string? Input, int Length)
        {
            if (Input == null)
            {
                return null;
            }

            if (Length < 1)
            {
                return "";
            }

            if (Input.Length < Length)
            {
                return Input;
            }

            return Input.Substring(Input.Length - Length, Length);
        }

        public static string Soundex(this string Input)
        {
            return Encoder.Encode(Input);
        }

        public static bool StartsWithEx(this string? Input, string Value, bool IgnoreCase)
        {
            CultureInfo provider = CultureInfo.CurrentCulture;

            if (Input == null || Value == null)
            {
                return false;
            }

            return Input.StartsWith(Value, IgnoreCase, provider);
        }

#if Windows
        /// <summary>
        /// Convert the target <see cref="string"/> into a <see cref="DateTime"/> and remove the current TimeZone offset
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>The current <see cref="Datetime"/> minus the current Timezone offset</returns>
        public static DateTime SubtractTimeZone(this string? Value)
        {
            DateTime localTime = ManagementDateTimeConverter.ToDateTime(Value);

            // Subtract the current timezone offset 
            localTime -= System.TimeZoneInfo.Utc.GetUtcOffset(DateTime.Now); //System.TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

            return localTime;
        }  
#endif

        // https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
        public static string ToBase64(this string Input)
        {
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(Input);
            //string Converted = "";

            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string ToLowerEx(this string Input)
        {
            CultureInfo provider = CultureInfo.CurrentCulture;

            if (Input == null)
            {
                return String.Empty;
            }

            return Input.ToLower(provider);
        }

        public static string ToTitleCase(this string Input)
        {
            if (Input == null)
            {
                return String.Empty;
            }

            TextInfo _CurrentCultureTextInfo = CultureInfo.CurrentCulture.TextInfo;

            return _CurrentCultureTextInfo.ToTitleCase(Input);
        }

        public static string ToUpperEx(this string Input)
        {
            CultureInfo provider = CultureInfo.CurrentCulture;

            if (Input == null)
            {
                return String.Empty;
            }

            return Input.ToUpper(provider);
        }

        /// <summary>
        /// Convert a string to an array of <see cref="byte"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>byte[] version of the input <see cref="string"/></returns>
        public static byte[]? ToByteArray(this string? Value)
        {
            if (Value == null)
            {
                return null;
            }

            return Encoding.ASCII.GetBytes(Value);
        }

        public static DateTime ToDate(this string Input)
        {
            string InputDateAsString = Input;

            if (Input == null)
            {
                return DateTime.MinValue;
            }

            //if (Date.Contains("Dec"))
            //{
            //    int i = 1;
            //}

            InputDateAsString = InputDateAsString.ToLower().Replace("january", "01");
            InputDateAsString = InputDateAsString.ToLower().Replace("february", "02");
            InputDateAsString = InputDateAsString.ToLower().Replace("march", "03");
            InputDateAsString = InputDateAsString.ToLower().Replace("april", "04");
            InputDateAsString = InputDateAsString.ToLower().Replace("may", "05");
            InputDateAsString = InputDateAsString.ToLower().Replace("june", "06");
            InputDateAsString = InputDateAsString.ToLower().Replace("july", "07");
            InputDateAsString = InputDateAsString.ToLower().Replace("august", "08");
            InputDateAsString = InputDateAsString.ToLower().Replace("september", "09");
            InputDateAsString = InputDateAsString.ToLower().Replace("october", "10");
            InputDateAsString = InputDateAsString.ToLower().Replace("november", "11");
            InputDateAsString = InputDateAsString.ToLower().Replace("december", "12");

            InputDateAsString = InputDateAsString.ToLower().Replace("jan", "01");
            InputDateAsString = InputDateAsString.ToLower().Replace("feb", "02");
            InputDateAsString = InputDateAsString.ToLower().Replace("mar", "03");
            InputDateAsString = InputDateAsString.ToLower().Replace("apr", "04");
            InputDateAsString = InputDateAsString.ToLower().Replace("may", "05");
            InputDateAsString = InputDateAsString.ToLower().Replace("jun", "06");
            InputDateAsString = InputDateAsString.ToLower().Replace("jul", "07");
            InputDateAsString = InputDateAsString.ToLower().Replace("aug", "08");
            InputDateAsString = InputDateAsString.ToLower().Replace("sep", "09");
            InputDateAsString = InputDateAsString.ToLower().Replace("oct", "10");
            InputDateAsString = InputDateAsString.ToLower().Replace("nov", "11");
            InputDateAsString = InputDateAsString.ToLower().Replace("dec", "12");
            InputDateAsString = InputDateAsString.Replace(".", "/");
            InputDateAsString = InputDateAsString.Replace(",", "/");
            InputDateAsString = InputDateAsString.Replace("-", "/");
            InputDateAsString = InputDateAsString.Replace(@"\", "/");

            while (InputDateAsString.Contains("  "))
            {
                InputDateAsString = InputDateAsString.Replace("  ", " ");
            }

            InputDateAsString = InputDateAsString.Replace(" ", "/");

            //// How many slashes are there?  There should be 2
            //string[] Split = Date.Split('/');

            //if (Split.Length != 3) // There should be 3 elements to any date
            //{
            //    switch (Split.Length)
            //    {
            //        case 0:
            //            if (Date.Length == 8) // ddmmyyyy
            //            {

            //            }
            //            else if (Date.Length == 6) // ddmmyy or dmyyyy
            //            {
            //            }
            //            else if (Date.Length == 5) // dmmyy or ddmyyyy
            //            {
            //            }
            //            else if (Date.Length == 4) // dmyy or yyyy
            //            {
            //            }
            //            else if (Date.Length == 2) // yy
            //            {
            //            }
            //            else // No idea what's going on!
            //            {
            //            }

            //            break;
            //        case 1:
            //            break;
            //        case 2:
            //            break;
            //        default:
            //            // more than 3!
            //            break;
            //    }
            //}

            return InputDateAsString.As<DateTime>(new DateTime(0001, 01, 01));
        }

        public static int WordCount(this string? Input)
        {
            if (Input == null)
            {
                return 0;
            }

            char[] delimiters = new char[] { ' ', '\r', '\n', '\t' };
            return Input.Split(delimiters, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static string[] Words(this string Input)
        {
            string[] Result = Array.Empty<string>();
            char[] delimiters = new char[] { ' ', '\r', '\n', '\t' };

            if (Input == null)
            {
                return Result;
            }

            // , System.StringComparison.OrdinalIgnoreCase

            return Input.Replace("/", "").Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool Yes(this string Input)
        {
            bool Result = Input.Trim().ToLowerEx().ContainsEx("y") ||
                     Input.Trim().ToLowerEx().ContainsEx("ok") ||
                     Input.Trim().ToLowerEx().ContainsEx("got it") ||
                     Input.Trim().ToLowerEx().ContainsEx("right") ||
                     Input.Trim().ToLowerEx().ContainsEx("correct") ||
                     Input.Trim().ToLowerEx().ContainsEx("true");

            return Result;
        }

        public static string Value(this string? Input)
        {
            string Result = "";

            if (Input != null)
            {
                Result = Input;
            }

            return Result;
        }

        public static string CreateSHA512Hash(this string Value)
        {
            byte[] message = Encoding.UTF8.GetBytes(Value);

            using (SHA512 alg = SHA512.Create())
            {
                string hex = "";

                byte[] hashValue = alg.ComputeHash(message);

                foreach (byte x in hashValue)
                {
                    hex += String.Format("{0:x2}", x);
                }

                return hex;
            }
        }

        #endregion

        #region struct

        /// <summary>
        /// Gets all items for an enum type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>() where T : struct
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }

        #endregion

        #region T

        /// <summary>
        /// Compares the properties of two objects of the same type and returns if all properties are equal.
        /// </summary>
        /// <param name="Value">The first object to compare.</param>
        /// <param name="Comparison">The second object to compare.</param>
        /// <returns><c>true</c> if all property values are equal, otherwise <c>false</c>.</returns>
        public static bool Approximates<T>(this T Value, T Comparison)
        {
            bool result = false;

            if (Value != null && Comparison != null)
            {
                Type objectType = Value.GetType();

                result = true; // assume by default they are equal

                foreach (PropertyInfo propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.GetIndexParameters().Length == 0))
                {
                    object? valueA;
                    object? valueB;

                    valueA = propertyInfo.GetValue(Value, null);
                    valueB = propertyInfo.GetValue(Comparison, null);

                    // if it is a primitive type, value type or implements IComparable, just directly try and compare the value
                    if (CanDirectlyCompare(propertyInfo.PropertyType))
                    {
                        if (!AreValuesEqual(valueA, valueB))
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                    }
                    // if it implements IEnumerable, then scan any items
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        IEnumerable<object> collectionItems1;
                        IEnumerable<object> collectionItems2;
                        int collectionItemsCount1;
                        int collectionItemsCount2;

                        // null check
                        if ((valueA == null && valueB != null) || (valueA != null && valueB == null))
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                        else if (valueA != null && valueB != null)
                        {
                            collectionItems1 = ((IEnumerable)valueA).Cast<object>();
                            collectionItems2 = ((IEnumerable)valueB).Cast<object>();
                            collectionItemsCount1 = collectionItems1.Count();
                            collectionItemsCount2 = collectionItems2.Count();

                            // check the counts to ensure they match
                            if (collectionItemsCount1 != collectionItemsCount2)
                            {
                                Console.WriteLine("Collection counts for property '{0}.{1}' do not match.", objectType.FullName, propertyInfo.Name);
                                result = false;
                            }
                            // and if they do, compare each item... this assumes both collections have the same order
                            else
                            {
                                for (int i = 0; i < collectionItemsCount1; i++)
                                {
                                    object collectionItem1;
                                    object collectionItem2;
                                    Type collectionItemType;

                                    collectionItem1 = collectionItems1.ElementAt(i);
                                    collectionItem2 = collectionItems2.ElementAt(i);
                                    collectionItemType = collectionItem1.GetType();

                                    if (CanDirectlyCompare(collectionItemType))
                                    {
                                        if (!AreValuesEqual(collectionItem1, collectionItem2))
                                        {
                                            Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.", i, objectType.FullName, propertyInfo.Name);
                                            result = false;
                                        }
                                    }
                                    else if (!Approximates(collectionItem1, collectionItem2))
                                    {
                                        Console.WriteLine("Item {0} in property collection '{1}.{2}' does not match.", i, objectType.FullName, propertyInfo.Name);
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!Approximates(propertyInfo.GetValue(Value, null), propertyInfo.GetValue(Comparison, null)))
                        {
                            Console.WriteLine("Mismatch with property '{0}.{1}' found.", objectType.FullName, propertyInfo.Name);
                            result = false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Cannot compare property '{0}.{1}'.", objectType.FullName, propertyInfo.Name);
                        result = false;
                    }
                }
            }
            else
            {
                result = object.Equals(Value, Comparison);
            }

            return result;
        }

        /// <summary>
        /// Perform a binary copy of the provided object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The object to copy</param>
        /// <returns>A <see cref="T"/></returns>
        public static T? DeepCopy<T>(this T item)
        {
            //BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            try
            {
                Serializer.Serialize(stream, item);
                _ = stream.Seek(0, SeekOrigin.Begin);
                T result = Serializer.Deserialize<T>(stream);
                stream.Close();

                return result;
            }
            catch (SerializationException)
            {
            }
            catch (Exception)
            {
                return default(T);
            }

            return item;
        }

        /// <summary>
        /// Deserialise to the provided type from the provided Filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Filename">The filename to load from</param>
        /// <returns></returns>
        public static T? LoadFromFile<T>(this T Value, string Filename)
        {
            T? Result = default(T);

            if (Filename.LengthOf() == 0)
            {
                return default(T);
            }

            try
            {
                using (FileStream Stream = System.IO.File.OpenRead(Filename))
                {
                    if (Stream == null)
                    {
                        return default(T);
                    }

                    XmlSerializer Serializer = new XmlSerializer(typeof(T));

                    //if (Serializer == null)
                    //{
                    //    return default(T);
                    //}
                    object? stream = Serializer.Deserialize(Stream);

                    if (stream == null)
                    {
                        return default(T);
                    }

                    Result = (T)stream;
                }
            }
            catch (Exception)
            {
                return default(T);
            }

            return Result;
        }

        /// <summary>
        /// Deserialise to the provided type from the provided Filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Filename">The filename to load from</param>
        /// <param name="MaxWaitTimeS">The maximum time to wait for the provided file to unlock</param>
        /// <returns></returns>
        public static T? LoadFromFile<T>(this T Value, string Filename, int MaxWaitTimeS)
        {
            T? Result = default(T);
            string Flagfile = Filename + ".Flag";
            DateTime MaxTimeStamp = DateTime.UtcNow.AddMilliseconds(MaxWaitTimeS * 1000); // Use UtcNow to prevent DaylightSavings issues 

            if (Filename.LengthOf() == 0)
            {
                return default(T);
            }

            // Wait a maximum of MaxWaitTimemS for the Flag file to be deleted 
            while (System.IO.File.Exists(Flagfile) || DateTime.UtcNow < MaxTimeStamp)
            {
                System.Threading.Thread.Sleep(1); // Highly unlikely that we can wait for 1mS (due to the Timer resolution), 
                                                  // but we will use that as our minimum time to decrease apparent CPU utilisation 
            }

            // If the Flagfile still exists, return a negative result 
            if (System.IO.File.Exists(Flagfile))
            {
                return default(T);
            }

            try
            {
                using (FileStream lockFile = new FileStream(Flagfile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
                {
                    using (FileStream Stream = System.IO.File.OpenRead(Filename))
                    {
                        XmlSerializer Serializer = new XmlSerializer(typeof(T));

                        object? stream = Serializer.Deserialize(Stream);

                        if (stream == null)
                        {
                            return default(T);
                        }

                        Result = (T)stream;
                    }

                    // We are finished with the Flag, so remove it 
                    System.IO.File.Delete(Flagfile);
                }
            }
            catch (Exception)
            {
                return default(T);
            }

            return Result;
        }

        /// <summary>
        /// Perform a copy of the provided object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Deep">If true (default), performs a DeepCopy</param>
        /// <returns><see cref="string"/> version of the provided object</returns>
        public static string Save<T>(this T Value, bool Deep = true)
        {
            try
            {
                XmlSerializer? Serialiser = new XmlSerializer(typeof(T));
                using (StringWriter Writer = new StringWriter())
                {
                    if (Deep)
                    {
                        Serialiser.Serialize(Writer, Value.DeepCopy());
                    }
                    else
                    {
                        Serialiser.Serialize(Writer, Value);
                    }

                    return Writer.ToString();
                }
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Serialise the target object to the provided Filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Filename">The filename to save to</param>
        /// <param name="Deep">If true (default), performs a DeepCopy</param>
        /// <returns><see cref="bool"/> indicating success (true) or failure (false)</returns>
        public static bool SaveToFile<T>(this T Value, string Filename, bool Deep = true)
        {
            bool Result;
            try
            {
                FileStream fs = new FileStream(Filename, FileMode.Create, FileAccess.Write, FileShare.None);
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                TextWriter? writer = null;

                // Create String representation of the object 
                string SerialisedObject = Value.Save(Deep);

                // Write this to disk 
                using (writer = new StreamWriter(fs))
                {
                    Serializer.Serialize(writer, Value);
                    writer.Flush();
                    writer.Close();
                }

                Result = true;
            }
            catch
            {
                Result = false;
            }

            return Result;
        }

        /// <summary>
        /// Serialise the target object to the provided Filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Filename">The filename to save to</param>
        /// <param name="Deep">If true (default), performs a DeepCopy</param>
        /// <param name="MaxWaitTimeS">The maximum time to wait for the provided file to unlock</param>
        /// <returns><see cref="bool"/> indicating success (true) or failure (false)</returns>
        public static bool SaveToFile<T>(this T Value, string Filename, bool Deep, int MaxWaitTimeS)
        {
            bool Result = false;
            string Flagfile = Filename + ".Flag";
            DateTime MaxTimeStamp = DateTime.UtcNow.AddMilliseconds(MaxWaitTimeS * 1000); // Use UtcNow to prevent DaylightSavings issues 

            // Wait a maximum of MaxWaitTimemS for the Flag file to be deleted 
            while (System.IO.File.Exists(Flagfile) || DateTime.UtcNow > MaxTimeStamp)
            {
                System.Threading.Thread.Sleep(1); // Highly unlikely that we can wait for 1mS (due to the Timer resolution), 
                                                  // but we will use that as our minimum time to decrease apparent CPU utilisation 
            }

            // If the Flagfile still exists, return a negative result 
            if (System.IO.File.Exists(Flagfile))
            {
                return false;
            }

            try
            {
                using (FileStream lockFile = new FileStream(Flagfile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
                {
                    FileStream fs = new FileStream(Filename, FileMode.Create, FileAccess.Write, FileShare.None);
                    XmlSerializer Serializer = new XmlSerializer(typeof(T));
                    TextWriter? writer = null;

                    // Create String representation of the object 
                    string SerialisedObject = Value.Save(Deep);

                    // Write this to disk 
                    using (writer = new StreamWriter(fs))
                    {
                        Serializer.Serialize(writer, Value);
                        writer.Flush();
                        writer.Close();
                    }

                    // We are finished with the Flag, so remove it 
                    System.IO.File.Delete(Flagfile);

                    Result = true;
                }
            }
            catch
            {
                Result = false;
            }

            return Result;
        }

        #endregion

        #region TabPage 

        public static TabPage HidePage(this TabPage Page)
        {
            TabControl Parent = (TabControl)Page.Parent;

            Parent.TabPages.Remove(Page);

            return Page;
        }

        public static void ShowPage(this TabPage Page, TabControl Tab)
        {
            Tab.TabPages.Add(Page);
        }

        #endregion

        #region TimeSpan 

        public static string ToReadableAgeString(this TimeSpan span)
        {
            return string.Format("{0:0}", span.Days / 365.25);
        }

        public static string ToReadableString(this TimeSpan span)
        {
            string formatted = string.Format("{0}{1}{2}{3}",
                span.Duration().Days > 0 ? string.Format("{0:0} day{1}, ", span.Days, span.Days == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Hours > 0 ? string.Format("{0:0} hour{1}, ", span.Hours, span.Hours == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Minutes > 0 ? string.Format("{0:0} min{1}, ", span.Minutes, span.Minutes == 1 ? String.Empty : "s") : string.Empty,
                span.Duration().Seconds > 0 ? string.Format("{0:0} sec{1}", span.Seconds, span.Seconds == 1 ? String.Empty : "s") : string.Empty);

            if (formatted.EndsWith(", "))
            {
                formatted = formatted.Substring(0, formatted.Length - 2);
            }

            if (string.IsNullOrEmpty(formatted))
            {
                formatted = "0 seconds";
            }

            return formatted;
        }

        #endregion

        #region type 

        /// <summary>
        /// Returns the specified Event from the target Object
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="EventName">The name of the Event to return</param>
        /// <returns></returns>
        public static FieldInfo? GetEventField(this Type? Value, string EventName)
        {
            FieldInfo? Field = null;

            if (Value == null)
            {
                return null;
            }

            while (Value != null)
            {
                // Find events defined as field
                Field = Value.GetField(EventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);

                if (Field != null && (Field.FieldType == typeof(MulticastDelegate) || Field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                {
                    break;
                }

                // Find events defined as property { add; remove; } 
                Field = Value.GetField("EVENT_" + EventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);

                if (Field != null)
                {
                    break;
                }

                Value = Value.BaseType;
            }

            return Field;
        }

        /// <summary>
        /// Converts an enum to a DataTable.
        /// </summary>
        /// <param name="EnumType">The enum to convert.</param>
        /// <param name="KeyFieldName">The desired name of the key column.</param>
        /// <param name="ValueFieldName">The desired name of the value column.</param>
        /// <returns>A DataTable with the name/value pairs from the enum.</returns>
        public static DataTable ToDataTable(this Type EnumType, string KeyFieldName, string ValueFieldName)
        {
            DataTable table = new DataTable();

            //Check inputs:
            if (KeyFieldName == String.Empty)
            {
                KeyFieldName = "Key";
            }

            if (ValueFieldName == String.Empty)
            {
                ValueFieldName = "Value";
            }

            if (KeyFieldName == ValueFieldName)
            {
                throw new Exception("Key and Value column names must be different.");
            }

            //Create the DataTable with the desired columns:
            _ = table.Columns.Add(KeyFieldName, typeof(string));
            _ = table.Columns.Add(ValueFieldName, Enum.GetUnderlyingType(EnumType));

            //Add the items from the enum:
            foreach (string name in Enum.GetNames(EnumType))
            {
                _ = table.Rows.Add(name, Enum.Parse(EnumType, name));
            }

            return table;
        }

        #endregion

        #region Uri

#if Windows
public static Bitmap GetBitmap(this Uri Path)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(Path);
            Bitmap bitmap;
            bitmap = new Bitmap(stream);

            stream.Flush();
            stream.Close();
            client.Dispose();

            return bitmap;
        }  
#endif

#if Windows
        public static bool SaveAsJPG(this Uri Path, string Filename)
        {
            return SaveImage(Path, Filename, ImageFormat.Jpeg);
        }  
#endif

#if Windows
        public static bool SaveAsPNG(this Uri Path, string Filename)
        {
            return SaveImage(Path, Filename, ImageFormat.Png);
        }  
#endif

#if Windows
        public static bool SaveImage(this Uri Path, string Filename, ImageFormat Format)
        {
            Bitmap Image = GetBitmap(Path);

            try
            {
                if (Image != null)
                {
                    Image.Save(Filename, Format);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }  
#endif

        #endregion

        #region XmlElement 

        public static bool ValueToBool(this XmlElement Node)
        {
            bool Result = false;

            try
            {
                Result = Convert.ToBoolean(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static bool[] ValueToBoolArray(this XmlElement Node)
        {
            bool[] Result = Array.Empty<bool>();

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToBoolean(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static DateTime ValueToDateTime(this XmlElement Node)
        {
            DateTime Result = DateTime.MinValue;

            try
            {
                Result = Convert.ToDateTime(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static DateTime[] ValueToDateTimeArray(this XmlElement Node)
        {
            DateTime[] Result = Array.Empty<DateTime>();

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToDateTime(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static int ValueToInt(this XmlElement Node)
        {
            int Result = 0;

            try
            {
                Result = Convert.ToInt32(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static short ValueToInt16(this XmlElement Node)
        {
            short Result = 0;

            try
            {
                Result = Convert.ToInt16(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static short[] ValueToInt16Array(this XmlElement Node)
        {
            short[] Result = Array.Empty<short>();

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToInt16(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static int ValueToInt32(this XmlElement Node)
        {
            int Result = 0;

            try
            {
                Result = Convert.ToInt32(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static int[] ValueToInt32Array(this XmlElement Node)
        {
            int[] Result = Array.Empty<int>();

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToInt32(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static long ValueToInt64(this XmlElement Node)
        {
            long Result = 0;

            try
            {
                Result = Convert.ToInt64(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static long[] ValueToInt64Array(this XmlElement Node)
        {
            long[] Result = Array.Empty<long>();

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToInt64(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static int[] ValueToIntArray(this XmlElement Node)
        {
            int[] Result = Array.Empty<int>();

            try
            {
                Result = (from string s in Node.InnerText
                          select (Convert.ToInt32(s))
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static string ValueToString(this XmlElement Node)
        {
            string Result = "";

            try
            {
                Result = Node.InnerText;
            }
            catch
            {
            }

            return Result;
        }

        public static string[] ValueToStringArray(this XmlElement Node)
        {
            string[] Result = Array.Empty<string>();

            try
            {
                Result = (from string s in Node.InnerText
                          select s
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static uint ValueToUInt(this XmlElement Node)
        {
            uint Result = 0;

            try
            {
                Result = Convert.ToUInt32(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static ushort ValueToUInt16(this XmlElement Node)
        {
            return Convert.ToUInt16(Node.InnerText);
        }

        public static ushort[] ValueToUInt16Array(this XmlElement Node)
        {
            ushort[] Result = Array.Empty<ushort>();

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToUInt16(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static uint ValueToUInt32(this XmlElement Node)
        {
            uint Result = 0;

            try
            {
                Result = Convert.ToUInt32(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static uint[] ValueToUInt32Array(this XmlElement Node)
        {
            uint[] Result = Array.Empty<uint>();

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToUInt32(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static ulong ValueToUInt64(this XmlElement Node)
        {
            ulong Result = 0;

            try
            {
                Result = Convert.ToUInt64(Node.InnerText);
            }
            catch
            {
            }

            return Result;
        }

        public static ulong[] ValueToUInt64Array(this XmlElement Node)
        {
            ulong[] Result = Array.Empty<ulong>();

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToUInt64(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        public static uint[] ValueToUIntArray(this XmlElement Node)
        {
            uint[] Result = Array.Empty<uint>();

            try
            {
                Result = (from string s in Node.InnerText
                          select (uint)Convert.ToInt32(s)
                         ).ToArray();
            }
            catch
            {
            }

            return Result;
        }

        #endregion

#if Windows
        public static bool WindowsThemeIsLight()
        {
            RegistryKey registry = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            return (int)registry.GetValue("SystemUsesLightTheme") == 1;
        }
#endif

        public static decimal GetRandomValue(decimal Minimum, decimal Maximum, decimal Precision = .001M)
        {
            Random rnd = new Random();

            Precision *= 1000 * 1000;

            int min = (int)(Minimum * Precision);
            int max = (int)(Maximum * Precision);

            decimal random_value = rnd.Next(min, max + 1);
            random_value /= Precision;

            return random_value;
        }

        /// <summary>
        /// Find and return the full path of the given filename below the current directory path
        /// </summary>
        /// <param name="filename">The file to find</param>
        /// <returns>Full path to the file</returns>
        public static string? FindFileBelow(string filename)
        {
            string? current = Directory.GetCurrentDirectory();

            while (current != null)
            {
                if (File.Exists(Path.Combine(current, filename)))
                {
                    return Path.Combine(current, filename);
                }

                current = Directory.GetParent(current)?.FullName;
            }

            return null;
        }

        #endregion

        public static class Encoder
        {
            private class EncodingResult
            {
                public EncodingResult(string curr, string prev)
                {
                    Curr = curr;
                    Prev = prev;
                }

                public string Curr
                {
                    get;
                }

                public bool IsValidEncoding
                {
                    get
                    {
                        return Curr != InvalidDigit;
                    }
                }

                public string Prev
                {
                    get;
                }
            }

            private const int MaxEncodedLength = 4;
            private const string InvalidDigit = "*";

            private static readonly Dictionary<char, int> _digitValues = new Dictionary<char, int>
            {
                {'b', 1}, {'f', 1}, {'p', 1}, {'v', 1},
                {'c', 2}, {'g', 2}, {'j', 2}, {'k', 2}, {'q', 2},
                {'d', 3}, {'t', 3},
                {'l', 4},
                {'m', 5}, {'n', 5},
                {'r', 6}
            };

            private static string ConvertCharacterToNumber(char c)
            {
                c = char.ToLower(c);
                return !_digitValues.ContainsKey(c) ? InvalidDigit : _digitValues[c].ToString();
            }

            public static string Encode(string word)
            {
                if (IsNullOrEmpty(word))
                {
                    return Empty;
                }

                return word
                    .Select((ch, index) => EncodeCharacter(word, ch, index))
                    .Where((encodedChar, index) =>
                                encodedChar.IsValidEncoding && encodedChar.Curr != encodedChar.Prev)
                    .Select(arg => arg.Curr)
                    .Concat(Enumerable.Repeat("0", MaxEncodedLength))
                    .Take(MaxEncodedLength)
                    .Aggregate((i, j) => i + j);
            }

            private static EncodingResult EncodeCharacter(string word, char ch, int index)
            {
                if (index == 0)
                {
                    return new EncodingResult(char.ToUpper(ch).ToString(), InvalidDigit);
                }

                return new EncodingResult(ConvertCharacterToNumber(ch), ConvertCharacterToNumber(word[index - 1]));
            }
        }

        private delegate ListView.ListViewItemCollection DelegateGetItems(ListView lst);
    }
}

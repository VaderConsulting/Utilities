using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Xml;
using System;
using System.Resources;

namespace Utilities
{
    public static class Extensions
    {
        private delegate ListView.ListViewItemCollection DelegateGetItems(ListView lst);

        #region Constants 

        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

        #endregion

        #region Events 

        #endregion

        #region Enums 

        #endregion

        #region DLL Imports 

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private extern static bool DestroyIcon(IntPtr handle);

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

        private static byte[] Hash(byte[] Value, byte[] Salt)
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

        #region T

        /// <summary>
        /// Perform a binary copy of the provided object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The object to copy</param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T item)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            try
            {
                formatter.Serialize(stream, item);
                stream.Seek(0, SeekOrigin.Begin);
                T Result = (T)formatter.Deserialize(stream);
                stream.Close();

                return Result;
            }
            catch (SerializationException)
            {
            }
            catch (Exception)
            {
                return item;
            }

            return item;
        }

        /// <summary>
        /// Deserialise to the provided type from the provided Filename
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <param name="Filename">The filename to load from</param>
        /// <param name="MaxWaitTimemS">The maximum time to wait for the provided file to unlock</param>
        /// <returns></returns>
        public static T LoadFromFile<T>(this T Value, string Filename, int MaxWaitTimemS = 60000)
        {
            T Result = default(T);
            string Flagfile = Filename + ".Flag";
            DateTime MaxTimeStamp = DateTime.UtcNow.AddMilliseconds(MaxWaitTimemS); // Use UtcNow to prevent DaylightSavings issues 

            if (Filename.LengthOf() == 0)
            {
                return default(T);
            }

            // Wait a maximum of MaxWaitTimemS for the Flag file to be deleted 
            while (System.IO.File.Exists(Flagfile) || DateTime.UtcNow > MaxTimeStamp)
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

                        Result = (T)Serializer.Deserialize(Stream);
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
            XmlSerializer Serialiser = null;

            try
            {
                Serialiser = new XmlSerializer(typeof(T));
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
        /// <param name="MaxWaitTimemS">The maximum time to wait for the provided file to unlock</param>
        /// <returns><see cref="bool"/> indicating success (true) or failure (false)</returns>
        public static bool SaveToFile<T>(this T Value, string Filename, bool Deep = true, int MaxWaitTimemS = 60000)
        {
            bool Result = false;
            string Flagfile = Filename + ".Flag";
            DateTime MaxTimeStamp = DateTime.UtcNow.AddMilliseconds(MaxWaitTimemS); // Use UtcNow to prevent DaylightSavings issues 

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
                    TextWriter writer = null;

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

        //public static T Load<T>(this T obj, string FileName) where T : new()
        //{
        //    try
        //    {
        //        using (TextReader Reader = new StreamReader(FileName))
        //        {
        //            if (obj == null)
        //            {
        //                obj = new T();
        //            }
        //            XmlSerializer Serializer = new XmlSerializer(obj.GetType());

        //            T Result = (T)Serializer.Deserialize(Reader);

        //            return Result;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return default(T);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static T ToObject<T>(this byte[] Value)
        {
            MemoryStream Stream = new MemoryStream();
            BinaryFormatter Formatter = new BinaryFormatter();

            Stream.Write(Value, 0, Value.Length);
            Stream.Seek(0, SeekOrigin.Begin);

            T Result = (T)Formatter.Deserialize(Stream);

            return Result;

        }

        #endregion

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
                FieldInfo fi = Value.GetType().GetEventField(EventName);

                if (fi == null)
                {
                    return;
                }

                fi.SetValue(Value, null);
            }
        }

        /// <summary>
        /// Convert the target object into an array of <see cref="Byte"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>Array of <see cref="Byte"/></returns>
        public static byte[] ToSerialisedByteArray(this object Value)
        {
            if (Value == null)
            {
                return null;
            }

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, Value);

            return ms.ToArray();

        }

        #endregion

        #region type 

        /// <summary>
        /// Returns the specified Event from the target Object
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="EventName">The name of the Event to return</param>
        /// <returns></returns>
        public static FieldInfo GetEventField(this Type Value, string EventName)
        {
            FieldInfo Field = null;

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

        #endregion

        #region string 

        /// <summary>
        /// Convert the target <see cref="string"/> into a <see cref="DateTime"/> and remove the current TimeZone offset
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>The current <see cref="DateTime"/> minus the current Timezone offset</returns>
        public static DateTime SubtractTimeZone(this string Value)
        {
            DateTime localTime = ManagementDateTimeConverter.ToDateTime(Value);

            // Subtract the current timezone offset 
            localTime -= System.TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

            return localTime;
        }

        /// <summary>
        /// Convert the target <see cref="string"/> into a <see cref="DateTime"/> and add the current TimeZone offset
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>The current <see cref="DateTime"/> plus the current Timezone offset</returns>
        public static DateTime AddTimeZone(this string Value)
        {
            DateTime localTime = ManagementDateTimeConverter.ToDateTime(Value);

            // Add the current timezone offset 
            localTime += System.TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);

            return localTime;
        }

        /// <summary>
        /// Convert the given <see cref="string"/> into the target type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Value"></param>
        /// <returns>The target value, as the target type</returns>
        public static T As<T>(this string Value)
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
        public static T As<T>(this string Value, T DefaultValue)
        {
            if (typeof(T) == typeof(bool))
            {
                return (T)Convert.ChangeType(AsBool(Value,
                                                    Convert.ToBoolean(DefaultValue)),
                                                    typeof(T));
            }

            T result = default(T);

            if (String.IsNullOrEmpty(Value))
            {
                return DefaultValue;
            }

            try
            {
                Type underlyingType = Nullable.GetUnderlyingType(typeof(T));

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
            finally
            {
            }

            return result;
        }

        /// <summary>
        /// Convert the given <see cref="string"/> into a <see cref="bool"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>The target value, as a <see cref="bool"/></returns>
        public static bool AsBool(this string Value)
        {
            return AsBool(Value, false);
        }

        /// <summary>
        /// Convert the given <see cref="string"/> into a <see cref="bool"/>.  If the value is null or Empty, return the default value instead
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="DefaultValue">The value to return if the target value is null or Empty</param>
        /// <returns>The target value, as a <see cref="bool"/></returns>
        public static bool AsBool(this string Value, bool DefaultValue)
        {
            if (String.IsNullOrEmpty(Value))
            {
                return DefaultValue;
            }

            switch (Value.ToLower())
            {
                case "1":
                case "t":
                case "true":
                    return true;
                case "0":
                case "f":
                case "false":
                    return false;
                default:
                    return DefaultValue;
            }
        }

        /// <summary>
        /// Search within the target <see cref="string"/> for the specified value
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SearchString">The <see cref="string"/> to search for</param>
        /// <returns><see cref="bool"/> indicating success (true) or failure (false)</returns>
        public static bool Exists(this string Value, string SearchString)
        {
            bool Result = false;
            string[] StringToArray = Value.Split(',');

            if (Array.IndexOf(StringToArray, SearchString) > -1)
            {
                Result = true;
            }

            return Result;
        }

        // http://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp 
        public static byte[] Hash(this string Value, byte[] salt)
        {
            return Hash(Encoding.UTF8.GetBytes(Value), salt);

            //return Encoding.UTF8.GetBytes(value).Hash(salt); 
        }

        /// <summary> 
        /// Return the length of the target <see cref="string"/> after performing a null coalescing check and Trim() 
        /// </summary> 
        /// <param name="Value">The <see cref="string"/> to query</param> 
        /// <returns>An <see cref="Int32"/> representing the length</returns> 
        public static int LengthOf(this string Value)
        {
            return (Value ?? string.Empty).Trim().Length;
        }

        public static string RemoveAlphaCharacters(this string Value)
        {
            return new string(Value.Where(c => char.IsDigit(c) || c == '.').ToArray());
        }

        public static string RemoveNumericCharacters(this string Value)
        {
            return new string(Value.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c) || c == '-').ToArray());
        }

        public static string Capitalise(this string Value)
        {
            string Result = Value;

            if (Value == null)
            {
                return null;
            }

            if (Value.Length > 1)
            {   // Capitalise everything to the right of a space or hyphen

                StringBuilder r = new StringBuilder();

                // First character is always uppercase
                r.Append(Value[0].ToString().ToUpper());

                for (int i = 1; i < Value.Length; i++)
                {
                    if (Value[i - 1] == (char)32 || Value[i - 1] == Convert.ToChar("-"))
                    {
                        r.Append(Value[i].ToString().ToUpper());
                    }
                    else
                    {
                        r.Append(Value[i].ToString());
                    }
                }

                Result = r.ToString();
            }
            else
            {
                return Value.ToUpper();
            }

            return Result;
        }

        /// <summary>
        /// Convert a string to an array of <see cref="byte"/>
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>byte[] version of the input <see cref="string"/></returns>
        public static byte[] ToByteArray(this string Value)
        {
            byte[] Result = null;

            Result = Encoding.ASCII.GetBytes(Value);

            return Result;
        }

        #endregion

        #region byte[] 

        /// <summary>
        /// Convert an array of <see cref="byte"/> to an image
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>Image contained within the input array of <see cref="byte"/></returns>
        public static Image ToImage(this byte[] Value)
        {
            MemoryStream ms = new MemoryStream(Value);
            Image returnImage = Image.FromStream(ms);

            return returnImage;
        }

        /// <summary>
        /// Convert an array of <see cref="byte"/> to a string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>String version of the input array of <see cref="byte"/></returns>
        public static string ToString(this byte[] Value)
        {
            string Result = null;

            Result = Encoding.ASCII.GetString(Value);

            return Result;
        }

        /// <summary>
        /// Convert an array of <see cref="byte"/> to a string
        /// </summary>
        /// <param name="Value"></param>
        /// <returns>String version of the input array of <see cref="byte"/></returns>
        public static string ToText(this byte[] Value, int Start, int Length)
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

            return (Result).Trim();
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
            PropertyInfo doubleBufferPropertyInfo = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

            doubleBufferPropertyInfo.SetValue(control, enable, null);
        }

        /// <summary> 
        /// Execute the Action asynchronously on the UI thread, does not block execution on the calling thread. 
        /// Usage:  this.AsyncOnUIThread(() => this.myLabel.Text = "Text Goes Here"); 
        /// </summary> 
        /// <param name="control">The Control to perform the Code against</param> 
        /// <param name="code">The Code to perform</param> 
        public static void OnUIThreadAsync(this Control control, Action code)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(code);
            }
            else
            {
                Cursor.Current = Cursors.AppStarting;
                code.Invoke();
                Cursor.Current = Cursors.Default;
            }
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

        #endregion

        #region FileInfo 

        /// <summary>
        /// Print the target file on the default printer
        /// </summary>
        /// <param name="value"></param>
        public static void Print(this FileInfo value)
        {
            Process p = new Process();
            p.StartInfo.FileName = value.FullName;
            p.StartInfo.Verb = "Print";
            p.Start();
        }

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

        #endregion

        #region Image 

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            return ms.ToArray();
        }

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
        public static Image Rotate(this Image img, float Angle)
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

        public static Image MakeTransparentRange(this Image img, Color ReplacementColor, int RedTolerance, int GreenTolerance, int BlueTolerance, int AlphaRange = 255)
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

        #region Icon 

        public static Icon ToIcon(this Image i, Size OutputSize)
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

        public static void Destroy(this Icon i)
        {
            if (i != null)
            {
                DestroyIcon(i.Handle);
            }
        }

        #endregion

        #region int 

        public static string ToMemorySize(this int value, int decimalPlaces = 0)
        {
            return ((long)value).ToMemorySize(decimalPlaces);
        }

        public static string FinancialQuarter(this int Value)
        {
            switch (Value)
                {
                case 1:
                    return "July";
                case 2:
                    return "October";
                case 3:
                    return "January";
                case 4:
                    return "April";
                default:
                    return "Error:  Invalid quarter for " + Value.ToString();
            }
        }

        public static string CalendarQuarter(this int Value)
        {
            switch (Value)
            {
                case 1:
                    return "January";
                case 2:
                    return "April";
                case 3:
                    return "July";
                case 4:
                    return "October";
                default:
                    return "Error:  Invalid quarter for " + Value.ToString();
            }
        }

        #endregion

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
                    temp.Add((ListViewItem)item.Clone());
                }

                return temp;
            }
            else
            {
                return (ListView.ListViewItemCollection)listView.Parent.Invoke(new DelegateGetItems(GetItems), new object[] { listView });
            }
        }

        #endregion

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
                    if (Data.Name == Name)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
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
                return Result;
            }

            return Result;
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
                return Result;
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
                return Result;
            }

            return Result;
        }

        #endregion

        #region XmlElement 

        public static string ValueToString(this XmlElement Node)
        {
            string Result = "";

            try
            {
                Result = Node.InnerText;
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static string[] ValueToStringArray(this XmlElement Node)
        {
            string[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select s).ToArray();
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static ushort ValueToUInt16(this XmlElement Node)
        {
            ushort Result = 0;

            Result = Convert.ToUInt16(Node.InnerText);

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
                return Result;
            }

            return Result;
        }

        public static short[] ValueToInt16Array(this XmlElement Node)
        {
            short[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToInt16(s)).ToArray();
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static ushort[] ValueToUInt16Array(this XmlElement Node)
        {
            ushort[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToUInt16(s)).ToArray();
            }
            catch
            {
                return Result;
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
                return Result;
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
                return Result;
            }

            return Result;
        }

        public static int[] ValueToInt32Array(this XmlElement Node)
        {
            int[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToInt32(s)).ToArray();
            }
            catch
            {
                return Result;
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
                return Result;
            }

            return Result;
        }

        public static long[] ValueToInt64Array(this XmlElement Node)
        {
            long[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToInt64(s)).ToArray();
            }
            catch
            {
                return Result;
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
                return Result;
            }

            return Result;
        }

        public static ulong[] ValueToUInt64Array(this XmlElement Node)
        {
            ulong[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToUInt64(s)).ToArray();
            }
            catch
            {
                return Result;
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
                return Result;
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
                return Result;
            }

            return Result;
        }

        public static int[] ValueToIntArray(this XmlElement Node)
        {
            int[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select (Convert.ToInt32(s))).ToArray();
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static uint[] ValueToUInt32Array(this XmlElement Node)
        {
            uint[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToUInt32(s)).ToArray();
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static uint[] ValueToUIntArray(this XmlElement Node)
        {
            uint[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select (uint)Convert.ToInt32(s)).ToArray();
            }
            catch
            {
                return Result;
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
                return Result;
            }

            return Result;
        }

        public static DateTime[] ValueToDateTimeArray(this XmlElement Node)
        {
            DateTime[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToDateTime(s)).ToArray();
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static bool ValueToBool(this XmlElement Node)
        {
            bool Result = false;

            try
            {
                Result = Convert.ToBoolean(Node.InnerText);
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        public static bool[] ValueToBoolArray(this XmlElement Node)
        {
            bool[] Result = { };

            try
            {
                Result = (from string s in Node.InnerText
                          select Convert.ToBoolean(s)).ToArray();
            }
            catch
            {
                return Result;
            }

            return Result;
        }

        #endregion

        #region ManagementClass 

        public static bool ContainsProperty(this System.Management.ManagementClass Class, string PropertyName)
        {
            PropertyInfo Info = Class.GetType().GetProperty(PropertyName);

            return Info != null;

        }

        #endregion

        #region List<T> 

        /// <summary> 
        /// A generic function that loops a List of any class type, looking for a boolean field value. 
        /// If it finds a TRUE value, then exit the loop and return TRUE. 
        /// </summary> 
        /// <typeparam name="T">Generic reference</typeparam> 
        /// <param name="List">Refers to this (current List object)</param> 
        /// <param name="FieldName">The field containing the boolean value</param> 
        /// <returns>Boolean True or False</returns> 
        public static bool AreAnyOfTheseRequired<T>(this List<T> List, string FieldName)
        {
            bool returnValue = false;
            foreach (T item in List)
            {
                if ((bool)item.GetType().GetProperty(FieldName).GetValue(item, null))
                {
                    returnValue = true;
                    break;
                }
            }

            return returnValue;
        }

        public static bool Approximates<T>(this List<T> Value, List<T> Comparison)
        {
            return Value.Count == Comparison.Count &&
                   Value.All(Comparison.Contains) &&
                   Comparison.All(Value.Contains);
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

            if (formatted.EndsWith(", ", StringComparison.CurrentCulture))
                formatted = formatted.Substring(0, formatted.Length - 2);

            if (string.IsNullOrEmpty(formatted))
                formatted = "0 seconds";

            return formatted;
        }

        #endregion

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

        #region Enums 

        /// <summary> 
        /// Gets all items for an enum value. 
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="Value">The value.</param> 
        /// <returns></returns> 
        public static IEnumerable<T> GetAllItems<T>(this Enum Value)
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }

        /// <summary> 
        /// Gets all items for an enum type. 
        /// </summary> 
        /// <typeparam name="T"></typeparam>  
        /// <returns></returns> 
        public static IEnumerable<T> GetAllItems<T>() where T : struct
        {
            foreach (object item in Enum.GetValues(typeof(T)))
            {
                yield return (T)item;
            }
        }

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
        /// foreach (var item in dummy.GetAllSelectedItems()) 
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
        /// if (dummy.Contains (EnumExample.ValueA)) 
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

        #region System.DirectoryServices.PropertyCollection 

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
                return false;
            }

            return false;
        }

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
                    Result = (string)Value[Name].Value;
                }
            }
            catch
            {
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
        }

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
                return Result;
            }

            return Result;
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

        #region System.IO.Ports.SerialPort

        //public static byte[] Bytes(this System.IO.Ports.SerialPort port, byte[] ReceivedData)
        //{
        //    byte[] Result = null;

        //    return Result;
        //}

        #endregion

        #region DateTime

        public static int Age(this DateTime DateOfBirth)
        {
            int age = DateTime.Now.Year - DateOfBirth.Year;

            if (DateTime.Now < DateOfBirth.AddYears(age))
            {
                age--;
            }

            if (age < 0)
            {
                age = 0;
            }

            return age;
        }

        public static string FinancialQuarter(this DateTime Value)
        {
            if (Value.Month >= 7 && Value.Month <= 9)
                return "July to September " + Value.Year;
            else if (Value.Month >= 10 && Value.Month <= 12)
                return "October to December " + Value.Year;
            else if (Value.Month >= 1 && Value.Month <= 3)
                return "January to March " + ((Value.Year)).ToString();
            else
                return "April to June " + ((Value.Year)).ToString();
        }

        public static string CalendarQuarter(this DateTime Value)
        {
            if (Value.Month >= 1 && Value.Month <= 3)
                return "January to March " + Value.Year;
            else if (Value.Month >= 4 && Value.Month <= 6)
                return "April to June " + Value.Year;
            else if (Value.Month >= 7 && Value.Month <= 9)
                return "July to September " + Value.Year;
            else
                return "October to December " + Value.Year;
        }

        #endregion

        #region bool

        /// <summary>
        /// Convert to a Database bit
        /// </summary>
        /// <param name="Value"></param>
        /// <returns><see cref="Int16"/></returns>
        public static Int16 ToBit(this bool Value)
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

        #region DataRow

        public static string GetStringField(this System.Data.DataRow Row, string Field)
        {
            string Result = "";

            if (Row.Table.Columns.Contains(Field))
            {
                Result = Row[Field].ToString();
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

        public static DateTime GetDateField(this System.Data.DataRow Row, string Field)
        {
            DateTime Result = DateTime.MinValue;

            if (Row.Table.Columns.Contains(Field))
            {
                Result = (DateTime)Row[Field];
            }

            return Result;
        }

        public static bool GetBoolField(this System.Data.DataRow Row, string Field)
        {
            bool Result = false;

            if (Row.Table.Columns.Contains(Field))
            {
                Result = (bool)Row[Field];
            }

            return Result;
        }

        #endregion

        #region ResourceManager

        public static MemoryStream GetMemoryStream(this ResourceManager resourceManager, String name)
        {
            object resource = resourceManager.GetObject(name);

            if (resource is byte[])
            {
                return new MemoryStream((byte[])resource);
            }
            else
            {
                throw new System.InvalidCastException("The specified resource is not a binary resource.");
            }
        }

        #endregion

        #endregion

    }
}
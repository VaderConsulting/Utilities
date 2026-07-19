namespace Utilities
{
    public static class Transform
    {
        [Serializable]
        public enum Type
        {
            None,
            Value,
            Character,
            Length,
            Substring,
            Database
        }

        [Serializable]
        public enum MethodType
        {
            Unknown = -1,
            // ===================
            None,
            // ===================
            Value_Copy,
            Value_RemoveAlphaChars,
            Value_RemoveNumericChars,
            Value_Lowercase,
            Value_Uppercase,
            Value_Capitalise,
            Value_Hash,
            //Value_Date,
            // ===================
            Character_RemoveChar,
            Character_LeftOfChar,
            Character_RightOfChar,
            Character_SwitchLeftTwoValues,
            Character_SwitchRightTwoValues,
            Character_SwitchLeftAndRightValues,
            Character_FirstValue,
            Character_SecondValue,
            Character_ThirdValue,
            Character_LeftTwoValues,
            Character_RightTwoValues,
            // ===================
            Length_RemoveLeftChars,
            Length_RemoveRightChars,
            // ===================
            Substring_RemoveChars,
            Substring_ExtractChars,
            // ===================
            Database_Mapping
        }

        [Serializable]
        public enum ValueMethod
        {
            Copy,
            RemoveAlphaChars,
            RemoveNumericChars,
            Lowercase,
            Uppercase,
            Capitalise,
            Hash//,
            //Date
        }

        [Serializable]
        public enum CharacterMethod
        {
            RemoveChar,
            LeftOfChar,
            RightOfChar,
            SwitchLeftTwoValues,
            SwitchRightTwoValues,
            SwitchLeftAndRightValues,
            FirstValue,
            SecondValue,
            ThirdValue,
            LeftTwoValues,
            RightTwoValues
        }

        [Serializable]
        public enum LengthMethod
        {
            RemoveLeftChars,
            RemoveRightChars
        }

        [Serializable]
        public enum SubstringMethod
        {
            RemoveChars,
            ExtractChars
        }

        [Serializable]
        public enum DatabaseMethod
        {
            Mapping
        }

        //public static string Execute(string Value, MethodType Method, string Character = "", int Count = 0, int Start = 0)
        //{
        //    string Result = "";

        //    switch (Method)
        //    {
        //        case MethodType.None:
        //            Result = Value;
        //            break;
        //        case MethodType.Value_RemoveAlphaChars:
        //            Result = Value.RemoveAlphaCharacters();
        //            break;
        //        case MethodType.Value_RemoveNumericChars:
        //            Result = Value.RemoveNumericCharacters();
        //            break;
        //        case MethodType.Value_Lowercase:
        //            Result = Value.ToLower();
        //            break;
        //        case MethodType.Value_Uppercase:
        //            Result = Value.ToUpper();
        //            break;
        //        case MethodType.Character_RemoveChar:
        //            Result = Execute(Value, CharacterMethod.RemoveChar, Character);
        //            break;
        //        case MethodType.Character_LeftOfChar:
        //            Result = Execute(Value, CharacterMethod.LeftOfChar, Character);
        //            break;
        //        case MethodType.Character_RightOfChar:
        //            Result = Execute(Value, CharacterMethod.RightOfChar, Character);
        //            break;
        //        case MethodType.Character_SwitchLeftTwoValues:
        //            Result = Execute(Value, CharacterMethod.SwitchLeftTwoValues, Character);
        //            break;
        //        case MethodType.Character_SwitchRightTwoValues:
        //            Result = Execute(Value, CharacterMethod.SwitchRightTwoValues, Character);
        //            break;
        //        case MethodType.Character_SwitchLeftAndRightValues:
        //            Result = Execute(Value, CharacterMethod.SwitchLeftAndRightValues, Character);
        //            break;
        //        case MethodType.Character_FirstValue:
        //            Result = Execute(Value, CharacterMethod.FirstValue, Character);
        //            break;
        //        case MethodType.Character_SecondValue:
        //            Result = Execute(Value, CharacterMethod.SecondValue, Character);
        //            break;
        //        case MethodType.Character_ThirdValue:
        //            Result = Execute(Value, CharacterMethod.ThirdValue, Character);
        //            break;
        //        case MethodType.Character_LeftTwoValues:
        //            Result = Execute(Value, CharacterMethod.LeftTwoValues, Character);
        //            break;
        //        case MethodType.Character_RightTwoValues:
        //            Result = Execute(Value, CharacterMethod.RightTwoValues, Character);
        //            break;
        //        case MethodType.Length_RemoveLeftChars:
        //            Result = Execute(Value, LengthMethod.RemoveLeftChars, Count);
        //            break;
        //        case MethodType.Length_RemoveRightChars:
        //            Result = Execute(Value, LengthMethod.RemoveRightChars, Count);
        //            break;
        //        case MethodType.Substring_RemoveChars:
        //            Result = Execute(Value, SubstringMethod.RemoveChars, Start, Count);
        //            break;
        //        case MethodType.Substring_ExtractChars:
        //            Result = Execute(Value, SubstringMethod.ExtractChars, Start, Count);
        //            break;
        //    }

        //    return Result;
        //}

        //private static string ExecuteMethod(string Value, SimpleMethod Method)
        //{
        //    return Execute(Value, Method);
        //}

        //private static string ExecuteMethod(string Value, CharacterMethod Method, string Character)
        //{
        //    return Execute(Value, Method, Character);
        //}

        //private static string ExecuteMethod(string Value, LengthMethod Method, int Count)
        //{
        //    return Execute(Value, Method, Count);
        //}

        //private static string ExecuteMethod(string Value, SubstringMethod Method, int Start, int Count)
        //{
        //    return Execute(Value, Method, Start, Count);
        //}

        //public static string Execute(string Value, MethodType Type)
        //{
        //    string Result = Value;

        //    switch (Type)
        //    {
        //        case MethodType.None:
        //            Result = Value;
        //            break;
        //        case MethodType.Value_RemoveAlphaChars:
        //            Result = Value.RemoveAlphaCharacters();
        //            break;
        //        case MethodType.Value_RemoveNumericChars:
        //            Result = Value.RemoveNumericCharacters();
        //            break;
        //        case MethodType.Value_Lowercase:
        //            Result = Value.ToLower();
        //            break;
        //        case MethodType.Value_Uppercase:
        //            Result = Value.ToUpper();
        //            break;
        //        case MethodType.Character_RemoveChar:
        //            break;
        //        case MethodType.Character_LeftOfChar:
        //            break;
        //        case MethodType.Character_RightOfChar:
        //            break;
        //        case MethodType.Character_SwitchLeftTwoValues:
        //            break;
        //        case MethodType.Character_SwitchRightTwoValues:
        //            break;
        //        case MethodType.Character_SwitchLeftAndRightValues:
        //            break;
        //        case MethodType.Character_FirstValue:
        //            break;
        //        case MethodType.Character_SecondValue:
        //            break;
        //        case MethodType.Character_ThirdValue:
        //            break;
        //        case MethodType.Character_LeftTwoValues:
        //            break;
        //        case MethodType.Character_RightTwoValues:
        //            break;
        //        case MethodType.Length_RemoveLeftChars:
        //            break;
        //        case MethodType.Length_RemoveRightChars:
        //            break;
        //        case MethodType.Substring_RemoveChars:
        //            break;
        //        case MethodType.Substring_ExtractChars:
        //            break;
        //    }

        //    return Result;
        //}

        public static string? Execute(string Value, MethodType Method, params string[] Parameters)
        {
            string? Result = "";

            switch (Method)
            {
                case MethodType.None:
                    Result = null;
                    break;
                case MethodType.Value_Copy:
                    Result = Execute(Value, ValueMethod.Copy);
                    break;
                case MethodType.Value_RemoveAlphaChars:
                    Result = Execute(Value, ValueMethod.RemoveAlphaChars);
                    break;
                case MethodType.Value_RemoveNumericChars:
                    Result = Execute(Value, ValueMethod.RemoveNumericChars);
                    break;
                case MethodType.Value_Lowercase:
                    Result = Execute(Value, ValueMethod.Lowercase);
                    break;
                case MethodType.Value_Uppercase:
                    Result = Execute(Value, ValueMethod.Uppercase);
                    break;
                case MethodType.Value_Capitalise:
                    Result = Execute(Value, ValueMethod.Capitalise);
                    break;
                case MethodType.Value_Hash:
                    Result = Execute(Value, ValueMethod.Hash);
                    break;
                //case MethodType.Value_Date:
                //    Result = Execute(Value, ValueMethod.Date);
                //    break;
                case MethodType.Character_RemoveChar:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.RemoveChar, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_LeftOfChar:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.LeftOfChar, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_RightOfChar:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.RightOfChar, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_SwitchLeftTwoValues:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.SwitchLeftTwoValues, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_SwitchRightTwoValues:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.SwitchRightTwoValues, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_SwitchLeftAndRightValues:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.SwitchLeftAndRightValues, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_FirstValue:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.FirstValue, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_SecondValue:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.SecondValue, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_ThirdValue:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.ThirdValue, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_LeftTwoValues:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.LeftTwoValues, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Character_RightTwoValues:
                    {
                        string[] SplitParams = Parameters.Take(Parameters.Length - 1).ToArray<string>();
                        string JoinParam = Parameters[Parameters.Length - 1];

                        Result = Execute(Value, CharacterMethod.RightTwoValues, SplitParams, JoinParam);
                        break;
                    }
                case MethodType.Length_RemoveLeftChars:
                    Result = Execute(Value, LengthMethod.RemoveLeftChars, Convert.ToInt32(Parameters[0]));
                    break;
                case MethodType.Length_RemoveRightChars:
                    Result = Execute(Value, LengthMethod.RemoveRightChars, Convert.ToInt32(Parameters[0]));
                    break;
                case MethodType.Substring_RemoveChars:
                    Result = Execute(Value, SubstringMethod.RemoveChars, Convert.ToInt32(Parameters[0]), Convert.ToInt32(Parameters[1]));
                    break;
                case MethodType.Substring_ExtractChars:
                    Result = Execute(Value, SubstringMethod.ExtractChars, Convert.ToInt32(Parameters[0]), Convert.ToInt32(Parameters[1]));
                    break;
                case MethodType.Database_Mapping:
                    // Can't do this here as we don't know the database values
                    throw new NotSupportedException("You cannot apply Database_Mapping Method at this time");
                case MethodType.Unknown:
                    break;
            }

            return Result;
        }

        public static string? Execute(string Value, ValueMethod Method)
        {
            string? Result = "";

            switch (Method)
            {
                case ValueMethod.Copy:
                    Result = Value;
                    break;
                case ValueMethod.RemoveAlphaChars:
                    Result = Value.RemoveAlphaCharacters();
                    break;
                case ValueMethod.RemoveNumericChars:
                    Result = Value.RemoveNumericCharacters();
                    break;
                case ValueMethod.Lowercase:
                    Result = Value.ToLower();
                    break;
                case ValueMethod.Uppercase:
                    Result = Value.ToUpper();
                    break;
                case ValueMethod.Capitalise:
                    Result = Value.Capitalise();
                    break;
                case ValueMethod.Hash:
                    Result = Value.ConstantLengthHash();
                    break;
                default:
                    break;
                    //case ValueMethod.Date:
                    //    Result = Value.As<DateTime>(DateTime.MinValue).ToString();
                    //    break;
            }

            return Result;
        }

        public static string Execute(string Value, CharacterMethod Type, string[] SplitCharacters, string JoinCharacter)
        {
            string Result = "";

            if (SplitCharacters.Length == 0)
            {
                return Value;
            }

            if (JoinCharacter.Length == 0)
            {
                return Value;
            }

            switch (Type)
            {
                case CharacterMethod.RemoveChar:
                    foreach (string s in SplitCharacters)
                    {
                        Result = Value.Replace(s, "");
                    }
                    break;
                case CharacterMethod.LeftOfChar:
                    {
                        try
                        {
                            Result = string.Join("/", Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToArray<string>()).Trim();
                        }
                        catch
                        {
                            Result = $"Error: Cannot retrieve right portion of {Value}, using split characters";
                        }
                        break;
                    }
                case CharacterMethod.RightOfChar:
                    {
                        try
                        {
                            string[] SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);

                            Result = SplitValues[0].Trim();
                        }
                        catch
                        {
                            Result = $"Error: Cannot retrieve left portion of {Value}, using split characters";
                        }
                        break;
                    }
                case CharacterMethod.SwitchLeftTwoValues:
                    {
                        try
                        {
                            string[] SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);
                            string Left1 = SplitValues[0];
                            string Left2 = SplitValues[1];
                            string Remainder = string.Join(JoinCharacter, SplitValues.Skip(2).ToArray<string>());

                            if (Left2.StartsWith(" "))
                            {
                                Left2 = Left2.TrimStart();
                                Left1 = " " + Left1;
                            }

                            if (Remainder != "")
                            {
                                Result = Left2 + JoinCharacter + Left1 + JoinCharacter + Remainder;
                            }
                            else
                            {
                                Result = Left2 + JoinCharacter + Left1;
                            }
                        }
                        catch
                        {
                            Result = $"Error: Cannot switch left two values of {Value}, using split characters";
                        }
                        break;
                    }
                case CharacterMethod.SwitchRightTwoValues:
                    {
                        try
                        {
                            string[] SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);
                            string Right1 = SplitValues[SplitValues.Length - 1];
                            string Right2 = SplitValues[SplitValues.Length - 2];
                            string[] Remainder = SplitValues.Take(SplitValues.Length - 2).ToArray<string>();

                            if (Right1.StartsWith(" "))
                            {
                                Right1 = Right1.TrimStart();
                                Right2 = " " + Right2;
                            }

                            if (Remainder.Length > 0)
                            {
                                Result = string.Join(JoinCharacter, Remainder) + JoinCharacter + Right1 + JoinCharacter + Right2;
                            }
                            else
                            {
                                Result = Right1 + JoinCharacter + Right2;
                            }
                        }
                        catch
                        {
                            Result = $"Error: Cannot switch right two values of {Value}, using split characters";
                        }
                        break;
                    }
                case CharacterMethod.SwitchLeftAndRightValues:
                    {
                        try
                        {
                            string[] SplitValues;
                            string Left;
                            string Right;
                            string Middle;

                            SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);

                            if (SplitValues.Length < 2)
                            {
                                return Value;
                            }

                            Left = SplitValues[0];
                            Right = SplitValues[SplitValues.Length - 1];
                            Middle = string.Join(JoinCharacter, SplitValues.Skip(1).Take(SplitValues.Length - 2).ToArray());

                            if (Right.StartsWith(" "))
                            {
                                Right = Right.TrimStart();
                                Left = " " + Left;
                            }

                            if (Middle != "")
                            {
                                Result = Right + JoinCharacter + Middle + JoinCharacter + Left;
                            }
                            else
                            {
                                Result = Right + JoinCharacter + Left;
                            }
                        }
                        catch
                        {
                            Result = $"Error: Cannot switch right two values of {Value}, using split characters";
                        }
                        break;
                    }
                case CharacterMethod.FirstValue:
                    {
                        try
                        {
                            string[] SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);
                            string Left = SplitValues[0].Trim();

                            Result = Left;
                        }
                        catch
                        {
                            Result = "Error: 1st value not found";
                        }
                        break;
                    }
                case CharacterMethod.SecondValue:
                    {
                        try
                        {
                            string[] SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);
                            string Left = SplitValues[1].Trim();

                            Result = Left;
                        }
                        catch
                        {
                            Result = "Error: 2nd value not found";
                        }
                        break;
                    }
                case CharacterMethod.ThirdValue:
                    {
                        try
                        {
                            string[] SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries); // char.Parse(Characters));
                            string Left = SplitValues[2].Trim();

                            Result = Left;
                        }
                        catch
                        {
                            Result = "Error: 3rd value not found";
                        }
                        break;
                    }
                case CharacterMethod.LeftTwoValues:
                    {
                        try
                        {
                            string[] SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);
                            string Left1 = SplitValues[0];
                            string Left2 = SplitValues[1];

                            Result = Left1 + JoinCharacter + Left2;
                        }
                        catch
                        {
                            Result = $"Error: Cannot retrieve left two values of {Value}, using split characters";
                        }
                        break;
                    }
                case CharacterMethod.RightTwoValues:
                    {
                        try
                        {
                            string[] SplitValues = Value.Split(SplitCharacters, StringSplitOptions.RemoveEmptyEntries);
                            string Right1 = SplitValues[SplitValues.Length - 1];
                            string Right2 = SplitValues[SplitValues.Length - 2];

                            Result = Right2 + JoinCharacter + Right1;
                        }
                        catch
                        {
                            Result = $"Error: Cannot retrieve right two values of {Value}, using split characters";
                        }
                        break;
                    }
            }

            return Result;
        }

        public static string Execute(string Value, LengthMethod Type, int Count)
        {
            string Result = Value;

            switch (Type)
            {
                case LengthMethod.RemoveLeftChars:
                    {
                        try
                        {
                            Result = Value.Substring(Count);
                        }
                        catch
                        {
                            Result = $"Error: Cannot remove {Count} characters from left of {Value}";
                        }
                        break;
                    }
                case LengthMethod.RemoveRightChars:
                    {
                        try
                        {
                            Result = Value.Substring(0, Value.Length - Count);
                        }
                        catch
                        {
                            Result = $"Error: Cannot remove {Count} characters from right of {Value}";
                        }
                        break;
                    }
            }

            return Result;
        }

        public static string Execute(string Value, SubstringMethod Type, int Start, int Count)
        {
            string Result = Value;

            switch (Type)
            {
                case SubstringMethod.RemoveChars:
                    {
                        try
                        {
                            Result = Value.Substring(Start - 1, Count);
                        }
                        catch
                        {
                            Result = $"Error: Cannot remove {Count} characters from {Value}";
                        }
                        break;
                    }
                case SubstringMethod.ExtractChars:
                    {
                        try
                        {
                            Result = Value.Substring(0, Start - 1) + Value.Substring(Start + Count - 1);
                        }
                        catch
                        {
                            Result = $"Error: Cannot extract {Count} characters from {Value}";
                        }
                        break;
                    }
            }

            return Result;
        }

        //public static string Execute(string Value, DatabaseMethod Type, string Result)
        //{
        //    //string Result = Value;

        //    // There is only one Database method

        //    return Result;
        //}
    }
}

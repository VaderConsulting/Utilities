using System;

namespace Utilities
{
    public static class Enums
    {
        public enum TimerState : int
        {
            Unknown = 0,
            Running = 1,
            Paused = 2
        }

        // https://assets.sportstg.com/assets/console/document/documents/20141204094258JFA_Grades_Policy,_procudures_and_guidelines_manual_Nov_2014.pdf
        public enum SeniorRanks : int
        {
            None = -1,
            RokKyu = 0,   // White
            GoKyu = 2,   // Yellow
            YonKyu = 4,   // Orange
            SanKyu = 6,   // Green
            NiKyu = 8,   // Blue
            IkKyu = 10,  // Brown
            ShoDan = 12,  // Black 1st Dan
            NiDan = 13,  // Black 2nd Dan
            SanDan = 14,  // Black 3rd Dan
            YonDan = 15,  // Black 4th Dan
            GoDan = 16,  // Black 5th Dan
            RokuDan = 17,  // Black 6th Dan
            ShichiDan = 18,  // Black 7th Dan
            HachiDan = 19,  // Black 8th Dan
            KuDan = 20,  // Black 9th Dan
            JuDan = 21   // Black 10th Dan
        }

        // https://assets.sportstg.com/assets/console/document/documents/20141204094258JFA_Grades_Policy,_procudures_and_guidelines_manual_Nov_2014.pdf
        public enum Colours : int
        {
            None = -1,
            White = 0,
            WhiteYellow = 1,
            Yellow = 2,
            YellowOrange = 3,
            Orange = 4,
            OrangeGreen = 5,
            Green = 6,
            GreenBlue = 7,
            Blue = 8,
            BlueBrown = 9,
            Brown = 10,
            Black = 11,
            Black1stDan = 12,
            Black2ndDan = 13,
            Black3rdDan = 14,
            Black4thDan = 15,
            Black5thDan = 16,
            Black6thDan = 17,
            Black7thDan = 18,
            Black8thDan = 19,
            Black9thDan = 20,
            Black10thDan = 21
        }

        public enum Sex : UInt16
        {
            Unknown = 0,
            Male = 1,
            Female = 2,
            Other = 3
        }

        public enum TechniqueType : int
        {
            Unknown = 0,
            Sweep = 1,
            Wheel = 2,
            Throw = 3,
            Drop = 4,
            Hold = 5,
            Strangle = 6,
            Lock = 7,
            Combination = 8,
            Counter = 9,
            Kata_Throw = 10,
            Kata_Ground = 11
        }

        public enum Winner : int
        {
            Unknown = 0,
            Player1 = 1,
            Player2 = 2,
            Draw = 3
        }

        public enum ConnectionState : int
        {
            Unknown = 0,
            Disconnected = 1,
            Connecting = 2,
            Connected = 3,
            Disconnecting = 4
        }

        /// <summary>Enum of Player colours</summary>
        public enum PlayerColors : int
        {
            None = 0,
            White = 1,
            Blue = 2
        }

        public enum InformationType : int
        {
            Player = 0,
            Nation = 1
        }

        public enum HorizontalPosition : int
        {
            None = 0,
            Left = 1,
            Right = 2
        }
    }
}

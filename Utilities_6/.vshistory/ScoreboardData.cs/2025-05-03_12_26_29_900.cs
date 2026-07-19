using System;

using Utilities;

namespace Utilities
{
    public class ScoreboardData
    {
        #region Enums

        public enum AgeGroupEnum : int
        {
            Cadet,
            Junior,
            Senior
        }

        public enum RoundEnum : int
        {
            RoundRobin,
            Elimination1,
            Elimination2,
            Elimination3,
            Elimination4,
            QuarterFinal,
            Repecharge,
            SemiFinal,
            Bronze,
            Final
        }

        public enum DisplayModeEnum : int
        {
            Logo,
            ContestInfo,
            WhiteInfo,
            BlueInfo,
            BothInfo,
            Scoreboard,
            Countdown,
            LastCall
        }

        public enum TimerFlagEnum : int
        {
            Stopped,
            Running
        }

        public enum OsaekomiTimerFlagEnum : int
        {
            Stopped,
            White,
            Blue
        }

        public enum WinnerEnum : int
        {
            None,
            White,
            Blue
        }

        public enum MatchResult : int
        {
            Unknown = 0,
            YuseiGachi = 1,   // Win by decision
            KikenGachi = 2,   // Win by withdrawal 
            FusenGachi = 3,   // Win by default (i.e. "walk-on")
            Fushogachi = 4,   // Win due to injury
            Hansokugachi = 5, // Win due to penalty
            Shikakugachi = 6, // Win due to disqualification
            HikeWake = 99     // Draw
        }

        public enum MatchState : int
        {
            Unknown = -3,
            Unconfigured = -2,
            Configuring = -1,
            ////////////////////////////
            Ready = 0,
            Starting = 1,
            Playing = 2,
            Pausing = 3,
            Paused = 4,
            Continuing = 5,
            ////////////////////////////
            GoldenScoreReady = 6,
            GoldenScoreStarting = 7,
            GoldenScoreStarted = 8,
            GoldenScorePaused = 9,
            GoldenScoreCompleting = 10,
            ////////////////////////////
            Completing = 11,
            Completed = 12
        }

        public enum TimerState : int
        {
            Unknown = 0,
            Running = 1,
            Paused = 2
        }

        #endregion

        #region Fields

        private IScoreboardProtocol _Protocol; // = null;

        #endregion

        #region Properties

        private string StartToken
        {
            get
            {
                return _Protocol.StartToken;
            }
        }

        private string ProtocolVersion
        {
            get
            {
                return _Protocol.ProtocolVersion;
            }
        }

        public string EventID
        {
            get
            {
                return _Protocol.EventID;
            }
        }

        public string Gender
        {
            get
            {
                return FromGender(_Protocol.Gender);
            }
        }

        public string Gender_Raw
        {
            get
            {
                return _Protocol.Gender;
            }
        }

        public string Category
        {
            get
            {
                return _Protocol.Category;
            }
        }

        public string AgeGroup
        {
            get
            {
                return FromAgeGroup(_Protocol.AgeGroup);
            }
        }

        public string AgeGroup_Raw
        {
            get
            {
                return _Protocol.AgeGroup;
            }
        }

        public string Round
        {
            get
            {
                return FromRound(_Protocol.Round);
            }
        }

        public string Round_Raw
        {
            get
            {
                return _Protocol.Round;
            }
        }

        public string ContestID
        {
            get
            {
                return _Protocol.ContestID;
            }
        }

        public string TimerFlag
        {
            get
            {
                return FromTimerFlag(_Protocol.TimerFlag);
            }
        }

        public string OsaekomiTimerFlag
        {
            get
            {
                return FromOsaekomiTimerFlag(_Protocol.OsaekomiTimerFlag);
            }
        }

        public Enums.TimerState TimerState
        {
            get
            {
                switch (FromTimerFlag(_Protocol.TimerFlag))
                {
                    case "Stopped":
                        return Enums.TimerState.Paused;
                    case "Running":
                        return Enums.TimerState.Running;
                    default:
                        return Enums.TimerState.Unknown;
                }
            }
        }

        public string TimerFlag_Raw
        {
            get
            {
                return _Protocol.TimerFlag;
            }
        }

        public string TimerMinute
        {
            get
            {
                return _Protocol.TimerMinute;
            }
        }

        public string TimerSecond
        {
            get
            {
                return _Protocol.TimerSecond;
            }
        }

        public TimeSpan Timer
        {
            get
            {
                return new TimeSpan(0, Convert.ToInt32(TimerMinute), Convert.ToInt32(TimerSecond));
            }
        }

        public string NationWhite
        {
            get
            {
                return _Protocol.NationWhite;
            }
        }

        public string IDWhite
        {
            get
            {
                return _Protocol.IDWhite;
            }
        }

        public string ShortNameWhite
        {
            get
            {
                return _Protocol.ShortNameWhite;
            }
        }

        public string WorldRankingListPositionWhite
        {
            get
            {
                return _Protocol.WorldRankingListPositionWhite;
            }
        }

        public string LongNameWhite
        {
            get
            {
                return _Protocol.LongNameWhite;
            }
        }

        public int IpponWhite
        {
            get
            {
                return Convert.ToInt32(_Protocol.IpponWhite);
            }
        }

        public int WazaAriWhite
        {
            get
            {
                return Convert.ToInt32(_Protocol.WazaAriWhite);
            }
        }

        private int YukoWhite
        {
            get
            {
                return Convert.ToInt32(_Protocol.YukoWhite);
            }
        }

        public int ShidoWhite
        {
            get
            {
                string Shido = FromShido(_Protocol.ShidoWhite);

                if (Shido != "Hansoku make")
                {
                    return Convert.ToInt32(Shido);
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool HansokuMakeWhite
        {
            get
            {
                string Shido = FromShido(_Protocol.ShidoWhite);

                if (Shido == "3")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string ShidoWhite_Raw
        {
            get
            {
                return _Protocol.ShidoWhite;
            }
        }

        public string TimerOsaekomiWhite
        {
            get
            {
                return _Protocol.TimerOsaekomiWhite;
            }
        }

        public string TeamScoreWhite
        {
            get
            {
                return _Protocol.TeamScoreWhite;
            }
        }

        public string NationBlue
        {
            get
            {
                return _Protocol.NationBlue;
            }
        }

        public string IDBlue
        {
            get
            {
                return _Protocol.IDBlue;
            }
        }

        public string ShortNameBlue
        {
            get
            {
                return _Protocol.ShortNameBlue;
            }
        }

        public string WorldRankingListPositionBlue
        {
            get
            {
                return _Protocol.WorldRankingListPositionBlue;
            }
        }

        public string LongNameBlue
        {
            get
            {
                return _Protocol.LongNameBlue;
            }
        }

        public int IpponBlue
        {
            get
            {
                return Convert.ToInt32(_Protocol.IpponBlue);
            }
        }

        public int WazaAriBlue
        {
            get
            {
                return Convert.ToInt32(_Protocol.WazaAriBlue);
            }
        }

        private int YukoBlue
        {
            get
            {
                return Convert.ToInt32(_Protocol.YukoBlue);
            }
        }

        public int ShidoBlue
        {
            get
            {
                string Shido = FromShido(_Protocol.ShidoBlue);

                if (Shido != "Hansoku make")
                {
                    return Convert.ToInt32(Shido);
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool HansokuMakeBlue
        {
            get
            {
                string Shido = FromShido(_Protocol.ShidoBlue);

                if (Shido == "3")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public string ShidoBlue_Raw
        {
            get
            {
                return _Protocol.ShidoBlue;
            }
        }

        public string TimerOsaekomiBlue
        {
            get
            {
                return _Protocol.TimerOsaekomiBlue;
            }
        }

        public string TeamScoreBlue
        {
            get
            {
                return _Protocol.TeamScoreBlue;
            }
        }

        public bool GoldenScore
        {
            get
            {
                return _Protocol.GoldenScore == "1";
            }
        }

        public string GoldenScore_Raw
        {
            get
            {
                return _Protocol.GoldenScore;
            }
        }

        public string Winner
        {
            get
            {
                return FromWinner(_Protocol.Winner);
            }
        }

        public string Winner_Raw
        {
            get
            {
                return _Protocol.Winner;
            }
        }

        public string RefereeID
        {
            get
            {
                return _Protocol.IDReferee;
            }
        }

        public string IDJudge1
        {
            get
            {
                return _Protocol.IDJudge1;
            }
        }

        public string IDJudge2
        {
            get
            {
                return _Protocol.IDJudge2;
            }
        }

        public int MatID
        {
            get
            {
                return Convert.ToInt32(_Protocol.IDMat);
            }
        }

        public string IDMat_Raw
        {
            get
            {
                return _Protocol.IDMat;
            }
        }

        public string DisplayMode
        {
            get
            {
                return FromDisplayMode(_Protocol.DisplayMode);
            }
        }

        public string DisplayMode_Raw
        {
            get
            {
                return _Protocol.DisplayMode;
            }
        }

        private string EndToken
        {
            get
            {
                return _Protocol.EndToken;
            }
        }

        #endregion

        #region Constructors and Destructor

        public ScoreboardData(IScoreboardProtocol protocol)
        {
            _Protocol = protocol;

            // Validate start and end tokens
            if (StartToken != "2" && StartToken != "123")
            {
                throw new InvalidOperationException("The StartToken is incorrect.");
            }

            if (EndToken != "3" && EndToken != "58" && EndToken != "110")
            {
                throw new InvalidOperationException("The EndToken is incorrect.");
            }
        }

        #endregion

        #region Private Methods

        private static string FromGender(string Value)
        {
            switch (Value.ToUpper())
            {
                case "M":
                    return "Mens";
                case "W":
                    return "Womens";
                default:
                    return ""; // "Other (" + Value + ")";
            }
        }

        private static string FromAgeGroup(string Value)
        {
            switch (Value.ToUpper())
            {
                case "S":
                    return "Senior";
                case "J":
                    return "Junior";
                case "C":
                    return "Cadet";
                default:
                    return ""; // "Other (" + Value + ")";
            }
        }

        private static string FromRound(string Value)
        {
            switch (Value.ToUpper())
            {
                case "0":
                    return "Round Robin";
                case "1":
                    return "Elimination round 1";
                case "2":
                    return "Elimination round 2";
                case "3":
                    return "Elimination round 3";
                case "4":
                    return "Elimination round 4";
                case "Q":
                    return "Quarter Final";
                case "R":
                    return "Repecharge";
                case "S":
                    return "Semi Final";
                case "B":
                    return "Bronze";
                case "F":
                    return "Final";
                default:
                    return ""; // "Other (" + Value + ")";
            }
        }

        private static string FromDisplayMode(string Value)
        {
            switch (Value.ToUpper())
            {
                case "1":
                    return "Logo";
                case "2":
                    return "Contest Info";
                case "3":
                    return "White Info";
                case "4":
                    return "Blue Info";
                case "5":
                    return "Both Info";
                case "6":
                    return "Scoreboard";
                case "C":
                    return "Countdown";
                case "L":
                    return "Last Call";
                default:
                    return ""; // "Other (" + Value + ")";
            }
        }

        private static string FromTimerFlag(string Value)
        {
            switch (Value.ToUpper())
            {
                case "0":
                    return "Stopped";
                case "1":
                    return "Running";
                default:
                    return ""; // "Other (" + Value + ")";
            }
        }

        private static string FromOsaekomiTimerFlag(string Value)
        {
            switch (Value.ToUpper())
            {
                case "0":
                    return "Stopped";
                case "W":
                    return "Osaekomi White";
                case "B":
                    return "Osaekomi Blue";
                default:
                    return ""; // "Other (" + Value + ")";
            }
        }

        private static string FromWinner(string Value)
        {
            switch (Value.ToUpper())
            {
                case "0":
                    return "-";
                case "W":
                    return "White";
                case "B":
                    return "Blue";
                default:
                    return ""; // "Other (" + Value + ")";
            }
        }

        private static string FromShido(string Value)
        {
            switch (Value.ToUpper())
            {
                case "H":
                    return "Hansoku make";
                default:
                    return Value;
            }
        }

        #endregion

        #region Public Methods

        public static string ToGender(string Value)
        {
            switch (Value)
            {
                case "Mens":
                    return "m";
                case "Womens":
                    return "w";
                default:
                    return Value;
            }
        }

        public static string ToAgeGroup(string Value)
        {
            switch (Value)
            {
                case "Senior":
                    return "s";
                case "Junior":
                    return "j";
                case "Cadet":
                    return "c";
                default:
                    return Value;
            }
        }

        public static string ToRound(string Value)
        {
            switch (Value)
            {
                case "Round Robin":
                    return "0";
                case "Elimination round 1":
                    return "1";
                case "Elimination round 2":
                    return "2";
                case "Elimination round 3":
                    return "3";
                case "Elimination round 4":
                    return "4";
                case "Quarter Final":
                    return "Q";
                case "Repecharge":
                    return "R";
                case "Semi-Final":
                    return "S";
                case "Bronze":
                    return "B";
                case "Final":
                    return "F";
                default:
                    return Value;
            }
        }

        public static string ToDisplayMode(string Value)
        {
            switch (Value)
            {
                case "Logo":
                    return "1";
                case "Contest Info":
                    return "2";
                case "White Info":
                    return "3";
                case "Blue Info":
                    return "4";
                case "Both Info":
                    return "5";
                case "Scoreboard":
                    return "6";
                case "Countdown":
                    return "C";
                case "Last Call":
                    return "L";
                default:
                    return Value;
            }
        }

        public static string ToTimerFlag(string Value)
        {
            switch (Value)
            {
                case "Stopped":
                    return "0";
                case "Running":
                    return "1";
                default:
                    return Value;
            }
        }

        public static string ToWinner(string Value)
        {
            switch (Value)
            {
                case "None":
                    return "0";
                case "White":
                    return "W";
                case "Blue":
                    return "B";
                default:
                    return Value;
            }
        }

        public static string ToShido(string Value)
        {
            switch (Value)
            {
                case "Hansoku make":
                    return "H";
                default:
                    return Value;
            }
        }

        public static string ToGender(Enums.Sex Value)
        {
            switch (Value)
            {
                case Enums.Sex.Male:
                    return "m";
                case Enums.Sex.Female:
                    return "w";
                default:
                    return "";
            }
        }

        public static string ToAgeGroup(AgeGroupEnum Value)
        {
            switch (Value)
            {
                case AgeGroupEnum.Senior:
                    return "s";
                case AgeGroupEnum.Junior:
                    return "j";
                case AgeGroupEnum.Cadet:
                    return "c";
                default:
                    return "";
            }
        }

        public static string ToRound(RoundEnum Value)
        {
            switch (Value)
            {
                case RoundEnum.RoundRobin:
                    return "0";
                case RoundEnum.Elimination1:
                    return "1";
                case RoundEnum.Elimination2:
                    return "2";
                case RoundEnum.Elimination3:
                    return "3";
                case RoundEnum.Elimination4:
                    return "4";
                case RoundEnum.QuarterFinal:
                    return "Q";
                case RoundEnum.Repecharge:
                    return "R";
                case RoundEnum.SemiFinal:
                    return "S";
                case RoundEnum.Bronze:
                    return "B";
                case RoundEnum.Final:
                    return "F";
                default:
                    return "";
            }
        }

        public static string ToDisplayMode(DisplayModeEnum Value)
        {
            switch (Value)
            {
                case DisplayModeEnum.Logo:
                    return "1";
                case DisplayModeEnum.ContestInfo:
                    return "2";
                case DisplayModeEnum.WhiteInfo:
                    return "3";
                case DisplayModeEnum.BlueInfo:
                    return "4";
                case DisplayModeEnum.BothInfo:
                    return "5";
                case DisplayModeEnum.Scoreboard:
                    return "6";
                default:
                    return "";
            }
        }

        public static string ToTimerFlag(TimerFlagEnum Value)
        {
            switch (Value)
            {
                case TimerFlagEnum.Stopped:
                    return "0";
                case TimerFlagEnum.Running:
                    return "1";
                default:
                    return "";
            }
        }

        public static string ToOsaekomiTimerFlag(OsaekomiTimerFlagEnum Value)
        {
            switch (Value)
            {
                case OsaekomiTimerFlagEnum.Stopped:
                    return "0";
                case OsaekomiTimerFlagEnum.White:
                    return "W";
                case OsaekomiTimerFlagEnum.Blue:
                    return "B";
                default:
                    return "";
            }
        }

        public static string ToWinner(WinnerEnum Value)
        {
            switch (Value)
            {
                case WinnerEnum.None:
                    return "0";
                case WinnerEnum.White:
                    return "W";
                case WinnerEnum.Blue:
                    return "B";
                default:
                    return "";
            }
        }

        #endregion
    }
}
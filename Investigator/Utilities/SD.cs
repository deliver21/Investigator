namespace Investigator.Utilities
{
    public class SD
    {
        public static string AppBaseUrl { get; set; }

        public static string privateVisibility = "Private";
        public static string publicVisibility = "Public";
        public static string restrictedVisibility = "Restricted";

        public const string CustomerRole = "Customer";
        public const string AdminRole = "Admin";

        public static string singleLineType = "SingleLine";
        public static string multiLineType = "MultiLine";
        public static string integerType = "Integer";
        public static string checkBoxType = "CheckBox";
        public static string phoneType = "Phone";
        public static string dateType = "Date";
        public static string file = "File";

        public const string EducationTopic = "Education";
        public const string PersonalTopic = "Personal";
        public const string ProfessionalTopic = "Professional";

        public static string UserManagerAPIBase { get; set; }

        public const string AppIdFacebook = "353431303785337";
        public const string AppSecretFacebook = "1374ec31638332c70d1fed8183616f68";

        //Form Status
        public const string ActiveStatus = "Active";
        public const string InactiveStatus = "Inactive";
        //Token Key
        public static string tokenCookie = "JWTToken";

    }
}

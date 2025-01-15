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

        public const string EducationTopic = "Education";
        public const string PersonalTopic = "Personal";
        public const string ProfessionalTopic = "Professional";

        public static string UserManagerAPIBase { get; set; }

        //Token Key
        public static string tokenCookie = "JWTToken";

    }
}

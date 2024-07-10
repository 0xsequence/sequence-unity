using Sequence.Authentication;

namespace Sequence.WaaS
{
    public enum IdentityType
    {
        Email,
        Guest,
        OIDC,
        PlayFab
    }

    public static class IdentityTypeExtensions
    {
        public static bool IsOIDC(this LoginMethod loginMethod)
        {
            return !(loginMethod == LoginMethod.Email || loginMethod == LoginMethod.Guest || loginMethod == LoginMethod.PlayFab || loginMethod == LoginMethod.None || loginMethod == LoginMethod.Custom);
        }

        public static IdentityType GetIdentityType(this string type)
        {
            switch (type)
            {
                case "Email":
                    return IdentityType.Email;
                case "Guest":
                    return IdentityType.Guest;
                case "OIDC":
                    return IdentityType.OIDC;
                case "PlayFab":
                    return IdentityType.PlayFab;
                default:
                    return IdentityType.Email;
            }
        }
    }
}
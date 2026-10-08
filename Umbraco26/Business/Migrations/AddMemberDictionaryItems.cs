using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Umbraco26.Business.Migrations
{
    /// <summary>Creates the dictionary items the member pages and the menu's member links read, in Swedish and English.</summary>
    public class AddMemberDictionaryItems(IMigrationContext context, IDictionaryItemService dictionaryItemService, ILanguageService languageService)
        : DictionaryItemsMigrationBase(context, dictionaryItemService, languageService)
    {
        protected override string ParentKey => "Member";

        protected override IEnumerable<(string Key, string Swedish, string English)> Items =>
        [
            ("Member.LoginLink", "Logga in", "Log in"),
            ("Member.AccountLink", "Mitt konto", "My account"),
            ("Member.Logout", "Logga ut", "Log out"),
            ("Member.Username", "Användarnamn", "Username"),
            ("Member.Password", "Lösenord", "Password"),
            ("Member.LoginButton", "Logga in", "Log in"),
            ("Member.LoginFailed", "Fel användarnamn eller lösenord, eller så är kontot spärrat.", "Wrong username or password, or the account is locked."),
            ("Member.CodeIntro", "Ange koden som din autentiseringsapp visar för att logga in.", "Enter the code your authenticator app shows to log in."),
            ("Member.Code", "Kod", "Code"),
            ("Member.Verify", "Verifiera", "Verify"),
            ("Member.CodeInvalid", "Koden stämmer inte. Ange koden som appen visar just nu.", "That code isn't right. Enter the code the app shows right now."),
            ("Member.LoggedInAs", "Inloggad som", "Logged in as"),
            ("Member.TwoFactorHeading", "Tvåstegsverifiering", "Two-factor authentication"),
            ("Member.TwoFactorOn", "Tvåstegsverifiering är på. När du loggar in anger du också en kod från din autentiseringsapp.", "Two-factor authentication is on. When you log in, you also enter a code from your authenticator app."),
            ("Member.TwoFactorOff", "Skydda kontot med en kod från en autentiseringsapp, till exempel Google Authenticator eller Microsoft Authenticator, när du loggar in.", "Protect your account with a code from an authenticator app, such as Google Authenticator or Microsoft Authenticator, when you log in."),
            ("Member.ScanQrCode", "Skanna QR-koden med appen och ange koden den visar.", "Scan the QR code with the app and enter the code it shows."),
            ("Member.QrCodeAlt", "QR-kod för autentiseringsappen", "QR code for the authenticator app"),
            ("Member.TurnOn", "Slå på", "Turn on"),
            ("Member.TurnOff", "Stäng av", "Turn off"),
        ];
    }
}

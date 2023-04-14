using System;
using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// Überprüft eine E-Mail-Adresse auf Korrektheit
/// </summary>
/// <remarks>
/// Original-Code von Microsoft (http://msdn.microsoft.com/en-us/library/01escwtf.aspx)
/// Modifiziert von gemelo (Peer Lessing) 15.09.2013
/// </remarks>
public class EmailValidator
{
    #region Felder und Eigenschaften

    private bool m_Invalid = false;

    #endregion Felder und Eigenschaften

    #region Statischer Konstruktor

    public static bool IsValidEmail(string value)
    {
        var validator = new EmailValidator();
        return validator.IsValidEmailInternal(value);
    }

    #endregion Statischer Konstruktor

    #region Konstruktor und Initialisierung

    private EmailValidator() { }

    #endregion Konstruktor und Initialisierung

    #region Private Methoden


    private bool IsValidEmailInternal(string value)
    {
        m_Invalid = false;
        if (string.IsNullOrEmpty(value)) return false;

        // Use IdnMapping class to convert Unicode domain names. 
        try
        {
#if V45
            value = Regex.Replace(value, @"(@)(.+)$", DomainMapper, RegexOptions.None, 
                TimeSpan.FromMilliseconds(200));
            
#else
            value = Regex.Replace(value, @"(@)(.+)$", this.DomainMapper);
#endif

        }
#if V45
        catch (RegexMatchTimeoutException)
        
#else
        catch (Exception)
#endif

        {
            return false;
        }

        if (m_Invalid)
            return false;

        // Return true if strIn is in valid e-mail format. 
        try
        {
#if V45
            return Regex.IsMatch(value,
                  @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|" +
                  @"[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                  @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|" +
                  @"(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                  RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            
#else
            return Regex.IsMatch(value,
              @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
              @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
              RegexOptions.IgnoreCase);
#endif
        }
#if V45
        catch (RegexMatchTimeoutException)
        
#else
        catch (Exception)
#endif
        {
            return false;
        }
    }

    private string DomainMapper(Match match)
    {
        // IdnMapping class with default property values.
        IdnMapping idn = new IdnMapping();

        string domainName = match.Groups[2].Value;
        try
        {
            domainName = idn.GetAscii(domainName);
        }
        catch (ArgumentException)
        {
            m_Invalid = true;
        }
        return match.Groups[1].Value + domainName;
    }

    #endregion Private Methoden
}

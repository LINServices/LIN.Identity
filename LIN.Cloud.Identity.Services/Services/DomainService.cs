using DnsClient;
using System.Text.RegularExpressions;

namespace LIN.Cloud.Identity.Services.Services;

internal class DomainService : IDomainService
{

    /// <summary>
    /// Verifica si el dominio es válido.
    /// </summary>
    /// <param name="dominio">Dominio.</param>
    public bool VerifyDomain(string dominio)
    {
        string patron = @"^(?!\-)([a-zA-Z0-9\-]{1,63}(?<!\-)\.)+[a-zA-Z]{2,}$";
        return Regex.IsMatch(dominio, patron);
    }


    /// <summary>
    /// Validar DNS.
    /// </summary>
    /// <param name="domain">Dominio.</param>
    /// <param name="code">Código de verificación.</param>
    public async Task<bool> VerifyDns(string domain, string code)
    {
        var lookup = new LookupClient();

        try
        {
            var resultado = await lookup.QueryAsync(domain, QueryType.TXT);
            var txtRecords = resultado.Answers.TxtRecords();

            if (txtRecords.Any())
            {
                foreach (var record in txtRecords)
                {
                    foreach (var text in record.Text)
                    {
                        if (text == code)
                            return true; // Registro TXT encontrado y verificado
                    }
                }
            }
            return false;

        }
        catch (Exception)
        {
        }
        return false; // Registro TXT no encontrado o no coincide con el código
    }

}
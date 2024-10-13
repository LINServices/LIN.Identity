using LIN.Cloud.Identity.Persistence.Models;

namespace LIN.Cloud.Identity.Data;


public class PassKeys(DataContext context)
{

    public async Task<ResponseBase> Create(PassKeyDBModel modelo)
    {
        // Pre.
        modelo.Id = 0;

        try
        {

            // Guardar la identidad.
            await context.PassKeys.AddAsync(modelo);
            context.SaveChanges();

            return new()
            {
                Response = Responses.Success,
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.ExistAccount
            };
        }

    }




    public async Task<ReadOneResponse<int>> Count(int id)
    {

        try
        {


            var time = DateTime.Now;


            var c = await (from a in context.PassKeys
                           where a.AccountId == id
                           where a.Time.Year == time.Year
                           && a.Time.Month == time.Month
                           && a.Time.Day == time.Day
                           select a).CountAsync();



            // Success.
            return new()
            {
                Response = Responses.Success,
                Model = c
            };

        }
        catch (Exception)
        {
            return new()
            {
                Response = Responses.NotRows
            };
        }

    }




}
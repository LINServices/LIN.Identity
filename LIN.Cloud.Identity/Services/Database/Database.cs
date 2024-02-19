using Microsoft.EntityFrameworkCore;

namespace LIN.Cloud.Identity.Services.Database;


public sealed class DataBase : DataContext
{


    /// <summary>
    /// Obtiene o establece si la DataBase esta en uso.
    /// </summary>
    private volatile bool OnUse = false;



    /// <summary>
    /// Obtiene si la DataBase esta en uso y la pone en uso.
    /// </summary>
    public bool OnUseAction
    {
        get
        {
            lock (this)
            {
                if (!OnUse)
                {
                    OnUse = true;
                    return false;
                }
                return true;
            }
        }
    }



    /// <summary>
    /// LLave para cerrar la conexión.
    /// </summary>
    public string Key = string.Empty;



    /// <summary>
    /// Nueva DataBase.
    /// </summary>
    public DataBase() : base(new DbContextOptionsBuilder<DataContext>().UseSqlServer(DataService.ConnectionString).Options)
    {
    }



    /// <summary>
    /// Destructor.
    /// </summary>
    ~DataBase()
    {
        Dispose();
    }




    /// <summary>
    /// Establece que la conexión esta en uso.
    /// </summary>
    public void SetOnUse()
    {
        lock (this)
        {
            OnUse = true;
        }
    }



    /// <summary>
    /// Cerrar la conexión con la base de datos.
    /// </summary>
    /// <param name="key">Llave</param>
    public void Close(string key)
    {
        lock (this)
        {
            if (Key != key)
                return;

            foreach (var entry in ChangeTracker.Entries())
                entry.State = EntityState.Detached;
            
            ChangeTracker.Clear();
            Key = string.Empty;
            OnUse = false;
        }
    }


}
namespace LIN.Identity.Areas.Organizations;


[Route("organizations")]
public class OrganizationsController : ControllerBase
{


    /// <summary>
    /// Crea una nueva organización.
    /// </summary>
    /// <param name="modelo">Modelo de la organización y el usuario administrador</param>
    [HttpPost("create")]
    public async Task<HttpCreateResponse> Create([FromBody] OrganizationModel modelo)
    {

        // Comprobaciones
        if (modelo == null || modelo.Domain.Length <= 0 || modelo.Name.Length <= 0 || modelo.Members.Count <= 0)
            return new(Responses.InvalidParam);


        // BD.
        var (context, connectionKey) = Conexión.GetOneConnection();


        // organización del modelo
        modelo.ID = 0;

        // Directorio
        modelo.Directory = new()
        {
            ID = 0,
            Creación = DateTime.Now,
            Nombre = "Directorio General: " + modelo.Name,
            Identity = new()
            {
                Id = 0,
                Type = IdentityTypes.Directory,
                Unique = "d_" + modelo.Domain
            },
            IdentityId = 0
        };



        foreach (var member in modelo.Members)
        {
            modelo.Directory.Members.Add(new()
            {
                Identity = member.Member.Identity,
                IdentityId =0,
                Directory = modelo.Directory,
                DirectoryId = 0
            });
            member.Member = Account.Process(member.Member);
            member.Rol = OrgRoles.SuperManager;
            member.Organization = modelo;
        }

        // Creaci�n de la organización
        var response = await Data.Organizations.Organizations.Create(modelo, context);

        // Evaluaci�n
        if (response.Response != Responses.Success)
            return new(response.Response);

        context.CloseActions(connectionKey);

        // Retorna el resultado
        return new CreateResponse()
        {
            LastID = response.Model.ID,
            Response = Responses.Success,
            Message = "Success"
        };

    }



    /// <summary>
    /// Obtiene una organización por medio del Id.
    /// </summary>
    /// <param name="id">ID de la organización</param>
    /// <param name="token">Token de acceso</param>
    [HttpGet("read/id")]
    public async Task<HttpReadOneResponse<OrganizationModel>> ReadOneByID([FromQuery] int id, [FromHeader] string token)
    {

        // Parámetros
        if (id <= 0)
            return new(Responses.InvalidParam);

        // Validar el token
        var (isValid, _, _, orgID, _) = Jwt.Validate(token);


        if (!isValid)
            return new(Responses.Unauthorized);


        // Obtiene la organización
        var response = await Data.Organizations.Organizations.Read(id);

        // Organización no encontrada.
        if (response.Response != Responses.Success)
            return new ReadOneResponse<OrganizationModel>()
            {
                Response = Responses.NotRows,
                Message = "No se encontró la organización."
            };

        // No es publica y no pertenece a ella
        if (!response.Model.IsPublic && orgID != response.Model.ID)
            return new ReadOneResponse<OrganizationModel>()
            {
                Response = Responses.Unauthorized,
                Message = "Esta organización es privada y tu usuario no esta vinculado a ella.",
                Model = new()
                {
                    ID = response.Model.ID,
                    IsPublic = false,
                    Name = "Organización privada"
                }
            };

        return new ReadOneResponse<OrganizationModel>()
        {
            Response = Responses.Success,
            Model = response.Model
        };


    }



}
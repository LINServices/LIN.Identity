namespace LIN.Cloud.Identity.Areas.Organizations;

[Route("[controller]")]
public class OrganizationsController(IOrganizationRepository organizationsData, IOrganizationMemberRepository directoryMembersData) : AuthenticationBaseController
{
    /// <summary>
    /// Crea una nueva organización.
    /// </summary>
    /// <param name="modelo">Modelo de la organización y el usuario administrador</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromBody] OrganizationModel modelo)
    {
        // Validar el modelo.
        if (modelo == null || string.IsNullOrWhiteSpace(modelo.Name) || modelo.Directory == null || modelo.Directory.Identity == null || string.IsNullOrWhiteSpace(modelo.Directory.Identity.Unique))
            return new()
            {
                Response = Responses.InvalidParam,
                Message = "Parámetros inválidos."
            };

        // Normalizar y completar el modelo.
        {
            modelo.Id = 0;
            modelo.Name = modelo.Name.Trim();
            modelo.Creation = DateTime.UtcNow;
            modelo.Directory.Members = [];
            modelo.Directory.Name = modelo.Directory.Name.Trim();
            modelo.Directory.Identity.EffectiveTime = DateTime.UtcNow;
            modelo.Directory.Identity.CreationTime = DateTime.UtcNow;
            modelo.Directory.Identity.EffectiveTime = DateTime.UtcNow.AddYears(10);
            modelo.Directory.Identity.Status = IdentityStatus.Enable;
        }

        var response = await organizationsData.Create(modelo);

        if (response.Response != Responses.Success)
            return new(response.Response);

        return new CreateResponse(Responses.Success, response.LastId);
    }

    /// <summary>
    /// Obtiene una organización por medio del Id.
    /// </summary>
    /// <param name="id">ID de la organización</param>
    [HttpGet("read/id")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<OrganizationModel>> ReadOneByID([FromQuery] int id)
    {

        if (id <= 0)
            return new(Responses.InvalidParam);

        var response = await organizationsData.Read(id);

        if (response.Response != Responses.Success)
            return new ReadOneResponse<OrganizationModel>()
            {
                Response = Responses.NotRows,
                Message = "No se encontró la organización."
            };

        // Si la organización no es pública, el usuario debe pertenecer a ella.
        if (!response.Model.IsPublic)
        {

            var iamIn = await directoryMembersData.IamIn(UserInformation.IdentityId, response.Model.Id);

            if (iamIn.Response != Responses.Success)
                return new ReadOneResponse<OrganizationModel>()
                {
                    Response = Responses.Unauthorized,
                    Message = "Esta organización es privada y tu usuario no esta vinculado a ella.",
                    Model = new()
                    {
                        Id = response.Model.Id,
                        IsPublic = false,
                        Name = "Organización privada"
                    }
                };
        }

        return new ReadOneResponse<OrganizationModel>(Responses.Success, response.Model);
    }

    /// <summary>
    /// Obtiene las organizaciones donde un usuario es miembro.
    /// </summary>
    [HttpGet("read/all")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<OrganizationModel>> ReadAll()
    {
        var response = await directoryMembersData.ReadAllMembers(UserInformation.IdentityId);

        return response;
    }

}
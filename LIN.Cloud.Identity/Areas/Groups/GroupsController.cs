﻿namespace LIN.Cloud.Identity.Areas.Groups;


[Route("groups")]
public class GroupsController : ControllerBase
{


    /// <summary>
    /// Crear nuevo grupo.
    /// </summary>
    /// <param name="group">Modelo del grupo.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpPost]
    [IdentityToken]
    public async Task<HttpCreateResponse> Create([FromBody] GroupModel group, [FromHeader] string token)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Confirmar el rol.
        var (_, roles) = await RolesIam.RolesOn(tokenInfo.IdentityId, group.OwnerId ?? 0);

        // Iam.
        bool iam = ValidateRoles.ValidateAlterMembers(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para crear grupos.",
                Response = Responses.Unauthorized
            };

        // Formato de la identidad.
        group.Identity.Type = IdentityType.Group;
        Services.Formats.Identities.Process(group.Identity);

        // Obtener el modelo.
        var response = await Data.Groups.Create(group);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return new CreateResponse()
        {
            Response = Responses.Success,
            LastID = response.Model.Id
        };

    }




    /// <summary>
    /// Obtener los grupos asociados a una organización.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="organization">Id de la organización.</param>
    [HttpGet("all")]
    [IdentityToken]
    public async Task<HttpReadAllResponse<GroupModel>> ReadAll([FromHeader] string token, [FromHeader] int organization)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Confirmar el rol.
        var (_, roles) = await RolesIam.RolesOn(tokenInfo.IdentityId, organization);

        // Iam.
        bool iam = ValidateRoles.ValidateRead(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        // Obtener el modelo.
        var response = await Data.Groups.ReadAll(organization);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return response;

    }




    /// <summary>
    /// Obtener un grupo según el Id.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="id">Id del grupo.</param>
    [HttpGet]
    [IdentityToken]
    public async Task<HttpReadOneResponse<GroupModel>> ReadOne([FromHeader] string token, [FromHeader] int id)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();


        // Obtener la organización.
        var orgId = await Data.Groups.GetOwner(id);

        // Si hubo un error.
        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };


        // Confirmar el rol.
        var (_, roles) = await RolesIam.RolesOn(tokenInfo.IdentityId, orgId.Model);

        // Iam.
        bool iam = ValidateRoles.ValidateRead(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        // Obtener el modelo.
        var response = await Data.Groups.Read(id);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return response;

    }



    /// <summary>
    /// Obtener un grupo según el Id.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="id">Id del grupo.</param>
    [HttpGet("identity")]
    [IdentityToken]
    public async Task<HttpReadOneResponse<GroupModel>> ReadIdentity([FromHeader] string token, [FromHeader] int id)
    {

        // Token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();


        // Obtener la organización.
        var orgId = await Data.Groups.GetOwnerByIdentity(id);

        // Si hubo un error.
        if (orgId.Response != Responses.Success)
            return new()
            {
                Message = "Hubo un error al encontrar la organización dueña de este grupo.",
                Response = Responses.Unauthorized
            };


        // Confirmar el rol.
        var (_, roles) = await RolesIam.RolesOn(tokenInfo.IdentityId, orgId.Model);

        // Iam.
        bool iam = ValidateRoles.ValidateRead(roles);

        // Si no tiene permisos.
        if (!iam)
            return new()
            {
                Message = "No tienes acceso para leer grupos.",
                Response = Responses.Unauthorized
            };

        // Obtener el modelo.
        var response = await Data.Groups.ReadByIdentity(id);

        // Si es erróneo
        if (response.Response != Responses.Success)
            return new()
            {
                Response = response.Response
            };

        // Retorna el resultado
        return response;

    }



}
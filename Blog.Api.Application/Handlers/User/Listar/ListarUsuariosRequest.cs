// Blog.Api.Application.Handlers.User.ListarUsuarios/ListarUsuariosRequest.cs

using MediatR;
using ErrorOr;
using System.Collections.Generic;

namespace Blog.Api.Application.Handlers.User.Listar;

// A requisição não precisa de dados, apenas solicita a lista.
public record ListarUsuariosRequest() : IRequest<ErrorOr<IEnumerable<UsuarioResponse>>>;
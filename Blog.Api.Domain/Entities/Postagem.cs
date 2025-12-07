// Em Blog.Api.Domain/Entities/Postagem.cs
using System;

namespace Blog.Api.Domain.Entities;
public class Postagem
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = default!;
    public string Conteudo { get; set; } = default!;
    public DateTime DataCriacao { get; set; }
    
    // Chave estrangeira para o usuário que criou a postagem
    public string AutorId { get; set; } = default!; 
    // Nota: O AutorId será o Id do IdentityUser (string)

    // Novo: Data da última atualização
    public DateTime? AtualizadoEm { get; set; }

    // Novo: Usuário que alterou a postagem
    public string? IdUsuarioAlteracao { get; set; }

    public bool Inativo { get; set; }

}
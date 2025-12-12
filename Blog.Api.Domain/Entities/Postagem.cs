// Em Blog.Api.Domain/Entities/Postagem.cs
using System;

namespace Blog.Api.Domain.Entities;
public class Postagem
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = default!;
    public string Conteudo { get; set; } = default!;
    public DateTime DataCriacao { get; set; }
    public string AutorId { get; set; } = default!; 
    public DateTime? AtualizadoEm { get; set; }
    public string? IdUsuarioAlteracao { get; set; }
    public bool Inativo { get; set; }

}
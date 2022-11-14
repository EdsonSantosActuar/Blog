using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Nome é obrigatório!")]
    public string Name { get; set; }

    [Required(ErrorMessage = "E-mail é obrigatório!")]
    [EmailAddress(ErrorMessage ="Formato inválido para o E-mail")]
    public string Email { get; set; }
}
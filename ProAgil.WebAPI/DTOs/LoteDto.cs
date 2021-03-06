using System.ComponentModel.DataAnnotations;

namespace ProAgil.WebAPI.DTOs
{
    //Nos DTOs é possível definir os valores que serão retornado e mapeados pelo AutoMapper. No AutoMapper é possível criar uma configuração onde nós referênciamos os DTOs e criamos suas "Opções".

    //No caso em questão são definidos apenas os dados importantes para retornar ao usuário, funcionando como uma espécie de "filtro"
    public class LoteDto
    {
        public int Id { get; set; }

        [Required (ErrorMessage="O nome do lote deve ser preenchido")]
        public string Nome { get; set; }

        public decimal Preco { get; set; }

        public string DataInicio { get; set; }

        public string DataFim { get; set; }

        [Range(2, 120000)]
        public int Quantidade { get; set; }
    }
}
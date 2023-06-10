using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Amyra.Models
{
    [Table("t_producto")]
    public class Producto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }  
        [Display(Name = "Nombre del producto")]   
        public string? Name { get; set; }
        [Display(Name = "Descripción")]   
        public string? Descripcion { get; set; }
        [Display(Name = "Precio")]   
        public Decimal Precio { get; set; }

        public Decimal PorcentajeDescuento { get; set; }

        public string? ImageName { get; set; }

        public string? Status { get; set; }
    }
}
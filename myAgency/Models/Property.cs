//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace myAgency.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Property
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Property()
        {
            this.Option = new HashSet<Option>();
        }
    
        public int id { get; set; }

        [Required]
        [Display(Name="Titre*")]
        public string title { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Required]
        [Display(Name = "Surface*")]
        public int surface { get; set; }

        [Required]
        [Display(Name = "Pièces*")]
        public int rooms { get; set; }

        [Required]
        [Display(Name = "Chambres*")]
        public int bedrooms { get; set; }

        [Required]
        [Display(Name = "Etage*")]
        public int floor { get; set; }

        [Required]
        [Display(Name = "Prix*")]
        public int price { get; set; }

        [Required]
        [Display(Name = "Chauffage")]
        public int heat { get; set; }

        [Required]
        [Display(Name = "Ville*")]
        public string city { get; set; }

        [Required]
        [Display(Name = "Adesse*")]
        public string address { get; set; }

        [Required]
        [Display(Name = "Code postal*")]
        public string postal_code { get; set; }

        [Display(Name = "Vendu")]
        public bool sold { get; set; }

        [Display(Name = "Date création")]
        public System.DateTime created_at { get; set; }

        [Display(Name = "Fichier")]
        public string filename { get; set; }

        [Display(Name = "Mise à jour")]
        public System.DateTime updated_at { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Display(Name = "Options")]
        public virtual ICollection<Option> Option { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace DataTransferSQLToEl.SQLDomainModel
{
    [Table("Person.Person")]
    public partial class Person
    {
        public Person()
        {
            EmailAddress = new HashSet<EmailAddress>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BusinessEntityID { get; set; }

        [Required]
        [StringLength(2)]
        public string PersonType { get; set; }

        public bool NameStyle { get; set; }

        [StringLength(8)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(10)]
        public string Suffix { get; set; }

        public int EmailPromotion { get; set; }

        [Column(TypeName = "xml")]
        public string AdditionalContactInfo { get; set; }

        [Column(TypeName = "xml")]
        public string Demographics { get; set; }

        public Guid rowguid { get; set; }

        public DateTime ModifiedDate { get; set; }

        public virtual ICollection<EmailAddress> EmailAddress { get; set; }

        public virtual Password Password { get; set; }
    }
}

namespace DataTransferSQLToEl.SQLDomainModel
{
	using System;
	using System.Data.Entity;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Linq;

	public partial class SQLDataModel : DbContext
	{
		public SQLDataModel()
			: base("name=SQLDataModel")
		{
		}

		public virtual DbSet<Address> Address { get; set; }
		public virtual DbSet<AddressType> AddressType { get; set; }
		public virtual DbSet<BusinessEntity> BusinessEntity { get; set; }
		public virtual DbSet<BusinessEntityAddress> BusinessEntityAddress { get; set; }
		public virtual DbSet<BusinessEntityContact> BusinessEntityContact { get; set; }
		public virtual DbSet<ContactType> ContactType { get; set; }
		public virtual DbSet<CountryRegion> CountryRegion { get; set; }
		public virtual DbSet<EmailAddress> EmailAddress { get; set; }
		public virtual DbSet<Password> Password { get; set; }
		public virtual DbSet<Person> Person { get; set; }
		public virtual DbSet<PersonPhone> PersonPhone { get; set; }
		public virtual DbSet<PhoneNumberType> PhoneNumberType { get; set; }
		public virtual DbSet<StateProvince> StateProvince { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Address>()
				.HasMany(e => e.BusinessEntityAddress)
				.WithRequired(e => e.Address)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<AddressType>()
				.HasMany(e => e.BusinessEntityAddress)
				.WithRequired(e => e.AddressType)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<BusinessEntity>()
				.HasMany(e => e.BusinessEntityAddress)
				.WithRequired(e => e.BusinessEntity)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<BusinessEntity>()
				.HasMany(e => e.BusinessEntityContact)
				.WithRequired(e => e.BusinessEntity)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<BusinessEntity>()
				.HasOptional(e => e.Person)
				.WithRequired(e => e.BusinessEntity);

			modelBuilder.Entity<ContactType>()
				.HasMany(e => e.BusinessEntityContact)
				.WithRequired(e => e.ContactType)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<CountryRegion>()
				.HasMany(e => e.StateProvince)
				.WithRequired(e => e.CountryRegion)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Password>()
				.Property(e => e.PasswordHash)
				.IsUnicode(false);

			modelBuilder.Entity<Password>()
				.Property(e => e.PasswordSalt)
				.IsUnicode(false);

			modelBuilder.Entity<Person>()
				.Property(e => e.PersonType)
				.IsFixedLength();

			modelBuilder.Entity<Person>()
				.HasMany(e => e.BusinessEntityContact)
				.WithRequired(e => e.Person)
				.HasForeignKey(e => e.PersonID)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Person>()
				.HasMany(e => e.EmailAddress)
				.WithRequired(e => e.Person)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Person>()
				.HasOptional(e => e.Password)
				.WithRequired(e => e.Person);

			modelBuilder.Entity<Person>()
				.HasMany(e => e.PersonPhone)
				.WithRequired(e => e.Person)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<PhoneNumberType>()
				.HasMany(e => e.PersonPhone)
				.WithRequired(e => e.PhoneNumberType)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<StateProvince>()
				.Property(e => e.StateProvinceCode)
				.IsFixedLength();

			modelBuilder.Entity<StateProvince>()
				.HasMany(e => e.Address)
				.WithRequired(e => e.StateProvince)
				.WillCascadeOnDelete(false);
		}
	}
}

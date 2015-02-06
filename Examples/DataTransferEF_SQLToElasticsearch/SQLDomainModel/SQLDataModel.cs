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
			// Turn off the Migrations, (NOT a code first Db)
			Database.SetInitializer<SQLDataModel>(null);
			//this.Configuration.LazyLoadingEnabled = false;

		}

		public virtual DbSet<Address> Address { get; set; }
		public virtual DbSet<CountryRegion> CountryRegion { get; set; }
		public virtual DbSet<EmailAddress> EmailAddress { get; set; }
		public virtual DbSet<Password> Password { get; set; }
		public virtual DbSet<Person> Person { get; set; }
		public virtual DbSet<StateProvince> StateProvince { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
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
				.HasMany(e => e.EmailAddress)
				.WithRequired(e => e.Person)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Person>()
				.HasOptional(e => e.Password)
				.WithRequired(e => e.Person);

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

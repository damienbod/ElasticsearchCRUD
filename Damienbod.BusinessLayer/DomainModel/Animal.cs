using System;

namespace Damienbod.BusinessLayer.DomainModel
{
	//[DataContract(Name = "City")]
    public class Animal
    {
        public const string SearchIndex = "animals";

		//[DataMember]
		//[Key]
        public int Id { get; set; }

        public string AnimalType { get; set; }

        public string TypeSpecificForAnimalType { get; set; }

        public string Description { get; set; }

        public string Gender { get; set; }

        public string LastLocation { get; set; }

		public DateTimeOffset DateOfBirth { get; set; }

		public DateTimeOffset CreatedTimestamp { get; set; }

		public DateTimeOffset UpdatedTimestamp { get; set; }

		// TODO these types are Datetime objects in the engine
    }
}
